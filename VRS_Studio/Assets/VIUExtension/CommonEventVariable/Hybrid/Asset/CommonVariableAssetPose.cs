#pragma warning disable 0649
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.CommonEventVariable
{
#if UNITY_2017_2_OR_NEWER
    [CreateAssetMenu(menuName = "Common Variable/Variable Pose", fileName = "CommonVariablePose")]
    public class CommonVariableAssetPose : CommonVariableAsset<Pose, CommonVariableAssetPose.OnChangeEvent>
    {
        [Serializable]
        public class OnChangeEvent : UnityEvent<Pose> { }
    }
#endif
}