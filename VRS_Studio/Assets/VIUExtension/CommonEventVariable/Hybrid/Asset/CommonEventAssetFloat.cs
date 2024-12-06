
#pragma warning disable 0649
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.CommonEventVariable
{
    [CreateAssetMenu(menuName = "Common Event/Event Float", fileName = "CommonEventFloat")]
    public class CommonEventAssetFloat : CommonEventAsset<float, CommonEventAssetFloat.TriggerEvent>
    {
        [Serializable]
        public class TriggerEvent : UnityEvent<float> { }
    }
}