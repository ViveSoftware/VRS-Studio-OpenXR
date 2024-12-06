using HTC.UnityPlugin.Pointer3D;
using UnityEngine;

public class TouchPoser : TouchPoserBase
{
    [Space]
    [Header("Visual Hand Component")]
    [SerializeField]
    private Transform visualHandRootTransform;
    [SerializeField]
    private GameObject touchPointerPrefabs;

    protected override void Start()
    {
        GameObject touchPointer = Instantiate(touchPointerPrefabs);
        touchPointer.transform.SetParent(Camera.main.transform.parent);
        touchPointer.transform.localPosition = Vector3.zero;
        touchPointer.transform.localRotation = Quaternion.identity;
        touchRaycaster = touchPointer.GetComponentInChildren<TouchRaycaster>();

        base.Start();
    }
    protected override void Update()
    {
        base.Update();

        if (!isTouching())
        {
            bool isVaild = true;

            /*#if UNITY_EDITOR
                        isVaild = true;
            #else
                        var origin = default(Vector3);
                        var direction = default(Vector3);

                        if (isRightHand) isVaild = WaveHandTrackingSubmodule.TryGetRightPinchRay(out origin, out direction);
                        else isVaild = WaveHandTrackingSubmodule.TryGetLeftPinchRay(out origin, out direction);
            #endif*/
            if (touchRaycaster)
            {
                touchRaycaster.enabled = isVaild;
            }
        }
    }

    protected override void MoveUsingTransform(bool isTouching)
    {
        if (!isTouching)
        {
            if (visualHandRootTransform)
            {
                visualHandRootTransform.localPosition = Vector3.Lerp(visualHandRootTransform.localPosition, Vector3.zero, 0.95f);
            }
        }
        else
        {
            if (rootTransform && touchTransform && touchRaycaster && visualHandRootTransform)
            {
                Vector3 positionDelta = rootTransform.position - touchTransform.position;
                Vector3 targetPosePosition = touchRaycaster.BreakPoints[1] + positionDelta;
                visualHandRootTransform.position = Vector3.Lerp(visualHandRootTransform.position, targetPosePosition, 0.5f);
            }
        }
    }
}