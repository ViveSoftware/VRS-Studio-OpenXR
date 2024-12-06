using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class VirtualPadBase : MonoBehaviour
{
    [Serializable]
    public class UnityEventV3 : UnityEvent<Vector3> { }
    [Serializable]
    public class UnityEventQt : UnityEvent<Quaternion> { }

    [SerializeField]
    private Transform inputTarget;
    [SerializeField]
    private bool moveOnOverflow;
    [SerializeField]
    private bool trackOnOverflow;

    public UnityEvent OnGotTracking;
    public UnityEventV3 OnTrackNormalizedPos;
    public UnityEventV3 OnTrackLocalPos;
    public UnityEventQt OnTrackLocalRot;
    public UnityEventV3 OnTrackWorldPos;
    public UnityEventQt OnTrackWorldRot;
    public UnityEvent OnLostTracking;

    public Transform InputTarget { get { return inputTarget; } set { inputTarget = value; } }

    public bool MoveOnOverflow { get { return moveOnOverflow; } set { moveOnOverflow = value; } }

    public bool TrackOnOverflow { get { return trackOnOverflow; } set { trackOnOverflow = value; } }

    private bool prevTracking;

    private bool isUpdating;

    public void SetInputTarget(Transform t) { InputTarget = t; }

    public void ResetInputTarget() { InputTarget = null; }

    public abstract bool PadContainsTarget(Vector3 localInputTargetPos);

    public abstract Vector3 ClosestLocalPointToPad(Vector3 localInputTargetPos);

    public abstract Vector3 TargetNormalizedLocalPos(Vector3 localInputTargetPos);

    protected virtual void Update()
    {
        var tracking = false;
        var worldPos = default(Vector3);
        var worldRot = default(Quaternion);
        var locPos = default(Vector3);
        var locRot = default(Quaternion);
        if (InputTarget != null)
        {
            worldPos = InputTarget.position;
            worldRot = InputTarget.rotation;
            locPos = transform.InverseTransformPoint(worldPos);
            locRot = Quaternion.Inverse(transform.rotation) * worldRot;
            tracking = true;
            if (!PadContainsTarget(locPos))
            {
                var closestLocPos = ClosestLocalPointToPad(locPos);
                if (moveOnOverflow)
                {
                    locPos = closestLocPos;
                    worldPos = transform.TransformPoint(locPos);
                    transform.position += worldPos - transform.TransformPoint(closestLocPos);
                    if (InputTarget.IsChildOf(transform)) { InputTarget.position = worldPos; }
                }
                else
                {
                    if (trackOnOverflow)
                    {
                        locPos = closestLocPos;
                        worldPos = transform.TransformPoint(locPos);
                    }
                    else
                    {
                        tracking = false;
                    }
                }
            }
        }

        isUpdating = true;
        if (tracking)
        {
            if (!prevTracking)
            {
                prevTracking = true;
                if (OnGotTracking != null) { OnGotTracking.Invoke(); }
            }

            if (OnTrackNormalizedPos != null)
            {
                OnTrackNormalizedPos.Invoke(TargetNormalizedLocalPos(locPos));
            }

            if (OnTrackLocalPos != null)
            {
                OnTrackLocalPos.Invoke(locPos);
            }

            if (OnTrackLocalRot != null)
            {
                OnTrackLocalRot.Invoke(locRot);
            }

            if (OnTrackWorldPos != null)
            {
                OnTrackWorldPos.Invoke(worldPos);
            }

            if (OnTrackWorldRot != null)
            {
                OnTrackWorldRot.Invoke(worldRot);
            }
        }
        else
        {
            if (prevTracking)
            {
                prevTracking = false;
                if (OnLostTracking != null) { OnLostTracking.Invoke(); }
            }
        }
        if (!enabled && prevTracking)
        {
            prevTracking = false;
            if (OnLostTracking != null) { OnLostTracking.Invoke(); }
        }
        isUpdating = false;
    }

    private void OnDisable()
    {
        if (!isUpdating && prevTracking)
        {
            prevTracking = false;
            if (OnLostTracking != null) { OnLostTracking.Invoke(); }
        }
    }
}
