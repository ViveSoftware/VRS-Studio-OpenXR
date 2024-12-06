#pragma warning disable 0649
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.CommonEventVariable
{
    public abstract class CommonVariableAssetBase : CommonHandlerAsset
    {
        protected sealed override CommonHandler HandlerBase { get { return VariableHandlerBase; } }
        public abstract CommonVariableHandlerBase VariableHandlerBase { get; }

        public int SetValueCount { get { return VariableHandlerBase.SetValueCount; } }

        protected abstract void InitHandler();

        public abstract void SetAssetValue();

        private void OnEnable() { if (Application.isPlaying) { InitHandler(); } }

        public void ResetSetValueCount() { VariableHandlerBase.ResetSetValueCount(); }
    }

    public abstract class CommonVariableAsset<T, TUnityEvent> : CommonVariableAssetBase
        where TUnityEvent : UnityEvent<T>
    {
        [SerializeField]
        private T value;
        [SerializeField]
        private TUnityEvent onChange;

        private CommonVariableHandler<T> handler;

        public sealed override CommonVariableHandlerBase VariableHandlerBase { get { return Handler; } }
        public CommonVariableHandler<T> Handler { get { InitHandler(); return handler; } }

        protected sealed override void InitHandler()
        {
            if (handler == null)
            {
                handler = CommonVariable.Get<T>(HandlerName, enableDebugMessage);
                handler.OnChange += OnChange;
            }
        }

        public sealed override void SetAssetValue() { SetValue(value); }

        public void SetValue(T value) { Handler.SetValue(value); }

        private void OnChange() { if (onChange != null) { onChange.Invoke(Handler.CurrentValue); } }
    }
}
