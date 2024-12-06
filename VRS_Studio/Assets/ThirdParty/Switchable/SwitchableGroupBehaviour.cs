#pragma warning disable 0649
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HTC.UnityPlugin.Utility.Switchable
{
    public interface ISwitchableGroupInitializeHandler : IEventSystemHandler
    {
        void OnSwitchableGroupInitialized(SwitchableGroupBehaviour group);
    }

    public interface ISwitchableGroupMemberRegisteredHandler : IEventSystemHandler
    {
        void OnSwitchableGroupMemberRegistered(SwitchableGroupBehaviour group, Switchable switchable);
    }

    public interface ISwitchableGroupMemberUnregisteredHandler : IEventSystemHandler
    {
        void OnSwitchableGroupMemberUnregistered(SwitchableGroupBehaviour group, Switchable switchable);
    }

    public interface ISwitchableGroupStatusChangedHandler : IEventSystemHandler
    {
        void OnSwitchableGroupStatusChanged(SwitchableGroupBehaviour group, GroupStatus status);
    }

    [Serializable]
    public class SwitchableGroupBehaviour : MonoBehaviour
    {
        private readonly SwitchableGroup switchableGroup = Switchable.CreateGroup();

        [SerializeField]
        private bool sequentialSwitch;
        [SerializeField]
        private bool allowSwitchOff;

        private bool isInitialized;

        protected bool changeMemberGuard { get; private set; }

        public bool SequentialSwitch { get { return switchableGroup.SequentialSwitch; } set { switchableGroup.SequentialSwitch = value; } }

        public bool AllowSwitchOff { get { return allowSwitchOff; } set { allowSwitchOff = value; } }

        public bool IsSwitching
        {
            get
            {
                if (switchableGroup.OpenedMember != null && switchableGroup.OpenedMember.Status != SwitchableStatus.Opened) { return true; }
                if (switchableGroup.ClosingMember != null) { return true; }
                return false;
            }
        }

        public Switchable OpenedMember { get { return switchableGroup.OpenedMember; } }
        public Switchable ClosingMemeber { get { return switchableGroup.ClosingMember; } }

        public event SwitchableGroup.Callback OnStatusChanged { add { switchableGroup.OnStatusChange += value; } remove { switchableGroup.OnStatusChange -= value; } }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (UnityEditor.EditorApplication.isPlaying && !isInitialized)
            {
                UnityEditor.EditorApplication.delayCall += Initialize;
            }
        }
#endif
        public void Initialize()
        {
            if (isInitialized) { return; }
            isInitialized = true;

            SequentialSwitch = sequentialSwitch;
            AllowSwitchOff = allowSwitchOff;

            switchableGroup.OnStatusChange += SendStatusChangedMessage;

            SendInitializeMessage();
        }

        protected virtual void Awake()
        {
            Initialize();
        }

        protected virtual void OnDestroy()
        {
            if (isInitialized)
            {
                switchableGroup.OnStatusChange -= SendStatusChangedMessage;
            }
        }

        public void CloseSwitchableBehaviourMember()
        {
            if (!allowSwitchOff) { return; }
            if (switchableGroup.OpenedMember == null) { return; }
            if (!switchableGroup.OpenedMember.IsOpen) { return; }
            if (!switchableGroup.OpenedMember.TryGetOwner<SwitchableBehaviour>(out var member))
            {
                Debug.LogWarning("CloseSwitchableBehaviourMember fail because opened member is not a SwitchableBehaviour");
                return;
            }
            if (!member.AllowInterrupt && IsSwitching)
            {
                Debug.LogWarning("CloseSwitchableBehaviourMember fail because some member is switching and AllowInterrupt option is off");
                return;
            }

            member.CloseStart();
        }

        public void SnapCloseSwitchableBehaviourMember()
        {
            if (!allowSwitchOff) { return; }
            if (switchableGroup.OpenedMember == null) { return; }
            if (!switchableGroup.OpenedMember.IsOpen) { return; }
            if (!switchableGroup.OpenedMember.TryGetOwner<SwitchableBehaviour>(out var member))
            {
                Debug.LogWarning("SnapCloseSwitchableBehaviourMember fail because opened member is not a SwitchableBehaviour");
                return;
            }

            member.SnapClose();
        }

        public void RegisterMember(Switchable switchable)
        {
            if (changeMemberGuard)
            {
                Debug.LogError("Attempting to RegisterMember while already changing member, this is not allowed.");
                return;
            }

            Initialize();
            if (switchable.Group != switchableGroup)
            {
                if (switchable.Group != null)
                {
                    switchable.Group.UnregisterMember(switchable);
                }

                switchableGroup.RegisterMember(switchable);

                changeMemberGuard = true;
                SendMemberRegisteredMessage(switchable);
                changeMemberGuard = false;
            }
        }

        public void UnregisterMember(Switchable switchable)
        {
            if (changeMemberGuard)
            {
                Debug.LogError("Attempting to UnregisterMember while already changing member, this is not allowed.");
                return;
            }

            Initialize();
            if (switchable.Group == switchableGroup)
            {
                switchableGroup.UnregisterMember(switchable);

                changeMemberGuard = true;
                SendMemberUnregisteredMessage(switchable);
                changeMemberGuard = false;
            }
        }

        private void SendInitializeMessage()
        {
            gameObject.Execute<ISwitchableGroupInitializeHandler>(handler => handler.OnSwitchableGroupInitialized(this));
        }

        private void SendMemberRegisteredMessage(Switchable switchable)
        {
            gameObject.Execute<ISwitchableGroupMemberRegisteredHandler>(handler => handler.OnSwitchableGroupMemberRegistered(this, switchable));
        }

        private void SendMemberUnregisteredMessage(Switchable switchable)
        {
            gameObject.Execute<ISwitchableGroupMemberUnregisteredHandler>(handler => handler.OnSwitchableGroupMemberUnregistered(this, switchable));
        }

        private void SendStatusChangedMessage(GroupStatus status)
        {
            gameObject.Execute<ISwitchableGroupStatusChangedHandler>(handler => handler.OnSwitchableGroupStatusChanged(this, status));
        }
    }
}