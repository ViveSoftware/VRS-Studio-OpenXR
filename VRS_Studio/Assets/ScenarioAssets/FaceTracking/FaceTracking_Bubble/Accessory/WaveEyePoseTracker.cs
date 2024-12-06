//========= Copyright 2016-2021, HTC Corporation. All rights reserved. ===========

using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace HTC.UnityPlugin.PoseTracker
{
    public class WaveEyePoseTracker : BasePoseTracker, INewPoseListener
    {
        [Serializable]
        public class UnityEventBool : UnityEvent<bool> { }

        [SerializeField]
        [FormerlySerializedAs("origin")]
        private Transform m_origin;
        [SerializeField]
        [FormerlySerializedAs("onIsValidChanged")]
        private UnityEventBool m_onIsValidChanged;
        public UnityEventBool onIsValidChanged { get { return m_onIsValidChanged; } }

        private bool m_isValid;

        protected void SetIsValid(bool value, bool forceSet = false)
        {
            if (ChangeProp.Set(ref m_isValid, value) || forceSet)
            {
                if (m_onIsValidChanged != null)
                {
                    m_onIsValidChanged.Invoke(value);
                }
            }
        }

        protected virtual void Start()
        {
            SetIsValid(false, true);
        }

        protected virtual void OnEnable()
        {
            VivePose.AddNewPosesListener(this);
        }

        protected virtual void OnDisable()
        {
            VivePose.RemoveNewPosesListener(this);
            SetIsValid(false);
        }

        public virtual void BeforeNewPoses() { }

        public virtual void OnNewPoses()
        {
            Vector3 origin = Vector3.zero;
            Vector3 direction = Vector3.zero;
            bool isValid = false;

            if (EyeManager.Instance != null &&
                EyeManager.Instance.GetCombinedEyeOrigin(out origin) &&
                EyeManager.Instance.GetCombindedEyeDirectionNormalized(out direction))
            {
                isValid = true;
            }

            if (isValid)
            {
                RigidPose pose = new RigidPose(origin, Quaternion.LookRotation(direction));

                if (m_origin != null && m_origin != transform.parent)
                {
                    TrackPose(pose, false);
                }
                else
                {
                    TrackPose(pose, true);
                }
            }

            SetIsValid(isValid);
        }

        public virtual void AfterNewPoses() { }
    }
}