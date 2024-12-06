using HTC.UnityPlugin.Vive;
using UnityEngine;

public class SnapTurn : MonoBehaviour
{
    public Transform cameraObj;

    private bool isSnapTurnRight = false;
    private bool isSnapTurnLeft = false;

    void Update()
    {
        var joystickX = ViveInput.GetAxisEx(ControllerRole.LeftHand, ControllerAxis.JoystickX);

        if (!isSnapTurnRight && (joystickX == 1f || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            isSnapTurnRight = true;
            transform.RotateAround(cameraObj.position, Vector3.up, 45);
        }
        else if (!isSnapTurnLeft && (joystickX == -1f || Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            isSnapTurnLeft = true;
            transform.RotateAround(cameraObj.position, Vector3.up, -45);
        }
        else if (joystickX < 1f && joystickX > -1f)
        {
            isSnapTurnRight = false;
            isSnapTurnLeft = false;
        }
    }
}
