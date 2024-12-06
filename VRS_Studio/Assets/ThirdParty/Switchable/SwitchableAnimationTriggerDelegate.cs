#pragma warning disable 0649
using UnityEngine;

namespace HTC.UnityPlugin.Utility.Switchable
{
    public class SwitchableAnimationTriggerDelegate : MonoBehaviour
    {
        [SerializeField]
        private SwitchableAnimationTrigger animationTrigger;

        public SwitchableAnimationTrigger AnimationTrigger { get { return animationTrigger; } set { animationTrigger = value; } }

        public void OpenEnd()
        {
            if (animationTrigger != null) { animationTrigger.NotifyLastSwitchableOpenEnd(); }
        }

        public void CloseEnd()
        {
            if (animationTrigger != null) { animationTrigger.NotifyLastSwitchableCloseEnd(); }
        }
    }
}