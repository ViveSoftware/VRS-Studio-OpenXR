#pragma warning disable 0649
using HTC.UnityPlugin.Utility;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HTC.Triton.LensUI
{
    public interface ILensUISelectableStateTransitionHandler : IEventSystemHandler
    {
        void OnLensUISelectableStateTransition(LensUISelectable selectable, LensUISelectable.State state, bool snap);
    }

    public class LensUISelectable : Selectable
    {
        public enum State
        {
            Normal = SelectionState.Normal,
            Highlighted = SelectionState.Highlighted,
            Pressed = SelectionState.Pressed,
            Selected = SelectionState.Selected,
            Disabled = SelectionState.Disabled,
        }

        public delegate void StateTransitionHandler(State state, bool snap);

        [FlagsFromEnum(typeof(PointerEventData.InputButton))]
        [SerializeField]
        private uint effectiveButton = EnumUtils.SetFlag(0u, (int)PointerEventData.InputButton.Left);
        [SerializeField]
        private bool selectOnPointerDown;
        [SerializeField]
        private bool deselectOnPointerEnter = true;

        public event StateTransitionHandler OnSelectableStateTransition;

        protected int enteredCount { get; private set; }

        protected int pressedCount { get; private set; }

        protected bool transitionGuard { get; private set; }

        public bool SelectOnPointerDown { get { return selectOnPointerDown; } set { selectOnPointerDown = value; } }

        public bool DeselectOnPointerEnter { get { return deselectOnPointerEnter; } set { deselectOnPointerEnter = value; } }

        public bool IsEffectiveButton(PointerEventData.InputButton buttonId) { return EnumUtils.GetFlag(effectiveButton, (int)buttonId); }

        public void SetEffectiveButton(PointerEventData.InputButton buttonId) { EnumUtils.SetFlag(effectiveButton, (int)buttonId); }

        public void UnsetEffectiveButton(PointerEventData.InputButton buttonId) { EnumUtils.UnsetFlag(effectiveButton, (int)buttonId); }

        public void ResetEffectiveButton() { effectiveButton = 0u; }

        private static PointerEventData defaultEventData = new PointerEventData(null);

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            transition = Transition.None;
        }
#endif
        protected override void InstantClearState()
        {
            base.InstantClearState();
            enteredCount = 0;
            pressedCount = 0;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (++enteredCount == 1)
            {
                base.OnPointerEnter(eventData);

                if (deselectOnPointerEnter)
                {
                    eventData.selectedObject = null;
                }
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (--enteredCount == 0)
            {
                base.OnPointerExit(eventData);
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (IsEffectiveButton(eventData) && ++pressedCount == 1)
            {
                PressDown(eventData);
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (IsEffectiveButton(eventData))
            {
                pressedCount = Mathf.Max(0, pressedCount - 1);
                if (pressedCount == 0)
                {
                    PressUp(eventData);
                }
            }
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                base.DoStateTransition(state, instant);
                return;
            }
#endif
            if (transitionGuard)
            {
                Debug.LogError("Attempting to change state to " + state + " while already transitioning.");
            }
            else
            {
                transitionGuard = true;

                base.DoStateTransition(state, instant);

                if (OnSelectableStateTransition != null)
                {
                    try
                    {
                        OnSelectableStateTransition((State)state, instant);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }

                gameObject.Execute<ILensUISelectableStateTransitionHandler>(handler => handler.OnLensUISelectableStateTransition(this, (State)state, instant));

                transitionGuard = false;
            }
        }

        protected bool IsEffectiveButton(PointerEventData eventData)
        {
            return EnumUtils.GetFlag(effectiveButton, (int)eventData.button);
        }

        private bool disableInteractableOnce = false;
        private void DisableInteractableOnce()
        {
            disableInteractableOnce = true;
        }

        public override bool IsInteractable()
        {
            if (disableInteractableOnce)
            {
                disableInteractableOnce = false;
                return false;
            }

            return base.IsInteractable();
        }

        private void PressDown(PointerEventData eventData = null)
        {
            // Do Select, this hacking is to let us able to set private field "isPointerDown" without triggering EventSystem.current.SetSelectedGameObject
            if (!selectOnPointerDown) { DisableInteractableOnce(); }

            if (eventData == null)
            {
                base.OnPointerDown(defaultEventData);
            }
            else
            {
                var prevButton = eventData.button;
                eventData.button = PointerEventData.InputButton.Left;
                base.OnPointerDown(eventData);
                eventData.button = prevButton;
            }
        }

        private void PressUp(PointerEventData eventData = null)
        {
            base.OnPointerUp(defaultEventData);
        }
    }
}