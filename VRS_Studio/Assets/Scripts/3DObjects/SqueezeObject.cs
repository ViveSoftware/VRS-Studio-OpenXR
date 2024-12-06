using HTC.UnityPlugin.Vive;
using HTC.UnityPlugin.VRModuleManagement;
using UnityEngine;
using UnityEngine.Playables;
using VIVE.OpenXR.Toolkits.RealisticHandInteraction;

public class SqueezeObject : MonoBehaviour
{
    [SerializeField]
    private Transform balloon;
    [SerializeField]
    private BasicGrabbable grabbedObj;
    [SerializeField]
    private PlayableDirector playableDirector;

    private BalloonState balloonState = BalloonState.Idle;
    private ViveColliderButtonEventData viveEventData;
    private IVRModuleDeviceState current;
    private float reset = 1.0f;
    private float time = 0.25f;
    private Pose grabberPose;
    private Vector3 balloon_orig_pos;
    private Quaternion balloon_orig_rot;
    private Rigidbody balloonRigidbody;

    private HandGrabInteractor handGrabber = null;
    private FingerRequirement requirement = new(GrabRequirement.Optional, GrabRequirement.Optional,
        GrabRequirement.Optional, GrabRequirement.Optional, GrabRequirement.Optional);

    public enum BalloonState
    {
        Invalid,
        Idle,
        Grabbed,
        Released,
        Reset,
        Waiting,
        Exploded,
    }

    private void OnEnable()
    {
        playableDirector.stopped += OnPlayableDirectorStopped;
    }

    private void OnDisable()
    {
        playableDirector.stopped -= OnPlayableDirectorStopped;
    }

    private void Start()
    {
        if (balloon)
        {
            balloon_orig_pos = balloon.position;
            balloon_orig_rot = balloon.rotation;
            balloonRigidbody = balloon.GetComponentInChildren<Rigidbody>();
        }
    }

    private void Update()
    {
        switch (balloonState)
        {
            case BalloonState.Idle:
                UpdateIdleState();
                break;
            case BalloonState.Grabbed:
                UpdateGrabbedState();
                break;
            case BalloonState.Released:
                ResetToIdleState();
                break;
            case BalloonState.Reset:
                ResetBalloon();
                break;
            case BalloonState.Exploded:
                UpdateExplodedState();
                break;
        }
    }

    private void UpdateIdleState()
    {
        playableDirector.time = 0;
        playableDirector.Evaluate();

        if (handGrabber == null && grabbedObj.isGrabbed && TryGetViveEventData(out viveEventData))
        {
            SetHandOffsets(viveEventData.viveRole.ToRole<HandRole>());
            balloonState = BalloonState.Grabbed;
        }
        else if (handGrabber != null)
        {
            balloonState = BalloonState.Grabbed;
        }

        CheckBalloonPositionForReset();
    }

    private void SetHandOffsets(HandRole role)
    {
        if (role == HandRole.RightHand)
        {
            current = VRModule.GetCurrentDeviceState(VRModule.GetRightControllerDeviceIndex());
        }
        else if (role == HandRole.LeftHand)
        {
            current = VRModule.GetCurrentDeviceState(VRModule.GetLeftControllerDeviceIndex());
        }
    }

    private void CheckBalloonPositionForReset()
    {
        if (balloon.position.y < -1f)
        {
            reset -= Time.deltaTime;
            if (reset < 0f)
            {
                balloonState = BalloonState.Reset;
            }
        }
    }

    private void UpdateGrabbedState()
    {
        if (!grabbedObj.isGrabbed && handGrabber == null)
        {
            balloonState = BalloonState.Released;
            return;
        }

        var gripAxis = UpdateGripAxis();

        if (gripAxis >= 1f)
        {
            time -= Time.deltaTime;
            if (time < 0f)
            {
                playableDirector.Play();
                balloonState = BalloonState.Waiting;
            }
        }
        else
        {
            playableDirector.time = 0.716f * gripAxis;
            playableDirector.Evaluate();
            time = 0.25f;
        }
    }

    private float UpdateGripAxis()
    {
        if (handGrabber == null && grabbedObj.isGrabbed)
        {
            //var pose = grabbedObj.currentGrabber.grabberOrigin;
            //balloon.position = pose.pos;
            //balloon.rotation = pose.rot;
            return current.GetAxisValue(VRModuleRawAxis.CapSenseGrip);
        }
        else if (handGrabber != null)
        {
            return Mathf.SmoothStep(0.0f, 1.0f, handGrabber.handGrabState.HandGrabNearStrength(requirement));
        }

        return 0.0f;
    }

    private void ResetToIdleState()
    {
        balloonState = BalloonState.Idle;
        time = 0.25f;
    }

    private void ResetBalloon()
    {
        reset = 1f;
        playableDirector.time = 0;
        playableDirector.Evaluate();
        balloon.position = balloon_orig_pos;
        balloon.rotation = balloon_orig_rot;
        balloonRigidbody.velocity = Vector3.zero;
        balloonRigidbody.angularVelocity = Vector3.zero;
        balloonState = BalloonState.Idle;
    }

    private void UpdateExplodedState()
    {
        if (!grabbedObj.isGrabbed && handGrabber == null)
        {
            reset -= Time.deltaTime;
            if (reset < 0f)
            {
                balloonState = BalloonState.Reset;
            }
        }
    }

    private bool TryGetViveEventData(out ViveColliderButtonEventData viveEventData)
    {
        return grabbedObj.grabbedEvent.TryGetViveButtonEventData(out viveEventData);
    }

    private void OnPlayableDirectorStopped(PlayableDirector director)
    {
        balloonState = BalloonState.Exploded;
        reset = 1f;
        time = 0.25f;
    }

    public void OnBeginGrabbed(IGrabbable grabbable)
    {
        if (grabbable.grabber is HandGrabInteractor grabInteractor)
        {
            handGrabber = grabInteractor;
        }
    }

    public void OnEndGrabbed(IGrabbable grabbable)
    {
        handGrabber = null;
    }
}
