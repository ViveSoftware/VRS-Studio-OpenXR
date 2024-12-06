using FancyScrollView.Example03;
using HTC.UnityPlugin.Vive;
using System.Collections;
using UnityEngine;
using VRSStudio.Spectator;
using static RobotAnchorManager;

public class AreaPivotController : MonoBehaviour
{
    [SerializeField] private Transform[] pivots;
    private Transform vrOrigin;

    private AnchorState currState = AnchorState.Invalid;
    private float timer = 1f;
    private IEnumerator coroutine;

    void Awake()
    {
        if (vrOrigin == null)
        {
            Debug.Log("[AreaPivotController][Awake] vrOrigin is null");
            vrOrigin = GameObject.Find("VROrigin").transform;
        }
    }

    void OnDisable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }

    void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer = 1f;

            if (Vector3.Distance(vrOrigin.position, pivots[4].position) < 3f)
            {
                VRSSpectatorManager.Instance.EnableRecordingText();
            }
            else
            {
                VRSSpectatorManager.Instance.DisableRecordingText();
            }
        }
    }

    public void Object3DPivot()
    {
        if (!VivePose.IsValidEx(ControllerRole.LeftHand)) return;

        currState = AnchorState.Object3D;
        vrOrigin.position = pivots[0].position;
        vrOrigin.rotation = FromToRotation(pivots[0]);
    }

    public void ThrowBottlesPivot()
    {
        if (!VivePose.IsValidEx(ControllerRole.LeftHand)) return;

        currState = AnchorState.ThrowBottles;
        vrOrigin.position = pivots[1].position;
        vrOrigin.rotation = FromToRotation(pivots[1]);
    }

    public void KeyboardPivot()
    {
        if (!VivePose.IsValidEx(ControllerRole.LeftHand)) return;

        currState = AnchorState.Keyboard;
        vrOrigin.position = pivots[2].position;
        vrOrigin.rotation = FromToRotation(pivots[2]);
    }

    public void VRMAvatarPivot()
    {
        if (!VivePose.IsValidEx(ControllerRole.LeftHand)) return;

        currState = AnchorState.VRMAvatar;
        vrOrigin.position = pivots[3].position;
        vrOrigin.rotation = FromToRotation(pivots[3]);
    }

    public void SpectatorPivot()
    {
        if (!VivePose.IsValidEx(ControllerRole.LeftHand)) return;

        currState = AnchorState.Spectator;
        vrOrigin.position = pivots[4].position;
        vrOrigin.rotation = FromToRotation(pivots[4]);
    }

    public void BubblesPivot()
    {
        if (!VivePose.IsValidEx(ControllerRole.LeftHand)) return;

        currState = AnchorState.Bubbles;
        vrOrigin.position = pivots[5].position;
        vrOrigin.rotation = FromToRotation(pivots[5]);
    }

    public void Sound3DPivot()
    {
        if (!VivePose.IsValidEx(ControllerRole.LeftHand)) return;

        currState = AnchorState.Sound3D;
        vrOrigin.position = pivots[6].position;
        vrOrigin.rotation = FromToRotation(pivots[6]);
    }

    public void Add3DObjectTutorial()
    {
        MenuPresenter.AddTutorialViewItem("3D Object Tutorial");
    }

    public void AddThrowBottlesTutorial()
    {
        MenuPresenter.AddTutorialViewItem("Bottles Tutorial");
    }

    public void Add3DSoundTutorial()
    {
        MenuPresenter.AddTutorialViewItem("3D Sound Tutorial");
    }

    public void AddBubblesTutorial()
    {
        MenuPresenter.AddTutorialViewItem("Bubbles Tutorial");
    }

    public void AddKeyboardTutorial()
    {
        MenuPresenter.AddTutorialViewItem("Keyboard Tutorial");
    }

    public void RemoveTutorial()
    {
        MenuPresenter.RemoveTutorialViewItem();
    }

    private Quaternion FromToRotation(Transform target)
    {
        var hmdPose = VivePose.GetPose(DeviceRole.Hmd);

        return Quaternion.FromToRotation(Vector3.ProjectOnPlane(hmdPose.forward, Vector3.up).normalized, Vector3.ProjectOnPlane(target.forward, Vector3.up).normalized);
    }
}
