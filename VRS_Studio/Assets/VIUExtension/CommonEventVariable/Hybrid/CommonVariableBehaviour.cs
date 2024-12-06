#pragma warning disable 0649
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.CommonEventVariable
{
    public abstract class CommonVariableBehaviourBase : CommonHandlerBehaviour { }

    public abstract class CommonVariableBehaviour<T, TAsset, TUnityEvent> : CommonVariableBehaviourBase
        where TAsset : CommonVariableAsset<T, TUnityEvent>
        where TUnityEvent : UnityEvent<T>
    {
        [SerializeField]
        private TAsset variableAsset;
        [SerializeField]
        private TUnityEvent onChange;

        protected override CommonHandlerAsset HandlerAssetBase { get { return variableAsset; } }

        private void OnEnable()
        {
            if (variableAsset != null)
            {
                variableAsset.Handler.OnChange += OnVarChange;
            }
            else
            {
                Debug.LogWarning("CommonVariableAsset not assigned!");
            }
        }

        private void OnDisable()
        {
            if (variableAsset != null)
            {
                variableAsset.Handler.OnChange -= OnVarChange;
            }
        }

        private void OnVarChange()
        {
            if (onChange != null) { onChange.Invoke(variableAsset.Handler.CurrentValue); }
        }
    }
}