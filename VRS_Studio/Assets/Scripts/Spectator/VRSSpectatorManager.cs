using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.XR;
using VIVE.OpenXR;
using VIVE.OpenXR.SecondaryViewConfiguration;
using VRSStudio.Common;
using VRSStudio.Common.Input;
using VRSStudio.Tracker;
using VRSController = VRSStudio.Common.Input.Controller;

namespace VRSStudio.Spectator
{
    public enum SpectatorMode
    {
        HMD,
        //Controller,
        Tracker,
        Selfie,
        FixedPose,
        ManualPose,
        Headset = HMD,
        FollowPose = Selfie,
    }

    public class SpectatorModeBehaviour
    {
        public SpectatorMode Mode { get; private set; }
        protected VRSSpectatorManager manager;

        public SpectatorModeBehaviour(VRSSpectatorManager manager, SpectatorMode mode)
        {
            this.manager = manager;
            Mode = mode;
        }

        /// <summary>
        /// Complete this mode's input update at VRSSpectatorManager's UpdateInput stage
        /// </summary>
        virtual public void UpdateInput() { }

        /// <summary>
        /// Is it ok if switch to this mode?
        /// </summary>
        /// <returns></returns>
        virtual public bool IsAvailable() { return true; }

        virtual public void OnEnterMode() { }
        virtual public void OnLeaveMode() { }
        virtual public void Update(bool reset) { }
        virtual public void OnDisable() { }
    }

    public class VRSSpectatorManager : MonoBehaviour
    {
        static string TAG = "SpectatorManager";

        public static VRSSpectatorManager Instance { get; private set; }

        static readonly InputUsage usagesCtrl = InputUsage.SecondaryButton | InputUsage.Primary2DAxis;
        private static readonly VRSController ctrlR = new VRSController(false, usagesCtrl);

        public Vector2 debugJoystick = new Vector2(0, 0);
        public bool debugBtn = false;

        [Tooltip("Spectator camera indicator")]
        public GameObject scIndicator = null;
        public Pose scOriginalPose;

        public SpectatorMode spectatorMode = SpectatorMode.HMD;

        [Tooltip("This preview will show before HMD when switch mode.")]
        public GameObject previewBeforeHMDPrefab;
        GameObject previewBeforeHMD;

        /// <summary>
        /// The visible camera object
        /// </summary>
        Renderer previewMonitorRendererSCI;
        /// <summary>
        /// Preview before HMD
        /// </summary>
        Renderer previewMonitorRendererPBH;

        // This is shared from preview prefab
        Material previewMaterial;
        Material previewMaterialLocal;
        bool useLocalPreviewMaterial;
        Camera previewCamera;

        bool isRecordStarted = false;

        RenderTexture previewTexture;
        bool isPreviewCameraNeedRender;

        public float fov = 60;

        // avatar hide (Hair, face), SpectatorHidden
        public LayerMask visibleLayers = -1 & ~(1 << 24);
        public Renderer previewMonitor;
        public Material capturedMat;
        public Material whiteMat;
        public GameObject textGO;
        public GameObject textRecordingGO;
        public UnityEngine.UI.Text onScreenText;

        private SpectatorMode prevSpectatorMode = SpectatorMode.Headset;

        private void Awake()
        {
            modeBehaviours[0] = new HMDMode(this, SpectatorMode.HMD);
            modeBehaviours[1] = new TrackerMode(this, SpectatorMode.Tracker);
            modeBehaviours[2] = new FollowMode(this, SpectatorMode.FollowPose);
            modeBehaviours[3] = new FixedMode(this, SpectatorMode.FixedPose);
            modeBehaviours[4] = new ManualMode(this, SpectatorMode.ManualPose);
            modeBehaviour = modeBehaviours[0];
        }

        private void OnEnable()
        {
            StartCoroutine(SetCallback());
            CheckPreviewMaterialLocal();
        }

        private IEnumerator SetCallback()
        {
            yield return new WaitUntil(() => SpectatorCameraBased.Instance != null);
            var sh = SpectatorCameraBased.Instance;
            sh.SpectatorCamera.cullingMask = visibleLayers;
            sh.OnSpectatorStart += OnSpectatorStart;
            sh.OnSpectatorStop += OnSpectatorStop;
            sh.SpectatorCamera.nearClipPlane = 0.001f;
        }

