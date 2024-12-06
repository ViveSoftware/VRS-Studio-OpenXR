#pragma warning disable 0649
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.CommonEventVariable
{
    [CreateAssetMenu(menuName = "Common Event/Event Quaternion", fileName = "CommonEventQuaternion", order = 1)]
    public class CommonEventAssetQuaternion : CommonEventAsset<Quaternion, CommonEventAssetQuaternion.TriggerEvent>
    {
        [Serializable]
        public class TriggerEvent : UnityEvent<Quaternion> { }
    }
}