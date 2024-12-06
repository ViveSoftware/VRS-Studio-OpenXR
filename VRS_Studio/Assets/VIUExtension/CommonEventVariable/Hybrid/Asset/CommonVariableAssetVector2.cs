#pragma warning disable 0649
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.CommonEventVariable
{
    [CreateAssetMenu(menuName = "Common Variable/Variable Vector2", fileName = "CommonVariableVector2")]
    public class CommonVariableAssetVector2 : CommonVariableAsset<Vector2, CommonVariableAssetVector2.OnChangeEvent>
    {
        [Serializable]
        public class OnChangeEvent : UnityEvent<Vector2> { }
    }
}