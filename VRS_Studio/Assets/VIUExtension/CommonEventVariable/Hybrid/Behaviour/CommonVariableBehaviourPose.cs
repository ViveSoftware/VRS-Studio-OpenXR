using UnityEngine;

namespace HTC.UnityPlugin.CommonEventVariable
{
#if UNITY_2017_2_OR_NEWER
    public class CommonVariableBehaviourPose : CommonVariableBehaviour<Pose, CommonVariableAssetPose, CommonVariableAssetPose.OnChangeEvent> { }
#endif
}