#pragma warning disable 0649
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.CommonEventVariable
{
    [CreateAssetMenu(menuName = "Common Event/Event Bool", fileName = "CommonEventBool", order = 1)]
    public class CommonEventAssetBool : CommonEventAsset<bool, CommonEventAssetBool.TriggerEvent>
    {
        [Serializable]
        public class TriggerEvent : UnityEvent<bool> { }
    }
}