using UnityEngine;
using VIVE.OpenXR;

public class UpdateTextPose : MonoBehaviour
{
    public GameObject text;

    private void Update()
    {
        Transform mainTrans = null;
        if (VIVERig.Instance != null)
        {
            mainTrans = VIVERig.Instance.transform;
        }
        else if (Camera.main != null)
        {
            mainTrans = Camera.main.transform;
        }
        else
        {
            return;
        }

        Vector3 direction = text.transform.position - mainTrans.position;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        text.transform.rotation = lookRotation;
    }
}
