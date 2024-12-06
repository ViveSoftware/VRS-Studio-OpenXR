using HTC.UnityPlugin.Pointer3D;
using HTC.UnityPlugin.PoseTracker;
using HTC.UnityPlugin.Vive;
using UnityEngine;

public class TouchPoser_Controller : TouchPoserBase
{
    [Space]
    [Header("Visual Controller Component")]
    [SerializeField]
    private Transform visualControllerRootTransform;
    [SerializeField]
    private GameObject touchPointerPrefabs;
    [SerializeField]
    private GameObject attachPoseTarget;

    protected override void Start()
    {
        GameObject touchPointer = Instantiate(touchPointerPrefabs);
        touchPointer.transform.SetParent(Camera.main.transform.parent);
        touchPointer.transform.localPosition = Vector3.zero;
        touchPointer.transform.localRotation = Quaternion.identity;
        touchPointer.GetComponentInChildren<PoseTracker>().target = attachPoseTarget.transform;
        touchRaycaster = touchPointer.GetComponentInChildren<TouchRaycaster>();

        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (!isTouching())
        {
            bool isVaild;

#if UNITY_EDITOR
            if (isRightHand) isVaild = ViveInput.GetTriggerValue(HandRole.RightHand) < 0.5f;
            else isVaild = ViveInput.GetTriggerValue(HandRole.LeftHand) < 0.5f;
#else
            if (isRightHand) isVaild = ViveInput.GetTriggerValue(HandRole.RightHand) < 0.5f;
            else isVaild = ViveInput.GetTriggerValue(HandRole.LeftHand) < 0.5f;
#endif

            if (touchRaycaster.FirstRaycastResult().gameObject == null)
                touchRaycaster.enabled = isVaild;
            else
                touchRaycaster.enabled = true;
        }
    }

    protected override void MoveUsingTransform(bool isTouching)
    {
        if (!isTouching)
        {
            touchRaycaster.transform.parent.position = touchTransform.position;
            visualControllerRootTransform.localPosition = Vector3.Lerp(visualControllerRootTransform.localPosition, Vector3.zero, 0.5f);
        }
        else
        {
            Vector3 positionDelta = rootTransform.position - touchTransform.position;
            Vector3 targetPosePosition = touchRaycaster.BreakPoints[1] + positionDelta;
            visualControllerRootTransform.position = Vector3.Lerp(visualControllerRootTransform.position, targetPosePosition, 0.5f);
        }
    }

    protected override void RotateUsingTransform()
    {
        transform.localEulerAngles = Vector3.zero;
    }
}
