using UnityEngine;

public class VRSStudioEnvController : MonoBehaviour
{
    public static VRSStudioEnvController Instance;
    public GameObject environment;

    private void Awake()
    {
        Instance = this;
    }

    public void SetEnvActive(bool enable)
    {
        environment.SetActive(enable);
    }

    public bool IsEnvActive()
    {
        return environment.activeSelf;
    }
}
