using HTC.ViveSoftware.ExpLab.HandInteractionDemo;
using UnityEngine;

public class ScreenKBRootManager : MonoBehaviour
{
    public GameObject Keyboard, Screen;
    public RobotScenarioControllerBase robotAssistantController;
    public CustomInputField customInputField;

    private Vector3 ScreenKBRoot_OffsetFromHMD = new Vector3(0f, -0.25f, 0.55f);

#if VRSSTUDIO_INTERNAL
    private TypingCheck typingCheckInstance = null;
#endif

    // Start is called before the first frame update
    void Start()
    {
        if (VRSStudioCameraRig.Instance != null)
        {
            transform.position = Camera.main.transform.position + ScreenKBRoot_OffsetFromHMD;
        }

        if (robotAssistantController != null)
        {
            robotAssistantController.InitializeRobot();
        }

#if VRSSTUDIO_INTERNAL
        if (typingCheckInstance == null)
		{
            typingCheckInstance = gameObject.AddComponent<TypingCheck>();
            typingCheckInstance.customInputField = customInputField;
        }
#endif
    }
}
