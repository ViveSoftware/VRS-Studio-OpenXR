#pragma warning disable 0649
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.CommonEventVariable
{
    [CreateAssetMenu(menuName = "Common Event/Event String", fileName = "CommonEventString")]
    public class CommonEventAssetString : CommonEventAsset<string, CommonEventAssetString.TriggerEvent>
    {
        [Serializable]
        public class TriggerEvent : UnityEvent<string> { }
    }
}