        private void OnDisable()
        {
            modeBehaviour.OnDisable();

            if (previewMaterialLocal != null)
                Destroy(previewMaterialLocal);
            previewMaterialLocal = null;

            if (previewTexture != null)
                previewTexture.Release();
            previewTexture = null;

            if (previewCamera != null)
            {
                Destroy(previewCamera.gameObject);
            }


            var sh = SpectatorCameraBased.Instance;
            if (sh)
            {
                sh.OnSpectatorStart -= OnSpectatorStart;
                sh.OnSpectatorStop -= OnSpectatorStop;
            }
        }

        private void OnSpectatorStart()
        {
            textRecordingGO.SetActive(false);
            isRecordStarted = true;
            UpdatePreviewMaterial();
        }

        private void OnSpectatorStop()
        {
            isRecordStarted = false;
            UpdatePreviewMaterial();
            textGO.SetActive(false);
            ResetLocalPreviewTexture();
        }

        private void ResetLocalPreviewTexture()
        {
            if (previewTexture != null)
            {
                RenderTexture.active = previewTexture;
                GL.Clear(true, true, Color.white);
            }
        }

        private void CheckPreviewTexture()
        {
            if (previewTexture == null)
            {
                previewTexture = new RenderTexture(480, 270, 32);
            }

            if (!previewTexture.IsCreated())
            {
                previewTexture.Create();
            }
        }

        readonly Timer timerRenderPreviewCamera = new Timer(0.05f); // 20FPS

        void Start()
        {
            if (Instance == null)
                Instance = this;
            else
                enabled = false;

            if (scIndicator == null) return;
            scOriginalPose.position = scIndicator.transform.position;
            scOriginalPose.rotation = scIndicator.transform.rotation;
            timerRenderPreviewCamera.Set();

            //CheckPreviewMaterialLocal();
        }

        void CheckPreviewRenderer()
        {
            var preview = RecursiveFind(scIndicator.transform, "PreviewMonitor");
            if (preview != null)
            {
                var mr = preview.GetComponent<MeshRenderer>();
                if (mr)
                {
                    if (previewMaterial == null)
                        previewMaterial = mr.sharedMaterial;
                    previewMonitorRendererSCI = mr;
                }
            }

            if (previewBeforeHMD == null) return; // to prevent null exception first time
            preview = RecursiveFind(previewBeforeHMD.transform, "PreviewMonitor");
            if (preview != null)
            {
                var mr = preview.GetComponent<MeshRenderer>();
                if (mr)
                {
                    if (previewMaterial == null)
                        previewMaterial = mr.sharedMaterial;
                    previewMonitorRendererPBH = mr;
                }
            }
        }

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

        readonly Button debugBtnSecR = new Button();
        readonly Axis2D debugAxis = new Axis2D();

        List<InputDevice> inputDevices = new List<InputDevice>();

        private void UpdateInput()
        {
            if (!ctrlR.dev.isValid) InputDeviceTools.GetController(ctrlR, inputDevices);
            if (ctrlR.dev.isValid) InputDeviceTools.UpdateController(ctrlR, usagesCtrl);
            //if (!ctrlL.dev.isValid) InputDeviceTools.GetController(ctrlL, inputDevices);
            //if (ctrlL.dev.isValid) InputDeviceTools.UpdateController(ctrlL);
            //if (!hmd.dev.isValid) InputDeviceTools.GetHMD(hmd, inputDevices);
            //if (hmd.dev.isValid) InputDeviceTools.UpdateHMD(hmd);
            modeBehaviour.UpdateInput();

            // You can use update the vector2 in editor to simulate joystick move
            debugAxis.Set(debugJoystick);

            // You can update the bool in editor to simulate button press
            debugBtnSecR.Set(debugBtn);
        }


        private void SetSpectatorIndicator(Vector3 pos, Quaternion rot)
        {
            if (scIndicator == null) return;
            scIndicator.transform.position = pos;
            scIndicator.transform.rotation = rot;
        }

