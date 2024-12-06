#pragma warning disable 0649
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.CommonEventVariable
{
    [CreateAssetMenu(menuName = "Common Variable/Variable Vector3", fileName = "CommonVariableVector3")]
    public class CommonVariableAssetVector3 : CommonVariableAsset<Vector3, CommonVariableAssetVector3.OnChangeEvent>
    {
        [Serializable]
        public class OnChangeEvent : UnityEvent<Vector3> { }
    }
}