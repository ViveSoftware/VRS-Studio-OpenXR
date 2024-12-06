using UnityEngine;
using VIVE.OpenXR;

public class HandInteractionHandler : MonoBehaviour
{
    public static HandInteractionHandler Instance { get; private set; } = null;

    [SerializeField]
    private Transform[] pivots;
    private bool handDemoArea = false;
    public float distance = 2.0f;

    private Transform GetRig()
    {
        if (VIVERig.Instance != null) return VIVERig.Instance.transform;
        else if (Camera.main != null) return Camera.main.transform;
        else return null;
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        Transform rig = GetRig();
        if (rig == null) return;
        handDemoArea = false;

        Vector3 rigPos = rig.position;
        rigPos.y = 0;
        for (int i = 0; i < pivots.Length; i++)
        {
            Vector3 pivotPos = pivots[i].position;
            pivotPos.y = 0;
            float currDistance = Vector3.Distance(rigPos, pivotPos);
            if (currDistance < distance)
            {
                handDemoArea = true;
                break;
            }
        }
    }

    public bool IsHandDemoNeeded()
    {
        return handDemoArea;
    }

}