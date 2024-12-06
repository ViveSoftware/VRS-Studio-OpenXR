using UnityEngine;

public class VRSStudioCameraRig : MonoBehaviour
{
    public static VRSStudioCameraRig Instance;

    public GameObject HMD, LeftHand, RightHand, BoundaryBarrier_L, BoundaryBarrier_R;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (HMD == null)
        {
            HMD = Camera.main.gameObject;
        }
    }
}
