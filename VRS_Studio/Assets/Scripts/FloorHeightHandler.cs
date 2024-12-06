using UnityEngine;
using VIVE.OpenXR;

public class FloorHeightHandler : MonoBehaviour
{
    [SerializeField]
    private Transform rig = null;
    private Vector3 lastRigPos = Vector3.zero;
    private readonly string hitFloor = "floor";
    private readonly string hitGravel = "gravel.001";

    private void Start()
    {
        if (rig == null && VIVERig.Instance != null)
        {
            rig = VIVERig.Instance.transform;
        }
        lastRigPos = rig.position;
    }

    void LateUpdate()
    {
        if (rig == null || lastRigPos == rig.position) return;

        if (Physics.Raycast(Camera.main.transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity))
        {
            var pos = rig.position;
            if (hit.transform.name == hitFloor)
            {
                rig.position = new Vector3(pos.x, 0.3f, pos.z);
            }
            else if (hit.transform.name == hitGravel)
            {
                rig.position = new Vector3(pos.x, 0.0f, pos.z);
            }
            lastRigPos = rig.position;
        }
    }
}
