using HTC.UnityPlugin.PoseTracker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using VIVE.OpenXR;
using VIVE.OpenXR.Toolkits.BodyTracking;
using VIVE.OpenXR.Toolkits.BodyTracking.RuntimeDependency;
using VRSStudio.Common;
using VRSStudio.Common.Input;
using VRSStudio.Spectator;
using VRSStudio.Tracker;
using static VIVE.OpenXR.Toolkits.BodyTracking.HumanoidTracking;
using VRSController = VRSStudio.Common.Input.Controller;

namespace VRSStudio.Avatar
{
    public class VRSBodyTrackingManager : MonoBehaviour
    {
        private static string TAG = "AvatarManager";

        public static VRSBodyTrackingManager Instance { get; private set; }

        #region Insepector
        [SerializeField]
        private BodyRoleData roleData;
        [SerializeField]
        private GameObject displayAvatar;
        [SerializeField]
        private GameObject calibAvatar;
        [SerializeField]
        private HumanoidTracking trackAvatar;

        [SerializeField]
        private SkinnedMeshRenderer body;
        [SerializeField]
        private SkinnedMeshRenderer hair;
        [SerializeField]
        private SkinnedMeshRenderer face;

        [Header("Left Body Settings")]
        [SerializeField]
        private Transform leftUpperLeg;
        [SerializeField]
        private Transform leftLowerLeg;
        [SerializeField]
        private Transform leftFoot;

        [Header("Right Body Settings")]
        [SerializeField]
        private Transform rightUpperLeg;
        [SerializeField]
        private Transform rightLowerLeg;
        [SerializeField]
        private Transform rightFoot;

        // BodyTracking start/stop
        [SerializeField]
        private bool debugBT = false;
        #endregion

        private static readonly InputUsage usagesCtrl = InputUsage.Position | InputUsage.IsTracked | InputUsage.PrimaryButton;
        private static VRSController ctrlL = new VRSController(true, usagesCtrl);
        private static VRSController ctrlR = new VRSController(false, usagesCtrl);

        private bool isTracking = false;
        private bool isCalibrating = false;
        private Vector3 initFootPos = Vector3.zero;
        private bool isAvatarUpdated = false;
        private float calibCamHeight = 0f;
        private List<Component> trackingComponents = new List<Component>();
        private List<uint> requestedTrackerId = new List<uint>();

        [SerializeField]
        private UnityEvent onBeginTracking = new UnityEvent();
        [SerializeField]
        private UnityEvent onEndTracking = new UnityEvent();

        bool GetCameraYawPose(Transform target, out Vector3 pos, out Quaternion rot)
        {
            if (target == null)
            {
                pos = Vector3.zero;
                rot = Quaternion.identity;
                return false;
            }
            var euler = target.rotation.eulerAngles;
            var forwardDir = Quaternion.Euler(0, euler.y, 0);
            pos = target.position;
            rot = forwardDir;
            return true;
        }

        // Return world position and rotation
        bool GetCameraYawPose(out Vector3 pos, out Quaternion rot)
        {
            if (Camera.main != null)
                return GetCameraYawPose(Camera.main.transform, out pos, out rot);
            else
                return GetCameraYawPose(null, out pos, out rot);
        }

        //Transform htOriginalParent;
        //Pose htOriginalTransform = new Pose();
        Bounds boundsBodyOrigin;

