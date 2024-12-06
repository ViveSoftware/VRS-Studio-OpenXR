public class RobotScenarioControllerSimple : RobotScenarioControllerBase
{
    // Start is called before the first frame update
    void Start()
    {
        InitialPositionAnchorObject = this.gameObject;
        InitializeRobot();
    }
}
