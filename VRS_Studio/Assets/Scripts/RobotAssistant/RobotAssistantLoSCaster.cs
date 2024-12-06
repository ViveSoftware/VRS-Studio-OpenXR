//#define DEBUG_ROBOT_ASSISTANT_LOS_CASTER
using UnityEngine;

public class RobotAssistantLoSCaster : MonoBehaviour
{
    public delegate void RobotAssistantLoS_Enter();
    public static event RobotAssistantLoS_Enter RobotAssistantLoS_EnterCallback;

    public delegate void RobotAssistantLoS_Exit();
    public static event RobotAssistantLoS_Exit RobotAssistantLoS_ExitCallback;

    public GameObject DebugObj;

    private int robotLayerMask = 1 << 9;
    private static bool m_isAlreadyInLoS = false;
    public static bool isAlreadyInLoS
    {
        get { return m_isAlreadyInLoS; }
        private set { m_isAlreadyInLoS = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
#if DEBUG_ROBOT_ASSISTANT_LOS_CASTER
        DebugObj.SetActive(true);
#else
        DebugObj.SetActive(false);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, 10f, robotLayerMask))
        {
            if (!isAlreadyInLoS) //New hit
            {
                isAlreadyInLoS = true;
                RobotAssistantLoS_EnterCallback?.Invoke();
            }
            else //Persistent hit
            {
#if DEBUG_ROBOT_ASSISTANT_LOS_CASTER
                DebugObj.SetActive(true);
                DebugObj.transform.position = hit.point;
#endif
            }
        }
        else
        {
            if (isAlreadyInLoS)
            {
#if DEBUG_ROBOT_ASSISTANT_LOS_CASTER
                DebugObj.SetActive(false);
#endif
                isAlreadyInLoS = false;
                RobotAssistantLoS_ExitCallback?.Invoke();
            }
        }
    }
}
