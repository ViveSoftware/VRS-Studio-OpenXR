using System.Collections;
using UnityEngine;

public class TeleportationTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TeleportTestCoroutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator TeleportTestCoroutine()
    {
        while (true)
        {
            //while (!RobotAssistantManager.Instance.robotAssistantAnimator.GetCurrentAnimatorStateInfo(1).IsName("FloatAirLoop"))
            //{
            //    yield return null;
            //}

            yield return RobotAssistantManager.Instance.RobotStartTeleport(RobotAssistantManager.Instance.transform.position + new Vector3(2, 0, 0));
            //yield return new WaitForSecondsRealtime(RobotAssistantManager.Instance.SetRobotPosition(RobotAssistantManager.Instance.transform.position + new Vector3(2, 0, 0)));
            //yield return new WaitForSecondsRealtime(RobotAssistantManager.Instance.SetRobotPosition(RobotAssistantManager.Instance.transform.position + new Vector3(0, 0, 2)));
            yield return RobotAssistantManager.Instance.RobotStartTeleport(RobotAssistantManager.Instance.transform.position + new Vector3(-2, 0, 0));
            //yield return new WaitForSecondsRealtime(RobotAssistantManager.Instance.SetRobotPosition(RobotAssistantManager.Instance.transform.position + new Vector3(-2, 0, 0)));
            //yield return new WaitForSecondsRealtime(RobotAssistantManager.Instance.SetRobotPosition(RobotAssistantManager.Instance.transform.position + new Vector3(0, 0, -2)));
        }
    }
}
