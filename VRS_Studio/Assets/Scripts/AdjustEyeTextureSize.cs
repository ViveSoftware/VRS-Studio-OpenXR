using UnityEngine;

public class AdjustEyeTextureSize : MonoBehaviour
{
    private void Awake()
    {
        UnityEngine.XR.XRSettings.eyeTextureResolutionScale = 1.44f;
    }
}
