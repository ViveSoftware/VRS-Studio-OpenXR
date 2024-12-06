using System;
using UnityEngine;

namespace HTC.UnityPlugin.Utility.Switchable
{
    public enum GroupStatus
    {
        AllClosed,
        OneOpened,
        QueuedAndClosing,
        OneClosing,
    }

    public abstract class SwitchableGroup
    {
        public delegate void Callback(GroupStatus status);

        public GroupStatus Status { get; protected set; }
        public virtual bool SequentialSwitch { get; set; }
        public abstract Switchable OpenedMember { get; }
        public abstract Switchable ClosingMember { get; }
        public abstract void RegisterMember(Switchable newMember);
        public abstract void UnregisterMember(Switchable member);

        public abstract event Callback OnStatusChange;
    }

    public partial class Switchable
    {
        protected class InternalGroup : SwitchableGroup
        {
            private Switchable openedMember;
            private Switchable closingMember;
            private bool isChangingStatus;

            public override event Callback OnStatusChange;

            public override Switchable OpenedMember { get { return openedMember; } }

            public override Switchable ClosingMember { get { return closingMember; } }

            public override bool SequentialSwitch
            {
                get { return base.SequentialSwitch; }
                set
                {
                    if (base.SequentialSwitch == value) { return; }

                    if (BeginChangingStatusGuardWarn())
                    {
                        base.SequentialSwitch = value;

                        if (openedMember != null && closingMember != null)
                        {
                            ChangeToStatus(true, openedMember, null, (openedMember, SwitchableStatus.Opened), (closingMember, SwitchableStatus.Closed));
                        }

                        EndChangingStatusGuard();
                    }
                }
            }

            public override void RegisterMember(Switchable member)
            {
                if (member.group != null) { throw new Exception("Already in a group"); }
                Debug.Assert(member.Status != SwitchableStatus.QueuingOpen);
                Debug.Assert(openedMember != member);
                Debug.Assert(closingMember != member);

                if (member.IsOpen)
                {
                    if (BeginChangingStatusGuardWarn())
                    {
                        member.group = this;

                        ChangeToStatus(true, member, null, (member, SwitchableStatus.Opened), (openedMember, SwitchableStatus.Closed), (closingMember, SwitchableStatus.Closed));

                        EndChangingStatusGuard();
                    }
                }
                else
                {
                    member.group = this;

                    ChangeToStatus(true, openedMember, closingMember, (member, SwitchableStatus.Closed));
                }
            }

            public override void UnregisterMember(Switchable member)
            {
                if (member.group != this) { return; }

                if (member == openedMember || member == closingMember)
                {
                    if (BeginChangingStatusGuardWarn())
                    {
                        member.group = null;

                        if (member == openedMember)
                        {
                            ChangeToStatus(true, null, closingMember, (member, SwitchableStatus.Opened), (closingMember, SwitchableStatus.Closed));
                        }
                        else // if (member == closingMember)
                        {
                            ChangeToStatus(true, openedMember, null, (member, SwitchableStatus.Closed), (openedMember, SwitchableStatus.Opened));
                        }

                        EndChangingStatusGuard();
                    }
                }
                else
                {
                    member.group = null;

                    member.ChangeToStatus(true, member.IsOpen ? SwitchableStatus.Opened : SwitchableStatus.Closed);
                }
            }

