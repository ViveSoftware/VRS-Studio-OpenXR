#pragma warning disable 0649
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.CommonEventVariable
{
    public abstract class CommonEventBehaviourBase : CommonHandlerBehaviour { }

    public class CommonEventBehaviour : CommonEventBehaviourBase
    {
        [SerializeField]
        private CommonEventAsset eventAsset;
        [SerializeField]
        private UnityEvent onTrigger;

        protected override CommonHandlerAsset HandlerAssetBase { get { return eventAsset; } }

        private void OnEnable()
        {
            if (eventAsset != null)
            {
                eventAsset.Handler.OnTrigger += OnEventTrigger;
            }
            else
            {
                Debug.LogWarning("CommonEventAsset not assigned!");
            }
        }

        private void OnDisable()
        {
            if (eventAsset != null)
            {
                eventAsset.Handler.OnTrigger -= OnEventTrigger;
            }
        }

        private void OnEventTrigger()
        {
            if (onTrigger != null) { onTrigger.Invoke(); }
        }
    }

    public abstract class CommonEventBehaviour<T, TAsset, TUnityEvent> : CommonEventBehaviourBase
        where TAsset : CommonEventAsset<T, TUnityEvent>
        where TUnityEvent : UnityEvent<T>
    {
        [SerializeField]
        private TAsset eventAsset;
        [SerializeField]
        private TUnityEvent onTrigger;

        protected override CommonHandlerAsset HandlerAssetBase { get { return eventAsset; } }

        private void OnEnable()
        {
            if (eventAsset != null)
            {
                eventAsset.Handler.OnTrigger += OnEventTrigger;
            }
            else
            {
                Debug.LogWarning("CommonEventAsset not assigned!");
            }
        }

        private void OnDisable()
        {
            if (eventAsset != null)
            {
                eventAsset.Handler.OnTrigger -= OnEventTrigger;
            }
        }

        private void OnEventTrigger(T value)
        {
            if (onTrigger != null) { onTrigger.Invoke(value); }
        }
    }

    public abstract class CommonEventBehaviour<T1, T2, TAsset, TUnityEvent> : CommonEventBehaviourBase
        where TAsset : CommonEventAsset<T1, T2, TUnityEvent>
        where TUnityEvent : UnityEvent<T1, T2>
    {
        [SerializeField]
        private TAsset eventAsset;
        [SerializeField]
        private TUnityEvent onTrigger;

        protected override CommonHandlerAsset HandlerAssetBase { get { return eventAsset; } }

        private void OnEnable()
        {
            if (eventAsset != null)
            {
                eventAsset.Handler.OnTrigger += OnEventTrigger;
            }
            else
            {
                Debug.LogWarning("CommonEventAsset not assigned!");
            }
        }

        private void OnDisable()
        {
            if (eventAsset != null)
            {
                eventAsset.Handler.OnTrigger -= OnEventTrigger;
            }
        }

        private void OnEventTrigger(T1 value1, T2 value2)
        {
            if (onTrigger != null) { onTrigger.Invoke(value1, value2); }
        }
    }
}
