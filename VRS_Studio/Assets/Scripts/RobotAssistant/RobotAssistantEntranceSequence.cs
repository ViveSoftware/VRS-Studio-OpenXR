using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections;
using UnityEngine;

public class RobotAssistantEntranceSequence : MonoBehaviour
{
    public RobotAssistantManager Instance = null;

    private string tutorialIntroLine = "Hello there! It seems that this is your first time here, let's warm ourselves up for a bit.";
    private string replayIntroLine = "Hey, you are back! Open the menu to try out different experiences.";

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = RobotAssistantManager.Instance;
        }

        RobotAssistantLoSCaster.RobotAssistantLoS_EnterCallback += PowerOnSequence;
    }

    #region LoS reaction
    private void PowerOnSequence()
    {
        RobotAssistantLoSCaster.RobotAssistantLoS_EnterCallback -= PowerOnSequence;
        StartCoroutine(PowerOnSequenceCoroutine());
    }

    private IEnumerator PowerOnSequenceCoroutine()
    {
        yield return StartCoroutine(Instance.PowerOnRobot());
        Instance.SetRobotPosition(new Vector3(transform.position.x, Camera.main.transform.position.y - 0.35f, transform.position.z), PowerOnSequenceComplete);
    }

    private async void PowerOnSequenceComplete()
    {
        //VRSStudioSceneManager.Instance.LoadNavMenuScene();

        //if (!VRSStudioHelper.TutorialCompletedBefore())
        //{
        //    ToTutorialSequence();
        //}
        //else
        //{
        //    ReplaySequence();
        //}
    }

    public Action<string> ToTutorialSequenceEvent;
    private void ToTutorialSequence()
    {
        ToTutorialSequenceEvent?.Invoke(tutorialIntroLine);
    }

    public void OnToTutorialVoiceBegin(object sender, SpeechSynthesisEventArgs e)
    {
        StartCoroutine(OnToTutorialVoiceBeginCoroutine());
    }

    IEnumerator OnToTutorialVoiceBeginCoroutine()
    {
        Instance.robotAssistantSpeechBubble.TextBoardShowup(true);
        yield return new WaitForSecondsRealtime(1.5f);
        Instance.robotAssistantSpeechBubble.RobotLines = tutorialIntroLine;
        Instance.robotAssistantSpeechBubble.typingInterval = 0.05f;
        yield return StartCoroutine(Instance.robotAssistantSpeechBubble.PlayTypingWordAnim());
        yield return new WaitForSecondsRealtime(1f);
        OnToTutorialVoiceEnd();
    }

    private void OnToTutorialVoiceEnd()
    {
        Instance.robotAssistantSpeechBubble.TextBoardShowup(false);
        //Instance.SetRobotPosition(new Vector3(-1.141f, 0.809f, 1.612f), VRSStudioSceneManager.Instance.InitialSceneLoadSequence);
    }

    public Action<string> ReplaySequenceEvent;
    private void ReplaySequence()
    {
        //VRSStudioSceneManager.Instance.InitialSceneLoadSequence();
        ReplaySequenceEvent?.Invoke(replayIntroLine);
    }

    public void ReplayVoiceBegin(object sender, SpeechSynthesisEventArgs e)
    {
        StartCoroutine(ReplayVoiceBeginCoroutine());
    }

    IEnumerator ReplayVoiceBeginCoroutine()
    {
        Instance.robotAssistantSpeechBubble.TextBoardShowup(true);
        yield return new WaitForSecondsRealtime(1.5f);
        Instance.robotAssistantSpeechBubble.RobotLines = replayIntroLine;
        Instance.robotAssistantSpeechBubble.typingInterval = 0.05f;
        yield return StartCoroutine(Instance.robotAssistantSpeechBubble.PlayTypingWordAnim());
        yield return new WaitForSecondsRealtime(1f);
    }

    #endregion
}
