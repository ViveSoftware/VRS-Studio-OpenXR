using System;
using UnityEngine;

namespace HTC.UnityPlugin.Utility.Switchable
{
    public enum SwitchableStatus
    {
        Closed,
        QueuingOpen,
        Opening,
        Opened,
        Closing,
    }

    [Serializable]
    public partial class Switchable
    {
        public delegate void Callback(bool snap);

        private object owner;
        private InternalGroup group;
        private bool isChangingStatus;
        private bool animateEndedFlag;

        public event Callback OnSwitch; // Invoked when IsOpen changed
        public event Callback OnBeforeOpen; // Invoked before IsOpen state changed from false to true
        public event Callback OnOpenStart; // Invoked when Open animate/transition should start
        public event Callback OnOpenEnd; // Invoked when Open animate/transition should start
        public event Callback OnBeforeClose; // Invoded before IsOpen state changed from true to false
        public event Callback OnCloseStart; // Invoked when Close animate/transition should start
        public event Callback OnCloseEnd; // Invoked when Close animate/transition should start

        public object Owner { get { return owner; } set { owner = value; } }

        public SwitchableGroup Group { get { return group; } }

        [SerializeField]
        public SwitchableStatus Status { get; private set; }

        public bool IsOpen { get { return Status == SwitchableStatus.QueuingOpen || Status == SwitchableStatus.Opening || Status == SwitchableStatus.Opened; } }

        public static SwitchableGroup CreateGroup()
        {
            return new InternalGroup();
        }

        public bool TryGetOwner<T>(out T r)
        {
            if (owner is T)
            {
                r = (T)owner;
                return true;
            }
            else
            {
                r = default(T);
                return false;
            }
        }

        public void OpenStart()
        {
            switch (Status)
            {
                case SwitchableStatus.Closing:
                case SwitchableStatus.Closed:
                    if (group == null)
                    {
                        if (BeginChangingStatusGuard(SwitchableStatus.Opening))
                        {
                            ChangeToStatus(false, SwitchableStatus.Opening);

                            if (GetAnimateEndedFlag())
                            {
                                ChangeToStatus(false, SwitchableStatus.Opened);
                            }

                            EndChangingStatusGuard();
                        }
                    }
                    else
                    {
                        group.NotifyOpenStart(this);
                    }
                    break;
            }
        }

        public void OpenEnd()
        {
            switch (Status)
            {
                case SwitchableStatus.Opening:
                    if (group == null)
                    {
                        if (BeginChangingStatusGuard())
                        {
                            ChangeToStatus(false, SwitchableStatus.Opened);

                            EndChangingStatusGuard();
                        }
                        else
                        {
                            SetAnimateEndedFlag();
                        }
                    }
                    else
                    {
                        group.NotifyOpenEnd(this);
                    }
                    break;
            }
        }

        public void SnapOpen()
        {
            switch (Status)
            {
                case SwitchableStatus.Closing:
                case SwitchableStatus.Closed:
                case SwitchableStatus.Opening:
                    if (group == null)
                    {
                        if (BeginChangingStatusGuard(SwitchableStatus.Opened))
                        {
                            ChangeToStatus(true, SwitchableStatus.Opened);

                            EndChangingStatusGuard();
                        }
                    }
                    else
                    {
                        group.NotifySnapOpen(this);
                    }
                    break;

                case SwitchableStatus.QueuingOpen:
                    group.NotifySnapOpen(this);
                    break;
            }
        }

        public void CloseStart()
        {
            switch (Status)
            {
                case SwitchableStatus.QueuingOpen:
                case SwitchableStatus.Opening:
                case SwitchableStatus.Opened:
                    if (group == null)
                    {
                        if (BeginChangingStatusGuard(SwitchableStatus.Closing))
                        {
                            ChangeToStatus(false, SwitchableStatus.Closing);

                            if (GetAnimateEndedFlag())
                            {
                                ChangeToStatus(false, SwitchableStatus.Closed);
                            }

                            EndChangingStatusGuard();
                        }
                    }
                    else
                    {
                        group.NotifyCloseStart(this);
                    }
                    break;
            }
        }

        public void CloseEnd()
        {
            switch (Status)
            {
                case SwitchableStatus.Closing:
                    if (group == null)
                    {
                        if (BeginChangingStatusGuard())
                        {
                            ChangeToStatus(false, SwitchableStatus.Closed);

                            EndChangingStatusGuard();
                        }
                        else
                        {
                            SetAnimateEndedFlag();
                        }
                    }
                    else
                    {
                        group.NotifyCloseEnd(this);
                    }
                    break;
            }
        }

