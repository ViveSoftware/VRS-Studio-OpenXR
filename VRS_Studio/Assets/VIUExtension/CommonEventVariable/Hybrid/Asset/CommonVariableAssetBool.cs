#pragma warning disable 0649
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.CommonEventVariable
{
    [CreateAssetMenu(menuName = "Common Variable/Variable Bool", fileName = "CommonVariableBool")]
    public class CommonVariableAssetBool : CommonVariableAsset<bool, CommonVariableAssetBool.OnChangeEvent>
    {
        [Serializable]
        public class OnChangeEvent : UnityEvent<bool> { }
    }
}