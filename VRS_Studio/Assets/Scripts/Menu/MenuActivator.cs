using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;
using System;
using UnityEngine;
using UnityEngine.Events;

public class MenuActivator : MonoBehaviour
{
    [Serializable]
    public class UnityEventFloat : UnityEvent<float> { }

    private const DeviceRole hmd = DeviceRole.Hmd;
    private const TrackedHandRole hand = TrackedHandRole.LeftHand;
    private const ControllerButton button = ControllerButton.System;

    public float HmdSightAngle = 110f;
    public float PalmSightAngle = 70f;
    [Range(0.01f, 10f)]
    public float ActivePoseDuration = 1.25f;
    public UnityEvent OnDefaultPosed;
    public UnityEvent OnDefaultUnposed;
    public UnityEventFloat OnProgress;
    public UnityEvent OnProgressDone;
    public UnityEvent OnActivated;
    public UnityEvent OnActivatedPosed;
    public UnityEvent OnActivatedUnposed;
    public UnityEvent OnDeactivated;

    private static State state;
    private float pressDownTime;
    private bool manualDeactivate;

    private enum State
    {
        Default,
        DefaultPosed,
        OnProgress,
        Activated,
        ActivatedPosed,
    }

    public void ManualDeactivate()
    {
        manualDeactivate = true;
    }

    private void NextState()
    {
        switch (state)
        {
            case State.Default:
                if (ActivePose())
                {
                    state = State.DefaultPosed;
                    OnDefaultPosed.Invoke();
                }
                break;
            case State.DefaultPosed:
                if (!ActivePose())
                {
                    state = State.Default;
                    OnDefaultUnposed.Invoke();
                }
                else if (pressDown())
                {
                    state = State.OnProgress;
                    pressDownTime = Time.unscaledTime;
                    OnDefaultUnposed.Invoke();
                }
                break;
            case State.OnProgress:
                if (!ActivePose() || !pressed())
                {
                    state = State.Default;
                    OnProgressDone.Invoke();
                }
                else if (Time.unscaledTime < pressDownTime + ActivePoseDuration)
                {
                    var pressDur = Time.unscaledTime - pressDownTime;
                    OnProgress.Invoke((pressDur / ActivePoseDuration) * 1.05f);
                }
                else
                {
                    state = State.Activated;
                    OnProgressDone.Invoke();
                    OnActivated.Invoke();
                    manualDeactivate = false;
                }
                break;
            case State.Activated:
                if (manualDeactivate || (ActivePose() && pressDown()))
                {
                    state = State.Default;
                    OnDeactivated.Invoke();
                }
                else if (ActivePose() && !pressed())
                {
                    state = State.ActivatedPosed;
                    OnActivatedPosed.Invoke();
                }
                break;
            case State.ActivatedPosed:
                if (manualDeactivate)
                {
                    state = State.Default;
                    OnDeactivated.Invoke();
                }
                else if (ActivePose())
                {
                    if (pressDown())
                    {
                        state = State.Default;
                        OnActivatedUnposed.Invoke();
                        OnDeactivated.Invoke();
                    }
                }
                else
                {
                    state = State.Activated;
                    OnActivatedUnposed.Invoke();
                }
                break;
        }
    }

    private void Update()
    {
        UpdatePress();
        NextState();
    }

    private bool ActivePose()
    {
        if (!VivePose.IsValidEx(hmd)) { return false; }
        var hmdPose = VivePose.GetPoseEx(hmd);

        if (!VivePose.TryGetHandJointPoseEx(hand, HandJointName.Palm, out var palmPose)) { return false; }

        var hmdToPalm = palmPose.pose.pos - hmdPose.pos;

        // palm in hmd sight
        if (Vector3.Angle(hmdPose.forward, hmdToPalm) > HmdSightAngle * 0.5f) { return false; }
        // palm facing hmd
        if (Vector3.Angle(palmPose.pose.up, hmdToPalm) > PalmSightAngle * 0.5f) { return false; }

        return true;
    }

    private int btnUpdateFrame = -1;
    private bool prevPressed;
    private bool currPressed;

    private void UpdatePress()
    {
        var fc = Time.frameCount;
        if (btnUpdateFrame == fc) { return; }
        btnUpdateFrame = fc;
        prevPressed = currPressed;

        if (VivePose.TryGetHandJointPoseEx(hand, HandJointName.ThumbTip, out var thumbPose) &&
            VivePose.TryGetHandJointPoseEx(hand, HandJointName.IndexTip, out var indexPose))
        {
            currPressed = (thumbPose.pose.pos - indexPose.pose.pos).sqrMagnitude <= 0.02f * 0.02f;
            return;
        }

        currPressed = ViveInput.GetPressEx(hand, button);
    }

    private bool pressed()
    {
        return currPressed;
    }

    private bool pressDown()
    {
        return !prevPressed && currPressed;
    }

    public static bool IsMenuActivated()
    {
        return state == State.Activated;
    }
}
