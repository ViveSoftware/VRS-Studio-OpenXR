using HTC.UnityPlugin.Vive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace VIVE.OpenXR.Toolkits.RealisticHandInteraction
{
    public class GrabJointUpdater : MonoBehaviour
    {
        private const int k_JointCount = (int)JointType.Count;
        private const int k_RootId = (int)JointType.Wrist;

        [SerializeField]
        private Handedness m_Handedness;
        private bool isLeft => m_Handedness == Handedness.Left;
        private HandPose handPose = null;
        [SerializeField]
        private Transform[] m_HandJoints = new Transform[k_JointCount];
        [SerializeField]
        private List<Component> m_DisableComponents = new List<Component>();
        private VivePoseTracker tracker = null;

        private bool isGrabbing = false;
        private HandGrabInteractable handGrabbable = null;
        private Quaternion[] grabJointsRotation = new Quaternion[k_JointCount];

        private void Awake()
        {
            bool empty = m_HandJoints.Any(x => x == null);
            if (empty)
            {
                ClearJoints();
                FindJoints();
            }
        }

        private void Update()
        {
            if (!IsUpdateAllowed()) { return; }

            Vector3 rootPosition = Vector3.zero;
            Quaternion rootRotation = Quaternion.identity;
            handPose.GetPosition((JointType)k_RootId, out rootPosition, local: true);
            handPose.GetRotation((JointType)k_RootId, out rootRotation, local: true);

            m_HandJoints[k_RootId].position = m_HandJoints[k_RootId].parent.position + rootPosition;
            m_HandJoints[k_RootId].rotation = m_HandJoints[k_RootId].parent.rotation * rootRotation;

            for (int i = 0; i < m_HandJoints.Length; i++)
            {
                if (m_HandJoints[i] == null || i == k_RootId) { continue; }

                Quaternion jointRotation = Quaternion.identity;
                handPose.GetRotation((JointType)i, out jointRotation, local: true);
                m_HandJoints[i].rotation = m_HandJoints[k_RootId].parent.rotation * jointRotation;
            }

            for (int i = 0; i < m_HandJoints.Length; i++)
            {
                if (m_HandJoints[i] == null) { continue; }

                Quaternion localRot = m_HandJoints[i].localRotation;
                Vector3 eulerAngles = localRot.eulerAngles;
                Quaternion newRot = Quaternion.Euler(-eulerAngles.x, eulerAngles.y, -eulerAngles.z);
                m_HandJoints[i].localRotation = newRot;
            }

            m_HandJoints[k_RootId].position = VIVERig.Instance.transform.position + VIVERig.Instance.transform.rotation * m_HandJoints[k_RootId].position;
            m_HandJoints[k_RootId].rotation = VIVERig.Instance.transform.rotation * m_HandJoints[k_RootId].rotation;

            if (handGrabbable != null)
            {
                for (int i = 0; i < m_HandJoints.Length; i++)
                {
                    if (i == k_RootId) { continue; }
                    m_HandJoints[i].rotation = m_HandJoints[i].parent.rotation * grabJointsRotation[i];
                }

                if (tracker)
                {
                    m_HandJoints[k_RootId].SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    Pose wristPose = new Pose(m_HandJoints[k_RootId].position, m_HandJoints[k_RootId].rotation);
                    handGrabbable.UpdatePositionAndRotation(wristPose);
                }

                if (handGrabbable.isContraint)
                {
                    Quaternion handRot = handGrabbable.transform.rotation * Quaternion.Inverse(handGrabbable.bestGrabPose.grabOffset.rotOffset);
                    Quaternion handRotDiff = handRot * Quaternion.Inverse(handGrabbable.bestGrabPose.grabOffset.sourceRotation);
                    Vector3 handPos = handGrabbable.transform.position - handRotDiff * handGrabbable.bestGrabPose.grabOffset.posOffset;
                    m_HandJoints[k_RootId].transform.SetPositionAndRotation(handPos, handRot);
                }
            }
        }

        public void OnBeginGrab(IGrabber grabber)
        {
            if (tracker == null) { FindVivePoseTracker(); }

            if (grabber.grabbable is HandGrabInteractable handGrabbable)
            {
                this.handGrabbable = handGrabbable;
                if (handGrabbable.bestGrabPose.recordedGrabRotations.Length == k_JointCount)
                {
                    grabJointsRotation = handGrabbable.bestGrabPose.recordedGrabRotations;
                    EnabledComponents(false);
                    if (m_HandJoints[k_RootId])
                    {
                        m_HandJoints[k_RootId].parent.localRotation = Quaternion.Euler(0f, 180f, 0f);
                        isGrabbing = true;
                    }
                }
            }
        }

        public void OnEndGrab(IGrabber grabber)
        {
            if (handGrabbable != null && !handGrabbable.isContraint &&
                handGrabbable.grabPoses.Contains(handGrabbable.bestGrabPose))
            {
                Pose wristPose = new Pose(m_HandJoints[k_RootId].position, m_HandJoints[k_RootId].rotation);
                handGrabbable.UpdatePositionAndRotation(wristPose);
            }
            isGrabbing = false;
            handGrabbable = null;

            if (m_HandJoints[k_RootId])
            {
                m_HandJoints[k_RootId].parent.localRotation = Quaternion.identity;
            }
            EnabledComponents(true);
        }

        private bool IsUpdateAllowed()
        {
            if (!isGrabbing) { return false; }

            handPose = HandPoseProvider.GetHandPose(isLeft ? HandPoseType.HAND_LEFT : HandPoseType.HAND_RIGHT);
            if (!handPose.IsTracked()) { return false; }

            return true;
        }

        private void EnabledComponents(bool enabled)
        {
            foreach (Component component in m_DisableComponents)
            {
                PropertyInfo enabledProperty = component.GetType().GetProperty("enabled");
                if (enabledProperty != null && enabledProperty.PropertyType == typeof(bool))
                {
                    enabledProperty.SetValue(component, enabled);
                }
            }
        }

        private bool FindVivePoseTracker()
        {
            if (tracker == null)
            {
                tracker = transform.GetComponentInParent<VivePoseTracker>();
            }
            return tracker != null;
        }

        #region JointInit

        #region Name Definition
        // The order of joint name MUST align with runtime's definition
        private readonly string[] JointsName = new string[]
        {
                "WaveBone_0",  // WVR_HandJoint_Palm = 0
				"WaveBone_1", // WVR_HandJoint_Wrist = 1
				"WaveBone_2", // WVR_HandJoint_Thumb_Joint0 = 2
				"WaveBone_3", // WVR_HandJoint_Thumb_Joint1 = 3
				"WaveBone_4", // WVR_HandJoint_Thumb_Joint2 = 4
				"WaveBone_5", // WVR_HandJoint_Thumb_Tip = 5
				"WaveBone_6", // WVR_HandJoint_Index_Joint0 = 6
				"WaveBone_7", // WVR_HandJoint_Index_Joint1 = 7
				"WaveBone_8", // WVR_HandJoint_Index_Joint2 = 8
				"WaveBone_9", // WVR_HandJoint_Index_Joint3 = 9
				"WaveBone_10", // WVR_HandJoint_Index_Tip = 10
				"WaveBone_11", // WVR_HandJoint_Middle_Joint0 = 11
				"WaveBone_12", // WVR_HandJoint_Middle_Joint1 = 12
				"WaveBone_13", // WVR_HandJoint_Middle_Joint2 = 13
				"WaveBone_14", // WVR_HandJoint_Middle_Joint3 = 14
				"WaveBone_15", // WVR_HandJoint_Middle_Tip = 15
				"WaveBone_16", // WVR_HandJoint_Ring_Joint0 = 16
				"WaveBone_17", // WVR_HandJoint_Ring_Joint1 = 17
				"WaveBone_18", // WVR_HandJoint_Ring_Joint2 = 18
				"WaveBone_19", // WVR_HandJoint_Ring_Joint3 = 19
				"WaveBone_20", // WVR_HandJoint_Ring_Tip = 20
				"WaveBone_21", // WVR_HandJoint_Pinky_Joint0 = 21
				"WaveBone_22", // WVR_HandJoint_Pinky_Joint0 = 22
				"WaveBone_23", // WVR_HandJoint_Pinky_Joint0 = 23
				"WaveBone_24", // WVR_HandJoint_Pinky_Joint0 = 24
				"WaveBone_25" // WVR_HandJoint_Pinky_Tip = 25
		};
        #endregion

        private void GetAllChildrenTransforms(Transform parent, ref List<Transform> childrenTransforms)
        {
            foreach (Transform child in parent)
            {
                childrenTransforms.Add(child);
                GetAllChildrenTransforms(child, ref childrenTransforms);
            }
        }

        private void FindJoints()
        {
            List<Transform> totalTransforms = new List<Transform>() { transform };
            GetAllChildrenTransforms(transform, ref totalTransforms);

            for (int i = 0; i < m_HandJoints.Length; i++)
            {
                Transform jointTransform = totalTransforms.FirstOrDefault(x => x.name == JointsName[i]);
                if (jointTransform != null)
                {
                    m_HandJoints[i] = jointTransform;
                }
            }
        }

        private void ClearJoints()
        {
            Array.Clear(m_HandJoints, 0, m_HandJoints.Length);
        }

        #endregion
    }
}
