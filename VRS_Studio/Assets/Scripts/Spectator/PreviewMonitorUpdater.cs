using UnityEngine;
using VRSStudio.Common;

public class PreviewMonitorUpdater : MonoBehaviour
{
    private Transform tInternal;
    private float updateTime = 0f;
    Timer updateTimer = new Timer(0.2f);
    Quaternion targetRotation = Quaternion.identity;

    void Start()
    {
        updateTimer.Set(updateTime);
        if (tInternal == null)
        {
            tInternal = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (updateTimer.Check())
        {
            updateTimer.Set();
            DoUpdate();
        }

        UpdateRotation(updateTimer.Progress() * 0.95f + 0.05f);
    }

    void DoUpdate()
    {
        Vector3 lookDir = (transform.position - tInternal.position);
        Vector3 lookDirLocal = transform.parent.InverseTransformDirection(lookDir);
        Vector3 projectedLookDirLocal = Vector3.ProjectOnPlane(lookDirLocal, Vector3.right);
        targetRotation = Quaternion.FromToRotation(Vector3.forward, -projectedLookDirLocal);
    }

    void UpdateRotation(float t)
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, t);

        if (Vector3.Dot(transform.localRotation * Vector3.forward, Vector3.forward) < 0)
        {
            transform.localScale = new Vector3(-1f, -1f, 1f);
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }
}
