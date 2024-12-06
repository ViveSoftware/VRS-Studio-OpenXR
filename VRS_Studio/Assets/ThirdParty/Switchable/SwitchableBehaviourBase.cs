using UnityEngine;
using UnityEngine.EventSystems;

namespace HTC.UnityPlugin.Utility.Switchable
{
    public interface ISwitchableInitializeHandler : IEventSystemHandler
    {
        void OnSwitchableInitialized(Switchable switchable);
    }

    public interface ISwitchableSwitchedHandler : IEventSystemHandler
    {
        void OnSwitchableSwitched(Switchable switchable, bool isOpened, bool snap);
    }

    public interface ISwitchableBeforeOpenHandler : IEventSystemHandler
    {
        void OnSwitchableBeforeOpen(Switchable switchable, bool snap);
    }

    public interface ISwitchableOpenStartHandler : IEventSystemHandler
    {
        void OnSwitchableOpenStart(Switchable switchable, bool snap);
    }

    public interface ISwitchableOpenEndHandler : IEventSystemHandler
    {
        void OnSwitchableOpenEnd(Switchable switchable, bool snap);
    }

    public interface ISwitchableBeforeCloseHandler : IEventSystemHandler
    {
        void OnSwitchableBeforeClose(Switchable switchable, bool snap);
    }

    public interface ISwitchableCloseStartHandler : IEventSystemHandler
    {
        void OnSwitchableCloseStart(Switchable switchable, bool snap);
    }

    public interface ISwitchableCloseEndHandler : IEventSystemHandler
    {
        void OnSwitchableCloseEnd(Switchable switchable, bool snap);
    }

    public interface ISwitchableBehaviour
    {
        SwitchableGroupBehaviour GroupBehaviour { get; set; }
        bool IsOpen { get; }
        bool AllowInterrupt { get; set; }
        SwitchableStatus Status { get; }
        object Owner { get; }

        event Switchable.Callback OnBeforeOpen;
        event Switchable.Callback OnBeforeClose;
        event Switchable.Callback OnSwitch;
        event Switchable.Callback OnOpenStart;
        event Switchable.Callback OnOpenEnd;
        event Switchable.Callback OnCloseStart;
        event Switchable.Callback OnCloseEnd;

        void Initialize();
        void OpenStart();
        void OpenEnd();
        void SnapOpen();
        void CloseStart();
        void CloseEnd();
        void SnapClose();
    }

    public class SwitchableBehaviourBase
    {
        private SwitchableGroupBehaviour groupBehaviour;
        private GameObject ownerGameObject;

        public Switchable Switchable { get; private set; }
        public bool IsInitialized { get; private set; }

        public SwitchableGroupBehaviour GroupBehaviour
        {
            get { return groupBehaviour; }
            set
            {
                if (!IsInitialized) { return; }
                if (groupBehaviour == value) { return; }
                if (groupBehaviour != null) { groupBehaviour.UnregisterMember(Switchable); }
                groupBehaviour = value;
                if (groupBehaviour != null) { groupBehaviour.RegisterMember(Switchable); }
            }
        }

        public SwitchableBehaviourBase()
        {
            Switchable = new Switchable();
            Switchable.OnBeforeOpen += SendBeforeOpenMessage;
            Switchable.OnBeforeClose += SendBeforeCloseMessage;
            Switchable.OnSwitch += SendSwitchedMessage;
            Switchable.OnOpenStart += SendOpenStartMessage;
            Switchable.OnOpenEnd += SendOpenEndMessage;
            Switchable.OnCloseStart += SendCloseStartMessage;
            Switchable.OnCloseEnd += SendCloseEndMessage;
        }

        public void Initialize(GameObject ownerGameObject, SwitchableGroupBehaviour startingGroup, bool startWithOpened)
        {
            if (IsInitialized) { return; }

            IsInitialized = true;

            if (startWithOpened)
            {
                Switchable.SnapOpen();
            }
            else
            {
                Switchable.SnapClose();
            }

            this.ownerGameObject = ownerGameObject;
            GroupBehaviour = startingGroup;

            SendInitializedMessage();
        }

        public void Dispose()
        {
            if (!IsInitialized) { return; }

            GroupBehaviour = null;
            ownerGameObject = null;

            IsInitialized = false;
        }

        private void SendInitializedMessage()
        {
            if (ownerGameObject != null) { ownerGameObject.Execute<ISwitchableInitializeHandler>(handler => handler.OnSwitchableInitialized(Switchable)); }
        }

        private void SendSwitchedMessage(bool snap)
        {
            if (ownerGameObject != null) { ownerGameObject.Execute<ISwitchableSwitchedHandler>(handler => handler.OnSwitchableSwitched(Switchable, Switchable.IsOpen, snap)); }
        }

        private void SendBeforeOpenMessage(bool snap)
        {
            if (ownerGameObject != null) { ownerGameObject.Execute<ISwitchableBeforeOpenHandler>(handler => handler.OnSwitchableBeforeOpen(Switchable, snap)); }
        }

        private void SendOpenStartMessage(bool snap)
        {
            if (ownerGameObject != null) { ownerGameObject.Execute<ISwitchableOpenStartHandler>(handler => handler.OnSwitchableOpenStart(Switchable, snap)); }
        }

        private void SendOpenEndMessage(bool snap)
        {
            if (ownerGameObject != null) { ownerGameObject.Execute<ISwitchableOpenEndHandler>(handler => handler.OnSwitchableOpenEnd(Switchable, snap)); }
        }

        private void SendBeforeCloseMessage(bool snap)
        {
            if (ownerGameObject != null) { ownerGameObject.Execute<ISwitchableBeforeCloseHandler>(handler => handler.OnSwitchableBeforeClose(Switchable, snap)); }
        }

        private void SendCloseStartMessage(bool snap)
        {
            if (ownerGameObject != null) { ownerGameObject.Execute<ISwitchableCloseStartHandler>(handler => handler.OnSwitchableCloseStart(Switchable, snap)); }
        }

        private void SendCloseEndMessage(bool snap)
        {
            if (ownerGameObject != null) { ownerGameObject.Execute<ISwitchableCloseEndHandler>(handler => handler.OnSwitchableCloseEnd(Switchable, snap)); }
        }
    }
}