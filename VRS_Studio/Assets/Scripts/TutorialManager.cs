using FancyScrollView.Example03;
using HTC.UnityPlugin.Vive;
using System.Collections;
using UnityEngine;
using VRSStudio.Avatar;
using VRSStudio.Spectator;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Teleportable teleportable;
    [SerializeField] private AudioClip[] menuClips;
    [SerializeField] private AudioClip[] teleportClips;
    [SerializeField] private AudioClip[] bodytrackingClips;
    [SerializeField] private AudioClip[] panoramaClips;
    [SerializeField] private AudioClip[] spectatorClips;
    [SerializeField] private AudioClip[] tutorialClips;
    [SerializeField] private GameObject[] indicators;

    private static TutorialManager instance;
    public static TutorialManager Instance { get { return instance; } private set { instance = value; } }

    private const string WelcomeIntro = "Hey there! Welcome to VRS Studio, your gateway to innovative virtual reality experiences.";
    private const string MenuOpen = "You can open the application menu at anytime to use different features or listen to the tutorials. First, please press the menu button on the left controller.";
    private const string MenuTriggerButton = "Press the trigger button to select an option from the menu.";
    private const string MenuXButton = "Press the X button to return to the previous page.";
    private const string MenuClose = "Press the menu button again to close the menu.";

    private const string TeleportSelectTutorial = "Next, select Tutorial under Teleport to start left controller teleportation tutorial.";
    private const string TeleportControllerConnected = "Make sure your left controller is properly connected.";
    private const string TeleportPushJoystickUp = "Hold the left controller firmly in your hand, push the joystick forward.";
    private const string TeleportCurveRaycast = "You'll notice a curve extending forward, with a marker appearing at the endpoint.";
    private const string TeleportReleaseJoystick = "Once you've selected your destination, release the joystick.";
    private const string TeleportTeleported = "Instantly, you'll be teleported to the spot where the marker is located.";
    private const string TeleportCongrats = "Congratulations! You've successfully finish teleportation tutorial!";

    private const string BodytrackingIndicator = "Please look around and teleport to the designated location for tutorial.";
    private const string BodytrackingIntro = "Are you ready to experience body tracking? Let's dive in and learn how to activate the avatar and do calibration with the controllers.";
    private const string BodytrackingControllerConnected = "Make sure your controllers are properly connected.";
    private const string BodytrackingXAButtons = "Press the A button on the right controller and the X button on the left controller for a second.";
    private const string BodytrackingFollowPose = "Follow the avatar's pose for calibration.";
    private const string BodytrackingExtendArms = "Hold the controllers and extend your arms straight in front of your chest.";
    private const string BodytrackingCalibrated = "That's it! You've successfully activated your avatar and body tracking is ready for your action and movement!";

    private const string SpectatorIntro = "Spectator Camera allows you to freely switch between different camera positions.";
    private const string SpectatorRecording = "Begin by enabling recording on the Vive menu.";
    private const string SpectatorSwitchView = "Next, press the menu button on the left controller to switch camera position under spectator. Here, you'll find various camera positions.";
    private const string SpectatorHeadset = "Choose a camera position align with the headset's viewpoint";
    private const string SpectatorSelfie = "or fixed in selfie position.";
    private const string SpectatorTracker = "Connect an ultimate tracker for mobility within the playground, allowing you to move to desired angles.";
    private const string SpectatorManual = "Without a connected tracker, you can still move the virtual camera independently.";

    private const string PanoramaIntro = "This sphere helps you take panoramic pictures.";
    private const string PanoramaAnywhere = "Just grab it and place it anywhere in the scene.";
    private const string PanoramaYButton = "Then, hit the 'Y' button on the left controller to take the shot.";
    private const string PanoramaGallery = "You can view the result in the system gallery afterwards!";

    private const string BottlesTutorial = "Throw the water bottle into the bucket! Lift the water bottle with the controller and then throw it into the bucket!";
    private const string NextDesignatedLoc = "Please teleport to the next designated location to experience.";
    private const string KeyboardTutorialText = "Please try to input text using the keyboard. You can adjust the keyboard placement to achieve a comfortable typing position.";

    private const string BubblesIntro = "Get ready for some bubble-blowing fun with VIVE Full Face Tracker.";
    private const string BubblesCalibration = "Please ensure you have the Face Tracker properly attached and calibrated.";
    private const string BubblesOShape = "Form your mouth into an 'O' shape and blow gently towards the Facial Tracker. Keep blowing to trigger bubbles in the area. Keep staring at the bubble as it expands.";

    private const string Sound3DTutorialText = "Let's begin experiencing the wonderful features of VR spatial sound! Are you ready to enter the world of 3D sound? Let's explore together by touching the painting on the wall!";
    private const string Object3DTutorialText = "Welcome to our Throwing Area ¡V the spot to experience the thrill of tossing items with precision and accuracy. Take your pick from balls and airships on the shelf.";

    private bool isInTutorial = false;
    private bool isInFullTutorial = false;
    private bool teleportTutorialSelected = false;
    private bool allowTeleport = true;
    private bool isArmStretched = false;
    private bool allowCalibrate = true;
    private float oneSecTimer = 1f;
    private float sixSecTimer = 6f;
    private bool hasRepeated = false;

    private Transform vrOrigin = null;
    private IEnumerator coroutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (vrOrigin == null)
            vrOrigin = GameObject.Find("VROrigin").transform;
    }

    private void Update()
    {
        if (hasRepeated && !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking)
        {
            if (oneSecTimer > 0f)
            {
                oneSecTimer -= Time.deltaTime;
            }
            else
            {
                oneSecTimer = 1f;
                hasRepeated = false;
                StopCoroutine(coroutine);
                isInTutorial = false;
                isInFullTutorial = false;
                TextAssistant.Instance.SetActive(false);

                for (int i = 0; i < indicators.Length; i++) indicators[i].SetActive(false);
            }
        }
    }

    public void TutorialSequence()
    {
        if (isInTutorial || isInFullTutorial) return;
        if (VRSBodyTrackingManager.Instance.IsTracking())
        {
            VRSBodyTrackingManager.Instance.StopTracking();
        }

        allowTeleport = false;
        isArmStretched = false;
        isInFullTutorial = true;
        coroutine = Tutorials();
        StartCoroutine(coroutine);
    }

    IEnumerator Tutorials()
    {
        yield return StartCoroutine(CheckControllersConnected());
        yield return StartCoroutine(PlayWelcomeAudio());
        yield return StartCoroutine(PlayMenuAudio());
        yield return new WaitForSeconds(1);
        TextAssistant.Instance.SetActive(true);
        TextAssistant.Instance.SetText(TeleportSelectTutorial);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(teleportClips[1], false);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        yield return new WaitUntil(() => CheckTeleportTutorialSelected());
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetActive(false);
        isInTutorial = true;
        yield return StartCoroutine(PlayTeleportAudio());
        yield return StartCoroutine(TeleportBodytrackingAudio());
        yield return StartCoroutine(PlayBodyTrackingAudio());
        yield return StartCoroutine(TeleportBubbleAudio());
        yield return StartCoroutine(PlayBubblesAudio());
        yield return StartCoroutine(TeleportSpectatorAudio());
        yield return StartCoroutine(PlaySpectatorAudio());
        yield return StartCoroutine(Teleport3DSoundAudio());
        yield return StartCoroutine(Play3DSoundAudio());
        isInTutorial = false;
        isInFullTutorial = false;
    }

    IEnumerator PlayWelcomeAudio()
    {
        RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();

        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetActive(true);
        TextAssistant.Instance.SetText(WelcomeIntro);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(teleportClips[0], false);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
    }

    public void MenuTutorial()
    {
        if (RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking) return;

        coroutine = PlayMenuAudio();
        StartCoroutine(coroutine);
    }

    IEnumerator PlayMenuAudio()
    {
        RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();

        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetActive(true);
        TextAssistant.Instance.SetText(MenuOpen);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(menuClips[0], false);
        yield return new WaitUntil(() => CheckMenuOpened());
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetText(MenuTriggerButton);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(menuClips[1], false);
        yield return new WaitUntil(() => CheckSubmenuOpened());
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetText(MenuXButton);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(menuClips[2], false);
        yield return new WaitUntil(() => CheckMainMenuOpened());
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        if (!isInFullTutorial)
        {
            TextAssistant.Instance.SetText(MenuClose);
            RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(menuClips[3], false);
            yield return new WaitUntil(() => CheckMainMenuClosed());
            yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        }
        TextAssistant.Instance.SetActive(false);
    }

    public void TeleportTutorial()
    {
        if (isInTutorial || isInFullTutorial)
        {
            teleportTutorialSelected = true;
            return;
        }

        allowTeleport = false;
        isInTutorial = true;
        coroutine = PlayTeleportAudio();
        StartCoroutine(coroutine);
    }

    IEnumerator PlayTeleportAudio()
    {
        RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();

        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetActive(true);
        TextAssistant.Instance.SetText(TeleportPushJoystickUp);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(teleportClips[3], false);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        yield return new WaitUntil(() => CheckPadUpPressed());
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetText(TeleportCurveRaycast);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(teleportClips[4], false);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetText(TeleportReleaseJoystick);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(teleportClips[5], false);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        yield return new WaitUntil(() => CheckPadUpReleased());
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetText(TeleportTeleported);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(teleportClips[6], false);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetText(TeleportCongrats);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(teleportClips[7], false);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);

        if (!isInFullTutorial) isInTutorial = false;
        TextAssistant.Instance.SetActive(false);
    }

    public void BodyTrackingTutorial()
    {
        if (isInTutorial || isInFullTutorial) return;
        if (VRSBodyTrackingManager.Instance.IsTracking())
        {
            VRSBodyTrackingManager.Instance.StopTracking();
        }

        allowTeleport = false;
        isArmStretched = false;
        isInTutorial = true;
        coroutine = PlayBodyTrackingAudio();
        StartCoroutine(coroutine);
    }

    IEnumerator PlayBodyTrackingAudio()
    {
        RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();

        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        allowCalibrate = false;
        TextAssistant.Instance.SetActive(true);
        TextAssistant.Instance.SetText(BodytrackingIntro);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(bodytrackingClips[0], false);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        if (!(VivePose.IsValidEx(ControllerRole.RightHand) && VivePose.IsValidEx(ControllerRole.LeftHand)))
        {
            TextAssistant.Instance.SetText(BodytrackingControllerConnected);
            RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(bodytrackingClips[2], false);
            yield return new WaitForSeconds(1);
            yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
            yield return new WaitUntil(() => CheckBothControllersValid());
        }
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetText(BodytrackingXAButtons);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(bodytrackingClips[3], false);
        allowCalibrate = true;
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => CheckButtonAXPressed());
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetText(BodytrackingFollowPose);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(bodytrackingClips[4], false);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetText(BodytrackingExtendArms);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(bodytrackingClips[5], false);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => CheckCRPose());
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetText(BodytrackingCalibrated);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(bodytrackingClips[6], false);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);

        isInTutorial = false;
        allowTeleport = true;
        TextAssistant.Instance.SetActive(false);
    }

    public void BubblesTutorial()
    {
        if (RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking) return;

        allowTeleport = false;
        isInTutorial = true;
        coroutine = PlayBubblesAudio();
        StartCoroutine(coroutine);
    }

    IEnumerator PlayBubblesAudio()
    {
        RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();

        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetActive(true);
        TextAssistant.Instance.SetText(BubblesIntro);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(tutorialClips[1], false);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);

        var origin = default(Vector3);
        var direction = default(Vector3);
        var isValid = default(bool);

        if (EyeManager.Instance != null)
        {
            isValid = EyeManager.Instance.GetCombindedEyeDirectionNormalized(out direction) && EyeManager.Instance.GetCombinedEyeOrigin(out origin);

            if (isValid)
            {
                TextAssistant.Instance.SetText(BubblesOShape);
                RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(tutorialClips[3], false);
            }
            else
            {
                TextAssistant.Instance.SetText(BubblesCalibration);
                RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(tutorialClips[2], true);
            }
        }
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);

        isInTutorial = false;
        allowTeleport = true;
        TextAssistant.Instance.SetActive(false);
    }

    public void SpectatorTutorial()
    {
        if (RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking) return;

        allowTeleport = false;
        isInTutorial = true;
        coroutine = PlaySpectatorAudio();
        StartCoroutine(coroutine);
    }

    IEnumerator PlaySpectatorAudio()
    {
        RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();

        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetActive(true);
        TextAssistant.Instance.SetText(SpectatorIntro);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(spectatorClips[0], true);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetText(SpectatorRecording);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(spectatorClips[1], true);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetText(SpectatorSwitchView);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(spectatorClips[2], true);
        yield return new WaitForSeconds(1);
        MenuController.Instance.OpenSpectatorSubmenu();
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetText(SpectatorHeadset);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(spectatorClips[3], true);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        VRSSpectatorManager.Instance.ChangeToNextMode(SpectatorMode.Headset);
        yield return new WaitForSeconds(4);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetText(SpectatorSelfie);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(spectatorClips[4], true);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        VRSSpectatorManager.Instance.ChangeToNextMode(SpectatorMode.Selfie);
        yield return new WaitForSeconds(4);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetText(SpectatorTracker);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(spectatorClips[5], true);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        VRSSpectatorManager.Instance.ChangeToNextMode(SpectatorMode.Tracker);
        yield return new WaitForSeconds(4);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetText(SpectatorManual);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(spectatorClips[6], true);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        VRSSpectatorManager.Instance.ChangeToNextMode(SpectatorMode.ManualPose);
        yield return new WaitForSeconds(4);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);

        isInTutorial = false;
        allowTeleport = true;
        TextAssistant.Instance.SetActive(false);
    }

    public void Object3DTutorial()
    {
        if (RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking) return;

        allowTeleport = false;
        isInTutorial = true;
        coroutine = Play3DObjectAudio();
        StartCoroutine(coroutine);
    }

    IEnumerator Play3DObjectAudio()
    {
        RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();

        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetActive(true);
        TextAssistant.Instance.SetText(Object3DTutorialText);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(tutorialClips[4], true);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);

        if (!isInFullTutorial) isInTutorial = false;
        allowTeleport = true;
        TextAssistant.Instance.SetActive(false);
    }

    public void Sound3DTutorial()
    {
        if (RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking) return;

        coroutine = Play3DSoundAudio();
        StartCoroutine(coroutine);
    }

    IEnumerator Play3DSoundAudio()
    {
        RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();

        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetActive(true);
        TextAssistant.Instance.SetText(Sound3DTutorialText);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(tutorialClips[5], true);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetActive(false);
    }

    public void KeyboardTutorial()
    {
        if (RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking) return;

        allowTeleport = false;
        isInTutorial = true;
        coroutine = PlayKeyboardAudio();
        StartCoroutine(coroutine);
    }

    IEnumerator PlayKeyboardAudio()
    {
        RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();

        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetActive(true);
        TextAssistant.Instance.SetText(KeyboardTutorialText);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(tutorialClips[6], true);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);

        if (!isInFullTutorial) isInTutorial = false;
        allowTeleport = true;
        TextAssistant.Instance.SetActive(false);
    }

    public void ThrowBottlesTutorial()
    {
        if (RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking) return;

        allowTeleport = false;
        isInTutorial = true;
        coroutine = PlayThrowBottlesAudio();
        StartCoroutine(coroutine);
    }

    IEnumerator PlayThrowBottlesAudio()
    {
        RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();

        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetActive(true);
        TextAssistant.Instance.SetText(BottlesTutorial);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(tutorialClips[7], true);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);

        if (!isInFullTutorial) isInTutorial = false;
        allowTeleport = true;
        TextAssistant.Instance.SetActive(false);
    }

    IEnumerator TeleportBodytrackingAudio()
    {
        RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();

        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        indicators[0].SetActive(true);
        TextAssistant.Instance.SetActive(true);
        TextAssistant.Instance.SetText(BodytrackingIndicator);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(bodytrackingClips[1], false);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        yield return new WaitUntil(() => IsUserTeleportedToIndicator(0, bodytrackingClips[1]));
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetActive(false);
    }

    IEnumerator TeleportBubbleAudio()
    {
        RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();

        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        indicators[1].SetActive(true);
        TextAssistant.Instance.SetActive(true);
        TextAssistant.Instance.SetText(NextDesignatedLoc);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(tutorialClips[0], false);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        yield return new WaitUntil(() => IsUserTeleportedToIndicator(1, tutorialClips[0]));
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetActive(false);
    }

    IEnumerator TeleportSpectatorAudio()
    {
        RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();

        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        indicators[2].SetActive(true);
        TextAssistant.Instance.SetActive(true);
        TextAssistant.Instance.SetText(NextDesignatedLoc);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(tutorialClips[0], false);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        yield return new WaitUntil(() => IsUserTeleportedToIndicator(2, tutorialClips[0]));
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetActive(false);
    }

    IEnumerator Teleport3DSoundAudio()
    {
        RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();

        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        indicators[3].SetActive(true);
        TextAssistant.Instance.SetActive(true);
        TextAssistant.Instance.SetText(NextDesignatedLoc);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(tutorialClips[0], false);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        yield return new WaitUntil(() => IsUserTeleportedToIndicator(3, tutorialClips[0]));
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetActive(false);
    }

    public void PanoramaTutorial()
    {
        if (RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking) return;

        coroutine = PlayPanoramaAudio();
        StartCoroutine(coroutine);
    }

    IEnumerator PlayPanoramaAudio()
    {
        RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();

        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetActive(true);
        TextAssistant.Instance.SetText(PanoramaIntro);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(panoramaClips[0], true);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetText(PanoramaAnywhere);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(panoramaClips[1], true);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetText(PanoramaYButton);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(panoramaClips[2], true);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetText(PanoramaGallery);
        RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(panoramaClips[3], true);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        TextAssistant.Instance.SetActive(false);
    }

    IEnumerator CheckControllersConnected()
    {
        RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();

        yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
        if (!(VivePose.IsValidEx(ControllerRole.RightHand) && VivePose.IsValidEx(ControllerRole.LeftHand)))
        {
            TextAssistant.Instance.SetActive(true);
            TextAssistant.Instance.SetText(BodytrackingControllerConnected);
            RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(bodytrackingClips[2], false);
            yield return new WaitForSeconds(1);
            yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
            yield return new WaitUntil(() => CheckBothControllersValid());
            TextAssistant.Instance.SetActive(false);
        }
    }

    private bool CheckMenuOpened()
    {
        if (MenuController.Instance.IsMenuOpened())
        {
            RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();
            sixSecTimer = 6f;
            hasRepeated = false;
            return true;
        }
        else if (RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking) return false;
        else
        {
            if (sixSecTimer > 0f)
            {
                sixSecTimer -= Time.deltaTime;
            }
            else
            {
                sixSecTimer = 6f;
                hasRepeated = true;
                RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(menuClips[0], true);
            }
        }

        return false;
    }

    private bool CheckSubmenuOpened()
    {
        if (MenuPresenter.Instance.IsInSubmenu())
        {
            RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();
            sixSecTimer = 6f;
            hasRepeated = false;
            return true;
        }
        else if (RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking) return false;
        else
        {
            if (sixSecTimer > 0f)
            {
                sixSecTimer -= Time.deltaTime;
            }
            else
            {
                sixSecTimer = 6f;
                hasRepeated = true;
                RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(menuClips[1], true);
            }
        }

        return false;
    }

    private bool CheckMainMenuOpened()
    {
        if (MenuController.Instance.IsMenuOpened() && !MenuPresenter.Instance.IsInSubmenu())
        {
            RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();
            sixSecTimer = 6f;
            hasRepeated = false;
            return true;
        }
        else if (RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking) return false;
        else
        {
            if (sixSecTimer > 0f)
            {
                sixSecTimer -= Time.deltaTime;
            }
            else
            {
                sixSecTimer = 6f;
                hasRepeated = true;
                RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(menuClips[2], true);
            }
        }

        return false;
    }

    private bool CheckMainMenuClosed()
    {
        if (!MenuController.Instance.IsMenuOpened() && !MenuPresenter.Instance.IsInSubmenu())
        {
            RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();
            sixSecTimer = 6f;
            hasRepeated = false;
            return true;
        }
        else if (RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking) return false;
        else
        {
            if (sixSecTimer > 0f)
            {
                sixSecTimer -= Time.deltaTime;
            }
            else
            {
                sixSecTimer = 6f;
                hasRepeated = true;
                RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(menuClips[3], true);
            }
        }

        return false;
    }

    private bool CheckTeleportTutorialSelected()
    {
        if (teleportTutorialSelected)
        {
            teleportTutorialSelected = false;
            RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();
            sixSecTimer = 6f;
            hasRepeated = false;
            return true;
        }
        else if (RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking) return false;
        else
        {
            if (sixSecTimer > 0f)
            {
                sixSecTimer -= Time.deltaTime;
            }
            else
            {
                sixSecTimer = 6f;
                hasRepeated = true;
                RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(teleportClips[1], true);
            }
        }

        return false;
    }

    private bool CheckPadUpPressed()
    {
        var padUpTouch = ViveInput.GetPressEx(ControllerRole.LeftHand, ControllerButton.DPadUpTouch);

        if (padUpTouch)
        {
            RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();
            sixSecTimer = 6f;
            hasRepeated = false;
            return true;
        }
        else if (RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking) return false;
        else
        {
            if (sixSecTimer > 0f)
            {
                sixSecTimer -= Time.deltaTime;
            }
            else
            {
                sixSecTimer = 6f;
                hasRepeated = true;
                RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(teleportClips[3], true);
            }
        }

        return false;
    }

    private bool CheckPadUpReleased()
    {
        var padUpTouchReleased = ViveInput.GetPressUpEx(ControllerRole.LeftHand, ControllerButton.DPadUpTouch);

        if (padUpTouchReleased)
        {
            allowTeleport = true;
            RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();
            sixSecTimer = 6f;
            hasRepeated = false;
            return true;
        }
        else if (RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking) return false;
        else
        {
            if (sixSecTimer > 0f)
            {
                sixSecTimer -= Time.deltaTime;
            }
            else
            {
                sixSecTimer = 6f;
                hasRepeated = true;
                RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(teleportClips[5], true);
            }
        }

        return false;
    }

    private bool IsUserTeleportedToIndicator(int idx, AudioClip ac)
    {
        if (!indicators[idx].activeSelf)
        {
            RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();
            sixSecTimer = 6f;
            hasRepeated = false;
            return true;
        }
        else if (RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking) return false;
        else
        {
            if (sixSecTimer > 0f)
            {
                sixSecTimer -= Time.deltaTime;
            }
            else
            {
                sixSecTimer = 6f;
                hasRepeated = true;
                RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(ac, true);
            }
        }

        return false;
    }

    private bool CheckBothControllersValid()
    {
        if (VivePose.IsValidEx(ControllerRole.RightHand) && VivePose.IsValidEx(ControllerRole.LeftHand))
        {
            RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();
            sixSecTimer = 6f;
            hasRepeated = false;
            return true;
        }
        else if (RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking) return false;
        else
        {
            if (sixSecTimer > 0f)
            {
                sixSecTimer -= Time.deltaTime;
            }
            else
            {
                sixSecTimer = 6f;
                hasRepeated = true;
                RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(bodytrackingClips[2], true);
            }
        }

        return false;
    }

    private bool CheckButtonAXPressed()
    {
        var buttonAPressed = ViveInput.GetPressEx(ControllerRole.RightHand, ControllerButton.AKey);
        var buttonXPressed = ViveInput.GetPressEx(ControllerRole.LeftHand, ControllerButton.AKey);

        if ((buttonAPressed && buttonXPressed) || VRSBodyTrackingManager.Instance.IsCalibrating())
        {
            if (oneSecTimer > 0f)
            {
                oneSecTimer -= Time.deltaTime;
            }
            else
            {
                RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();
                oneSecTimer = 1f;
                sixSecTimer = 6f;
                hasRepeated = false;
                return true;
            }
        }
        else if (RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking) return false;
        else
        {
            if (sixSecTimer > 0f)
            {
                sixSecTimer -= Time.deltaTime;
            }
            else
            {
                sixSecTimer = 6f;
                hasRepeated = true;
                RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(bodytrackingClips[3], true);
            }
        }

        return false;
    }

    private bool CheckCRPose()
    {
        var hmdPose = VivePose.GetPose(DeviceRole.Hmd, vrOrigin);
        Vector3 rightCRPos = VivePose.GetPoseEx(ControllerRole.RightHand, vrOrigin).pos;
        Vector3 leftCRPos = VivePose.GetPoseEx(ControllerRole.LeftHand, vrOrigin).pos;

        Vector3 vt1 = Vector3.ProjectOnPlane(rightCRPos - hmdPose.pos, Vector3.up).normalized;
        Vector3 vt2 = Vector3.ProjectOnPlane(leftCRPos - hmdPose.pos, Vector3.up).normalized;
        Vector3 vtf = Vector3.ProjectOnPlane(hmdPose.forward, Vector3.up).normalized;
        float v1 = Vector3.Dot(vt1, vt2);
        Vector3 v2 = Vector3.Cross(vtf, vt1);
        Vector3 v3 = Vector3.Cross(vt2, vtf);

        Vector3 hmdPos = Vector3.ProjectOnPlane(hmdPose.pos, Vector3.up);
        Vector3 rCRPos = Vector3.ProjectOnPlane(rightCRPos, Vector3.up);
        Vector3 lCRPos = Vector3.ProjectOnPlane(leftCRPos, Vector3.up);

        if (Vector3.Distance(rCRPos, hmdPos) > 0.45f && Vector3.Distance(lCRPos, hmdPos) > 0.45f
            && v1 > 0.8 && v2.y > 0f && v3.y > 0f)
        {
            RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();
            isArmStretched = true;
            sixSecTimer = 6f;
            hasRepeated = false;
            return true;
        }
        else if (RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking) return false;
        else
        {
            if (sixSecTimer > 0f)
            {
                sixSecTimer -= Time.deltaTime;
            }
            else
            {
                sixSecTimer = 6f;
                hasRepeated = true;
                RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(bodytrackingClips[5], true);
            }
        }

        return false;
    }

    public bool IsInTutorial()
    {
        return isInTutorial;
    }

    public bool IsInFullTutorial()
    {
        return isInFullTutorial;
    }

    public bool IsArmStretched()
    {
        return isArmStretched;
    }

    public bool AllowCalibrate()
    {
        return allowCalibrate;
    }

    public void CanTeleport()
    {
        if (isInTutorial && !allowTeleport) teleportable.AbortTeleport();
    }
}
