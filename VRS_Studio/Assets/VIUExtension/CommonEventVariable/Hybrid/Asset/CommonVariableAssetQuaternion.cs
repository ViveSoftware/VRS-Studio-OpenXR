#pragma warning disable 0649
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.CommonEventVariable
{
    [CreateAssetMenu(menuName = "Common Variable/Variable Quaternion", fileName = "CommonVariableQuaternion")]
    public class CommonVariableAssetQuaternion : CommonVariableAsset<Quaternion, CommonVariableAssetQuaternion.OnChangeEvent>
    {
        [Serializable]
        public class OnChangeEvent : UnityEvent<Quaternion> { }
    }
}