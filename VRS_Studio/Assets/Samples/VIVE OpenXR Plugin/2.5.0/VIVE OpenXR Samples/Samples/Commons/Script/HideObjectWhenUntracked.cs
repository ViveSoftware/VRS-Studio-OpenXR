// Copyright HTC Corporation All Rights Reserved.

using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace VIVE.OpenXR.Samples
{
    public class HideObjectWhenUntracked : MonoBehaviour
    {
        #region Log
        const string LOG_TAG = "VIVE.OpenXR.Samples.HideObjectWhenUntracked";
        StringBuilder m_sb = null;
        StringBuilder sb
        {
            get
            {
                if (m_sb == null) { m_sb = new StringBuilder(); }
                return m_sb;
            }
        }
        void DEBUG(StringBuilder msg) { Debug.LogFormat("{0} {1}", LOG_TAG, msg); }
        int printFrame = 0;
        protected bool printIntervalLog = false;
        #endregion

        #region Inspector
        public bool IsLeft = false;
        private string hand
        {
            get
            {
                return (IsLeft ? " Left " : " Right ");
            }
        }

        [SerializeField]
        private InputActionReference m_IsActive;
        public InputActionReference IsActive { get => m_IsActive; set => m_IsActive = value; }

        [SerializeField]
        private InputActionReference m_TrackingState;
        public InputActionReference TrackingState { get => m_TrackingState; set => m_TrackingState = value; }

        [SerializeField]
        private GameObject m_ObjectToHide = null;
        public GameObject ObjectToHide { get { return m_ObjectToHide; } set { m_ObjectToHide = value; } }
        #endregion

        private void Update()
        {
            printFrame++;
            printFrame %= 300;
            printIntervalLog = (printFrame == 0);

            if (m_ObjectToHide == null) { return; }

            string errMsg = "";
            bool isActive = false;
            int trackingState = 0;
            bool positionTracked = false, rotationTracked = false;

            if (OpenXRHelper.VALIDATE(m_IsActive, out errMsg))
            {
                if (m_IsActive.action.activeControl.valueType == typeof(float))
                    isActive = m_IsActive.action.ReadValue<float>() > 0;
                if (m_IsActive.action.activeControl.valueType == typeof(bool))
                    isActive = m_IsActive.action.ReadValue<bool>();
            }
            else
            {
                if (printIntervalLog)
                {
                    sb.Clear().Append("Update() ")
                        .Append(hand).Append(", ")
                        .Append(m_ObjectToHide != null ? m_ObjectToHide.name : "").Append(", ")
                        .Append(m_IsActive.action.name).Append(", ")
                        .Append(errMsg);
                    DEBUG(sb);
                }
            }
            if (OpenXRHelper.VALIDATE(m_TrackingState, out errMsg))
            {
                trackingState = m_TrackingState.action.ReadValue<int>();
            }
            else
            {
                if (printIntervalLog)
                {
                    sb.Clear().Append("Update() ")
                        .Append(hand).Append(", ")
                        .Append(m_ObjectToHide != null ? m_ObjectToHide.name : "").Append(", ")
                        .Append(m_TrackingState.action.name).Append(", ")
                        .Append(errMsg);
                    DEBUG(sb);
                }
            }

            if (printIntervalLog)
            {
                sb.Clear().Append("Update() ")
                    .Append(hand).Append(", ")
                    .Append(m_ObjectToHide != null ? m_ObjectToHide.name : "").Append(", ")
                    .Append("isActive: ").Append(isActive).Append(", ")
                    .Append("trackingState: ").Append(trackingState);
                DEBUG(sb);
            }

            positionTracked = ((uint)trackingState & (uint)InputTrackingState.Position) != 0;
            rotationTracked = ((uint)trackingState & (uint)InputTrackingState.Rotation) != 0;

            bool tracked = isActive /*&& positionTracked */&& rotationTracked; // Show the object with 3DoF.
            m_ObjectToHide.SetActive(tracked);
        }
    }
}
