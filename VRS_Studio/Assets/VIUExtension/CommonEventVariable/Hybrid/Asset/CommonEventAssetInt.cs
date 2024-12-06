#pragma warning disable 0649
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.CommonEventVariable
{
    [CreateAssetMenu(menuName = "Common Event/Event Int", fileName = "CommonEventInt")]
    public class CommonEventAssetInt : CommonEventAsset<int, CommonEventAssetInt.TriggerEvent>
    {
        [Serializable]
        public class TriggerEvent : UnityEvent<int> { }
    }
}