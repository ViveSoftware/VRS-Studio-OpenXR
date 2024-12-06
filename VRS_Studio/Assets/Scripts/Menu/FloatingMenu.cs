using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;
using UnityEngine;

public class FloatingMenu : MonoBehaviour
{
    private struct V3DPointer
    {
        public Bounds bound;
        public Quaternion rot;
    }

    public Bounds cursorBound;
    public float scale = 0.5f;
    public HandRole hand;
    public Transform CursorTr;

    private Bounds pointerBound;
    private RigidPose pointerPose;
    private RigidPose lastNormalizedPose;

    private void Start()
    {
        pointerBound.size = cursorBound.size * scale;
    }

    private void Update()
    {
        UpdatePointerBound();
        UpdateCursor();
    }

    private void UpdatePointerBound()
    {
        pointerBound.size = cursorBound.size * scale;
        if (!VivePose.IsValidEx(hand)) { return; }
        pointerPose = VivePose.GetPoseEx(hand);
        if (pointerBound.Contains(pointerPose.pos)) { return; }
        var closest = pointerBound.ClosestPoint(pointerPose.pos);
        pointerBound.center += pointerPose.pos - closest;
    }

    private void UpdateCursor()
    {
        if (VivePose.IsValidEx(hand))
        {
            CursorTr.gameObject.SetActive(true);
            CursorTr.position = transform.position + cursorBound.center + (pointerPose.pos - pointerBound.center) / scale;
            CursorTr.rotation = Quaternion.LookRotation(Vector3.down, pointerPose.up);
        }
        else
        {
            CursorTr.gameObject.SetActive(false);
        }
    }
}
