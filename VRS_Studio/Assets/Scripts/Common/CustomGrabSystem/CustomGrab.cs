using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using VIVE.OpenXR;
using VRSStudio.Common.Input;
using VRSController = VRSStudio.Common.Input.Controller;

namespace VRSStudio.Input.CustomGrab
{
    public class CustomGrab : MonoBehaviour
    {
        public static string TAG = "CustomGrab";

        public class GrabController : VRSController
        {
            public Vector3 velocityW;
            public Vector3 angularVelocityW;

            public GrabController(bool isLeft) : base(isLeft) { }
            public GrabController(bool isLeft, InputUsage usages) : base(isLeft, usages) { }
        }

        // trigger and grip are used in CustomGrabber
        static readonly InputUsage ctrlUsages = InputUsage.TriggerButton | InputUsage.GripButton | InputUsage.Velocity | InputUsage.AngularVelocity | InputUsage.Position | InputUsage.Rotation;
        static readonly InputUsage handUsages = InputUsage.Velocity | InputUsage.AngularVelocity;
        static readonly InputUsage hmdUsages = InputUsage.Position;

        public readonly HMD hmd = new HMD(hmdUsages);
        public readonly GrabController ctrlL = new GrabController(true, ctrlUsages);
        public readonly GrabController ctrlR = new GrabController(false, ctrlUsages);
        public readonly Common.Input.Hand handL = new Common.Input.Hand(true, handUsages);
        public readonly Common.Input.Hand handR = new Common.Input.Hand(false, handUsages);

        public Transform rig;
        [SerializeField]
        GameObject grabberPrefabL;
        [SerializeField]
        GameObject grabberPrefabR;

        public GameObject indicatorL;
        public GameObject indicatorR;

        [SerializeField]
        private UnityEngine.InputSystem.InputActionProperty leftGrabberPose;
        [SerializeField]
        private UnityEngine.InputSystem.InputActionProperty rightGrabberPose;

        public static CustomGrab Instance { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            if (Instance == null)
                Instance = this;
            else
                enabled = false;

            if (leftGrabberPose != null && leftGrabberPose.action != null)
            {
                leftGrabberPose.action.Enable();
            }
            if (rightGrabberPose != null && rightGrabberPose.action != null)
            {
                rightGrabberPose.action.Enable();
            }
        }

        private void UpdateCameraRig()
        {
            if (rig != null) return;

            //if (VRSStudioCameraRig.Instance != null)
            //    rig = VRSStudioCameraRig.Instance.ViveCameraRig.transform;
            if (rig == null && VIVERig.Instance != null)
                rig = VIVERig.Instance.transform;
            if (rig == null && Camera.main != null)
                rig = Camera.main.transform.parent;
        }

        const int InverseX = -1;
        const int InverseY = -1;
        const int InverseZ = -1;
        Quaternion DP2AngularVelocityRotation = Quaternion.Euler(-27.5f, 0, 0);
        private Vector3 DP2Problem(Vector3 angularVelocity)
        {
            angularVelocity.x = InverseX * angularVelocity.x;
            angularVelocity.y = InverseY * angularVelocity.y;
            angularVelocity.z = InverseZ * angularVelocity.z;
            angularVelocity = DP2AngularVelocityRotation * angularVelocity;
            return angularVelocity;
        }

