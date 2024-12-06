using UnityEngine;

namespace HTC.UnityPlugin.CommonEventVariable
{
#if UNITY_2017_2_OR_NEWER
    public class CommonEventBehaviourPose : CommonEventBehaviour<Pose, CommonEventAssetPose, CommonEventAssetPose.TriggerEvent> { }
#endif
}