            public void NotifyOpenStart(Switchable member)
            {
                Debug.Assert(member != openedMember);

                if (BeginChangingStatusGuardWarn())
                {
                    if (member == closingMember)
                    {
                        if (SequentialSwitch)
                        {
                            ChangeToStatus(false, member, null, (member, SwitchableStatus.Opening), (openedMember, SwitchableStatus.Closed));
                        }
                        else
                        {
                            ChangeToStatus(false, member, openedMember, (member, SwitchableStatus.Opening), (openedMember, SwitchableStatus.Closing));
                        }
                    }
                    else
                    {
                        if (SequentialSwitch)
                        {
                            if (closingMember == null)
                            {
                                if (openedMember == null)
                                {
                                    ChangeToStatus(false, member, null, (member, SwitchableStatus.Opening));
                                }
                                else
                                {
                                    ChangeToStatus(false, member, openedMember, (member, SwitchableStatus.QueuingOpen), (openedMember, SwitchableStatus.Closing));
                                }
                            }
                            else
                            {
                                ChangeToStatus(false, member, closingMember, (member, SwitchableStatus.QueuingOpen), (openedMember, SwitchableStatus.Closed));
                            }
                        }
                        else
                        {
                            ChangeToStatus(false, member, openedMember, (member, SwitchableStatus.Opening), (openedMember, SwitchableStatus.Closing));
                        }
                    }

                    // resolve Open/Close End call during above callbacks
                    if (openedMember.Status == SwitchableStatus.QueuingOpen)
                    {
                        if (closingMember.GetAnimateEndedFlag())
                        {
                            ChangeToStatus(false, openedMember, null, (openedMember, SwitchableStatus.Opening), (closingMember, SwitchableStatus.Closed));

                            if (openedMember.GetAnimateEndedFlag())
                            {
                                ChangeToStatus(false, openedMember, null, (openedMember, SwitchableStatus.Opened));
                            }
                        }
                    }
                    else // if(openedMember.Status == SwitchableStatus.Opening)
                    {
                        if (closingMember == null)
                        {
                            if (openedMember.GetAnimateEndedFlag())
                            {
                                ChangeToStatus(false, openedMember, null, (openedMember, SwitchableStatus.Opened));
                            }
                        }
                        else
                        {
                            if (openedMember.GetAnimateEndedFlag() && closingMember.GetAnimateEndedFlag())
                            {
                                ChangeToStatus(false, openedMember, null, (openedMember, SwitchableStatus.Opened), (closingMember, SwitchableStatus.Closed));
                            }
                            else if (openedMember.GetAnimateEndedFlag())
                            {
                                ChangeToStatus(false, openedMember, closingMember, (openedMember, SwitchableStatus.Opened));

                                if (closingMember.GetAnimateEndedFlag())
                                {
                                    ChangeToStatus(false, openedMember, null, (closingMember, SwitchableStatus.Closed));
                                }
                            }
                            else if (closingMember.GetAnimateEndedFlag())
                            {
                                ChangeToStatus(false, openedMember, null, (closingMember, SwitchableStatus.Closed));

                                if (openedMember.GetAnimateEndedFlag())
                                {
                                    ChangeToStatus(false, openedMember, null, (openedMember, SwitchableStatus.Opened));
                                }
                            }
                        }
                    }

                    EndChangingStatusGuard();
                }
            }

            public void NotifyOpenEnd(Switchable member)
            {
                Debug.Assert(member.Status == SwitchableStatus.Opening);
                Debug.Assert(Status == GroupStatus.OneOpened);
                Debug.Assert(member == openedMember);

                if (BeginChangingStatusGuard())
                {
                    ChangeToStatus(false, member, closingMember, (member, SwitchableStatus.Opened));

                    if (closingMember != null && closingMember.GetAnimateEndedFlag())
                    {
                        ChangeToStatus(false, member, null, (closingMember, SwitchableStatus.Closed));
                    }

                    EndChangingStatusGuard();
                }
                else
                {
                    member.SetAnimateEndedFlag();
                }
            }

            public void NotifySnapOpen(Switchable member)
            {
                if (BeginChangingStatusGuardWarn())
                {
                    if (member == openedMember)
                    {
                        ChangeToStatus(true, member, null, (member, SwitchableStatus.Opened), (closingMember, SwitchableStatus.Closed));
                    }
                    else if (member == closingMember)
                    {
                        ChangeToStatus(true, member, null, (member, SwitchableStatus.Opened), (openedMember, SwitchableStatus.Closed));
                    }
                    else
                    {
                        ChangeToStatus(true, member, null, (member, SwitchableStatus.Opened), (openedMember, SwitchableStatus.Closed), (closingMember, SwitchableStatus.Closed));
                    }

                    EndChangingStatusGuard();
                }
            }

            public void NotifyCloseStart(Switchable member)
            {
                Debug.Assert(member == openedMember);

                if (BeginChangingStatusGuardWarn())
                {
                    if (member.Status == SwitchableStatus.QueuingOpen)
                    {
                        ChangeToStatus(false, null, closingMember, (member, SwitchableStatus.Closed));
                    }
                    else // if (member.Status == SwitchableStatus.Opening || member.Status == SwitchableStatus.Opened)
                    {
                        ChangeToStatus(false, null, member, (member, SwitchableStatus.Closing));

                        if (closingMember.GetAnimateEndedFlag())
                        {
                            ChangeToStatus(false, null, null, (closingMember, SwitchableStatus.Closed));
                        }
                    }

                    EndChangingStatusGuard();
                }
            }

