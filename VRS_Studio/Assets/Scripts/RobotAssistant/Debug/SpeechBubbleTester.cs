using UnityEngine;

public class SpeechBubbleTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RobotAssistantManager robotAssistantManager = RobotAssistantManager.Instance;
        RobotAssistantSpeechBubble robotAssistantSpeechBubble = robotAssistantManager.robotAssistantSpeechBubble;

        robotAssistantSpeechBubble.TextBoardShowup(true);
        robotAssistantSpeechBubble.RobotLines = "Hey you, you’re finally awake. You were trying to cross the border right? Walked right into that Imperial ambush same as us and that thief over there.";

        //robotAssistantSpeechBubble.UpdateSpeechBubbleCanvasParameter(0.8f, 1f, 0.1f, HorizontalWrapMode.Wrap);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
