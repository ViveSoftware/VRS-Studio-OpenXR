using UnityEngine;

public class GenericRootManager : MonoBehaviour
{
    public Vector3 Root_OffsetFromHMD = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        UpdateRootPosition();
    }

    public void UpdateRootPosition()
    {
        if (VRSStudioCameraRig.Instance != null)
        {
            transform.position = Camera.main.transform.position + Root_OffsetFromHMD;
        }
    }
}
