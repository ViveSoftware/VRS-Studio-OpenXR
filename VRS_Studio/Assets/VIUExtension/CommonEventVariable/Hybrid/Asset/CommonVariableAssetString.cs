#pragma warning disable 0649
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.CommonEventVariable
{
    [CreateAssetMenu(menuName = "Common Variable/Variable String", fileName = "CommonVariableString")]
    public class CommonVariableAssetString : CommonVariableAsset<string, CommonVariableAssetString.OnChangeEvent>
    {
        [Serializable]
        public class OnChangeEvent : UnityEvent<string> { }
    }
}