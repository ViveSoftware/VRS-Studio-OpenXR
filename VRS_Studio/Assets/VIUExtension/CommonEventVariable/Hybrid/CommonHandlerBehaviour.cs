#pragma warning disable 0649
using UnityEngine;

namespace HTC.UnityPlugin.CommonEventVariable
{
    public abstract class CommonHandlerBehaviour : MonoBehaviour
    {
        protected abstract CommonHandlerAsset HandlerAssetBase { get; }

        public string HandlerName { get { return HandlerAssetBase == null ? string.Empty : HandlerAssetBase.HandlerName; } }
    }
}
