#pragma warning disable 0649
using UnityEngine;

namespace HTC.UnityPlugin.Utility.Switchable
{
    public class SwitchableAnimationTrigger : MonoBehaviour
        , ISwitchableOpenStartHandler
        , ISwitchableOpenEndHandler
        , ISwitchableCloseStartHandler
        , ISwitchableCloseEndHandler
    {
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private bool keepStateOnDisable = true;
        [SerializeField]
        private string openStartTrigger = "OpenStart";
        [SerializeField]
        private string openSnapTrigger = "OpenSnap";
        [SerializeField]
        private string closeStartTrigger = "CloseStart";
        [SerializeField]
        private string closeSnapTrigger = "CloseSnap";

        private bool isAnimatorDirty = true;
        private int openStartTriggerID;
        private int openSnapTriggerID;
        private int closeStartTriggerID;
        private int closeSnapTriggerID;

        private Switchable lastSwitchable;

        public Animator Animator
        {
            get
            {
                return animator;
            }
            set
            {
                animator = value;
                if (value != null && animator != value) { SetAnimatorDirty(); }
            }
        }

        public bool KeepStateOnDisable
        {
            get
            {
                return keepStateOnDisable;
            }
            set
            {
                keepStateOnDisable = value;
                if (animator != null) { animator.keepAnimatorStateOnDisable = value; }
            }
        }

        public string OpenStartTrigger
        {
            get { return openStartTrigger; }
            set { openStartTrigger = value; openStartTriggerID = GetValidTriggerID(value); }
        }

        public string OpenSnapTrigger
        {
            get { return openSnapTrigger; }
            set { openSnapTrigger = value; openSnapTriggerID = GetValidTriggerID(value); }
        }

        public string CloseStartTrigger
        {
            get { return closeStartTrigger; }
            set { closeStartTrigger = value; closeStartTriggerID = GetValidTriggerID(value); }
        }

        public string CloseSnapTrigger
        {
            get { return closeSnapTrigger; }
            set { closeSnapTrigger = value; closeSnapTriggerID = GetValidTriggerID(value); }
        }

        public void SetAnimatorDirty() { isAnimatorDirty = true; }

        public void RefreshAnimator(bool force = false)
        {
            if ((isAnimatorDirty || force) && animator != null)
            {
                animator.keepAnimatorStateOnDisable = keepStateOnDisable;
                openStartTriggerID = GetValidTriggerID(openStartTrigger);
                openSnapTriggerID = GetValidTriggerID(openSnapTrigger);
                closeStartTriggerID = GetValidTriggerID(closeStartTrigger);
                closeSnapTriggerID = GetValidTriggerID(closeSnapTrigger);
            }
        }
#if UNITY_EDITOR
        protected virtual void Reset()
        {
            animator = GetComponentInChildren<Animator>();

            if (animator == null)
            {
                animator = gameObject.AddComponent<Animator>();
            }
        }
#endif
        protected virtual void Awake()
        {
            RefreshAnimator();
        }

        public void NotifyLastSwitchableOpenEnd()
        {
            if (lastSwitchable != null) { lastSwitchable.OpenEnd(); }
        }

        public void NotifyLastSwitchableCloseEnd()
        {
            if (lastSwitchable != null) { lastSwitchable.CloseEnd(); }
        }

        void ISwitchableOpenStartHandler.OnSwitchableOpenStart(Switchable switchable, bool snap)
        {
            if (!snap)
            {
                RefreshAnimator();
                lastSwitchable = switchable;
                InternalSetTrigger(openStartTriggerID);
            }
        }

        void ISwitchableOpenEndHandler.OnSwitchableOpenEnd(Switchable switchable, bool snap)
        {
            if (snap && switchable.Status == SwitchableStatus.Opened)
            {
                RefreshAnimator();
                InternalSetTrigger(openSnapTriggerID);
            }
        }

        void ISwitchableCloseStartHandler.OnSwitchableCloseStart(Switchable switchable, bool snap)
        {
            if (!snap)
            {
                RefreshAnimator();
                lastSwitchable = switchable;
                InternalSetTrigger(closeStartTriggerID);
            }
        }

        void ISwitchableCloseEndHandler.OnSwitchableCloseEnd(Switchable switchable, bool snap)
        {
            if (snap && switchable.Status == SwitchableStatus.Closed)
            {
                RefreshAnimator();
                InternalSetTrigger(closeSnapTriggerID);
            }
        }

        private void InternalSetTrigger(int id)
        {
            if (animator == null) { return; }
            animator.ResetTrigger(openStartTriggerID);
            animator.ResetTrigger(openSnapTriggerID);
            animator.ResetTrigger(closeStartTriggerID);
            animator.ResetTrigger(closeSnapTriggerID);
            if (id != 0) { animator.SetTrigger(id); }
        }

        private int GetValidTriggerID(string name)
        {
            if (animator != null)
            {
                var id = Animator.StringToHash(name);
                if (id != 0)
                {
                    for (int i = animator.parameterCount - 1; i >= 0; --i)
                    {
                        var parameter = animator.GetParameter(i);
                        if (parameter.nameHash == id && parameter.type == AnimatorControllerParameterType.Trigger)
                        {
                            return id;
                        }
                    }

                    var msg = "Can't find trigger named " + name + " in target animator. [ ";
                    for (int i = 0, imax = animator.parameterCount; i < imax; ++i)
                    {
                        var parameter = animator.GetParameter(i);
                        if (parameter.type == AnimatorControllerParameterType.Trigger)
                        {
                            msg += parameter.name + " ";
                        }
                    }
                    Debug.LogWarning(msg + "]");
                }
            }

            return 0;
        }
    }
}