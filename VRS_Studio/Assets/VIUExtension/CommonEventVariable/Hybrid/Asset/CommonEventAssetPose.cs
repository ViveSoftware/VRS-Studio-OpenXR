#pragma warning disable 0649
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.CommonEventVariable
{
#if UNITY_2017_2_OR_NEWER
    [CreateAssetMenu(menuName = "Common Event/Event Pose", fileName = "CommonEventPose")]
    public class CommonEventAssetPose : CommonEventAsset<Pose, CommonEventAssetPose.TriggerEvent>
    {
        [Serializable]
        public class TriggerEvent : UnityEvent<Pose> { }
    }
#endif
}