        private void ResetSpectatorIndicator()
        {
            if (scIndicator == null) return;
            scIndicator.transform.position = scOriginalPose.position;
            scIndicator.transform.rotation = scOriginalPose.rotation;
        }

        private bool GetSpectatorIndicator(out Vector3 pos, out Quaternion rot)
        {
            if (scIndicator == null)
            {
                pos = Vector3.zero;
                rot = Quaternion.identity;
                return false;
            }
            pos = scIndicator.transform.position;
            rot = scIndicator.transform.rotation;
            return true;
        }

        private Transform GetRig()
        {
            if (VIVERig.Instance != null) return VIVERig.Instance.transform;
            else if (Camera.main != null) return Camera.main.transform.parent;
            else return null;
        }

        Transform RecursiveFind(Transform root, string name)
        {
            if (root.childCount == 0)
                return null;
            Transform result = root.Find(name);
            if (result != null) return result;

            for (int i = 0; i < root.childCount; i++)
            {
                result = RecursiveFind(root.GetChild(i), name);
                if (result != null)
                    break;
            }

            return result;
        }

        void CheckPreviewCamera()
        {
            if (scIndicator == null) return;
            CheckPreviewTexture();
            if (previewCamera == null)
            {
                var prevCamObj = new GameObject("PreviewCamera");
                previewCamera = prevCamObj.AddComponent<Camera>();
                previewCamera.targetTexture = previewTexture;

                prevCamObj.transform.SetParent(scIndicator.transform, false);
                prevCamObj.transform.localPosition = Vector3.zero;
                prevCamObj.transform.localRotation = Quaternion.identity;
                previewCamera.fieldOfView = 60;
                previewCamera.enabled = false;
                previewCamera.stereoTargetEye = StereoTargetEyeMask.None;
            }
        }

        void CheckPreviewMaterialLocal()
        {
            CheckPreviewTexture();
            CheckPreviewRenderer();
            if ((previewMonitorRendererSCI != null || previewMonitorRendererPBH != null) && previewMaterialLocal == null)
                previewMaterialLocal = new Material(previewMaterial);
            previewMaterialLocal.SetTexture("_MainTex", previewTexture);
        }

        void UpdatePreviewMaterial()
        {
            if (textGO.activeSelf) return;

            if (isRecordStarted && useLocalPreviewMaterial)
            {
                if (previewMonitorRendererSCI)
                    previewMonitorRendererSCI.sharedMaterial = previewMaterial;

                if (previewMonitorRendererPBH)
                    previewMonitorRendererPBH.sharedMaterial = previewMaterial;

                useLocalPreviewMaterial = false;
                return;
            }

            if (!isRecordStarted && !useLocalPreviewMaterial)
            {
                CheckPreviewMaterialLocal();
                if (previewMonitorRendererSCI)
                    previewMonitorRendererSCI.sharedMaterial = previewMaterialLocal;

                if (previewMonitorRendererPBH)
                    previewMonitorRendererPBH.sharedMaterial = previewMaterialLocal;

                useLocalPreviewMaterial = true;
                return;
            }
        }

        Timer timerPreviewBeforeHMD = new Timer(1);

        /// <summary>
        /// if the preview before HMD is showing
        /// </summary>
        bool isShowingPreviewBeforeHMD = false;

