#pragma warning disable 0649
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.CommonEventVariable
{
    [CreateAssetMenu(menuName = "Common Event/Event Vector2", fileName = "CommonEventVector2")]
    public class CommonEventAssetVector2 : CommonEventAsset<Vector2, CommonEventAssetVector2.TriggerEvent>
    {
        [Serializable]
        public class TriggerEvent : UnityEvent<Vector2> { }
    }
}