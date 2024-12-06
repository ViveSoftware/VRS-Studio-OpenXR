using HTC.UnityPlugin.Pointer3D;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TouchInteractable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private TouchRaycaster currentTouchRaycaster;
    private Vector3 basePos;

    [SerializeField]
    [Tooltip("The max hover range for checking state")]
    private float maxHoverDistance = 0.05f;

    [Tooltip("Prevent interactive UI confilct with base by 3d pointer")]
    public float defaultFloatDistance = 1f;

    [Tooltip("If touch tips in hover range, that max float local value in UGUI.")]
    public float maxFloatDistance = 20f;

    [Serializable]
    public class FloatEvent : UnityEvent<float> { };
    public FloatEvent OnPressEvent = new FloatEvent();
    public UnityEvent OnPointerExitEvent = new UnityEvent();
    public UnityEvent OnPointerDownEvent = new UnityEvent();
    public UnityEvent OnPointerUpEvent = new UnityEvent();

#if UNITY_EDITOR
    private bool isDebugTouch;
#endif

    private void Start()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -defaultFloatDistance);
    }

    private void Update()
    {
        if (currentTouchRaycaster != null)
        {
            float touchDistance =
                Vector3.Dot(currentTouchRaycaster.transform.position - basePos, -1f * transform.forward) - currentTouchRaycaster.mouseButtonLeftRange;

            if (currentTouchRaycaster != null)
            {
                if (touchDistance < defaultFloatDistance / 1000f || touchDistance > maxHoverDistance)
                {
                    touchDistance = 0;
                }
            }
            else
                touchDistance = 0;

            float closestDistance = Math.Min(touchDistance * 1000f, maxFloatDistance);
            if (closestDistance < defaultFloatDistance)
            {
                closestDistance = defaultFloatDistance;
            }
            OnPressEvent.Invoke(-closestDistance);
            return;
        }

#if UNITY_EDITOR
        if (isDebugTouch)
        {
            OnPressEvent.Invoke(-maxFloatDistance);
        }
#endif      
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Pointer3DRaycaster currentPointer;

#if UNITY_EDITOR
        if (eventData.TryGetRaycaster3D(out currentPointer))
        {
            if (!currentPointer.GetComponent<TouchRaycaster>()) return;
        }
        isDebugTouch = true;
#endif

        if (!eventData.TryGetRaycaster3D(out currentPointer)) return;
        if (!currentPointer.GetComponent<TouchRaycaster>()) return;

        currentTouchRaycaster = currentPointer.GetComponent<TouchRaycaster>();

        basePos = transform.position;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Pointer3DRaycaster currentPointer;

#if UNITY_EDITOR
        if (eventData.TryGetRaycaster3D(out currentPointer))
        {
            if (!currentPointer.GetComponent<TouchRaycaster>()) return;
        }
        isDebugTouch = false;
        OnPointerExitEvent.Invoke();
#endif

        if (!eventData.TryGetRaycaster3D(out currentPointer)) return;
        if (currentPointer != currentTouchRaycaster) return;

        OnPointerExitEvent.Invoke();
        currentTouchRaycaster = null;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
#if UNITY_EDITOR
        OnPointerDownEvent.Invoke();
#endif

    }
    public void OnPointerUp(PointerEventData eventData)
    {
#if UNITY_EDITOR
        OnPointerUpEvent.Invoke();
#endif
    }
}