        /// <summary>
        /// Show the preview in front of HMD.
        /// </summary>
        /// <returns></returns>
        IEnumerator ShowPreviewBeforeHMD()
        {
            timerPreviewBeforeHMD.Set();

            if (previewBeforeHMD == null)
                previewBeforeHMD = Instantiate(previewBeforeHMDPrefab);
            if (previewBeforeHMD == null) yield break;

            if (!previewBeforeHMD.activeInHierarchy)
                previewBeforeHMD.SetActive(true);

            // Set latest mode text
            Transform ModeDesc = RecursiveFind(previewBeforeHMD.transform, "ModeDesc");
            if (ModeDesc != null)
            {
                var text = ModeDesc.GetComponent<UnityEngine.UI.Text>();
                if (text != null)
                    text.text = spectatorMode.ToString(); // + "Mode";
            }

            // Not to move already shown preview pose
            if (isShowingPreviewBeforeHMD)
                yield break;
            isShowingPreviewBeforeHMD = true;

            bool hasCamera = GetCameraYawPose(out Vector3 pos, out Quaternion rot);
            if (!hasCamera) yield break;

            var newPos = pos + rot * Vector3.forward * 0.75f;
            var newRot = rot * Quaternion.Euler(0, 180, 0);
            previewBeforeHMD.transform.position = newPos;
            previewBeforeHMD.transform.rotation = newRot;

            UpdatePreviewMaterial();

            bool usePreview = !isRecordStarted;
            if (usePreview)
                CheckPreviewCamera();

            while (!timerPreviewBeforeHMD.Check())
            {
                if (usePreview)
                    isPreviewCameraNeedRender = true;
                yield return null;
            }

            isPreviewCameraNeedRender = false;

            previewBeforeHMD.SetActive(false);

            isShowingPreviewBeforeHMD = false;
        }

        /// <summary>
        /// if long press, cancel the function key(B)'s button up event.
        /// </summary>
        bool cancelLastIsUp = false;

        /// <summary>
        /// Check if next mode is valid.  Switch to next mode.
        /// </summary>
        public void ChangeToNextMode(SpectatorMode? mode = null)
        {
            needChangeMode = false;

            // First click show preview.  Only allowed to change mode in preview show up.
            //if (isShowingPreviewBeforeHMD)
            {
                int max = (int)SpectatorMode.ManualPose + 1;
                var prv = spectatorMode;
                int cur = (int)prv;
                do
                {
                    cur = (mode == null) ? cur + 1 : (int)mode;  // aka. next
                    cur = cur % max;
                    //Log.d(TAG, "Current Mode is " + prv + " Next Mode is " + cur);
                    if (!modeBehaviours[cur].IsAvailable() && (SpectatorMode)cur == SpectatorMode.Tracker)
                    {
                        if (previewMonitor)
                        {
                            previewMonitor.sharedMaterial = whiteMat;
                        }
                        if (!isRecordStarted) textRecordingGO.SetActive(false);
                        textGO.SetActive(true);
                    }
                    else
                    {
                        if (previewMonitor)
                        {
                            previewMonitor.sharedMaterial = previewMaterial;
                        }
                        textGO.SetActive(false);
                        if (!isRecordStarted) textRecordingGO.SetActive(true);
                    }
                    //if (!modeBehaviours[cur].IsAvailable())
                    //    continue;
                    spectatorMode = (SpectatorMode)cur;
                    modeBehaviour = modeBehaviours[cur];
                    break;
                } while (true);

                modeBehaviours[(int)prv].OnLeaveMode();
                modeBehaviours[(int)spectatorMode].OnEnterMode();
                if (SpectatorCameraBased.Instance != null)
                {
                    SpectatorCameraBased.Instance.SetViewFromHmd(spectatorMode == SpectatorMode.HMD || spectatorMode == SpectatorMode.Headset);
                }
            }

            StartCoroutine(ShowPreviewBeforeHMD());
        }


        bool needChangeMode = false;
        public void OnModeDisabled()
        {
            needChangeMode = true;
        }

