#pragma warning disable 0649
using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.ViveSoftware.ExpLab.HandInteractionDemo
{
    public class SimpleColliderEventHandler : MonoBehaviour
        , IColliderEventHoverEnterHandler
        , IColliderEventHoverExitHandler
        , IColliderEventPressDownHandler
        , IColliderEventPressUpHandler
        , IColliderEventClickHandler
    {
        [SerializeField]
        private bool lastHoveredOnly = true;
        [SerializeField]
        [FlagsFromEnum(typeof(ControllerButton))]
        private ulong primaryButton = 0ul;
        [SerializeField]
        [FlagsFromEnum(typeof(ColliderButtonEventData.InputButton))]
        private uint secondaryButton = ~0u;
        [SerializeField]
        private UnityEvent onFirstHoverEntered = new UnityEvent();
        [SerializeField]
        private UnityEvent onAllHoversLeaved = new UnityEvent();
        [SerializeField]
        private UnityEvent onFirstButtonPressed = new UnityEvent();
        [SerializeField]
        private UnityEvent onAllButtonReleased = new UnityEvent();
        [SerializeField]
        private UnityEvent onButtonDown = new UnityEvent();
        [SerializeField]
        private UnityEvent onButtonUp = new UnityEvent();
        [SerializeField]
        private UnityEvent onButtonClicked = new UnityEvent();

        private HashSet<ColliderHoverEventData> hoverEntered = new HashSet<ColliderHoverEventData>();
        private HashSet<ColliderButtonEventData> buttonPressed = new HashSet<ColliderButtonEventData>();

        public void OnColliderEventHoverEnter(ColliderHoverEventData eventData)
        {
            if (hoverEntered.Add(eventData) && hoverEntered.Count == 1)
            {
                if (onFirstHoverEntered != null)
                {
                    onFirstHoverEntered.Invoke();
                }
            }
        }

        public void OnColliderEventHoverExit(ColliderHoverEventData eventData)
        {
            if (hoverEntered.Remove(eventData) && hoverEntered.Count == 0)
            {
                if (onAllHoversLeaved != null)
                {
                    onAllHoversLeaved.Invoke();
                }
            }
        }

        public void OnColliderEventPressDown(ColliderButtonEventData eventData)
        {
            if (!IsValidGrabButton(eventData)) { return; }

            if (buttonPressed.Add(eventData))
            {
                if (onButtonDown != null)
                {
                    onButtonDown.Invoke();
                }

                if (buttonPressed.Count == 1 && onFirstButtonPressed != null)
                {
                    onFirstButtonPressed.Invoke();
                }
            }
        }

        public void OnColliderEventPressUp(ColliderButtonEventData eventData)
        {
            if (buttonPressed.Remove(eventData))
            {
                if (onButtonUp != null)
                {
                    onButtonUp.Invoke();
                }

                if (buttonPressed.Count == 0 && onAllButtonReleased != null)
                {
                    onAllButtonReleased.Invoke();
                }
            }
        }

        public void OnColliderEventClick(ColliderButtonEventData eventData)
        {
            if (!IsValidGrabButton(eventData)) { return; }

            if (onButtonClicked != null)
            {
                onButtonClicked.Invoke();
            }
        }

        protected bool IsValidGrabButton(ColliderButtonEventData eventData)
        {
            if (primaryButton > 0ul)
            {
                ViveColliderButtonEventData viveEventData;
                if (eventData.TryGetViveButtonEventData(out viveEventData) && EnumUtils.GetFlag(primaryButton, (int)viveEventData.viveButton)) { return true; }
            }

            return secondaryButton > 0u && EnumUtils.GetFlag(secondaryButton, (int)eventData.button);
        }
    }
}