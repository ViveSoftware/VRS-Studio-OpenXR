#pragma warning disable 0649
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.CommonEventVariable
{
    public abstract class CommonEventAssetBase : CommonHandlerAsset
    {
        protected sealed override CommonHandler HandlerBase { get { return EventHandlerBase; } }
        public abstract CommonEventHandlerBase EventHandlerBase { get; }

        public abstract int ParamCount { get; }
        public int TriggerCount { get { return EventHandlerBase.TriggerCount; } }

        protected abstract void InitHandler();

        public abstract void Trigger();

        private void OnEnable() { if (Application.isPlaying) { InitHandler(); } }

        public void ResetTriggerCount() { EventHandlerBase.ResetTriggerCount(); }
    }

    [CreateAssetMenu(menuName = "Common Event/Event", fileName = "CommonEvent")]
    public class CommonEventAsset : CommonEventAssetBase
    {
        [SerializeField]
        private UnityEvent onTrigger;

        private CommonEventHandler handler;

        public override int ParamCount { get { return CommonEventHandler.PARAM_COUNT; } }
        public override CommonEventHandlerBase EventHandlerBase { get { return Handler; } }
        public CommonEventHandler Handler { get { InitHandler(); return handler; } }

        protected sealed override void InitHandler()
        {
            if (handler == null)
            {
                handler = CommonEvent.Get(HandlerName, enableDebugMessage);
                handler.OnTrigger += OnTrigger;
            }
        }

        public sealed override void Trigger() { Handler.Trigger(); }

        private void OnTrigger() { if (onTrigger != null) { onTrigger.Invoke(); } }
    }

    public abstract class CommonEventAsset<T, TUnityEvent> : CommonEventAssetBase
        where TUnityEvent : UnityEvent<T>
    {
        [SerializeField]
        protected T paramValue1;
        [SerializeField]
        private TUnityEvent onTrigger;

        private CommonEventHandler<T> handler;

        public sealed override int ParamCount { get { return CommonEventHandler<T>.PARAM_COUNT; } }
        public sealed override CommonEventHandlerBase EventHandlerBase { get { return Handler; } }
        public CommonEventHandler<T> Handler { get { InitHandler(); return handler; } }
        public T ParamValue1 { get { return paramValue1; } set { paramValue1 = value; } }

        protected sealed override void InitHandler()
        {
            if (handler == null)
            {
                handler = CommonEvent.Get<T>(HandlerName, enableDebugMessage);
                handler.OnTrigger += OnTrigger;
            }
        }

        public sealed override void Trigger() { Handler.Trigger(paramValue1); }

        public void Trigger1(T value) { Handler.Trigger(value); }

        private void OnTrigger(T value) { if (onTrigger != null) { onTrigger.Invoke(value); } }
    }

    public abstract class CommonEventAsset<T1, T2, TUnityEvent> : CommonEventAssetBase
        where TUnityEvent : UnityEvent<T1, T2>
    {
        [SerializeField]
        protected T1 paramValue1;
        [SerializeField]
        protected T2 paramValue2;
        [SerializeField]
        private TUnityEvent onTrigger;

        private CommonEventHandler<T1, T2> handler;

        public sealed override int ParamCount { get { return CommonEventHandler<T1, T2>.PARAM_COUNT; } }
        public sealed override CommonEventHandlerBase EventHandlerBase { get { return Handler; } }
        public CommonEventHandler<T1, T2> Handler { get { InitHandler(); return handler; } }
        public T1 ParamValue1 { get { return paramValue1; } set { paramValue1 = value; } }
        public T2 ParamValue2 { get { return paramValue2; } set { paramValue2 = value; } }

        protected sealed override void InitHandler()
        {
            if (handler == null)
            {
                handler = CommonEvent.Get<T1, T2>(HandlerName);
                handler.OnTrigger += OnTrigger;
            }

        }

        public sealed override void Trigger() { Handler.Trigger(paramValue1, paramValue2); }

        public void Trigger1(T1 value) { Handler.Trigger(value, paramValue2); }

        public void Trigger2(T2 value2) { Handler.Trigger(paramValue1, value2); }

        public void Trigger1And2(T1 value1, T2 value2) { Handler.Trigger(value1, value2); }

        private void OnTrigger(T1 value1, T2 value2) { if (onTrigger != null) { onTrigger.Invoke(value1, value2); } }
    }
}