            public void NotifyCloseEnd(Switchable member)
            {
                Debug.Assert(member.Status == SwitchableStatus.Closing);
                Debug.Assert(member != openedMember);

                if (member != closingMember)
                {
                    ChangeToStatus(false, openedMember, closingMember, (member, SwitchableStatus.Closed));
                    // no callback triggered, no need to check openedMember animate end
                }
                else if (BeginChangingStatusGuard()) // member == closingMember
                {
                    if (SequentialSwitch)
                    {
                        ChangeToStatus(false, openedMember, null, (member, SwitchableStatus.Closed), (openedMember, SwitchableStatus.Opening));
                    }
                    else
                    {
                        ChangeToStatus(false, openedMember, null, (member, SwitchableStatus.Closed));
                    }

                    if (openedMember != null && openedMember.Status == SwitchableStatus.Opening && openedMember.GetAnimateEndedFlag())
                    {
                        ChangeToStatus(false, openedMember, null, (openedMember, SwitchableStatus.Opened));
                    }

                    EndChangingStatusGuard();
                }
                else
                {
                    member.SetAnimateEndedFlag();
                }
            }

            public void NotifySnapClose(Switchable member)
            {
                Debug.Assert(member.Status != SwitchableStatus.Closed);

                if (member != openedMember && member != closingMember)
                {
                    ChangeToStatus(true, openedMember, closingMember, (member, SwitchableStatus.Closed));
                }
                else if (BeginChangingStatusGuardWarn())
                {
                    if (member == openedMember)
                    {
                        ChangeToStatus(true, null, null, (member, SwitchableStatus.Closed), (closingMember, SwitchableStatus.Closed));
                    }
                    else // if (member == closingMember)
                    {
                        ChangeToStatus(true, openedMember, null, (member, SwitchableStatus.Closed), (openedMember, SwitchableStatus.Opened));
                    }

                    EndChangingStatusGuard();
                }
            }

            private bool BeginChangingStatusGuardWarn()
            {
                var passed = BeginChangingStatusGuard();
                if (!passed) { Debug.LogError("Attempt to change group status while alreading chagneing"); }
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

            private void ChangeToStatus(bool snap, Switchable incomingOpen, Switchable incomingClose, params (Switchable mb, SwitchableStatus st)[] chMbSt)
            {
                // invoke before member status change event
                foreach (var ms in chMbSt) { if (ms.mb != null) { ms.mb.InvokeEventBeforeStatusChange(snap, ms.st); } }

                // preserve last member status and change member status to new status
                for (var i = chMbSt.Length - 1; i >= 0; --i)
                {
                    if (chMbSt[i].mb != null)
                    {
                        chMbSt[i].mb.ResetAnimateEndedFlag();
                        (chMbSt[i].st, chMbSt[i].mb.Status) = (chMbSt[i].mb.Status, chMbSt[i].st);
                    }
                }

                if (incomingOpen != null) { incomingOpen.ResetAnimateEndedFlag(); }
                if (incomingClose != null) { incomingClose.ResetAnimateEndedFlag(); }

                // change group member status
                openedMember = incomingOpen;
                closingMember = incomingClose;

                // preserve last group status and change group status to new status
                GroupStatus newStatus;
                if (openedMember == null)
                {
                    newStatus = closingMember == null ? GroupStatus.AllClosed : GroupStatus.OneClosing;
                }
                else
                {
                    newStatus = openedMember.Status == SwitchableStatus.QueuingOpen ? GroupStatus.QueuedAndClosing : GroupStatus.OneOpened;
                }

                // invoke group status changed event if status changed
                if (Status != newStatus)
                {
                    Status = newStatus;
                    if (OnStatusChange != null) { OnStatusChange.Invoke(newStatus); }
                }

                // invoke member status changed event
                foreach (var ms in chMbSt) { if (ms.mb != null) { ms.mb.InvokeEventForStatusChange(snap, ms.st); } }
            }
        }
    }
}
