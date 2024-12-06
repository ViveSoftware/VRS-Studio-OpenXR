// Copyright HTC Corporation All Rights Reserved.

using UnityEngine;
using UnityEngine.InputSystem;

namespace VIVE.OpenXR.Samples
{
    public class Focus3ModelAction : MonoBehaviour
    {
        #region Inspector
        public bool isLeft = false;
        public InputActionAsset actionAsset = null;
        public InputActionReference trigger = null;
        public InputActionReference grip = null;
        public InputActionReference thumbstick = null;
        public InputActionReference primaryClick = null;
        public InputActionReference secondaryClick = null;
        #endregion

        private GameObject m_TriggerButton = null;
        private GameObject m_GripButton = null;
        private Vector3 gripButtonPos = Vector3.zero;
        private GameObject m_ThumbstickButton = null;
        private GameObject m_PrimaryButton = null;
        private Vector3 primaryButtonPos = Vector3.zero;
        private GameObject m_SecondaryButton = null;
        private Vector3 secondaryButtonPos = Vector3.zero;

        private void OnEnable()
        {
            if (actionAsset != null && !actionAsset.enabled) { actionAsset.Enable(); }
        }

        void Start()
        {
            m_TriggerButton = transform.GetChild(0).gameObject;

            m_GripButton = transform.GetChild(1).gameObject;
            if (m_GripButton != null) { gripButtonPos = m_GripButton.transform.localPosition; }

            m_ThumbstickButton = transform.GetChild(2).gameObject;

            m_PrimaryButton = transform.GetChild(3).gameObject;
            if (m_PrimaryButton != null) { primaryButtonPos = m_PrimaryButton.transform.localPosition; }

            m_SecondaryButton = transform.GetChild(4).gameObject;
            if (m_SecondaryButton != null) { secondaryButtonPos = m_SecondaryButton.transform.localPosition; }
        }

        void Update()
        {
            OnTrigger();
            OnGrip();
            OnThumbstick();
            OnPrimaryClick();
            OnSecondaryClick();
        }

        void OnTrigger()
        {
            if (CommonHelper.GetAnalog(trigger, out float value, out string msg))
            {
                m_TriggerButton.transform.localRotation = Quaternion.Euler(value * -15f, 0, 0);
            }
        }
        void OnGrip()
        {
            if (CommonHelper.GetAnalog(grip, out float value, out string msg))
            {
                if (isLeft)
                    m_GripButton.transform.localPosition = gripButtonPos + Vector3.right * value * 0.002f;
                else
                    m_GripButton.transform.localPosition = gripButtonPos + Vector3.left * value * 0.002f;
            }
        }
        void OnThumbstick()
        {
            if (CommonHelper.GetVector2(thumbstick, out Vector2 value, out string msg))
            {
                m_ThumbstickButton.transform.localRotation = Quaternion.Euler(value.y * -25f, 0, value.x * 25f);
            }
        }
        void OnPrimaryClick()
        {
            if (CommonHelper.GetAnalog(primaryClick, out float value, out string msg))
            {
                m_PrimaryButton.transform.localPosition = primaryButtonPos + Vector3.down * (value > 0.5f ? 0.00125f : 0);
            }
        }
        void OnSecondaryClick()
        {
            if (CommonHelper.GetAnalog(secondaryClick, out float value, out string msg))
            {
                m_SecondaryButton.transform.localPosition = secondaryButtonPos + Vector3.down * (value > 0.5f ? 0.00125f : 0);
            }
        }
    }
}