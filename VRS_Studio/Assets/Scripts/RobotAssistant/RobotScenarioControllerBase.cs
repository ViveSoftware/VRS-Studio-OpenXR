using System.Collections;
using UnityEngine;

public class RobotScenarioControllerBase : MonoBehaviour
{
    public GameObject InitialPositionAnchorObject;

    public void InitializeRobot()
    {
        RobotAssistantManager.Instance.robotAssistantSpeechBubble.TextBoardShowup(false);
        StartCoroutine(Initialization());
    }

    private IEnumerator Initialization()
    {
        yield return StartCoroutine(RobotAssistantManager.Instance.RobotStartTeleport(InitialPositionAnchorObject.transform.position));
    }
}
