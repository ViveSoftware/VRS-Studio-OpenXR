#pragma warning disable 0649
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HTC.Triton.LensUI
{
    public class LensUIButton : LensUISelectable
        , IPointerClickHandler
        , ISubmitHandler
    {
        public delegate void ButtonClickedHandler();

        [SerializeField]
        private bool repeat;
        [SerializeField]
        private bool repeatAnimation;
        [SerializeField]
        private float repeatDelay = 0.5f;
        [SerializeField]
        private float repeatInterval = 0.25f;

        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("onClick")]
        private Button.ButtonClickedEvent _onClick = new Button.ButtonClickedEvent();

        public bool Repeat { get { return repeat; } set { repeat = value; } }

        public bool RepeatAnimation { get { return repeatAnimation; } set { repeatAnimation = value; } }

        public float RepeatDelay { get { return repeatDelay; } set { repeatDelay = value; } }

        public float RepeatInterval { get { return repeatInterval; } set { repeatInterval = value; } }

        public Button.ButtonClickedEvent onClick { get { return _onClick; } set { _onClick = value; } }

        public event ButtonClickedHandler OnClick;

        private void Press()
        {
            if (!IsActive() || !IsInteractable()) { return; }

            UISystemProfilerApi.AddMarker("LensUIButton.onClick", this);
            if (onClick != null) { onClick.Invoke(); }
            if (OnClick != null) { OnClick.Invoke(); }
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (pressedCount == 0 && IsEffectiveButton(eventData))
            {
                Press();
            }
        }

        public virtual void OnLensUISubmit(BaseEventData eventData)
        {
            if (pressedCount == 0)
            {
                Press();
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            var wasPressed = IsPressed();

            base.OnPointerDown(eventData);

            if (repeat && !wasPressed && IsPressed())
            {
                StartRepeat();
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            var wasPressed = IsPressed();

            base.OnPointerUp(eventData);

            if (isRepeating && wasPressed && !IsPressed())
            {
                StopRepeat();
            }
        }

        private void StartRepeat()
        {
            repeatIsDone = false;
            nextRepeatTime = Time.unscaledTime + repeatDelay;
            if (repeatCoroutine == null)
            {
                repeatCoroutine = StartCoroutine(RepeatCoroutine());
            }
        }

        private void StopRepeat()
        {
            repeatIsDone = true;
        }

        private Coroutine repeatCoroutine;
        private bool repeatIsDone = true;
        private float nextRepeatTime;
        private bool isRepeating { get { return !repeatIsDone; } }
        private IEnumerator RepeatCoroutine()
        {
            var isTranisitioning = false;
            var nextTransitTime = 0f;

            while (!repeatIsDone)
            {
                var now = Time.unscaledTime;

                if (now >= nextRepeatTime)
                {
                    Press();

                    nextRepeatTime += repeatInterval;

                    if (repeatAnimation)
                    {
                        DoStateTransition(SelectionState.Highlighted, false);
                        isTranisitioning = true;
                        nextTransitTime = now + Mathf.Min(repeatInterval / 2f, colors.fadeDuration);
                    }
                }

                if (isTranisitioning && now >= nextTransitTime)
                {
                    DoStateTransition(currentSelectionState, false);
                    isTranisitioning = false;
                }

                yield return null;
            }

            repeatCoroutine = null;
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();

            if (!IsActive() || !IsInteractable()) { return; }

            DoStateTransition(SelectionState.Pressed, false);

            submitTransitEndTime = Time.unscaledTime + colors.fadeDuration;
            if (submitTransitCoroutine == null)
            {
                submitTransitCoroutine = StartCoroutine(SubmitTransition());
            }
        }

        private Coroutine submitTransitCoroutine;
        private float submitTransitEndTime;
        private IEnumerator SubmitTransition()
        {
            while (true)
            {
                if (Time.unscaledTime >= submitTransitEndTime)
                {
                    DoStateTransition(currentSelectionState, false);
                    break;
                }

                yield return null;
            }

            submitTransitCoroutine = null;
        }
    }
}