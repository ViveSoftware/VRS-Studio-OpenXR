using UnityEngine;

public class TouchInteractableVisual : MonoBehaviour
{
    public TouchInteractable touchInteractable;

    [Tooltip("Item auto scale feeback in hover range")]
    public bool _isScale;

    private void OnEnable()
    {
        if (touchInteractable != null)
        {
            OnSubscribeEvent();
        }
    }

    private void OnDisable()
    {
        if (touchInteractable != null)
        {
            touchInteractable.OnPressEvent.RemoveListener(OnPress);
            touchInteractable.OnPointerExitEvent.RemoveListener(OnPointerExit);
            touchInteractable.OnPointerDownEvent.AddListener(OnPointerDown);
            touchInteractable.OnPointerUpEvent.AddListener(OnPointerUp);
        }
    }

    public void OnSubscribeEvent()
    {
        touchInteractable.OnPressEvent.AddListener(OnPress);
        touchInteractable.OnPointerExitEvent.AddListener(OnPointerExit);
        touchInteractable.OnPointerDownEvent.AddListener(OnPointerDown);
        touchInteractable.OnPointerUpEvent.AddListener(OnPointerUp);
    }

    public virtual void OnPress(float value)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, value);
        if (_isScale)
        {
            if (value < touchInteractable.maxFloatDistance)
            {
                transform.localScale = Vector3.one * (1 + 0.2f * Mathf.Abs(value / touchInteractable.maxFloatDistance));
            }
            else
            {
                transform.localScale = Vector3.one;
            }
        }
    }
    public virtual void OnPointerExit()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -touchInteractable.defaultFloatDistance);
        if (_isScale)
            transform.localScale = Vector3.one;
    }
    public virtual void OnPointerDown()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -touchInteractable.defaultFloatDistance);
        if (_isScale)
            transform.localScale = Vector3.one;
    }
    public virtual void OnPointerUp()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -touchInteractable.defaultFloatDistance);
        if (_isScale)
            transform.localScale = Vector3.one;
    }
}