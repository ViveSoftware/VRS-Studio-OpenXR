#pragma warning disable 0649
using System;
using UnityEngine;

namespace HTC.UnityPlugin.Utility.Switchable
{
    [Serializable]
    public class SwitchableBehaviour : MonoBehaviour
    {
        private SwitchableBehaviourBase switchableBase = new SwitchableBehaviourBase();
        private bool isInitialized;

        [SerializeField]
        private SwitchableGroupBehaviour groupBehaviour;
        [SerializeField]
        private bool isOpen;
        [SerializeField]
        private bool allowInterrupt = true;

        public SwitchableGroupBehaviour GroupBehaviour { get { Initialize(); return switchableBase.GroupBehaviour; } set { Initialize(); switchableBase.GroupBehaviour = value; } }

        public bool IsOpen { get { Initialize(); return switchableBase.Switchable.IsOpen; } }

        public bool AllowInterrupt { get { return allowInterrupt; } set { allowInterrupt = value; } }

        public SwitchableStatus Status { get { Initialize(); return switchableBase.Switchable.Status; } }

        public object Owner { get { Initialize(); return switchableBase.Switchable.Owner; } set { Initialize(); switchableBase.Switchable.Owner = value; } }

        public event Switchable.Callback OnSwitch { add { switchableBase.Switchable.OnSwitch += value; } remove { switchableBase.Switchable.OnSwitch -= value; } }

        public event Switchable.Callback OnBeforeOpen { add { switchableBase.Switchable.OnBeforeOpen += value; } remove { switchableBase.Switchable.OnBeforeOpen -= value; } }

        public event Switchable.Callback OnOpenStart { add { switchableBase.Switchable.OnOpenStart += value; } remove { switchableBase.Switchable.OnOpenStart -= value; } }

        public event Switchable.Callback OnOpenEnd { add { switchableBase.Switchable.OnOpenEnd += value; } remove { switchableBase.Switchable.OnOpenEnd -= value; } }

        public event Switchable.Callback OnBeforeClose { add { switchableBase.Switchable.OnBeforeClose += value; } remove { switchableBase.Switchable.OnBeforeClose -= value; } }

        public event Switchable.Callback OnCloseStart { add { switchableBase.Switchable.OnCloseStart += value; } remove { switchableBase.Switchable.OnCloseStart -= value; } }

        public event Switchable.Callback OnCloseEnd { add { switchableBase.Switchable.OnCloseEnd += value; } remove { switchableBase.Switchable.OnCloseEnd -= value; } }

        public void Initialize()
        {
            if (isInitialized) { return; }
            isInitialized = true;

            switchableBase.Switchable.Owner = this;
            switchableBase.Initialize(gameObject, groupBehaviour, isOpen);

            isOpen = switchableBase.Switchable.IsOpen;
            switchableBase.Switchable.OnSwitch += OnSwitchableSwitch;
        }

        protected virtual void Awake()
        {
            Initialize();

            GroupBehaviour = groupBehaviour;
        }

        protected virtual void OnDestroy()
        {
            if (isInitialized)
            {
                switchableBase.Switchable.OnSwitch -= OnSwitchableSwitch;
                switchableBase.Dispose();
            }
        }

        private void OnSwitchableSwitch(bool snap)
        {
            isOpen = switchableBase.Switchable.IsOpen;
        }

        public virtual void OpenStart()
        {
            Initialize();

            if (IsOpen) { return; }

            if (!allowInterrupt)
            {
                if (Status != SwitchableStatus.Closed) { return; }
                if (GroupBehaviour != null && GroupBehaviour.IsSwitching) { return; }
            }

            switchableBase.Switchable.OpenStart();
        }

        public virtual void OpenEnd()
        {
            Initialize();

            switchableBase.Switchable.OpenEnd();
        }

        public void SnapOpen()
        {
            Initialize();

            switchableBase.Switchable.SnapOpen();
        }

        public virtual void CloseStart()
        {
            Initialize();

            if (!IsOpen) { return; }

            if (!allowInterrupt)
            {
                if (Status != SwitchableStatus.Opened) { return; }
                if (GroupBehaviour != null && GroupBehaviour.IsSwitching) { return; }
            }

            if (GroupBehaviour != null && !GroupBehaviour.AllowSwitchOff) { return; }

            switchableBase.Switchable.CloseStart();
        }

        public virtual void CloseEnd()
        {
            Initialize();

            switchableBase.Switchable.CloseEnd();
        }

        public void SnapClose()
        {
            Initialize();

            if (Status == SwitchableStatus.Closed) { return; }

            if (GroupBehaviour != null && !GroupBehaviour.AllowSwitchOff) { return; }

            switchableBase.Switchable.SnapClose();
        }
    }
}