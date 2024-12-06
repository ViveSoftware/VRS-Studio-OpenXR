using System;
using System.Collections;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private GameObject transitionCam;
    [SerializeField] private GameObject transitionMask;

    [SerializeField] private Renderer TransitionRenderer;
    private Material TransitionMaterial;
    private const int TransitionOutFrames = 20;
    private const int TransitionInFrames = 20;
    private const int TransitionInLobbyFrames = 40;
    private float TransitionTimeLimit = 2f;
    private bool IsInFadeInTransition = false;
    private bool IsInFadeOutTransition = false;
    private Coroutine fadeInCoroutine = null;
    private Coroutine fadeOutCoroutine = null;

    private void setTransitionActivate(bool isActivate)
    {
        transitionCam?.gameObject.SetActive(isActivate);
        transitionMask?.gameObject.SetActive(isActivate);
    }

    void OnEnable()
    {
        IsInFadeInTransition = IsInFadeOutTransition = false;
        EnterSceneTransition();
    }

    private void CheckTransitionResult()
    {
        if (IsInFadeInTransition)
        {
            Debug.Log("[Launcher][SceneTransition] Still in transition, force to end it.");
            StopAllCoroutines();
            IsInFadeInTransition = IsInFadeOutTransition = false;
            fadeInCoroutine = fadeOutCoroutine = null;
            HideBackground();
        }
    }

    public void EnterSceneTransition()
    {
        if (fadeInCoroutine != null)
            StopCoroutine(fadeInCoroutine);
        this.CancelInvoke("CheckTransitionResult");

        fadeInCoroutine = StartCoroutine(FadeIn());
        this.Invoke("CheckTransitionResult", TransitionTimeLimit);
    }

    public void LeaveSceneTransition(Action done = null)
    {
        if (IsInFadeInTransition)
        {
            Debug.Log("[Launcher][SceneTransition] Skip fade In.");
            if (fadeInCoroutine != null)
            {
                StopCoroutine(fadeInCoroutine);
                fadeInCoroutine = null;
            }
            this.CancelInvoke("CheckTransitionResult");
        }

        if (fadeOutCoroutine != null)
            StopCoroutine(fadeOutCoroutine);

        fadeOutCoroutine = StartCoroutine(FadeOut(done));
    }

    public void ShowAsBackground(float alpha)
    {
        Debug.Log("[Launcher][SceneTransition] Show background (" + alpha + ").");
        setTransitionActivate(true);

        if (TransitionRenderer != null)
        {
            TransitionMaterial = TransitionRenderer.material;

            TransitionMaterial.color = new Color(
                TransitionMaterial.color.r,
                TransitionMaterial.color.g,
                TransitionMaterial.color.b,
                alpha);
            TransitionRenderer.material = TransitionMaterial;
        }
    }

    public void HideBackground()
    {
        Debug.Log("[Launcher][SceneTransition] Hide background.");
        if (TransitionRenderer != null)
        {
            TransitionMaterial = TransitionRenderer.material;

            TransitionMaterial.color = new Color(
                TransitionMaterial.color.r,
                TransitionMaterial.color.g,
                TransitionMaterial.color.b,
                0.0f);
            TransitionRenderer.material = TransitionMaterial;
        }
        setTransitionActivate(false);
    }

    void OnDisable()
    {
        Debug.Log("[Launcher][SceneTransition] OnDisable");
        if (TransitionMaterial != null)
        {
            Destroy(TransitionMaterial);
        }
        StopAllCoroutines();
        CancelInvoke();
    }

    /*void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EnterSceneTransition();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LeaveSceneTransition();
        }
        #endif
    }*/

    IEnumerator FadeOut(Action done = null)
    {

        IsInFadeOutTransition = true;

        setTransitionActivate(true);

        if (TransitionRenderer != null)
        {
            TransitionMaterial = TransitionRenderer.material;
            float startAlpha = TransitionMaterial.color.a;
            Debug.Log("[Launcher][SceneTransition] Start to fade out. : " + startAlpha);
            for (int i = 0; i < TransitionOutFrames; i++)
            {
                float alpha = startAlpha + ((float)i / (float)TransitionOutFrames);
                TransitionMaterial.color = new Color(
                    TransitionMaterial.color.r,
                    TransitionMaterial.color.g,
                    TransitionMaterial.color.b,
                    alpha
                );
                TransitionRenderer.material = TransitionMaterial;

                if (alpha >= 1)
                    break;
                yield return new WaitForEndOfFrame();
            }

            TransitionMaterial.color = new Color(
                    TransitionMaterial.color.r,
                    TransitionMaterial.color.g,
                    TransitionMaterial.color.b,
                    1f
                );
            TransitionRenderer.material = TransitionMaterial;
        }

        yield return null;
        Debug.Log("[Launcher][SceneTransition] End of fade out.");
        if (done != null)
            done.Invoke();

        IsInFadeOutTransition = false;
        fadeOutCoroutine = null;
    }

    IEnumerator FadeIn()
    {
        while (IsInFadeOutTransition)
        {
            yield return null;
        }

        Debug.Log("[Launcher][SceneTransition] Start to fade in.");
        IsInFadeInTransition = true;

        int TransitionFrames = TransitionInFrames;

        //if (Htc.Vive.Launcher.LauncherSceneManager.Instance.IsLobbyScene)
        //{
        //    TransitionFrames = TransitionInLobbyFrames;
        //}

        if (TransitionRenderer != null)
        {
            TransitionMaterial = TransitionRenderer.material;

            TransitionMaterial.color = new Color(
                    TransitionMaterial.color.r,
                    TransitionMaterial.color.g,
                    TransitionMaterial.color.b,
                    1f
                );
            TransitionRenderer.material = TransitionMaterial;

            for (int i = 0; i < TransitionFrames; i++)
            {
                TransitionMaterial.color = new Color(
                    TransitionMaterial.color.r,
                    TransitionMaterial.color.g,
                    TransitionMaterial.color.b,
                    1f - ((float)i / (float)TransitionFrames)
                );
                TransitionRenderer.material = TransitionMaterial;
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            Debug.Log("[Launcher][SceneTransition] TransitionRenderer is NULL.");
        }

        setTransitionActivate(false);

        yield return null;
        Debug.Log("[Launcher][SceneTransition] End to fade in.");
        IsInFadeInTransition = false;
        this.CancelInvoke("CheckTransitionResult");
        fadeInCoroutine = null;
    }

    static private SceneTransition instance;
    public static SceneTransition Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(SceneTransition)) as SceneTransition;
                if (!instance)
                {
                    Debug.LogError("There need to be one active SceneTransition script on a GameObject in your scene.");
                }
            }
            return instance;
        }
    }
}
