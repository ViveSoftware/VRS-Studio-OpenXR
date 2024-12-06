using UnityEngine;

public class RingScaler : MonoBehaviour
{
    void Update()
    {
        var scale = Vector3.one;

        if (transform.localScale.x > 0.9f)
            transform.localScale = new Vector3(0.2f, 1f, 0.2f);
        else
            transform.localScale = Vector3.Lerp(transform.localScale, scale, Time.deltaTime);
    }
}
