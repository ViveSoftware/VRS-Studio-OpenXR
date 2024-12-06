using HTC.UnityPlugin.Pointer3D;
using HTC.UnityPlugin.Vive;
using UnityEngine;

public class TeleportHandler : MonoBehaviour
{
    public ProjectileGenerator projectileGen;

    private bool isVelocityIncreased = false;
    private float time = 1f;

    void Update()
    {
        var isPadUpTouch = ViveInput.GetPressEx(ControllerRole.LeftHand, ControllerButton.DPadUpTouch);

        if (isPadUpTouch && !isVelocityIncreased)
        {
            if (time > 0f)
            {
                time -= Time.deltaTime;
            }
            else
            {
                isVelocityIncreased = true;
                projectileGen.velocity = 3f;
                time = 1f;
            }
        }
        else if (isVelocityIncreased && !isPadUpTouch)
        {
            isVelocityIncreased = false;
            projectileGen.velocity = 1.5f;
        }
        else time = 1f;
    }
}
