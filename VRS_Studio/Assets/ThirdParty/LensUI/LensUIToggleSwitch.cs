#pragma warning disable 0649
using HTC.UnityPlugin.Utility.Switchable;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HTC.Triton.LensUI
{
    public class LensUIToggleSwitch : LensUISelectable
        , IPointerClickHandler
        , ISubmitHandler
    {
        public enum AnimateTypeEnum
        {
            SnapWhenInactive,
            AlwaysSnap,
            AlwaysAnimate,
        }

        private SwitchableBehaviourBase switchableBase = new SwitchableBehaviourBase();
        private bool isInitialized;

        [SerializeField]
        private bool isOn;
        [SerializeField]
        private AnimateTypeEnum switchAnimateType;
        [SerializeField]
        private SwitchableGroupBehaviour group;

        // NOTE: Don't know why if this field is private (even with SerializeField attr) then 
        // it only shows "On Value Changed" (with out boolean argument) in editor inspector
        public Toggle.ToggleEvent onValueChanged = new Toggle.ToggleEvent();

        public AnimateTypeEnum SwitchAnimateType { get { return switchAnimateType; } set { switchAnimateType = value; } }

        public SwitchableGroupBehaviour GroupBehaviour { get { Initialize(); return switchableBase.GroupBehaviour; } set { Initialize(); switchableBase.GroupBehaviour = value; } }

        public virtual bool IsOn
        {
            get { return switchableBase.Switchable.IsOpen; }
            set
            {
                if (value)
                {
                    switch (switchAnimateType)
                    {
                        case AnimateTypeEnum.SnapWhenInactive:
                            if (isActiveAndEnabled) { switchableBase.Switchable.OpenStart(); } else { switchableBase.Switchable.SnapOpen(); }
                            break;
                        case AnimateTypeEnum.AlwaysSnap:
                            switchableBase.Switchable.SnapOpen();
                            break;
                        case AnimateTypeEnum.AlwaysAnimate:
                            switchableBase.Switchable.OpenStart();
                            break;
                    }
                }
                else
                {
                    if (IsOn && switchableBase.GroupBehaviour != null && !switchableBase.GroupBehaviour.AllowSwitchOff) { return; }

                    switch (switchAnimateType)
                    {
                        case AnimateTypeEnum.SnapWhenInactive:
                            if (isActiveAndEnabled) { switchableBase.Switchable.CloseStart(); } else { switchableBase.Switchable.SnapClose(); }
                            break;
                        case AnimateTypeEnum.AlwaysSnap:
                            switchableBase.Switchable.SnapClose();
                            break;
                        case AnimateTypeEnum.AlwaysAnimate:
                            switchableBase.Switchable.CloseStart();
                            break;
                    }
                }
            }
        }

        public void Initialize()
        {
            if (isInitialized) { return; }
            isInitialized = true;

            switchableBase.Initialize(gameObject, group, isOn);

            switchableBase.Switchable.OnSwitch += InvokeOnValueChangedEvent;
        }

        protected override void Awake()
        {
            Initialize();
        }

        protected override void OnDestroy()
        {
            if (isInitialized)
            {
                switchableBase.Switchable.OnSwitch -= InvokeOnValueChangedEvent;

                switchableBase.Dispose();
            }
        }

        public void NotifyAnimationEnded()
        {
            if (IsOn) { switchableBase.Switchable.OpenEnd(); }
            else { switchableBase.Switchable.CloseEnd(); }
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (pressedCount == 0 && IsEffectiveButton(eventData) && IsInteractable())
            {
                Toggle();
            }
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            if (IsInteractable())
            {
                Toggle();
            }
        }

        public virtual void Toggle()
        {
            IsOn = !IsOn;
        }

        private void InvokeOnValueChangedEvent(bool snap)
        {
            isOn = switchableBase.Switchable.IsOpen;
            UISystemProfilerApi.AddMarker("LensUIToggleSwitch.value", this);
            if (onValueChanged != null) { onValueChanged.Invoke(isOn); }
        }
    }
}