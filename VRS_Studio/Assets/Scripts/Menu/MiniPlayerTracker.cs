using UnityEngine;
using UnityEngine.Events;

public class MiniPlayerTracker : MonoBehaviour
{
    public Transform Stage;
    public Transform PlayerFloor;
    public Transform PlayerHead;

    public UnityEvent OnTrack;
    public UnityEvent OnLostTrack;

    private bool lastTracked;

    private void Update()
    {
        var tracked = Stage != null && PlayerFloor != null && PlayerHead != null;

        if (!tracked)
        {
            if (lastTracked)
            {

            }
        }

    }
}
