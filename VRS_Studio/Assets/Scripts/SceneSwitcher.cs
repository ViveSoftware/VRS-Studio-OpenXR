using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{
    public Transform target;

    public Vector3 prevPos;
    public Quaternion prevRot;
    private bool isTeleported = false;

    private void Awake()
    {
        if (target == null)
        {
            Debug.Log("[SceneSwitcher][Awake] target is null");
            target = GameObject.Find("VROrigin").transform;
        }
    }

    private void OnDisable()
    {
        target.transform.position = prevPos;
        target.transform.rotation = prevRot;
    }

    void Update()
    {
        if (!isTeleported && Vector3.Distance(target.position, transform.position) > 0.5f)
        {
            prevPos = target.transform.position;
            prevRot = target.transform.rotation;
            target.transform.position = new Vector3(transform.position.x, target.transform.position.y, transform.position.z);
            target.transform.rotation = transform.rotation;
            isTeleported = true;
        }
    }
}
