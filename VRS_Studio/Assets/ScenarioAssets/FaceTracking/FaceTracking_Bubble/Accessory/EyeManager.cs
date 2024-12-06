using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class EyeManager : MonoBehaviour
{
    [SerializeField]
    private InputActionProperty eyePoseAction;
    private bool isEyePoseActionEnabled = false;
    private int lastUpdateFrame = -1;
    private Vector3 origin = Vector3.zero;
    private Vector3 direction = Vector3.zero;
    private bool isTracked = false;

    public static EyeManager Instance { get; private set; }

    private void OnEnable()
    {
        if (eyePoseAction != null && eyePoseAction.action != null)
        {
            eyePoseAction.action.Enable();
            isEyePoseActionEnabled = true;
            Instance = this;
        }
    }

    public bool GetCombinedEyeOrigin(out Vector3 origin)
    {
        UpdateIfNeeded();
        origin = this.origin;
        return isTracked;
    }

    public bool GetCombindedEyeDirectionNormalized(out Vector3 direction)
    {
        UpdateIfNeeded();
        direction = this.direction;
        return isTracked;
    }

    public bool IsTracked()
    {
        UpdateIfNeeded();
        return isTracked;
    }

    private void UpdateIfNeeded()
    {
        if (Time.frameCount <= lastUpdateFrame) { return; }
        lastUpdateFrame = Time.frameCount;
        isTracked = false;

        if (isEyePoseActionEnabled)
        {
            try
            {
                PoseState pose = eyePoseAction.action.ReadValue<PoseState>();
                origin = pose.position;
                direction = pose.rotation * Vector3.forward;
                isTracked = pose.isTracked ||
                            (((pose.trackingState & UnityEngine.XR.InputTrackingState.Position) != 0) &&
                            ((pose.trackingState & UnityEngine.XR.InputTrackingState.Rotation) != 0));
            }
            catch
            {
                isTracked = false;
            }
        }
    }
}
