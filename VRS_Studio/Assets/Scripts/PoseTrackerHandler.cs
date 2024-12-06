using HTC.UnityPlugin.PoseTracker;
using UnityEngine;

public class PoseTrackerHandler : MonoBehaviour
{
    public PoseTracker poseTracker;

    void Start()
    {
        var vrOrigin = GameObject.Find("VROrigin").transform;
        poseTracker.target = vrOrigin;
    }
}
