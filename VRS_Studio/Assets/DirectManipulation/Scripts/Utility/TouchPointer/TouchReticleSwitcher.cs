using UnityEngine;


public class TouchReticleSwitcher : MonoBehaviour
{
    [SerializeField] private bool lefthand;

    public RectTransform normalCursor;
    public RectTransform scrollCursor;

    private void Awake()
    {
        if (lefthand)
            scrollCursor.anchoredPosition = new Vector2(-0.01f, 0f);
        else
            scrollCursor.anchoredPosition = new Vector2(0.01f, 0f);
    }

    private void Update()
    {
        OnCursorSwitchEnvent();
    }

    private void OnCursorSwitchEnvent()
    {
        bool scrollVaild = false;
        scrollCursor.gameObject.SetActive(scrollVaild);
        normalCursor.gameObject.SetActive(!scrollCursor.gameObject.activeSelf);
    }
}