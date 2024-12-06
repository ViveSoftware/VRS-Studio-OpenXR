using HTC.UnityPlugin.Pointer3D;
using HTC.UnityPlugin.PoseTracker;
using UnityEngine;

public class TouchInteractor : MonoBehaviour
{
    public Transform attachTransform;
    public TouchRaycaster touchRaycaster;

    public float hoverMinRange = 0.02f;
    public float pressRange = 0.015f;
    public float pressDepth = 0.25f;
    public float pressPositionThreshold = 0.015f;
    public float defaultPositionThreshold = 0.005f;


    private bool isHover;
    private bool isPress;
    private Vector3 pressPos;
    private Vector3 projectPos;
    private Vector3 pressNormalized;
    private float currentPressDepth;

    private Vector3 ProjectVectorOnPlane(Vector3 target, Vector3 normal)
    {
        return target - Vector3.Dot(target, normal) * normal;
    }

    private void Update()
    {
        isHover = touchRaycaster.FirstRaycastResult().gameObject;
        isPress = touchRaycaster.FirstRaycastResult().gameObject &&
                touchRaycaster.CurrentFrameHitRange < touchRaycaster.mouseButtonLeftRange;
        Vector3 targetPos = Vector3.zero;
        var poseStablizer = attachTransform.GetComponent<PoseStablizer>();

        if (isHover)
        {
            touchRaycaster.NearDistance = 0;
            touchRaycaster.mouseButtonLeftRange = pressRange;
        }
        else
        {
            touchRaycaster.NearDistance = hoverMinRange;
            touchRaycaster.mouseButtonLeftRange = -pressRange;
        }


        if (isPress)
        {
            Vector3 targetForward = touchRaycaster.FirstRaycastResult().gameObject.transform.forward;

            pressPos = touchRaycaster.HoverEventData.pressPosition3D;
            projectPos = pressPos + ProjectVectorOnPlane(attachTransform.position - pressPos, targetForward);
            pressNormalized = attachTransform.position - projectPos;
            currentPressDepth = pressNormalized.magnitude;

            if (currentPressDepth < pressDepth &&
                targetForward * currentPressDepth == pressNormalized)
            {
                targetPos = projectPos;
            }
            else
            {
                isPress = false;
                targetPos = attachTransform.position;
            }
            poseStablizer.positionThreshold = Mathf.Lerp(poseStablizer.positionThreshold, pressPositionThreshold, 0.25f);
        }
        else
        {
            targetPos = attachTransform.position;
            poseStablizer.positionThreshold = Mathf.Lerp(poseStablizer.positionThreshold, defaultPositionThreshold, 0.25f);
        }

        touchRaycaster.transform.position = targetPos;
        touchRaycaster.transform.rotation = attachTransform.rotation;
    }
}
