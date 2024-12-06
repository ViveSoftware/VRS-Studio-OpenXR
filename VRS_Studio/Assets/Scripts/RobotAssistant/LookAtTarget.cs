using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    public Transform target = null;
    public float rotaSpeed = 0.08f;
    public Vector3 weight;

    private void Update()
    {
        if (target != null)
        {
            LookAtPoint(target.transform.position);
        }
    }

    public void LookAtPoint(Vector3 point)
    {
        Vector3 dir = point - transform.position;
        dir = new Vector3(dir.x * weight.x, dir.y * weight.y, dir.z * weight.z);

        Quaternion dirRot = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, dirRot, rotaSpeed);
    }
}
