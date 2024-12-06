using HTC.UnityPlugin.Vive;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TooltipManager : MonoBehaviour
{
    public DefaultTooltipRenderer tooltipRightRenderer;
    public DefaultTooltipRenderer tooltipLeftRenderer;
    public DefaultTooltipRenderDataAsset tooltipRightAsset;
    public DefaultTooltipRenderDataAsset tooltipLeftAsset;
    public Transform rightCR;
    public Transform leftCR;
    public GameObject rightCRTooltip;
    public GameObject leftCRTooltip;

    private Transform vrOrigin;
    private bool manualOff = false;
    private float timer = 1f;
    private bool isActive3DSPScene = false;

    void Awake()
    {
        if (vrOrigin == null)
        {
            Debug.Log("[TooltipManager][Awake] vrOrigin is null");
            vrOrigin = GameObject.Find("VROrigin").transform;
        }
    }

    void Start()
    {
        tooltipRightRenderer.SetTooltipData(tooltipRightAsset);
        tooltipLeftRenderer.SetTooltipData(tooltipLeftAsset);
    }

    void Update()
    {
        Scene soundScene = SceneManager.GetSceneByName("v130_demo");
        Scene currentScene = SceneManager.GetActiveScene();
        isActive3DSPScene = false;
        if (soundScene.IsValid() && soundScene == currentScene)
        {
            isActive3DSPScene = true;
        }

        if (MenuController.Instance != null && MenuController.Instance.IsMenuOpened() || isActive3DSPScene)
        {
            rightCRTooltip.SetActive(false);
            leftCRTooltip.SetActive(false);
            return;
        }

        var leftGripPressed = ViveInput.GetPressEx(ControllerRole.LeftHand, ControllerButton.Grip);
        var rightGripPressed = ViveInput.GetPressEx(ControllerRole.RightHand, ControllerButton.Grip);

        if (leftGripPressed && rightGripPressed)
        {
            if (timer > 0f)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                timer = 1f;
                manualOff = !manualOff;
            }
        }

        if (manualOff)
        {
            leftCRTooltip.SetActive(false);
            rightCRTooltip.SetActive(false);
            return;
        }

        var leftCRPose = VivePose.GetPoseEx(ControllerRole.LeftHand, vrOrigin);
        var rightCRPose = VivePose.GetPoseEx(ControllerRole.RightHand, vrOrigin);
        Vector3 screenPointLeft = Camera.main.WorldToViewportPoint(leftCRPose.pos);
        bool onScreenLeft = screenPointLeft.z > 0 && screenPointLeft.x > 0 && screenPointLeft.x < 1 && screenPointLeft.y > 0 && screenPointLeft.y < 1;
        Vector3 screenPointRight = Camera.main.WorldToViewportPoint(rightCRPose.pos);
        bool onScreenRight = screenPointRight.z > 0 && screenPointRight.x > 0 && screenPointRight.x < 1 && screenPointRight.y > 0 && screenPointRight.y < 1;

        leftCRTooltip.SetActive(onScreenLeft);
        rightCRTooltip.SetActive(onScreenRight);
    }
}