        IEnumerator StartTracking()
        {
            Log.d(TAG, "BeginTracking()");
            Transform rig = null;
            if (rig == null && VIVERig.Instance != null)
                rig = VIVERig.Instance.transform;
            else
                yield break;

            availableTrackingMode = GetTrackingMode();
            if (availableTrackingMode != TrackingMode.Arm && VRSTrackerManager.Instance)
            {
                UpdateRole();
                VRSTrackerManager.Instance.RequestTrackers(requestedTrackerId, OnReallocateTracker);
                isCalibrating = true;
            }
            trackAvatar.gameObject.SetActive(true);
            trackAvatar.Tracking = availableTrackingMode;
            Log.d(TAG, $"TrackingMode set to {trackAvatar.Tracking}");
            trackAvatar.AvatarOffset = rig.transform;
            trackAvatar.BeginCalibration();
            if (!(TutorialManager.Instance != null && TutorialManager.Instance.IsInTutorial()))
            {
                isTracking = true;
                onBeginTracking?.Invoke();
                isCalibrating = false;
            }
            displayAvatar.SetActive(false);

            boundsBodyOrigin = body.localBounds;

            // pos and rot are in world coord.
            bool hasMainCam = GetCameraYawPose(out Vector3 pos, out Quaternion rot);
            if (hasMainCam)
            {
                var pos_front = pos + rot * Vector3.forward * 1.5f;
                var rot_180 = Quaternion.Euler(0, 180, 0) * rot;

                calibAvatar.SetActive(true);
                calibAvatar.transform.position = new Vector3(pos_front.x, rig.position.y, pos_front.z);
                calibAvatar.transform.rotation = rot_180;
            }
            if (TutorialManager.Instance != null && TutorialManager.Instance.IsInTutorial())
            {
                yield return new WaitUntil(() => TutorialManager.Instance.IsArmStretched());
                isTracking = true;
                onBeginTracking?.Invoke();
                isCalibrating = false;
            }
            else yield return new WaitForSeconds(2.5f);
            trackAvatar.BeginTracking();

            if (hasMainCam)
                calibAvatar.SetActive(false);

            if (VIVERig.Instance && Camera.main.transform)
            {
                StartCoroutine(WaitAvatarAndRigUpdate());
            }
        }

        // If tracker is used on other feature.  Stop tracking.
        private void OnReallocateTracker(List<uint> rolesWillBeReallocated)
        {
            StopTracking();
        }

        public void StopTracking()
        {
            if (!isTracking) return;
            Log.d(TAG, "StopTracking()");
            StopCoroutine("StartTracking");
            trackAvatar.StopTracking();
            trackAvatar.gameObject.SetActive(false);

            body.localBounds = boundsBodyOrigin;
            hair.localBounds = boundsBodyOrigin;
            face.localBounds = boundsBodyOrigin;

            displayAvatar.SetActive(true);
            calibAvatar.SetActive(false);
            isTracking = false;
            onEndTracking?.Invoke();
            isCalibrating = false;
            isAvatarUpdated = false;
            EnableTrackingComponents(false);

            if (availableTrackingMode != TrackingMode.Arm && VRSTrackerManager.Instance)
                VRSTrackerManager.Instance.FreeAllTrackers(OnReallocateTracker);
        }

        public bool IsCalibrating()
        {
            return calibAvatar.activeSelf;
        }

        readonly Timer timerTracking = new Timer(0.75f);
        readonly Timer timerCheckMode = new Timer(0.50f);

        List<InputDevice> inputDevices = new List<InputDevice>();

        TrackingMode availableTrackingMode = TrackingMode.Arm;

        private void UpdateInput()
        {
            if (!ctrlR.dev.isValid) InputDeviceTools.GetController(ctrlR, inputDevices);
            if (ctrlR.dev.isValid) InputDeviceTools.UpdateController(ctrlR);
            if (!ctrlL.dev.isValid) InputDeviceTools.GetController(ctrlL, inputDevices);
            if (ctrlL.dev.isValid) InputDeviceTools.UpdateController(ctrlL);
        }

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;

                if (leftFoot && rightFoot)
                {
                    initFootPos = (leftFoot.position + rightFoot.position) / 2;
                }