        private void Update()
        {
            UpdateInput();
            var sh = SpectatorCameraBased.Instance;
            if (sh == null) return;

            bool reset = false;
            // Change Spectator Mode
            if (!cancelLastIsUp && ctrlR.btnSec.IsLongPressed())
            {
                cancelLastIsUp = true;
                reset = true;
            }

            if (TutorialManager.Instance != null && !TutorialManager.Instance.IsInFullTutorial()
                && !cancelLastIsUp && (ctrlR.btnSec.IsUp || debugBtnSecR.IsUp) || needChangeMode)
                ChangeToNextMode();

            if (cancelLastIsUp && ctrlR.btnSec.IsUp)
            {
                cancelLastIsUp = false;
            }

            modeBehaviour.Update(reset);

            if (isPreviewCameraNeedRender)
            {
                if (timerRenderPreviewCamera.Check())
                {
                    previewCamera?.Render();
                    timerRenderPreviewCamera.Set();
                }
                isPreviewCameraNeedRender = false;
            }

            switch (spectatorMode)
            {
                case SpectatorMode.ManualPose:
                case SpectatorMode.FollowPose:
                case SpectatorMode.Tracker:
                case SpectatorMode.FixedPose:
                    {
                        bool hasUpdate = ctrlR.axisJoy.Up.IsPressed || ctrlR.axisJoy.Down.IsPressed;
                        if (ctrlR.axisJoy.Up.IsPressed)
                            fov -= 30 * Time.unscaledDeltaTime;
                        if (ctrlR.axisJoy.Down.IsPressed)
                            fov += 30 * Time.unscaledDeltaTime;
                        if (hasUpdate)
                        {
                            fov = Mathf.Clamp(fov, 1, 179);
                            sh.SpectatorCamera.fieldOfView = fov;
                            StartCoroutine(ShowPreviewBeforeHMD());
                        }
                    }
                    break;
            }

            if (prevSpectatorMode != spectatorMode)
            {
                onScreenText.text = spectatorMode.ToString();
            }

            prevSpectatorMode = spectatorMode;
        }

        public void RegisterFixedPose(VRSSpectatorFixedPose fp)
        {
            ((FixedMode)modeBehaviours[(int)SpectatorMode.FixedPose])?.RegisterFixedPose(fp);
        }

        public void UnregisterFixedPose(VRSSpectatorFixedPose fp)
        {
            ((FixedMode)modeBehaviours[(int)SpectatorMode.FixedPose])?.UnregisterFixedPose(fp);
        }

        public bool IsTrackerAvailable()
        {
            return ((TrackerMode)modeBehaviours[(int)SpectatorMode.Tracker]).IsAvailable();
        }

        public int GetCurrentTracker()
        {
            return ((TrackerMode)modeBehaviours[(int)SpectatorMode.Tracker]).GetCurrentTracker();
        }

        public void SetTracker(uint tid)
        {
            ((TrackerMode)modeBehaviours[(int)SpectatorMode.Tracker]).SetTracker(tid);
        }

        public bool IsRecordingStarted()
        {
            return isRecordStarted;
        }

        public void EnableRecordingText()
        {
            if (!isRecordStarted && !textGO.activeSelf) textRecordingGO.SetActive(true);
        }

        public void DisableRecordingText()
        {
            textRecordingGO.SetActive(false);
        }

        #region nested classes
        public class HMDMode : SpectatorModeBehaviour
        {
            public HMDMode(VRSSpectatorManager manager, SpectatorMode mode) : base(manager, mode) { }

            public override void OnEnterMode()
            {
                var sh = SpectatorCameraBased.Instance;
                if (sh == null) return;

                sh.SpectatorCamera.cullingMask = manager.visibleLayers;
                sh.SpectatorCamera.transform.SetPositionAndRotation(sh.MainCamera.transform.position, sh.MainCamera.transform.rotation);
                sh.SpectatorCamera.fieldOfView = 60.0f;
                manager.ResetSpectatorIndicator();
            }
        }

        public class TrackerMode : SpectatorModeBehaviour
        {
            static readonly InputUsage usagesTracker = InputUsage.Position | InputUsage.Rotation;
            private static readonly VRSStudio.Common.Input.Tracker tracker = new VRSStudio.Common.Input.Tracker((VRSStudio.Common.Input.Tracker.TrackerID)(-1), usagesTracker);

            public TrackerMode(VRSSpectatorManager manager, SpectatorMode mode) : base(manager, mode) { }
            List<uint> requestedTrackerId = new List<uint>();

            public override bool IsAvailable()
            {
                UpdateInput();
                return tracker.dev.isValid;
            }

            public override void OnEnterMode()
            {
                var sh = SpectatorCameraBased.Instance;
                if (sh == null) return;

                sh.SpectatorCamera.cullingMask = manager.visibleLayers;
                sh.SpectatorCamera.fieldOfView = manager.fov;
                ReleaseTracker();
            }

            public override void OnLeaveMode()
            {
                ReleaseTracker();
            }

            public override void UpdateInput()
            {
                if (!tracker.dev.isValid) GetTracker();
                if (requestedTrackerId.Count > 0 && tracker.dev.isValid) InputDeviceTools.UpdateXRTrackingDevice(tracker, usagesTracker);
            }

