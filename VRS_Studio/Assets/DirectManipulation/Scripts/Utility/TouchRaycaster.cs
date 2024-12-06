using UnityEngine.EventSystems;

namespace HTC.UnityPlugin.Pointer3D
{
    public class TouchRaycaster : Pointer3DRaycaster
    {
        public float mouseButtonLeftRange = 0.02f;

        private float prevHitRange = float.MaxValue;
        private float currHitRange = float.MaxValue;
        public float PreviousFrameHitRange { get { return prevHitRange; } }
        public float CurrentFrameHitRange { get { return currHitRange; } }

        public float GetButtonHitRange(PointerEventData.InputButton btn)
        {
            switch (btn)
            {
                default:
                case PointerEventData.InputButton.Left: return mouseButtonLeftRange;
            }
        }

        protected override void Start()
        {
            base.Start();
            buttonEventDataList.Add(new TouchEventData(this, EventSystem.current, PointerEventData.InputButton.Left));
        }

        public override void Raycast()
        {
            base.Raycast();

            prevHitRange = currHitRange;

            var hitResult = FirstRaycastResult();
            currHitRange = hitResult.isValid ? hitResult.distance : float.MaxValue;
        }

        public class TouchEventData : Pointer3DEventData
        {
            public TouchRaycaster touchPointerRaycaster { get; private set; }

            public TouchEventData(TouchRaycaster ownerRaycaster, EventSystem eventSystem, InputButton btn) : base(ownerRaycaster, eventSystem)
            {
                touchPointerRaycaster = ownerRaycaster;
                button = btn;
            }

            public override bool GetPress()
            {
                var hitRange = touchPointerRaycaster.GetButtonHitRange(button);
                return touchPointerRaycaster.CurrentFrameHitRange <= hitRange;
            }

            public override bool GetPressDown()
            {
                var hitRange = touchPointerRaycaster.GetButtonHitRange(button);
                return touchPointerRaycaster.PreviousFrameHitRange > hitRange && touchPointerRaycaster.CurrentFrameHitRange <= hitRange;
            }

            public override bool GetPressUp()
            {
                var hitRange = touchPointerRaycaster.GetButtonHitRange(button);
                return touchPointerRaycaster.PreviousFrameHitRange <= hitRange && touchPointerRaycaster.CurrentFrameHitRange > hitRange;
            }
        }
    }
}