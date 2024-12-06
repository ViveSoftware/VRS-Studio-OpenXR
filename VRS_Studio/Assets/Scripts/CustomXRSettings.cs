using UnityEngine;
using UnityEngine.XR;

public class CustomXRSettings : MonoBehaviour
{
    void Awake()
    {
        XRSettings.eyeTextureResolutionScale = 1.44f;
        Debug.Log("[CustomXRSettings] eyeTextureResolutionScale = " + XRSettings.eyeTextureResolutionScale);
    }
}
