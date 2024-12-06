using HTC.UnityPlugin.CommonEventVariable;
using HTC.UnityPlugin.Vive;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerHandModelHandler : MonoBehaviour
{
    private Animator animator;

    public HandRole m_viveRole = HandRole.RightHand;

    [SerializeField]
    private InputActionProperty triggerTouchAction = new InputActionProperty();
    private bool triggerTouchActionEnabled = false;
    private bool isTriggerTouch = false;

    private CommonVariableHandler<bool> isLeftHandHovering = CommonVariable.Get<bool>("LeftHand_isHovering");
    private CommonVariableHandler<bool> isRightHandHovering = CommonVariable.Get<bool>("RightHand_isHovering");

    private CommonVariableHandler<bool> isLeftHandTouching = CommonVariable.Get<bool>("LeftHand_isTouching");
    private CommonVariableHandler<bool> isRightHandTouching = CommonVariable.Get<bool>("RightHand_isTouching");

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (triggerTouchAction != null && triggerTouchAction.action != null)
        {
            triggerTouchAction.action.Enable();
            triggerTouchActionEnabled = true;
        }

        ViveInput.AddPressDown(m_viveRole, ControllerButton.AKey, OnAKeyDownEvent);
        ViveInput.AddPressDown(m_viveRole, ControllerButton.BKey, OnBKeyDownEvent);
        ViveInput.AddPress(m_viveRole, ControllerButton.AKeyTouch, OnAKeyDownEvent);
        ViveInput.AddPress(m_viveRole, ControllerButton.BKeyTouch, OnBKeyDownEvent);
        ViveInput.AddPressUp(m_viveRole, ControllerButton.AKey, OnAKeyUpEvent);
        ViveInput.AddPressUp(m_viveRole, ControllerButton.BKey, OnBKeyUpEvent);
        ViveInput.AddPressUp(m_viveRole, ControllerButton.AKeyTouch, OnAKeyUpEvent);
        ViveInput.AddPressUp(m_viveRole, ControllerButton.BKeyTouch, OnBKeyUpEvent);

        isLeftHandTouching.OnChange += TouchVibration;
        isRightHandTouching.OnChange += TouchVibration;
    }

    private void OnDisable()
    {
        ViveInput.RemovePressDown(m_viveRole, ControllerButton.AKey, OnAKeyDownEvent);
        ViveInput.RemovePressDown(m_viveRole, ControllerButton.BKey, OnBKeyDownEvent);
        ViveInput.RemovePress(m_viveRole, ControllerButton.AKeyTouch, OnAKeyDownEvent);
        ViveInput.RemovePress(m_viveRole, ControllerButton.BKeyTouch, OnBKeyDownEvent);
        ViveInput.RemovePressUp(m_viveRole, ControllerButton.AKey, OnAKeyUpEvent);
        ViveInput.RemovePressUp(m_viveRole, ControllerButton.BKey, OnBKeyUpEvent);
        ViveInput.RemovePressUp(m_viveRole, ControllerButton.AKeyTouch, OnAKeyUpEvent);
        ViveInput.RemovePressUp(m_viveRole, ControllerButton.BKeyTouch, OnBKeyUpEvent);

        isLeftHandTouching.OnChange -= TouchVibration;
        isRightHandTouching.OnChange -= TouchVibration;
    }

    float tempTriggerValue;
    float tempStickValue;
    private void Update()
    {
        if (triggerTouchActionEnabled)
        {
            isTriggerTouch = triggerTouchAction.action.ReadValue<float>() > 0;
        }

        tempTriggerValue = animator.GetFloat("TriggerValue");

        if (ViveInput.GetTriggerValue(m_viveRole) > 0)
            tempTriggerValue = Mathf.Lerp(tempTriggerValue, ViveInput.GetTriggerValue(m_viveRole), 0.5f);
        else if (isTriggerTouch)
            tempTriggerValue = Mathf.Lerp(tempTriggerValue, 0f, 0.5f);
        else
            tempTriggerValue = Mathf.Lerp(tempTriggerValue, -1f, 0.5f);

        animator.SetFloat("TriggerValue", tempTriggerValue);

        animator.SetFloat("GripValue", ViveInput.GetAxis(m_viveRole, ControllerAxis.CapSenseGrip));
        animator.SetFloat("XAxis", ViveInput.GetAxis(m_viveRole, ControllerAxis.JoystickX));
        animator.SetFloat("YAxis", ViveInput.GetAxis(m_viveRole, ControllerAxis.JoystickY));

        tempStickValue = animator.GetFloat("StickValue");
        if (ViveInput.GetPressDown(m_viveRole, ControllerButton.Joystick))
            tempStickValue = Mathf.Lerp(tempStickValue, 1f, 0.5f);
        else if (ViveInput.GetPress(m_viveRole, ControllerButton.JoystickTouch))
            tempStickValue = Mathf.Lerp(tempStickValue, 0.5f, 0.5f);
        else
            tempStickValue = Mathf.Lerp(tempStickValue, 0f, 0.35f);

        animator.SetFloat("StickValue", tempStickValue);
    }

    private void OnAKeyDownEvent()
    {
        animator.SetTrigger("AKeyDown");
        animator.SetTrigger("XKeyDown");
    }
    private void OnAKeyUpEvent()
    {
        animator.SetTrigger("AKeyUp");
        animator.SetTrigger("XKeyUp");
    }
    private void OnBKeyDownEvent()
    {
        animator.SetTrigger("BKeyDown");
        animator.SetTrigger("YKeyDown");
    }
    private void OnBKeyUpEvent()
    {
        animator.SetTrigger("BKeyUp");
        animator.SetTrigger("YKeyUp");
    }

    private void TouchVibration()
    {
        var isTouching = (m_viveRole == HandRole.RightHand) && isRightHandTouching.CurrentValue
            || (m_viveRole == HandRole.LeftHand) && isLeftHandTouching.CurrentValue;

        if (isTouching) ViveInput.TriggerHapticVibration(m_viveRole, 0.05f);
    }

    public void SetBoolTrue(string name) => animator.SetBool(name, true);
    public void SetBoolFalse(string name) => animator.SetBool(name, false);
}