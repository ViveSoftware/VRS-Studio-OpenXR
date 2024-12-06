#pragma warning disable 0649
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.CommonEventVariable
{
    [CreateAssetMenu(menuName = "Common Event/Event Vector3", fileName = "CommonEventVector3", order = 1)]
    public class CommonEventAssetVector3 : CommonEventAsset<Vector3, CommonEventAssetVector3.TriggerEvent>
    {
        [Serializable]
        public class TriggerEvent : UnityEvent<Vector3> { }
    }
}