                if (VIVERig.Instance && trackAvatar)
                {
                    FindComponent(trackAvatar.transform, typeof(PoseEaser));
                    FindComponent(trackAvatar.transform, typeof(PoseStablizer));
                    FindComponent(trackAvatar.transform, typeof(PoseTracker));
                    EnableTrackingComponents(false);
                    trackAvatar.gameObject.SetActive(false);
                }
            }
        }

        private void FindComponent(Transform transform, Type type)
        {
            Component component = trackAvatar.GetComponent(type);
            if (component != null)
            {
                trackingComponents.Add(component);
            }
        }

        private void EnableTrackingComponents(bool enable)
        {
            foreach (Component component in trackingComponents)
            {
                Type type = component.GetType();
                if (type == typeof(PoseEaser))
                {
                    PoseEaser poseEaser = component as PoseEaser;
                    if (poseEaser != null)
                        poseEaser.enabled = enable;
                }
                else if (type == typeof(PoseStablizer))
                {
                    PoseStablizer poseStablizer = component as PoseStablizer;
                    if (poseStablizer != null)
                        poseStablizer.enabled = enable;
                }
                else if (type == typeof(PoseTracker))
                {
                    PoseTracker poseTracker = component as PoseTracker;
                    if (poseTracker != null)
                        poseTracker.enabled = enable;
                }
            }
        }

        private void Update()
        {
            UpdateInput();
            UpdateRole();

            if (debugBT)
            {
                debugBT = false;
                if (isTracking)
                    StopTracking();
                else
                    StartCoroutine(StartTracking());
            }

            // Log.d(TAG, $"L {btnPriL.IsPressed} R {btnPriR.IsPressed}");

            if (ctrlL.btnPri.IsPressed && ctrlR.btnPri.IsPressed)
            {
                if (timerTracking.IsSet)
                {
                    if (timerTracking.IsPaused)
                    { }
                    else if (timerTracking.Check())
                    {
                        if (isTracking)
                            StopTracking();
                        else
                            StartCoroutine(StartTracking());
                    }
                }
                else
                {
                    timerTracking.Set();
                }
            }
            else
            {
                if (timerTracking.IsSet)
                    timerTracking.Reset();
            }

            var hasCamera = GetCameraYawPose(out Vector3 pos, out Quaternion rot);
            if (isTracking && hasCamera)
            {
                // Update AABB of avatar's components
                var center = Quaternion.Inverse(body.transform.rotation) * (pos - body.transform.position);
                if (body != null)
                {
                    var b = body.localBounds;
                    b.center = center;
                    b.size = Vector3.one * 2.5f;
                    body.localBounds = b;
                }
                if (hair != null)
                {
                    var b = hair.localBounds;
                    b.center = center;
                    b.size = Vector3.one + Vector3.up * 0.5f;
                    hair.localBounds = b;
                }
                if (face != null)
                {
                    var b = face.localBounds;
                    b.center = center;
                    b.size = Vector3.one + Vector3.forward * 0.5f;
                    face.localBounds = b;
                }
            }

            if (IsSquatSimulationNeeded())
            {
                float camHeight = Camera.main.transform.position.y - VIVERig.Instance.transform.position.y;
                if (camHeight < calibCamHeight)
                {
                    float maxThreshold = calibCamHeight * 1.00f;
                    float minThreshold = calibCamHeight * 0.60f;
                    int upperLegRot = Mathf.RoundToInt(MappingValue(camHeight, minThreshold, maxThreshold, -130, 0));
                    int lowerLegRot = Mathf.RoundToInt(MappingValue(camHeight, minThreshold, maxThreshold, 130, 0));
                    if (leftUpperLeg)
                    {
                        leftUpperLeg.transform.localRotation = Quaternion.Euler(upperLegRot, 0, 0);
                    }
                    if (rightUpperLeg)
                    {
                        rightUpperLeg.transform.localRotation = Quaternion.Euler(upperLegRot, 0, 0);
                    }
                    if (leftLowerLeg)
                    {
                        leftLowerLeg.transform.localRotation = Quaternion.Euler(lowerLegRot, 0, 0);
                    }
                    if (rightLowerLeg)
                    {
                        rightLowerLeg.transform.localRotation = Quaternion.Euler(lowerLegRot, 0, 0);
                    }
                }
            }

            if (!isTracking && timerCheckMode.Check())
            {
                availableTrackingMode = GetTrackingMode();
            }
        }

        private const string roleWaist = "Waist";
        private const string roleLeftAnkle = "LeftAnkle";
        private const string roleRightAnkle = "RightAnkle";
        public Dictionary<string, uint> roleIdMapping { get; private set; } = new Dictionary<string, uint>()
        {
            {roleWaist, int.MaxValue},
            {roleLeftAnkle, int.MaxValue},
            {roleRightAnkle, int.MaxValue},
        };
        private void UpdateRole()
        {
            void UpdateId(string key, uint newId, List<uint> roleIds)
            {
                if (roleIdMapping[key] != newId)
                {
                    roleIdMapping[key] = newId;
                    if (key == roleWaist) { roleData.SetTrackerIndex(TrackerLocation.Waist, (int)newId); }
                    else if (key == roleLeftAnkle) { roleData.SetTrackerIndex(TrackerLocation.AnkleLeft, (int)newId); }
                    else if (key == roleRightAnkle) { roleData.SetTrackerIndex(TrackerLocation.AnkleRight, (int)newId); }
                }
                roleIds.Add(newId);
            }

            requestedTrackerId.Clear();
            if (isCalibrating || isTracking) { return; }
            if (VRSTrackerManager.Instance == null || BodyManager.Instance == null) { return; }

            GetFreeTrackers(out uint[] freeIds);
            int availableCount = Math.Min(freeIds.Length, 3);
            if (availableCount > 0)
            {
                UpdateId(roleWaist, freeIds[0], requestedTrackerId);
                if (availableCount >= 3)
                {
                    UpdateId(roleLeftAnkle, freeIds[1], requestedTrackerId);
                    UpdateId(roleRightAnkle, freeIds[2], requestedTrackerId);
                }
                else
                {
                    roleIdMapping[roleLeftAnkle] = int.MaxValue;
                    roleIdMapping[roleRightAnkle] = int.MaxValue;
                }
            }
            else
            {
                roleIdMapping[roleWaist] = int.MaxValue;
                roleIdMapping[roleLeftAnkle] = int.MaxValue;
                roleIdMapping[roleRightAnkle] = int.MaxValue;
            }
        }

        private IEnumerator WaitAvatarAndRigUpdate()
        {
            yield return new WaitUntil(() => IsAvatarAndRigUpdate() == true);
            calibCamHeight = Camera.main.transform.position.y - VIVERig.Instance.transform.position.y;
            EnableTrackingComponents(true);
            isAvatarUpdated = true;
        }

        private bool IsAvatarAndRigUpdate()
        {
            if (leftFoot && rightFoot)
            {
                Vector3 avatarFootPos = (leftFoot.position + rightFoot.position) / 2;
                return avatarFootPos.y != initFootPos.y;
            }
            return false;
        }

        private bool IsSquatSimulationNeeded()
        {
            return isTracking && availableTrackingMode == TrackingMode.Arm && isAvatarUpdated && VIVERig.Instance != null && Camera.main.transform != null;
        }

        private float MappingValue(float currentValue, float minValue, float maxValue, float leftValue, float rightValue)
        {
            // Ensure the current value is within the min and max bounds
            float clampedValue = Mathf.Clamp(currentValue, minValue, maxValue);

            // Normalize the current value within the original range
            float normalizedValue = (clampedValue - minValue) / (maxValue - minValue);

            // Map the normalized value to the new range
            float mappedValue = leftValue + (normalizedValue * (rightValue - leftValue));

            // Ensure the mapped value is within the left and right bounds
            float clampedMappedValue = Mathf.Clamp(mappedValue, Mathf.Min(leftValue, rightValue), Mathf.Max(leftValue, rightValue));

            return clampedMappedValue;
        }

        public TrackingMode GetAvailableTrackingMode()
        {
            return availableTrackingMode;
        }

        public void OnDrawGizmosSelected()
        {
            var bounds = face.localBounds;
            Gizmos.matrix = face.transform.localToWorldMatrix;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(bounds.center, bounds.extents * 2);
        }

        public TrackingMode GetTrackingMode()
        {
            var vrstm = VRSTrackerManager.Instance;
            if (vrstm == null) { return TrackingMode.Arm; }

            if (GetFreeTrackers(out var freeIds))
            {
                if (freeIds.Length > 0 && freeIds.Length < 3) return TrackingMode.UpperBody;
                else if (freeIds.Length >= 3) return TrackingMode.FullBody;
            }
            return TrackingMode.Arm;
        }

        public void StartBodyTracking()
        {
            StartCoroutine(StartTracking());
        }

        public bool IsTracking()
        {
            return isTracking;
        }

        private bool GetFreeTrackers(out uint[] trackers)
        {
            trackers = Array.Empty<uint>();
            var vrstm = VRSTrackerManager.Instance;

            if (vrstm && vrstm.GetFreeIdList(out var freeIds) && freeIds.Length > 0)
            {
                if (freeIds.Length != 1 && freeIds.Length != 3 && VRSSpectatorManager.Instance)
                {
                    uint tid = (uint)VRSSpectatorManager.Instance.GetCurrentTracker();
                    trackers = freeIds.Where(x => x != tid).ToArray();
                }
                else
                {
                    trackers = freeIds;
                }
                return true;
            }
            return false;
        }
    }
}
