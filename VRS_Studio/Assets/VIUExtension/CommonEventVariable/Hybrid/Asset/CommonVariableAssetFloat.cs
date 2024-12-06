#pragma warning disable 0649
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.CommonEventVariable
{
    [CreateAssetMenu(menuName = "Common Variable/Variable Float", fileName = "CommonVariableFloat")]
    public class CommonVariableAssetFloat : CommonVariableAsset<float, CommonVariableAssetFloat.OnChangeEvent>
    {
        [Serializable]
        public class OnChangeEvent : UnityEvent<float> { }
    }
}