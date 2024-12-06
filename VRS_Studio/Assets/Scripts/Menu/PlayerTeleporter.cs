using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{
    /// <summary>
    /// The actual transfrom that will be moved Ex. CameraRig
    /// </summary>
    public Transform target;
    /// <summary>
    /// The actual pivot point that want to be teleported to the pointed location Ex. CameraHead
    /// </summary>
    public Transform pivot;

    public Transform user;
    public Transform virtualPad;
    public MeshRenderer meshRenderer;
    public Material origMat, redMat;

    private bool isGrabbed = false;
    private bool falseTrigger = false;
    private static bool userTeleported = false;

    public void Teleport(Vector3 localPos)
    {
        if (target == null || pivot == null) { return; }

        var telWorldPos = transform.TransformPoint(new Vector3(localPos.x, 0f, localPos.z));

        var pivotTargetLocPos = target.InverseTransformPoint(pivot.position);
        var pivotTargetOnFloorLocPos = new Vector3(pivotTargetLocPos.x, 0f, pivotTargetLocPos.z);
        var pivotOnFloorWorldPos = target.TransformPoint(pivotTargetOnFloorLocPos);
        target.position += telWorldPos - pivotOnFloorWorldPos;
    }

    private void Awake()
    {
        if (target == null)
        {
            Debug.Log("[PlayerTeleporter][Awake] target is null");
            target = GameObject.Find("VROrigin").transform;
        }

        if (pivot == null)
        {
            Debug.Log("[PlayerTeleporter][Awake] pivot is null");
            pivot = GameObject.Find("Camera").transform;
        }
    }

    private void Update()
    {
        if (isGrabbed)
        {
            if (user.localPosition.y >= 5f && (user.localPosition.x < 12f && user.localPosition.x > -12f)
                && (user.localPosition.z < 12f && user.localPosition.z > -12f)) falseTrigger = false;

            if (!falseTrigger) meshRenderer.material = origMat;
            return;
        }

        user.localPosition = new Vector3(target.position.x, 0.13f, target.position.z);
        user.localRotation = Quaternion.identity;
    }

    public void IsUserGrabbed()
    {
        origMat = meshRenderer.material;
        meshRenderer.material = redMat;

        isGrabbed = true;
        falseTrigger = true;
        virtualPad.gameObject.SetActive(false);
    }

    public void IsUserReleased()
    {
        meshRenderer.material = origMat;

        if (!falseTrigger && user.localPosition.y <= 2.5f)
        {
            userTeleported = true;
            Teleport(new Vector3(user.localPosition.x, 0.13f, user.localPosition.z));
        }

        isGrabbed = false;
        falseTrigger = false;
        virtualPad.gameObject.SetActive(true);
    }

    public static bool TeleportByUser()
    {
        var temp = userTeleported;
        userTeleported = false;
        return temp;
    }

    public void TeleportByUser(bool v)
    {
        userTeleported = v;
    }
}