            public override void Update(bool reset)
            {
                var sh = SpectatorCameraBased.Instance;
                if (sh == null || !VRSSpectatorManager.Instance.IsRecordingStarted()) return;

                bool disable = false;
                if (requestedTrackerId.Count == 0 || !tracker.dev.isValid)
                {
                    Log.d(TAG, "No Tracker");
                    //disable = true;
                }

                var rig = manager.GetRig();
                //if (!rig) disable = true;
                if (disable)
                {
                    manager.ResetSpectatorIndicator();
                    if (manager.spectatorMode == Mode)
                        manager.OnModeDisabled();
                    return;
                }

                // Use rig, make tracker pose to world space
                var pos = rig.TransformPoint(tracker.position);
                // Let Tracker's upper face be camera's forward.
                var up = Quaternion.Euler(-90, 180, 0);
                var rot = rig.rotation * tracker.rotation * up;
                sh.SpectatorCamera.transform.SetPositionAndRotation(pos, rot);
                manager.SetSpectatorIndicator(pos, rot);
            }

            /// <summary>
            /// Get a free tracker id to use.
            /// </summary>
            private void GetTracker()
            {
                var vrstm = VRSTrackerManager.Instance;
                if (!vrstm)
                {
                    Log.d(TAG, "No VRSTrackerManager");
                    return;
                }
                Profiler.BeginSample("GetFreeTracker");

                // Release old tracker.  Avoid the tracker is occupied by self and not able to get free id.
                ReleaseTracker();

                if (vrstm.GetFreeIdList(out uint[] freeIds))
                {
                    uint tid = (uint)tracker.trackerId;
                    bool exists = Array.Exists(freeIds, x => x == tid);
                    if (!exists)
                    {
                        tid = freeIds[0];
                    }
                    UpdateTracker(tid);
                }
                else
                {
                    if (VRSSpectatorManager.Instance.IsRecordingStarted()) Log.d(TAG, "No free tracker");
                }
                Profiler.EndSample();
            }

            private void UpdateTracker(uint tid)
            {
                if (VRSTrackerManager.Instance.GetInputDevice(tid, out var dev))
                {
                    var tids = new List<uint>() { tid };
                    if (VRSTrackerManager.Instance.RequestTrackers(tids, OnTrackerReallocate))
                        requestedTrackerId.Add(tid);
                    tracker.dev = dev;
                    tracker.trackerId = (Common.Input.Tracker.TrackerID)(int)tid;
                }
                else
                    Log.d(TAG, "No input device for tracker id " + tid);
            }

            public int GetCurrentTracker()
            {
                return (int)tracker.trackerId;
            }

            public void SetTracker(uint tid)
            {
                var vrstm = VRSTrackerManager.Instance;
                if (!vrstm)
                {
                    Log.d(TAG, "No VRSTrackerManager");
                    return;
                }

                if (vrstm.GetInputDevice(tid, out var dev))
                {
                    tracker.dev = dev;
                    tracker.trackerId = (Common.Input.Tracker.TrackerID)(int)tid;
                }
                else Log.d(TAG, "No input device for tracker id " + tid);
            }

            void ReleaseTracker()
            {
                tracker.trackerId = (Common.Input.Tracker.TrackerID)(-1);
                tracker.dev = default;

                if (requestedTrackerId.Count == 0) { return; }
                var vrstm = VRSTrackerManager.Instance;
                if (vrstm)
                    vrstm.FreeTrackers(requestedTrackerId, OnTrackerReallocate);
                requestedTrackerId.Clear();
            }

            private void OnTrackerReallocate(List<uint> tids)
            {
                ReleaseTracker();
            }

            public override void OnDisable()
            {
                ReleaseTracker();
            }
        }

        public class FollowMode : SpectatorModeBehaviour
        {
            // Predefined follow poses
            public List<Pose> followPoses = new List<Pose>();
            private Pose currentFollowPose;
            private int followPoseIdx = -1;

