using HTC.UnityPlugin.ColliderEvent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace HTC.ViveSoftware.ExpLab.HandInteractionDemo
{
    public class HighlightEffect : MonoBehaviour
        , IColliderEventHoverEnterHandler
        , IColliderEventHoverExitHandler
        , IColliderEventPressDownHandler
        , IColliderEventPressUpHandler
    {
        [SerializeField]
        [FormerlySerializedAs("onFirstHoverEntered")]
        private UnityEvent onHighlightEnable;
        [SerializeField]
        [FormerlySerializedAs("onAllHoverLeaved")]
        private UnityEvent onHighlightDisable;

        private bool isHighlighted;
        private HashSet<ColliderHoverEventData> hovers = new HashSet<ColliderHoverEventData>();
        private HashSet<ColliderButtonEventData> presses = new HashSet<ColliderButtonEventData>();

        public void OnColliderEventHoverEnter(ColliderHoverEventData eventData)
        {
            hovers.Add(eventData);
            UpdateHighlightState();
        }

        public void OnColliderEventHoverExit(ColliderHoverEventData eventData)
        {
            hovers.Remove(eventData);
            UpdateHighlightState();
        }

        public void OnColliderEventPressDown(ColliderButtonEventData eventData)
        {
            presses.Add(eventData);
            UpdateHighlightState();
        }

        public void OnColliderEventPressUp(ColliderButtonEventData eventData)
        {
            presses.Remove(eventData);
            UpdateHighlightState();
        }

        private void OnDisable()
        {
            hovers.Clear();
        }

        private void UpdateHighlightState()
        {
            var shouldHighlight = hovers.Count > 0 && presses.Count == 0;

            if (isHighlighted)
            {
                if (!shouldHighlight)
                {
                    isHighlighted = false;
                    if (onHighlightDisable != null)
                    {
                        onHighlightDisable.Invoke();
                    }
                }
            }
            else
            {
                if (shouldHighlight)
                {
                    isHighlighted = true;
                    if (onHighlightEnable != null)
                    {
                        onHighlightEnable.Invoke();
                    }
                }
            }
        }
    }
}