using HTC.UnityPlugin.PoseTracker;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;
using UnityEngine;
using UnityEngine.Events;

public class OnHandTracker : BasePoseTracker
{
    [SerializeField]
    private bool onLeft = true;
    [SerializeField]
    private Transform trackOrigin;
    [SerializeField]
    private float height = 0.05f;
    [SerializeField]
    private float dist = 0.15f;
    [SerializeField]
    private float far = -0.1f;
    [SerializeField]
    private UnityEvent onPoseValid;
    [SerializeField]
    private UnityEvent onPoseInvalid;

    public float Height { get { return height; } set { height = value; } }

    private bool prevPoseValid;

    private void Awake()
    {
        if (trackOrigin == null)
        {
            Debug.Log("[OnHandTracker][Awake] trackOrigin is null");
            trackOrigin = GameObject.Find("VROrigin").transform;
        }
    }

    private void Update()
    {
        var poseValid = TryGetPose(out var pos);

        if (!poseValid)
        {
            if (prevPoseValid)
            {
                if (onPoseInvalid != null) { onPoseInvalid.Invoke(); }
            }
        }
        else
        {
            if (!prevPoseValid)
            {
                if (onPoseValid != null) { onPoseValid.Invoke(); }
            }

            if (trackOrigin != null && trackOrigin != transform.parent)
            {
                TrackPose(new RigidPose()
                {
                    pos = trackOrigin.TransformPoint(pos),
                    rot = transform.rotation,
                }, false);
            }
            else
            {
                TrackPose(new RigidPose()
                {
                    pos = pos,
                    rot = transform.localRotation,
                }, true);
            }
        }

        prevPoseValid = poseValid;
    }

    private HandRole mainHand { get { return onLeft ? HandRole.LeftHand : HandRole.RightHand; } }

    private HandRole oppHand { get { return onLeft ? HandRole.RightHand : HandRole.LeftHand; } }

    private bool TryGetPose(out Vector3 pos)
    {
        pos = default;
        if (!VivePose.IsValid(mainHand)) { return false; }

        if (VivePose.GetHandJointCount(mainHand) == 0)
        {
            return false;
        }

        var joints = VivePose.GetAllHandJoints(mainHand);

        // Hide mini map when palm is not facing up
        if (!(joints[HandJointName.Palm].pose.rot.eulerAngles.z > 130f && joints[HandJointName.Palm].pose.rot.eulerAngles.z < 200f)
            || joints[HandJointName.Palm].pose.rot.eulerAngles.x < 325f) return false;

        var posSum = Vector3.zero;
        posSum += joints[HandJointName.ThumbTip].pose.pos;
        posSum += joints[HandJointName.IndexTip].pose.pos;
        posSum += joints[HandJointName.MiddleTip].pose.pos;
        posSum += joints[HandJointName.RingTip].pose.pos;
        posSum += joints[HandJointName.PinkyTip].pose.pos;
        var yMax = float.MinValue;
        for (int i = joints.MinInt, imax = joints.MaxInt; i < imax; ++i)
        {
            var j = joints[i];
            if (!j.isValid) { continue; }
            var y = j.pose.pos.y;
            if (yMax < y) { yMax = y; }
        }
        pos = new Vector3(posSum.x * 0.2f, yMax, posSum.z * 0.2f) + new Vector3(0f, height, 0f);

        var hmdPose = VivePose.GetPose(DeviceRole.Hmd);

        var hmd2main = pos - hmdPose.pos;

        var oth = new Vector2(hmd2main.z, hmd2main.x);
        if (onLeft) { oth.y = -oth.y; } else { oth.x = -oth.x; }
        var distV2 = oth.normalized * dist;
        var farV2 = hmd2main.normalized * far;
        pos += new Vector3(distV2.x, 0f, distV2.y) + new Vector3(farV2.x, 0f, farV2.y);

        return true;
    }
}