        readonly List<InputDevice> inputDevices = new List<InputDevice>();
        private void UpdateInputDevices()
        {
            hmd.position = Vector3.zero;

            if (!hmd.dev.isValid) InputDeviceTools.GetHMD(hmd, inputDevices);
            if (hmd.dev.isValid) InputDeviceTools.UpdateHMD(hmd);

            if (!ctrlL.dev.isValid) InputDeviceTools.GetController(ctrlL, inputDevices);
            if (ctrlL.dev.isValid)
            {
                InputDeviceTools.UpdateController(ctrlL);

#if UNITY_EDITOR
                ctrlL.angularVelocity = DP2Problem(ctrlL.angularVelocity);
#endif
                var m = rig.localToWorldMatrix;
                ctrlL.velocityW = m * ctrlL.velocity;
                ctrlL.angularVelocityW = m * (ctrlL.rotation * ctrlL.angularVelocity);
            }

            if (!ctrlR.dev.isValid) InputDeviceTools.GetController(ctrlR, inputDevices);
            if (ctrlR.dev.isValid)
            {
                InputDeviceTools.UpdateController(ctrlR);

#if UNITY_EDITOR
                ctrlR.angularVelocity = DP2Problem(ctrlR.angularVelocity);
#endif
                var m = rig.localToWorldMatrix;
                ctrlR.velocityW = m * ctrlR.velocity;
                ctrlR.angularVelocityW = m * (ctrlR.rotation * ctrlR.angularVelocity);

                //Log.d(TAG, Log.CSB.AppendVector3("velocity", velocity).Append(" ").AppendVector3("angularV", angularVelocity), true);
            }

            if (!handR.dev.isValid) InputDeviceTools.GetHand(handR, inputDevices);
            if (handR.dev.isValid)
            {
                InputDeviceTools.UpdateHand(handR);

                //Log.d(TAG, Log.CSB.Append("dev=HR ").AppendVector3("velocity", handR.velocity).Append(" ").AppendVector3("angularV", handR.angularVelocity).Append(" ").AppendVector3("pos", handR.position), true);
            }

            if (!handL.dev.isValid) InputDeviceTools.GetHand(handL, inputDevices);
            if (handL.dev.isValid)
            {
                InputDeviceTools.UpdateHand(handL);
            }
        }

        void CheckCollider(ref InputDevice id, ref GameObject obj)
        {
            if (rig == null) return;
            if (id.isValid && obj == null)
            {
                if (obj == null)
                {
                    if (id.characteristics.HasFlag(InputDeviceCharacteristics.Left))
                        obj = Instantiate(grabberPrefabL);
                    else if (id.characteristics.HasFlag(InputDeviceCharacteristics.Right))
                        obj = Instantiate(grabberPrefabR);
                    else
                        ;  // Neutral
                }
                else
                {
                    // TODO make local indicator
                }

                obj.transform.SetParent(rig, false);
                var collider = obj.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.isTrigger = true;
                }
                else
                {
                    // TODO if collider not exist
                }
            }
        }

        void UpdateIndicatorPose(GameObject obj, InputDevice id, Vector3 pos, Quaternion rot)
        {
            if (id.isValid && obj != null)
            {
                obj.transform.localPosition = pos;
                obj.transform.localRotation = rot;
            }
        }

        void UpdateCollider()
        {
            CheckCollider(ref ctrlL.dev, ref indicatorL);
            CheckCollider(ref ctrlR.dev, ref indicatorR);
            if (leftGrabberPose != null && leftGrabberPose.action != null)
            {
                var leftPose = leftGrabberPose.action.ReadValue<PoseState>();
                UpdateIndicatorPose(indicatorL, ctrlL.dev, leftPose.position, leftPose.rotation);
            }
            if (rightGrabberPose != null && rightGrabberPose.action != null)
            {
                var rightPose = rightGrabberPose.action.ReadValue<PoseState>();
                UpdateIndicatorPose(indicatorR, ctrlR.dev, rightPose.position, rightPose.rotation);
            }
        }

        float lastUpdateTime;

        public void UpdateInput()
        {
            if (Mathf.Approximately(Time.unscaledTime, lastUpdateTime))
                return;
            lastUpdateTime = Time.unscaledTime;

            UpdateCameraRig();
            if (rig == null) return;
            UpdateInputDevices();
            UpdateCollider();
        }

        public void Update()
        {
            UpdateInput();
        }

        private void OnDisable()
        {
            if (indicatorL)
            {
                Destroy(indicatorL);
                indicatorL = null;
            }
            if (indicatorR)
            {
                Destroy(indicatorR);
                indicatorR = null;
            }
        }
    }
}