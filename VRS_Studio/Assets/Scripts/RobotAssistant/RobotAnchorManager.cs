using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAnchorManager : MonoBehaviour
{
    private const string FirstVisitPrefKey = "FirstVisit";
    private const string KeyboardVisitPrefKey = "KeyboardVisit";
    private const string Object3DVisitPrefKey = "Object3DVisit";
    private const string BottlesVisitPrefKey = "BottlesVisit";

    [SerializeField] private GameObject robot;
    [SerializeField] private GameObject[] anchors;
    [SerializeField] private AudioClip[] audioClips;

    private static RobotAnchorManager instance;
    public static RobotAnchorManager Instance { get { return instance; } private set { instance = value; } }

    private const string WelcomeIntro = "Hey there! Welcome to VRS Studio, your gateway to innovative virtual reality experiences.";

    public enum AnchorState
    {
        Invalid = -1,
        RoomCenter,
        Keyboard,
        Spectator,
        ThrowBottles,
        Object3D,
        VRMAvatar,
        Sound3D,
        Bubbles,
    }

    private static AnchorState currState = AnchorState.Invalid;
    private static AnchorState prevState = AnchorState.Invalid;
    private static List<AnchorState> visitedList = new List<AnchorState>();

    private bool isFirstVisit = false;
    private bool isKeyboardVisited = false;
    private bool isObject3DVisited = false;
    private bool isBottlesVisited = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        isFirstVisit = !PlayerPrefs.HasKey(FirstVisitPrefKey);
        if (isFirstVisit)
        {
            PlayerPrefs.SetInt(FirstVisitPrefKey, 0);
            PlayerPrefs.Save();
        }

        isKeyboardVisited = PlayerPrefs.HasKey(KeyboardVisitPrefKey);
        isObject3DVisited = PlayerPrefs.HasKey(Object3DVisitPrefKey);
        isBottlesVisited = PlayerPrefs.HasKey(BottlesVisitPrefKey);

        StartCoroutine(RobotAssistantManager.Instance.PowerOnRobot());
    }

    void Update()
    {
        if (RobotAssistantManager.Instance != null && !RobotAssistantManager.Instance.IsIdle()) return;

        if (RobotAssistantManager.Instance != null && isFirstVisit && !visitedList.Contains(AnchorState.RoomCenter)
            && RobotAssistantManager.Instance.IsIdle() && !TutorialManager.Instance.IsInFullTutorial())
        {

            visitedList.Add(AnchorState.RoomCenter);
            StartCoroutine(PlayRoomCenterAudio());
            return;
        }

        if (TutorialManager.Instance != null && TutorialManager.Instance.IsInFullTutorial()) return;

        switch (currState)
        {
            //case AnchorState.RoomCenter:
            //    if (!visitedList.Contains(currState)
            //        && Vector3.Distance(robot.transform.position, anchors[(int)AnchorState.RoomCenter].transform.position) == 0f)
            //    {
            //        visitedList.Add(currState);
            //        StartCoroutine(PlayRoomCenterAudio());
            //    }
            //    break;
            case AnchorState.Keyboard:
                if (!isKeyboardVisited && !visitedList.Contains(currState)
                    && Vector3.Distance(robot.transform.position, anchors[(int)AnchorState.Keyboard].transform.position) == 0f)
                {
                    visitedList.Add(currState);
                    TutorialManager.Instance.KeyboardTutorial();
                    isKeyboardVisited = true;
                    PlayerPrefs.SetInt(KeyboardVisitPrefKey, 0);
                    PlayerPrefs.Save();
                }
                break;
            //case AnchorState.Spectator:
            //    if (!visitedList.Contains(currState)
            //        && Vector3.Distance(robot.transform.position, anchors[(int)AnchorState.Spectator].transform.position) == 0f)
            //    {
            //        visitedList.Add(currState);
            //        TutorialManager.Instance.SpectatorTutorial();
            //    }
            //    break;
            case AnchorState.Object3D:
                if (!isObject3DVisited && !visitedList.Contains(currState)
                    && Vector3.Distance(robot.transform.position, anchors[(int)AnchorState.Object3D].transform.position) == 0f)
                {
                    visitedList.Add(currState);
                    TutorialManager.Instance.Object3DTutorial();
                    isObject3DVisited = true;
                    PlayerPrefs.SetInt(Object3DVisitPrefKey, 0);
                    PlayerPrefs.Save();
                }
                break;
            case AnchorState.ThrowBottles:
                if (!isBottlesVisited && !visitedList.Contains(currState)
                    && Vector3.Distance(robot.transform.position, anchors[(int)AnchorState.ThrowBottles].transform.position) == 0f)
                {
                    visitedList.Add(currState);
                    TutorialManager.Instance.ThrowBottlesTutorial();
                    isBottlesVisited = true;
                    PlayerPrefs.SetInt(BottlesVisitPrefKey, 0);
                    PlayerPrefs.Save();
                }
                break;
                //case AnchorState.VRMAvatar:
                //    if (!visitedList.Contains(currState)
                //        && Vector3.Distance(robot.transform.position, anchors[(int)AnchorState.VRMAvatar].transform.position) == 0f)
                //    {
                //        visitedList.Add(currState);
                //        TutorialManager.Instance.BodyTrackingTutorial();
                //    }
                //    break;
                //case AnchorState.Bubbles:
                //    if (!visitedList.Contains(currState)
                //        && Vector3.Distance(robot.transform.position, anchors[(int)AnchorState.Bubbles].transform.position) == 0f)
                //    {
                //        visitedList.Add(currState);
                //        TutorialManager.Instance.BubblesTutorial();
                //    }
                //    break;
                //case AnchorState.Sound3D:
                //    if (!visitedList.Contains(currState)
                //        && Vector3.Distance(robot.transform.position, anchors[(int)AnchorState.Sound3D].transform.position) == 0f)
                //    {
                //        //visitedList.Add(currState);
                //        if (prevState != currState) TutorialManager.Instance.Sound3DTutorial();
                //        prevState = currState;
                //    }
                //    break;
        }
    }

    public void RoomCenterAction()
    {
        currState = AnchorState.RoomCenter;
        StartCoroutine(MoveToRoomCenter());
    }

    IEnumerator MoveToRoomCenter()
    {
        yield return new WaitUntil(() => RobotAssistantManager.Instance.IsIdle());
        if (visitedList.Contains(currState)) StartCoroutine(RobotAssistantManager.Instance.RobotStartTeleport(anchors[(int)AnchorState.RoomCenter].transform.position));
        else RobotAssistantManager.Instance.SetRobotPosition(anchors[(int)AnchorState.RoomCenter].transform.position);
    }

    public void KeyboardAction()
    {
        currState = AnchorState.Keyboard;
        StartCoroutine(MoveToKeyboard());
    }

    IEnumerator MoveToKeyboard()
    {
        yield return new WaitUntil(() => RobotAssistantManager.Instance.IsIdle());
        StartCoroutine(RobotAssistantManager.Instance.RobotStartTeleport(anchors[(int)AnchorState.Keyboard].transform.position));
    }

    public void SpectatorAction()
    {
        currState = AnchorState.Spectator;
        StartCoroutine(MoveToSpectator());
    }

    IEnumerator MoveToSpectator()
    {
        yield return new WaitUntil(() => RobotAssistantManager.Instance.IsIdle());
        StartCoroutine(RobotAssistantManager.Instance.RobotStartTeleport(anchors[(int)AnchorState.Spectator].transform.position));
    }

    public void ThrowBottlesAction()
    {
        currState = AnchorState.ThrowBottles;
        StartCoroutine(MoveToThrowBottles());
    }

    IEnumerator MoveToThrowBottles()
    {
        yield return new WaitUntil(() => RobotAssistantManager.Instance.IsIdle());
        StartCoroutine(RobotAssistantManager.Instance.RobotStartTeleport(anchors[(int)AnchorState.ThrowBottles].transform.position));
    }

    public void Object3DAction()
    {
        currState = AnchorState.Object3D;
        StartCoroutine(MoveToObject3D());
    }

    IEnumerator MoveToObject3D()
    {
        yield return new WaitUntil(() => RobotAssistantManager.Instance.IsIdle());
        StartCoroutine(RobotAssistantManager.Instance.RobotStartTeleport(anchors[(int)AnchorState.Object3D].transform.position));
    }

    public void VRMAvatarAction()
    {
        currState = AnchorState.VRMAvatar;
        StartCoroutine(MoveToVRMAvatar());
    }

    IEnumerator MoveToVRMAvatar()
    {
        yield return new WaitUntil(() => RobotAssistantManager.Instance.IsIdle());
        StartCoroutine(RobotAssistantManager.Instance.RobotStartTeleport(anchors[(int)AnchorState.VRMAvatar].transform.position));
    }

    public void Sound3DAction()
    {
        currState = AnchorState.Sound3D;
        StartCoroutine(MoveToSound3D());
    }

    IEnumerator MoveToSound3D()
    {
        yield return new WaitUntil(() => RobotAssistantManager.Instance.IsIdle());
        StartCoroutine(RobotAssistantManager.Instance.RobotStartTeleport(anchors[(int)AnchorState.Sound3D].transform.position));
    }

    public void BubblesAction()
    {
        currState = AnchorState.Bubbles;
        StartCoroutine(MoveToBubbles());
    }

    IEnumerator MoveToBubbles()
    {
        yield return new WaitUntil(() => RobotAssistantManager.Instance.IsIdle());
        StartCoroutine(RobotAssistantManager.Instance.RobotStartTeleport(anchors[(int)AnchorState.Bubbles].transform.position));
    }

    IEnumerator PlayRoomCenterAudio()
    {
        yield return new WaitUntil(() => RobotAssistantManager.Instance.IsIdle());
        RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();

        if (isFirstVisit)
        {
            isFirstVisit = false;
            TutorialManager.Instance.TutorialSequence();
        }
        else
        {
            TextAssistant.Instance.SetActive(true);
            TextAssistant.Instance.SetText(WelcomeIntro);
            RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(audioClips[0], true);
            yield return new WaitForSeconds(1);
            yield return new WaitUntil(() => !RobotAssistantManager.Instance.robotAssistantAudioSource.IsSpeaking);
            TutorialManager.Instance.MenuTutorial();
        }
    }

    public void ResetTutorial()
    {
        visitedList.Clear();
        isFirstVisit = true;
        isKeyboardVisited = false;
        isObject3DVisited = false;
        isBottlesVisited = false;
        PlayerPrefs.DeleteKey(KeyboardVisitPrefKey);
        PlayerPrefs.DeleteKey(Object3DVisitPrefKey);
        PlayerPrefs.DeleteKey(BottlesVisitPrefKey);
        PlayerPrefs.Save();
    }
}