            public FollowMode(VRSSpectatorManager manager, SpectatorMode mode) : base(manager, mode) { }
            public override void OnEnterMode()
            {
                var sh = SpectatorCameraBased.Instance;
                if (sh == null) return;

                sh.SpectatorCamera.cullingMask = manager.visibleLayers;
                sh.SpectatorCamera.fieldOfView = manager.fov;
            }

            public override void Update(bool reset)
            {
                var fpc = followPoses.Count;
                if (fpc <= 0)
                {
                    followPoses.Add(new Pose(Vector3.forward, Quaternion.Euler(0, 180, 0)));
                    fpc += 1;
                }

                if (fpc > 0)
                {
                    bool idxChanged = false;
                    if (followPoseIdx == -1 || reset)
                    {
                        // Initialize
                        followPoseIdx = 0;
                        idxChanged = true;
                    }

                    if (ctrlR.axisJoy.Left.IsDown || manager.debugAxis.Left.IsDown)
                    {
                        if (--followPoseIdx < 0)
                            followPoseIdx = fpc - 1;
                        if (followPoseIdx < fpc && followPoseIdx >= 0)
                            idxChanged = true;
                    }

                    if (ctrlR.axisJoy.Right.IsDown || manager.debugAxis.Right.IsDown)
                    {
                        if (++followPoseIdx >= fpc)
                            followPoseIdx = 0;
                        if (followPoseIdx < fpc && followPoseIdx >= 0)
                            idxChanged = true;
                    }

                    if (idxChanged)
                    {
                        currentFollowPose = followPoses[followPoseIdx];
                        manager.StartCoroutine(manager.ShowPreviewBeforeHMD());
                    }

                    UpdateFollowPose();
                }
            }

            Vector3 posT = Vector3.zero;
            Quaternion rotT = Quaternion.identity;

            private void UpdateFollowPose()
            {
                var sh = SpectatorCameraBased.Instance;
                if (sh == null) return;

                var hasCamera = manager.GetCameraYawPose(out Vector3 pos, out Quaternion rot);
                if (!hasCamera) return;

                var newPos = pos + rot * currentFollowPose.position;
                var newRot = rot * currentFollowPose.rotation;

                posT = Vector3.Lerp(posT, newPos, 0.5f);
                rotT = Quaternion.Lerp(rotT, newRot, 0.5f);
                sh.SpectatorCamera.transform.SetPositionAndRotation(posT, rotT);
                manager.SetSpectatorIndicator(posT, rotT);
            }
        }

        public class FixedMode : SpectatorModeBehaviour
        {
            private readonly List<Transform> fixedPoses = new List<Transform>();
            private int fixedPoseIdx = -1;
            private Transform currentFixedPose;

            public FixedMode(VRSSpectatorManager manager, SpectatorMode mode) : base(manager, mode) { }

            public override bool IsAvailable()
            {
                return fixedPoses.Count != 0;
            }

            public override void OnEnterMode()
            {
                var sh = SpectatorCameraBased.Instance;
                if (sh == null) return;

                sh.SpectatorCamera.cullingMask = manager.visibleLayers;
                sh.SpectatorCamera.fieldOfView = manager.fov;
            }

            public override void Update(bool reset)
            {
                if (manager.spectatorMode == Mode)
                    manager.OnModeDisabled();
                return;

                var fpc = fixedPoses.Count;
                if (fpc > 0)
                {
                    bool idxChanged = false;
                    if (fixedPoseIdx == -1 || reset)
                    {
                        fixedPoseIdx = 0;
                        idxChanged = true;
                    }

                    if (ctrlR.axisJoy.Left.IsDown || manager.debugAxis.Left.IsDown)
                    {
                        if (--fixedPoseIdx < 0)
                            fixedPoseIdx = fpc - 1;
                        if (fixedPoseIdx < fpc && fixedPoseIdx >= 0)
                            idxChanged = true;
                    }

                    if (ctrlR.axisJoy.Right.IsDown || manager.debugAxis.Right.IsDown)
                    {
                        if (++fixedPoseIdx >= fpc)
                            fixedPoseIdx = 0;
                        if (fixedPoseIdx < fpc && fixedPoseIdx >= 0)
                            idxChanged = true;
                    }

                    if (idxChanged)
                    {
                        currentFixedPose = fixedPoses[fixedPoseIdx];
                        manager.StartCoroutine(manager.ShowPreviewBeforeHMD());
                    }

                    UpdateFixedPose();
                }
            }

