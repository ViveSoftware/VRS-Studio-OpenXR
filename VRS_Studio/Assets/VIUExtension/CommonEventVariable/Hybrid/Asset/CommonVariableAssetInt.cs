#pragma warning disable 0649
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.CommonEventVariable
{
    [CreateAssetMenu(menuName = "Common Variable/Variable Int", fileName = "CommonVariableInt")]
    public class CommonVariableAssetInt : CommonVariableAsset<int, CommonVariableAssetInt.OnChangeEvent>
    {
        [Serializable]
        public class OnChangeEvent : UnityEvent<int> { }
    }
}