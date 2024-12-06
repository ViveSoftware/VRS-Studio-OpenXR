using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;
using HTC.UnityPlugin.VRModuleManagement;
using UnityEngine;

using VRSStudio.Utils;

public class SpawnObjects : MonoBehaviour
{
    public Transform[] airship;
    public Transform[] roboball;

    private ViveRoleProperty role = ViveRoleProperty.New(TrackedHandRole.LeftHand);

    private JointPose[] joints = new JointPose[20];

    private Vector3[] airship_orig_pos = new Vector3[6];
    private Quaternion[] airship_orig_rot = new Quaternion[6];
    private Vector3[] roboball_orig_pos = new Vector3[5];
    private Quaternion[] roboball_orig_rot = new Quaternion[5];

    private void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            airship_orig_pos[i] = airship[i].position;
            airship_orig_rot[i] = airship[i].rotation;
        }

        for (int i = 0; i < 5; i++)
        {
            roboball_orig_pos[i] = roboball[i].position;
            roboball_orig_rot[i] = roboball[i].rotation;
        }
    }

    void Update()
    {
        if (!VRModule.GetCurrentDeviceState(role.GetDeviceIndex()).isPoseValid) return;

        VivePose.TryGetHandJointPose(role, HandJointName.ThumbMetacarpal, out joints[0]);
        VivePose.TryGetHandJointPose(role, HandJointName.ThumbProximal, out joints[1]);
        VivePose.TryGetHandJointPose(role, HandJointName.ThumbDistal, out joints[2]);
        VivePose.TryGetHandJointPose(role, HandJointName.ThumbTip, out joints[3]);

        VivePose.TryGetHandJointPose(role, HandJointName.IndexProximal, out joints[4]);
        VivePose.TryGetHandJointPose(role, HandJointName.IndexIntermediate, out joints[5]);
        VivePose.TryGetHandJointPose(role, HandJointName.IndexDistal, out joints[6]);
        VivePose.TryGetHandJointPose(role, HandJointName.IndexTip, out joints[7]);

        VivePose.TryGetHandJointPose(role, HandJointName.MiddleProximal, out joints[8]);
        VivePose.TryGetHandJointPose(role, HandJointName.MiddleIntermediate, out joints[9]);
        VivePose.TryGetHandJointPose(role, HandJointName.MiddleDistal, out joints[10]);
        VivePose.TryGetHandJointPose(role, HandJointName.MiddleTip, out joints[11]);

        VivePose.TryGetHandJointPose(role, HandJointName.RingProximal, out joints[12]);
        VivePose.TryGetHandJointPose(role, HandJointName.RingIntermediate, out joints[13]);
        VivePose.TryGetHandJointPose(role, HandJointName.RingDistal, out joints[14]);
        VivePose.TryGetHandJointPose(role, HandJointName.RingTip, out joints[15]);

        VivePose.TryGetHandJointPose(role, HandJointName.PinkyProximal, out joints[16]);
        VivePose.TryGetHandJointPose(role, HandJointName.PinkyIntermediate, out joints[17]);
        VivePose.TryGetHandJointPose(role, HandJointName.PinkyDistal, out joints[18]);
        VivePose.TryGetHandJointPose(role, HandJointName.PinkyTip, out joints[19]);

        var thumbV1 = (joints[2].pose.pos - joints[1].pose.pos).normalized;
        var thumbV2 = (joints[3].pose.pos - joints[1].pose.pos).normalized;
        var indexV1 = (joints[5].pose.pos - joints[4].pose.pos).normalized;
        var indexV2 = (joints[7].pose.pos - joints[4].pose.pos).normalized;
        var middleV1 = (joints[9].pose.pos - joints[8].pose.pos).normalized;
        var middleV2 = (joints[11].pose.pos - joints[8].pose.pos).normalized;
        var ringV1 = (joints[13].pose.pos - joints[12].pose.pos).normalized;
        var ringV2 = (joints[15].pose.pos - joints[12].pose.pos).normalized;
        var pinkyV1 = (joints[17].pose.pos - joints[16].pose.pos).normalized;
        var pinkyV2 = (joints[19].pose.pos - joints[16].pose.pos).normalized;

        HandState handState = new HandState();
        handState.thumb = GetAngle(thumbV1, thumbV2, Vector3.Cross(thumbV1, thumbV2));
        handState.index = GetAngle(indexV1, indexV2, Vector3.Cross(indexV1, indexV2));
        handState.middle = GetAngle(middleV1, middleV2, Vector3.Cross(middleV1, middleV2));
        handState.ring = GetAngle(ringV1, ringV2, Vector3.Cross(ringV1, ringV2));
        handState.pinky = GetAngle(pinkyV1, pinkyV2, Vector3.Cross(pinkyV1, pinkyV2));

        if (Vector3.Distance(joints[11].pose.pos, joints[3].pose.pos) < 0.04f
            && handState.thumb < 10f && handState.index < 15f && handState.middle < 40f)
        {
            if (Vector3.Distance(joints[7].pose.pos, joints[3].pose.pos) < 0.04f && Vector3.Distance(joints[11].pose.pos, joints[1].pose.pos) < 0.04f)
            {
                for (int i = 0; i < 6; i++)
                {
                    airship[i].position = airship_orig_pos[i];
                    airship[i].rotation = airship_orig_rot[i];
                }

                for (int i = 0; i < 5; i++)
                {
                    roboball[i].position = roboball_orig_pos[i];
                    roboball[i].rotation = roboball_orig_rot[i];
                    roboball[i].GetComponentInChildren<Rigidbody>().velocity = Vector3.zero;
                    roboball[i].GetComponentInChildren<Rigidbody>().angularVelocity = Vector3.zero;
                }
            }
        }
    }

    private float GetAngle(Vector3 v1, Vector3 v2, Vector3 n)
    {
        var angle = Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)),
                                Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;

        return angle;
    }
}
