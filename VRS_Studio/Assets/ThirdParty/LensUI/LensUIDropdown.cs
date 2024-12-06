#pragma warning disable 0649
using HTC.UnityPlugin.Utility.Switchable;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HTC.Triton.LensUI
{
    using AnimateTypeEnum = LensUIToggleSwitch.AnimateTypeEnum;

    public class LensUIDropdown : LensUISelectable
        , IPointerClickHandler
        , ISubmitHandler
    {
        private SwitchableBehaviourBase switchableBase = new SwitchableBehaviourBase();
        private bool isInitialized;

        [SerializeField]
        private bool isOpen;
        [SerializeField]
        private AnimateTypeEnum switchAnimateType;
        [SerializeField]
        private SwitchableGroupBehaviour group;
        [SerializeField]
        private GameObject blocker;

        // NOTE: Don't know why if this field is private (even with SerializeField attr) then 
        // it only shows "On Value Changed" (with out boolean argument) in editor inspector
        public Toggle.ToggleEvent onValueChanged = new Toggle.ToggleEvent();

        public AnimateTypeEnum SwitchAnimateType { get { return switchAnimateType; } set { switchAnimateType = value; } }

        public SwitchableGroupBehaviour GroupBehaviour { get { Initialize(); return switchableBase.GroupBehaviour; } set { Initialize(); switchableBase.GroupBehaviour = value; } }

        public virtual bool IsOpen
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

                    if (blocker != null) { blocker.SetActive(true); }
                }
                else
                {
                    if (IsOpen && switchableBase.GroupBehaviour != null && !switchableBase.GroupBehaviour.AllowSwitchOff) { return; }

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

                    if (blocker != null)
                    {
                        if (group == null || group.OpenedMember == null)
                        {
                            blocker.SetActive(false);
                        }
                    }
                }
            }
        }

        public void Initialize()
        {
            if (isInitialized) { return; }
            isInitialized = true;

            switchableBase.Initialize(gameObject, group, isOpen);

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
            if (IsOpen) { switchableBase.Switchable.OpenEnd(); }
            else { switchableBase.Switchable.CloseEnd(); }
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (pressedCount == 0 && IsEffectiveButton(eventData))
            {
                Toggle();
            }
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            Toggle();
        }

        public virtual void Toggle()
        {
            IsOpen = !IsOpen;
        }

        private void InvokeOnValueChangedEvent(bool snap)
        {
            isOpen = switchableBase.Switchable.IsOpen;
            UISystemProfilerApi.AddMarker("LensUIToggleSwitch.value", this);
            if (onValueChanged != null) { onValueChanged.Invoke(isOpen); }
        }
    }
}