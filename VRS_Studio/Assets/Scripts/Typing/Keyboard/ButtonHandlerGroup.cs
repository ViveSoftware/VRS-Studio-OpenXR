using System;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.ViveSoftware.ExpLab.HandInteractionDemo
{
    public class ButtonHandlerGroup : MonoBehaviour
    {
        [Serializable]
        public class UnityEventKey : UnityEvent<string, KeyCode> { }

        [SerializeField]
        private AnimationCurve buttonPressAnimCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField]
        private AnimationCurve buttonReleaseAnimCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField]
        private float buttonPressDuration = 0.15f;
        [SerializeField]
        private float buttonReleaseDuration = 0.1f;
        [SerializeField]
        private float pressedDepth = 15f;
        [SerializeField]
        private AudioClip keyHoveredClip;
        [SerializeField]
        private AudioClip keyDownClip;
        [SerializeField]
        private AudioClip keyUpClip;
        [SerializeField]
        private UnityEventKey onKeyDown = new UnityEventKey();
        [SerializeField]
        private UnityEventKey onKeyUp = new UnityEventKey();
        [SerializeField]
        private UnityEventKey onKeyClick = new UnityEventKey();

        public AnimationCurve ButtonPressAnimCurve { get { return buttonPressAnimCurve; } }
        public AnimationCurve ButtonReleaseAnimCurve { get { return buttonReleaseAnimCurve; } }
        public float ButtonPressDuration { get { return buttonPressDuration; } set { buttonPressDuration = value; } }
        public float ButtonReleaseDuration { get { return buttonReleaseDuration; } set { buttonReleaseDuration = value; } }
        public float PressedDepth { get { return pressedDepth; } set { pressedDepth = value; } }
        public AudioClip KeyHoveredClip { get { return keyHoveredClip; } set { keyHoveredClip = value; } }
        public AudioClip KeyDownClip { get { return keyDownClip; } set { keyDownClip = value; } }
        public AudioClip KeyUpClip { get { return keyUpClip; } set { keyUpClip = value; } }

        public event UnityAction<string, KeyCode> OnKeyDown { add { onKeyDown.AddListener(value); } remove { onKeyDown.RemoveListener(value); } }
        public event UnityAction<string, KeyCode> OnKeyUp { add { onKeyUp.AddListener(value); } remove { onKeyUp.RemoveListener(value); } }
        public event UnityAction<string, KeyCode> OnKeyClick { add { onKeyClick.AddListener(value); } remove { onKeyClick.RemoveListener(value); } }

        public void NotifyKeyDown(string keyCmd, KeyCode keyCode)
        {
            if (onKeyDown != null) { onKeyDown.Invoke(keyCmd, keyCode); }
        }

        public void NotifyKeyUp(string keyCmd, KeyCode keyCode)
        {
            if (onKeyUp != null) { onKeyUp.Invoke(keyCmd, keyCode); }
        }

        public void NotifyKeyClick(string keyCmd, KeyCode keyCode)
        {
            if (onKeyClick != null) { onKeyClick.Invoke(keyCmd, keyCode); }
        }
    }
}