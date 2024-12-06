using HTC.UnityPlugin.Pointer3D;
using UnityEngine;

public class TouchReticlePoser : MonoBehaviour
{
    public TouchRaycaster raycaster;

    public bool showOnHitOnly = true;
    public Animator reticleAnimator;
    public Transform reticleForTouchRay;

    public float hitDistance;
    public GameObject hitTarget;
    public float clueDistance = 0.08f;

    private bool isReticleVisible;
    public bool IsReticleVisible { get { return isReticleVisible; } }

    public Transform reticleForDefaultRay { get { return reticleForTouchRay; } set { reticleForTouchRay = value; } }


    protected virtual void LateUpdate()
    {
        var targetReticle = reticleForTouchRay;

        var points = raycaster.BreakPoints;
        var pointCount = points.Count;
        var result = raycaster.FirstRaycastResult();

        if ((showOnHitOnly && !result.isValid) || pointCount <= 1)
        {
            if (isReticleVisible) isReticleVisible = false;
            if (reticleForTouchRay != null) { reticleForTouchRay.gameObject.SetActive(false); }
            return;
        }

        if (result.isValid)
        {
            hitTarget = result.gameObject;
            hitDistance = result.distance;

            if (hitDistance < clueDistance)
            {
                isReticleVisible = true;
                float value = 1 - (hitDistance - raycaster.mouseButtonLeftRange) / clueDistance;
                reticleAnimator.Play("VisualClueAnimation", 0, value);
            }
            else
            {
                isReticleVisible = false;
            }

            targetReticle = reticleForTouchRay;

            if (targetReticle != null)
            {
                targetReticle.position = result.worldPosition;
                targetReticle.rotation = Quaternion.LookRotation(result.worldNormal, raycaster.transform.forward);
                targetReticle.eulerAngles = new Vector3(targetReticle.eulerAngles.x, targetReticle.eulerAngles.y, 0);
            }
        }
        else
        {
            if (targetReticle != null)
            {
                targetReticle.position = points[pointCount - 1];
                targetReticle.rotation = Quaternion.LookRotation(points[pointCount - 1] - points[pointCount - 2], raycaster.transform.forward);
            }

            hitTarget = null;
            hitDistance = 0f;
            isReticleVisible = false;
        }

        reticleForTouchRay.gameObject.SetActive(isReticleVisible);
    }

    protected virtual void OnDisable()
    {
        if (isReticleVisible)
        {
            isReticleVisible = false;
            if (reticleForTouchRay != null) { reticleForTouchRay.gameObject.SetActive(false); }
        }
    }
}