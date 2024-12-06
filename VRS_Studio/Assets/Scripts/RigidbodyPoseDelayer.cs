using HTC.UnityPlugin.PoseTracker;
using HTC.UnityPlugin.Utility;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyPoseDelayer : BasePoseModifier
{
    public float poseSpeedFactor = 4f;

    private bool firstPose = true;
    private Rigidbody delayedRigidbody = null;

    protected override void OnEnable()
    {
        base.OnEnable();
        ResetFirstPose();

        delayedRigidbody = GetComponent<Rigidbody>();
    }

    public override void ModifyPose(ref RigidPose pose, bool useLocal)
    {
        if (firstPose)
        {
            firstPose = false;
        }
        else
        {
            float poseChangeSpeed = poseSpeedFactor / delayedRigidbody.mass;

            var originalPos = useLocal ? transform.localPosition : transform.position;
            pose.pos = Vector3.Lerp(originalPos, pose.pos, poseChangeSpeed);

            var originalQuat = useLocal ? transform.localRotation : transform.rotation;

            pose.rot = Quaternion.Slerp(originalQuat, pose.rot, poseChangeSpeed);
        }
    }

    public void ResetFirstPose() { firstPose = true; }
}
