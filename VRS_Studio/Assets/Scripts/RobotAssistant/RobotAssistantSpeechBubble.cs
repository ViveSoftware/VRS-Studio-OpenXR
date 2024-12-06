using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RobotAssistantSpeechBubble : MonoBehaviour
{
    public float typingInterval = 0.01f; //in seconds
    public Text textMesh = null;
    public RectTransform speechBubbleCanvasTransform = null;
    public RectTransform speechBubbleBackgroundTransform = null;
    public RectTransform speechBubbleTextTransform = null;

    public float speechBubbleScale = 1f;

    private bool isActive = false;
    private string robotLines = "";

    public bool IsTyping { get { return (typingCoroutine != null); } }
    public bool IsActive { get { return isActive; } }

    public string RobotLines
    {
        get { return robotLines; }
        set
        {
            robotLines = value;
        }
    }

    public void UpdateSpeechBubbleCanvasParameter(float height, float width, float borderOffset, HorizontalWrapMode wrapMode)
    {
        textMesh.horizontalOverflow = wrapMode;


        speechBubbleCanvasTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        speechBubbleCanvasTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

        //Adjust Background quad scale
        speechBubbleBackgroundTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        speechBubbleBackgroundTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

        //Adjust text rect transform 
        float textFieldActualHeight = (height - borderOffset) / speechBubbleTextTransform.transform.localScale.y;
        float textFieldActualWidth = (width - borderOffset) / speechBubbleTextTransform.transform.localScale.x;

        speechBubbleTextTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textFieldActualHeight);
        speechBubbleTextTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textFieldActualWidth);
    }

    public void ClearSpeechBubble()
    {
        RobotLines = "";
        textMesh.text = RobotLines;
    }

    public void UpdateSpeechBubblePosOffset(Vector3 posOffset)
    {
        transform.position = posOffset;
    }

    public void TextBoardShowup(bool act)
    {
        if (isActive != act)
        {
            isActive = act;
            Vector3 scale = isActive ? Vector3.one * speechBubbleScale : Vector3.zero;

            transform.DOScale(scale, 0.1f);
        }
    }

    public IEnumerator PlayTypingWordAnim()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = TypeAnimCoroutine();
        yield return StartCoroutine(typingCoroutine);
    }

    private IEnumerator typingCoroutine = null;

    private IEnumerator TypeAnimCoroutine()
    {
        int inx = 0;
        int length = robotLines.Length;

        textMesh.text = "";

        float speed = typingInterval;
        if (speed != 0)
        {
            while (inx < length)
            {
                yield return new WaitForSeconds(speed);

                string word = robotLines.Substring(inx, 1);
                textMesh.text += word;

                inx++;
            }
        }
        else
        {
            yield return new WaitForSeconds(0.2f);
            textMesh.text = robotLines;
        }

        typingCoroutine = null;
    }
}