            private void UpdateFixedPose()
            {
                var sh = SpectatorCameraBased.Instance;
                if (sh == null) return;

                sh.SpectatorCamera.transform.SetPositionAndRotation(currentFixedPose.position, currentFixedPose.rotation);
                manager.SetSpectatorIndicator(currentFixedPose.position, currentFixedPose.rotation);
            }

            public void RegisterFixedPose(VRSSpectatorFixedPose fp)
            {
                if (fp == null || fp.transform == null) return;
                fixedPoses.Add(fp.transform);
            }

            public void UnregisterFixedPose(VRSSpectatorFixedPose fp)
            {
                if (fp == null || fp.transform == null) return;
                int count = fixedPoses.Count;
                int idx = fixedPoses.IndexOf(fp.transform);

                // Not exist
                if (idx < 0) return;

                if (count > 1)
                {
                    if (idx <= fixedPoseIdx)
                    {
                        fixedPoseIdx--;
                        currentFixedPose = fixedPoses[fixedPoseIdx];
                    }
                }
                else
                {
                    // Final one
                    fixedPoseIdx = -1;
                    if (manager.spectatorMode == Mode)
                        manager.OnModeDisabled();
                }
                fixedPoses.Remove(fp.transform);
            }
        }

        public class ManualMode : SpectatorModeBehaviour
        {
            // In world space
            private Pose manualPose = new Pose(Vector3.up * -9998, Quaternion.Euler(0, 180, 0));

            public ManualMode(VRSSpectatorManager manager, SpectatorMode mode) : base(manager, mode) { }
            public override void OnLeaveMode()
            {
                if (manager.scIndicator != null)
                {
                    // If leave Manual mode, disable collider avoid effect the other rigidbody;
                    var collider = manager.scIndicator.gameObject.GetComponent<Collider>();
                    if (collider) collider.enabled = false;
                }
            }
            public override void OnEnterMode()
            {
                if (manager.scIndicator != null)
                {
                    var collider = manager.scIndicator.gameObject.GetComponent<Collider>();
                    if (collider) collider.enabled = true;
                }
                var sh = SpectatorCameraBased.Instance;
                if (sh == null) return;

                sh.SpectatorCamera.cullingMask = manager.visibleLayers;
                sh.SpectatorCamera.fieldOfView = manager.fov;
                if (Mathf.Approximately(manualPose.position.y, -9998f))
                    ResetManualPose();
                manager.SetSpectatorIndicator(manualPose.position, manualPose.rotation);
            }

            public override void Update(bool reset)
            {
                if (reset)
                    ResetManualPose();
                UpdateManualPose();
            }

            private void ResetManualPose()
            {
                var sh = SpectatorCameraBased.Instance;
                if (sh == null) return;

                var hasCamera = manager.GetCameraYawPose(out Vector3 pos, out Quaternion rot);
                if (!hasCamera) return;

                // Easy to Grab
                var newPos = pos + rot * (Vector3.forward * 0.4f + Vector3.up * -0.2f);
                var newRot = rot * Quaternion.Euler(0, 180, 0);

                manager.SetSpectatorIndicator(newPos, newRot);
                manualPose.position = newPos;
                manualPose.rotation = newRot;
            }


            private void UpdateManualPose()
            {
                var sh = SpectatorCameraBased.Instance;
                if (sh == null) return;

                if (manager.GetSpectatorIndicator(out var pos, out var rot))
                {
                    if (manualPose.position != pos || manualPose.rotation != rot)
                        manager.isPreviewCameraNeedRender = true;
                    manualPose.position = pos;
                    manualPose.rotation = rot;
                }
                sh.SpectatorCamera.transform.SetPositionAndRotation(manualPose.position, manualPose.rotation);
            }
        }

        SpectatorModeBehaviour[] modeBehaviours = new SpectatorModeBehaviour[5];
        SpectatorModeBehaviour modeBehaviour = null;
        #endregion
    }
}