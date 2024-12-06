#pragma warning disable 0649
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.CommonEventVariable
{
    [CreateAssetMenu(menuName = "Common Event/Event Int Int", fileName = "CommonEventIntInt")]
    public class CommonEventAssetIntInt : CommonEventAsset<int, int, CommonEventAssetIntInt.TriggerEvent>
    {
        [Serializable]
        public class TriggerEvent : UnityEvent<int, int> { }
    }
}