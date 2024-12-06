using HTC.UnityPlugin.Pointer3D;
using UnityEngine;

public class TouchPoserBase : MonoBehaviour
{
    [SerializeField]
    protected bool isRightHand;

    [SerializeField]
    protected Transform rootTransform;

    [SerializeField]
    protected Transform touchTransform;

    [SerializeField]
    protected TouchRaycaster touchRaycaster;
    protected CanvasRaycastMethod touchCanvasMethod;
    protected PhysicsRaycastMethod touchPhysicsMethod;

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
        touchCanvasMethod = touchRaycaster.GetComponent<CanvasRaycastMethod>();
        touchPhysicsMethod = touchRaycaster.GetComponent<PhysicsRaycastMethod>();
    }

    protected virtual void OnEnable()
    {
        if (touchRaycaster)
            touchRaycaster.enabled = true;
    }

    protected virtual void OnDisable()
    {
        if (touchRaycaster)
            touchRaycaster.enabled = false;
    }

    protected virtual bool isTouching()
    {
        if (touchRaycaster)
        {
            bool isTouching = touchRaycaster.FirstRaycastResult().gameObject && touchRaycaster.CurrentFrameHitRange < touchRaycaster.mouseButtonLeftRange;
            return isTouching;
        }
        return false;
    }

    protected virtual void Update()
    {
        MoveUsingTransform(isTouching());
        RotateUsingTransform();
    }

    protected virtual void MoveUsingTransform(bool isTouching)
    {

    }

    protected virtual void RotateUsingTransform()
    {

    }
}