        public void SnapClose()
        {
            switch (Status)
            {
                case SwitchableStatus.QueuingOpen:
                case SwitchableStatus.Opening:
                case SwitchableStatus.Opened:
                case SwitchableStatus.Closing:
                    if (group == null)
                    {
                        if (BeginChangingStatusGuard(SwitchableStatus.Closed))
                        {
                            ChangeToStatus(true, SwitchableStatus.Closed);

                            EndChangingStatusGuard();
                        }
                    }
                    else
                    {
                        group.NotifySnapClose(this);
                    }
                    break;
            }
        }

        private bool BeginChangingStatusGuard(SwitchableStatus status)
        {
            var passed = BeginChangingStatusGuard();
            if (!passed) { Debug.LogError("Attempt to change status to " + status + " while alreading chagneing status to " + Status); }
            return passed;
        }

        private bool BeginChangingStatusGuard()
        {
            if (isChangingStatus)
            {
                return false;
            }
            else
            {
                isChangingStatus = true;
                return true;
            }
        }

        private void EndChangingStatusGuard()
        {
            isChangingStatus = false;
        }

        private void ResetAnimateEndedFlag()
        {
            animateEndedFlag = false;
        }

        private void SetAnimateEndedFlag()
        {
            animateEndedFlag = true;
        }

        private bool GetAnimateEndedFlag()
        {
            return animateEndedFlag;
        }

        private void ChangeToStatus(bool snap, SwitchableStatus s)
        {
            InvokeEventBeforeStatusChange(snap, s);
            ResetAnimateEndedFlag();
            (s, Status) = (Status, s);
            InvokeEventForStatusChange(snap, s);
        }

