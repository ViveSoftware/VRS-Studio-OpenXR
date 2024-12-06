using HTC.UnityPlugin.PoseTracker;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;
using UnityEngine;
using UnityEngine.Events;

public class MenuTipPoseTracker : BasePoseTracker
{
    public Transform target;
    public TrackedHandRole Hand = TrackedHandRole.LeftHand;
    public DeviceRole Hmd = DeviceRole.Hmd;
    public float distance = 0.015f;
    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;

    private bool lastActive;

    public void Update()
    {
        if (IsPoseValid(out var pose))
        {
            if (!lastActive)
            {
                OnActivate.Invoke();
                lastActive = true;
            }

            if (target != null && target != transform.parent)
            {
                pose = new RigidPose(target.transform) * pose;
                TrackPose(pose, false);
            }
            else
            {
                TrackPose(pose, true);
            }
        }
        else
        {
            if (lastActive)
            {
                OnDeactivate.Invoke();
                lastActive = false;
            }
        }
    }

    private bool IsPoseValid(out RigidPose pose)
    {
        pose = default;
        if (!VivePose.IsValid(Hmd)) { return false; }
        var hmdPose = VivePose.GetPose(Hmd);

        if (!VivePose.TryGetHandJointPoseEx(Hand, HandJointName.ThumbTip, out var thumbPose)) { return false; }
        if (!VivePose.TryGetHandJointPoseEx(Hand, HandJointName.IndexTip, out var indexPose)) { return false; }

        var thumbToIndex = indexPose.pose.pos - thumbPose.pose.pos;
        var thumbIndexMid = thumbPose.pose.pos + thumbToIndex * 0.5f;
        pose.pos = thumbIndexMid + Vector3.ClampMagnitude(hmdPose.pos - thumbIndexMid, distance);
        pose.rot = Quaternion.LookRotation(thumbPose.pose.pos - hmdPose.pos, Vector3.up);
        return true;
    }
}
