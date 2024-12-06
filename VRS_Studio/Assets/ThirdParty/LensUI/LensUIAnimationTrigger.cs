#pragma warning disable 0649
using UnityEngine;

namespace HTC.Triton.LensUI
{
    public class LensUIAnimationTrigger : MonoBehaviour
        , ILensUISelectableStateTransitionHandler
    {
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private bool keepStateOnDisable;
        [SerializeField]
        private string normalTrigger = "Normal";
        [SerializeField]
        private string highlightedTrigger = "Highlighted";
        [SerializeField]
        private string pressedTrigger = "Pressed";
        [SerializeField]
        private string selectedTrigger = "Selected";
        [SerializeField]
        private string disabledTrigger = "Disabled";

        private bool isAnimatorDirty = true;
        private int normalTriggerID;
        private int highlightedTriggerID;
        private int pressedTriggerID;
        private int selectedTriggerID;
        private int disabledTriggerID;

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

        public string NormalTrigger
        {
            get { return normalTrigger; }
            set { normalTrigger = value; normalTriggerID = GetValidTriggerID(value); }
        }

        public string HighlightedTrigger
        {
            get { return highlightedTrigger; }
            set { highlightedTrigger = value; highlightedTriggerID = GetValidTriggerID(value); }
        }

        public string PressedTrigger
        {
            get { return pressedTrigger; }
            set { pressedTrigger = value; pressedTriggerID = GetValidTriggerID(value); }
        }

        public string SelectedTrigger
        {
            get { return selectedTrigger; }
            set { selectedTrigger = value; selectedTriggerID = GetValidTriggerID(value); }
        }

        public string DisabledTrigger
        {
            get { return disabledTrigger; }
            set { disabledTrigger = value; disabledTriggerID = GetValidTriggerID(value); }
        }

        public void SetAnimatorDirty() { isAnimatorDirty = true; }

        public void RefreshAnimator(bool force = false)
        {
            if ((isAnimatorDirty || force) && animator != null)
            {
                animator.keepAnimatorStateOnDisable = keepStateOnDisable;
                normalTriggerID = GetValidTriggerID(normalTrigger);
                highlightedTriggerID = GetValidTriggerID(highlightedTrigger);
                pressedTriggerID = GetValidTriggerID(pressedTrigger);
                selectedTriggerID = GetValidTriggerID(selectedTrigger);
                disabledTriggerID = GetValidTriggerID(disabledTrigger);
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

        void ILensUISelectableStateTransitionHandler.OnLensUISelectableStateTransition(LensUISelectable selectable, LensUISelectable.State state, bool snap)
        {
            RefreshAnimator();

            switch (state)
            {
                case LensUISelectable.State.Normal: InternalSetTrigger(normalTriggerID); break;
                case LensUISelectable.State.Highlighted: InternalSetTrigger(highlightedTriggerID); break;
                case LensUISelectable.State.Pressed: InternalSetTrigger(pressedTriggerID); break;
                case LensUISelectable.State.Selected: InternalSetTrigger(selectedTriggerID); break;
                case LensUISelectable.State.Disabled: InternalSetTrigger(disabledTriggerID); break;
            }
        }

        private void InternalSetTrigger(int id)
        {
            if (animator == null) { return; }
            animator.ResetTrigger(normalTriggerID);
            animator.ResetTrigger(highlightedTriggerID);
            animator.ResetTrigger(pressedTriggerID);
            animator.ResetTrigger(selectedTriggerID);
            animator.ResetTrigger(disabledTriggerID);
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