        private void InvokeEventBeforeStatusChange(bool snap, SwitchableStatus incomingStatus)
        {
            // OnBefor Open/Close event should invoke if
            // 1. IsOpen will change
            // 2. will perform animation
            try
            {
                switch (Status)
                {
                    case SwitchableStatus.Closed:
                        switch (incomingStatus)
                        {
                            case SwitchableStatus.QueuingOpen:
                            case SwitchableStatus.Opening:
                                if (OnBeforeOpen != null) { OnBeforeOpen.Invoke(false); }
                                return;
                            case SwitchableStatus.Opened:
                                if (OnBeforeOpen != null) { OnBeforeOpen.Invoke(true); }
                                return;
                            //case SwitchableStatus.Closing:
                            case SwitchableStatus.Closed:
                                return;
                        }
                        break;
                    case SwitchableStatus.QueuingOpen:
                        switch (incomingStatus)
                        {
                            case SwitchableStatus.Opening:
                            case SwitchableStatus.Opened:
                                return;
                            //case SwitchableStatus.Closing:
                            case SwitchableStatus.Closed:
                                if (OnBeforeClose != null) { OnBeforeClose.Invoke(snap); }
                                return;
                            case SwitchableStatus.QueuingOpen:
                                return;
                        }
                        break;
                    case SwitchableStatus.Opening:
                        switch (incomingStatus)
                        {
                            case SwitchableStatus.Opened:
                                return;
                            case SwitchableStatus.Closing:
                                if (OnBeforeClose != null) { OnBeforeClose.Invoke(false); }
                                return;
                            case SwitchableStatus.Closed:
                                if (OnBeforeClose != null) { OnBeforeClose.Invoke(true); }
                                return;
                            //case SwitchableStatus.QueuingOpen:
                            case SwitchableStatus.Opening:
                                return;
                        }
                        break;
                    case SwitchableStatus.Opened:
                        switch (incomingStatus)
                        {
                            case SwitchableStatus.Closing:
                                if (OnBeforeClose != null) { OnBeforeClose.Invoke(false); }
                                return;
                            case SwitchableStatus.Closed:
                                if (OnBeforeClose != null) { OnBeforeClose.Invoke(true); }
                                return;
                            //case SwitchableStatus.QueuingOpen:
                            //case SwitchableStatus.Opening:
                            case SwitchableStatus.Opened:
                                return;
                        }
                        break;
                    case SwitchableStatus.Closing:
                        switch (incomingStatus)
                        {
                            case SwitchableStatus.Closed:
                                return;
                            case SwitchableStatus.QueuingOpen:
                                if (OnCloseEnd != null) { OnCloseEnd.Invoke(true); }
                                if (OnBeforeOpen != null) { OnBeforeOpen.Invoke(false); }
                                return;
                            case SwitchableStatus.Opening:
                                if (OnBeforeOpen != null) { OnBeforeOpen.Invoke(false); }
                                return;
                            case SwitchableStatus.Opened:
                                if (OnBeforeOpen != null) { OnBeforeOpen.Invoke(true); }
                                return;
                            case SwitchableStatus.Closing:
                                return;
                        }
                        break;
                }

                Debug.LogWarning("Wrong switchable status changing from " + Status + " to " + incomingStatus);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void InvokeEventForStatusChange(bool snap, SwitchableStatus prevStatus)
        {
            try
            {
                switch (prevStatus)
                {
                    case SwitchableStatus.Closed:
                        switch (Status)
                        {
                            case SwitchableStatus.QueuingOpen:
                                if (OnSwitch != null) { OnSwitch.Invoke(false); }
                                return;
                            case SwitchableStatus.Opening:
                                if (OnSwitch != null) { OnSwitch.Invoke(false); }
                                if (OnOpenStart != null) { OnOpenStart.Invoke(false); }
                                return;
                            case SwitchableStatus.Opened:
                                if (OnSwitch != null) { OnSwitch.Invoke(true); }
                                if (OnOpenStart != null) { OnOpenStart.Invoke(true); }
                                if (OnOpenEnd != null) { OnOpenEnd.Invoke(true); }
                                return;
                            //case SwitchableStatus.Closing:
                            case SwitchableStatus.Closed:
                                return;
                        }
                        break;
                    case SwitchableStatus.QueuingOpen:
                        switch (Status)
                        {
                            case SwitchableStatus.Opening:
                                if (OnOpenStart != null) { OnOpenStart.Invoke(false); }
                                return;
                            case SwitchableStatus.Opened:
                                if (OnOpenStart != null) { OnOpenStart.Invoke(true); }
                                if (OnOpenEnd != null) { OnOpenEnd.Invoke(true); }
                                return;
                            //case SwitchableStatus.Closing:
                            case SwitchableStatus.Closed:
                                if (OnSwitch != null) { OnSwitch.Invoke(snap); }
                                return;
                            case SwitchableStatus.QueuingOpen:
                                return;
                        }
                        break;
                    case SwitchableStatus.Opening:
                        switch (Status)
                        {
                            case SwitchableStatus.Opened:
                                if (OnOpenEnd != null) { OnOpenEnd.Invoke(snap); }
                                return;
                            case SwitchableStatus.Closing:
                                if (OnOpenEnd != null) { OnOpenEnd.Invoke(false); }
                                if (OnSwitch != null) { OnSwitch.Invoke(false); }
                                if (OnCloseStart != null) { OnCloseStart.Invoke(false); }
                                return;
                            case SwitchableStatus.Closed:
                                if (OnOpenEnd != null) { OnOpenEnd.Invoke(true); }
                                if (OnSwitch != null) { OnSwitch.Invoke(true); }
                                if (OnCloseStart != null) { OnCloseStart.Invoke(true); }
                                if (OnCloseEnd != null) { OnCloseEnd.Invoke(true); }
                                return;
                            //case SwitchableStatus.QueuingOpen:
                            case SwitchableStatus.Opening:
                                return;
                        }
                        break;
                    case SwitchableStatus.Opened:
                        switch (Status)
                        {
                            case SwitchableStatus.Closing:
                                if (OnSwitch != null) { OnSwitch.Invoke(false); }
                                if (OnCloseStart != null) { OnCloseStart.Invoke(false); }
                                return;
                            case SwitchableStatus.Closed:
                                if (OnSwitch != null) { OnSwitch.Invoke(true); }
                                if (OnCloseStart != null) { OnCloseStart.Invoke(true); }
                                if (OnCloseEnd != null) { OnCloseEnd.Invoke(true); }
                                return;
                            //case SwitchableStatus.Opening:
                            //case SwitchableStatus.QueuingOpen:
                            case SwitchableStatus.Opened:
                                return;
                        }
                        break;
                    case SwitchableStatus.Closing:
                        switch (Status)
                        {
                            case SwitchableStatus.Closed:
                                if (OnCloseEnd != null) { OnCloseEnd.Invoke(snap); }
                                return;
                            case SwitchableStatus.QueuingOpen:
                                if (OnSwitch != null) { OnSwitch.Invoke(snap); }
                                break;
                            case SwitchableStatus.Opening:
                                if (OnCloseEnd != null) { OnCloseEnd.Invoke(false); }
                                if (OnSwitch != null) { OnSwitch.Invoke(false); }
                                if (OnOpenStart != null) { OnOpenStart.Invoke(false); }
                                return;
                            case SwitchableStatus.Opened:
                                if (OnCloseEnd != null) { OnCloseEnd.Invoke(true); }
                                if (OnSwitch != null) { OnSwitch.Invoke(true); }
                                if (OnOpenStart != null) { OnOpenStart.Invoke(true); }
                                if (OnOpenEnd != null) { OnOpenEnd.Invoke(true); }
                                return;
                            case SwitchableStatus.Closing:
                                return;
                        }
                        break;
                }

                Debug.LogWarning("Wrong switchable status changing from " + prevStatus + " to " + Status);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}