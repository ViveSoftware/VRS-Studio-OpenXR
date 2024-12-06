#pragma warning disable 0649
using UnityEngine;

namespace HTC.UnityPlugin.CommonEventVariable
{
    public abstract class CommonHandlerAsset : ScriptableObject
    {
        [SerializeField]
        protected bool enableDebugMessage;
        [SerializeField]
        [Tooltip("Event/Var handle name will be asset name if not overridden")]
        private string overrideHandleName;
        [SerializeField]
        [TextArea(1, 20)]
        private string description;

        protected abstract CommonHandler HandlerBase { get; }

        public string HandlerName { get { return string.IsNullOrEmpty(overrideHandleName) ? name : overrideHandleName; } }

        public bool EnableDebugMessage
        {
            get
            {
                return HandlerBase == null ? enableDebugMessage : HandlerBase.EnableDebugMessage;
            }
            set
            {
                enableDebugMessage = value;
                if (HandlerBase != null) { HandlerBase.EnableDebugMessage = value; }
            }
        }
    }
}