using FancyScrollView;
using FancyScrollView.Example03;
using HTC.UnityPlugin.Vive;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using VIVE.OpenXR.SecondaryViewConfiguration;
using VRSStudio.Avatar;
using VRSStudio.Spectator;

public class MenuController : MonoBehaviour
{
    [SerializeField] Transform menuCanvas;
    [SerializeField] Transform curvePointer;
    [SerializeField] ScrollView scrollView = default;
    [SerializeField] Scroller scroller = default;

    private bool dPadUp = false;
    private bool dPadDown = false;
    private bool menuDown = false;
    private bool triggerDown = false;
    private bool keyADown = false;

    private SpectatorMode currMode;
    private SpectatorMode prevMode = SpectatorMode.ManualPose;
    private bool isSpectatorStarted = false;

    private static MenuController instance;
    public static MenuController Instance { get { return instance; } private set { instance = value; } }

    private GameObject camera360 = null;
    private GameObject cam = null;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(Find360Camera());
        StartCoroutine(FindCam());
        StartCoroutine(InitMenu());
    }

    void OnEnable()
    {
        StartCoroutine(SetCallback());
    }

    void OnDisable()
    {
        var sh = SpectatorCameraBased.Instance;
        if (sh)
        {
            sh.OnSpectatorStart -= OnSpectatorStart;
            sh.OnSpectatorStop -= OnSpectatorStop;
        }
    }

    void Update()
    {
        Scene soundScene = SceneManager.GetSceneByName("v130_demo");
        Scene currentScene = SceneManager.GetActiveScene();
        if (soundScene.IsValid() && soundScene == currentScene)
        {
            if (menuCanvas.gameObject.activeSelf)
            {
                menuCanvas.gameObject.SetActive(false);
                curvePointer.gameObject.SetActive(true);
            }
            return;
        }

        if (camera360 == null) StartCoroutine(Find360Camera());
        if (cam == null) StartCoroutine(FindCam());

#if !UNITY_EDITOR
        if (menuCanvas.gameObject.activeSelf && (!VivePose.IsConnectedEx(ControllerRole.LeftHand) || !VivePose.IsValidEx(ControllerRole.LeftHand)))
        {
            menuCanvas.gameObject.SetActive(false);
            curvePointer.gameObject.SetActive(true);
            return;
        }
#endif

        dPadUp = ViveInput.GetPressDownEx(ControllerRole.LeftHand, ControllerButton.DPadUpTouch);
        if ((dPadUp || Input.GetKeyDown(KeyCode.UpArrow))
#if !UNITY_EDITOR
            && !TutorialManager.Instance.IsInTutorial()
#endif
            )
        {
            scrollView.SelectPrevCell();
        }

        dPadDown = ViveInput.GetPressDownEx(ControllerRole.LeftHand, ControllerButton.DPadDownTouch);
        if ((dPadDown || Input.GetKeyDown(KeyCode.DownArrow))
#if !UNITY_EDITOR
            && !TutorialManager.Instance.IsInTutorial()
#endif
            )
        {
            scrollView.SelectNextCell();
        }

        menuDown = ViveInput.GetPressDownEx(ControllerRole.LeftHand, ControllerButton.System);
        if ((menuDown || Input.GetKeyDown(KeyCode.M))
#if !UNITY_EDITOR
            && !TutorialManager.Instance.IsInTutorial()
#endif
            )
        {
            menuCanvas.gameObject.SetActive(!menuCanvas.gameObject.activeSelf);
            curvePointer.gameObject.SetActive(!menuCanvas.gameObject.activeSelf);

            if (menuCanvas.gameObject.activeSelf)
            {
                MenuPresenter.Instance.UpdateMainMenu();
            }
            else
            {
                bool setPrevIndex = false;
                var index = scrollView.GetSelectedIndex();
                if (index >= 0)
                {
                    var item = scrollView.GetSelectedItemData();
                    if (item.Type == ItemData.MenuType.Main)
                    {
                        MenuPresenter.Instance.SetPrevSelectedIndex(index);
                        setPrevIndex = true;
                    }
                }
                if (!setPrevIndex)
                {
                    // 0: Menu, 1: Teleport, 2: Bodytracking, 3: Spectator
                    var msg = scrollView.GetFirstItemData().Message;
                    switch (msg)
                    {
                        case var s when msg.Contains("Menu"):
                            MenuPresenter.Instance.SetPrevSelectedIndex(0);
                            break;
                        case var s when msg.Contains("Teleport"):
                            MenuPresenter.Instance.SetPrevSelectedIndex(1);
                            break;
                        case var s when msg.Contains("Bodytracking"):
                            MenuPresenter.Instance.SetPrevSelectedIndex(2);
                            break;
                        case var s when msg.Contains("Spectator"):
                            MenuPresenter.Instance.SetPrevSelectedIndex(3);
                            break;
                    }
                }
            }
        }

        triggerDown = ViveInput.GetPressDownEx(ControllerRole.LeftHand, ControllerButton.Trigger);
        if (menuCanvas.gameObject.activeSelf && (triggerDown || Input.GetKeyDown("space")))
        {
            var item = scrollView.GetSelectedItemData();
            var camera = Camera.main.transform;
            switch (item.Message)
            {
                case var s when item.Message.StartsWith("<"):
                    Debug.Log("[MenuController] BackToMainMenu");
                    MenuPresenter.Instance.BackToMainMenu();
                    break;
                case var s when item.Message.StartsWith("Bodytracking:"):
                    Debug.Log("[MenuController] Bodytracking switch");
                    if (TutorialManager.Instance != null && TutorialManager.Instance.IsInFullTutorial()) return;
                    if (VRSBodyTrackingManager.Instance.IsTracking())
                    {
                        Debug.Log("[MenuController] Stop bodytracking");
                        VRSBodyTrackingManager.Instance.StopTracking();
                    }
                    else
                    {
                        Debug.Log("[MenuController] Start bodytracking");
                        VRSBodyTrackingManager.Instance.StartBodyTracking();
                    }
                    break;
                case var s when item.Message.StartsWith("Spectator")
                || item.Message.StartsWith("Bodytracking")
                //|| item.Message.StartsWith("Palm Map")
                || item.Message.StartsWith("Teleport")
                || item.Message.StartsWith("Menu"):
                    Debug.Log("[MenuController] Show submenu");
                    MenuPresenter.Instance.ShowSubmenu();
                    break;
                case var s when item.Message.StartsWith("View"):
                    Debug.Log("[MenuController] Spectator view");
                    if (TutorialManager.Instance != null && TutorialManager.Instance.IsInFullTutorial()) return;
                    Debug.Log("[MenuController] Spectator change to next view");
                    VRSSpectatorManager.Instance.ChangeToNextMode();
                    break;
                case var s when item.Message.Equals("Tutorial"):
                    Debug.Log("[MenuController] Tutorial");
                    if (scrollView.GetFirstItemData().Message.Contains("Bodytracking"))
                    {
                        Debug.Log("[MenuController] Bodytracking tutorial");
                        TutorialManager.Instance.BodyTrackingTutorial();
                        menuCanvas.gameObject.SetActive(false);
                        curvePointer.gameObject.SetActive(true);
                    }
                    else if (scrollView.GetFirstItemData().Message.Contains("Spectator"))
                    {
                        Debug.Log("[MenuController] Spectator tutorial");
                        TutorialManager.Instance.SpectatorTutorial();
                    }
                    else if (scrollView.GetFirstItemData().Message.Contains("Teleport"))
                    {
                        Debug.Log("[MenuController] Teleport tutorial");
                        TutorialManager.Instance.TeleportTutorial();
                        menuCanvas.gameObject.SetActive(false);
                        curvePointer.gameObject.SetActive(true);
                    }
                    else if (scrollView.GetFirstItemData().Message.Contains("Menu"))
                    {
                        Debug.Log("[MenuController] Menu tutorial");
                        TutorialManager.Instance.MenuTutorial();
                        menuCanvas.gameObject.SetActive(false);
                        curvePointer.gameObject.SetActive(true);
                    }
                    break;
                case var s when item.Message.EndsWith(" Tutorial"):
                    Debug.Log("[MenuController] Dynamic tutorial");
                    if (item.Message.Contains("3D Object"))
                    {
                        Debug.Log("[MenuController] 3D Object tutorial");
                        TutorialManager.Instance.Object3DTutorial();
                    }
                    else if (item.Message.Contains("3D Sound"))
                    {
                        Debug.Log("[MenuController] 3D Sound tutorial");
                        TutorialManager.Instance.Sound3DTutorial();
                    }
                    else if (item.Message.Contains("Bottles"))
                    {
                        Debug.Log("[MenuController] Bottles tutorial");
                        TutorialManager.Instance.ThrowBottlesTutorial();
                    }
                    else if (item.Message.Contains("Bubbles"))
                    {
                        Debug.Log("[MenuController] Bubbles tutorial");
                        TutorialManager.Instance.BubblesTutorial();
                    }
                    else if (item.Message.Contains("Keyboard"))
                    {
                        Debug.Log("[MenuController] Keyboard tutorial");
                        TutorialManager.Instance.KeyboardTutorial();
                    }
                    else if (item.Message.Contains("Reset"))
                    {
                        Debug.Log("[MenuController] Reset Tutorial");
                        RobotAnchorManager.Instance.ResetTutorial();
                        menuCanvas.gameObject.SetActive(false);
                        curvePointer.gameObject.SetActive(true);
                    }
                    break;
                case var s when item.Message.Equals("Panorama"):
                    Debug.Log("[MenuController] Panorama ++");
                    camera360.transform.position = camera.position + Vector3.ProjectOnPlane(camera.forward, Vector3.up).normalized * 1f;
                    Debug.Log("[MenuController] Panorama tutorial manager is null?" + TutorialManager.Instance == null);
                    TutorialManager.Instance.PanoramaTutorial();
                    Debug.Log("[MenuController] Panorama --");
                    break;
                case var s when item.Message.Equals("Reset Camera"):
                    cam.transform.position = camera.position + Vector3.ProjectOnPlane(camera.forward, Vector3.up).normalized * 1f;
                    cam.transform.rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.ProjectOnPlane(-camera.forward, Vector3.up).normalized);
                    break;
            }
        }

        keyADown = ViveInput.GetPressDownEx(ControllerRole.LeftHand, ControllerButton.AKey);
        if (keyADown)
        {
            if (MenuPresenter.Instance != null && MenuPresenter.Instance.IsInSubmenu() && menuCanvas.gameObject.activeSelf) MenuPresenter.Instance.BackToMainMenu();
        }

        if (MenuPresenter.Instance != null) MenuPresenter.Instance.UpdateBodytrackingStatus();

        if (VRSSpectatorManager.Instance == null) return;

        currMode = VRSSpectatorManager.Instance.spectatorMode;
        if (currMode != prevMode)
        {
            if (MenuPresenter.Instance != null)
            {
                MenuPresenter.Instance.UpdateSpectatorViewItem("View: " + currMode.ToString());

                if (currMode == SpectatorMode.ManualPose)
                {
                    MenuPresenter.Instance.AddSpectatorResetCameraViewItem();
                }
                else if (prevMode == SpectatorMode.ManualPose)
                {
                    MenuPresenter.Instance.RemoveSpectatorResetCameraViewItem();
                }
            }
        }

        prevMode = currMode;
    }

    IEnumerator SetCallback()
    {
        yield return new WaitUntil(() => SpectatorCameraBased.Instance != null);
        var sh = SpectatorCameraBased.Instance;

        sh.OnSpectatorStart += OnSpectatorStart;
        sh.OnSpectatorStop += OnSpectatorStop;
    }

    IEnumerator Find360Camera()
    {
        yield return new WaitUntil(() => Is360CameraFound());
    }

    private bool Is360CameraFound()
    {
        if (camera360 == null)
        {
            camera360 = GameObject.Find("360Camera");
        }

        return (camera360 != null);
    }

    IEnumerator FindCam()
    {
        yield return new WaitUntil(() => IsCamFound());
    }

    private bool IsCamFound()
    {
        if (cam == null)
        {
            cam = GameObject.Find("Cam");
        }

        return (cam != null);
    }

    private void OnSpectatorStart()
    {
        isSpectatorStarted = true;
    }

    private void OnSpectatorStop()
    {
        isSpectatorStarted = false;
    }

    public bool IsMenuOpened()
    {
        return menuCanvas.gameObject.activeSelf;
    }

    public void OpenMenu()
    {
        if (menuCanvas.gameObject.activeSelf) return;

        menuCanvas.gameObject.SetActive(true);
        curvePointer.gameObject.SetActive(false);

        if (menuCanvas.gameObject.activeSelf) MenuPresenter.Instance.UpdateMainMenu();
    }

    public void OpenSpectatorSubmenu()
    {
        if (menuCanvas.gameObject.activeSelf)
            MenuPresenter.Instance.ShowSpectatorSubmenu();
        else
        {
            menuCanvas.gameObject.SetActive(true);
            curvePointer.gameObject.SetActive(false);
            MenuPresenter.Instance.ShowSpectatorSubmenu();
        }
    }

    IEnumerator InitMenu()
    {
        OpenMenu();
        yield return new WaitForSeconds(1);
        menuCanvas.gameObject.SetActive(false);
        curvePointer.gameObject.SetActive(true);
    }
}
