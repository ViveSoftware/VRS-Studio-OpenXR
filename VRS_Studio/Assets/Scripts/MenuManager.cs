using HTC.UnityPlugin.Vive;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public Teleportable teleportable;
    public MenuTipPoseTracker menuPoseTracker;
    public ViveJointPoseTracker jointPoseTracker;

    void Start()
    {
        var vrOrigin = GameObject.Find("VROrigin").transform;
        if (teleportable.target == null)
        {
            teleportable.target = vrOrigin;
        }

        if (menuPoseTracker.target == null)
        {
            menuPoseTracker.target = vrOrigin;
        }

        if (jointPoseTracker.origin == null)
        {
            jointPoseTracker.origin = vrOrigin;
        }

        if (teleportable.pivot == null)
        {
            teleportable.pivot = GameObject.Find("Camera").transform;
        }
    }
}
