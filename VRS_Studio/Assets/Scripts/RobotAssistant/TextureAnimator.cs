using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TextureAnimInfo
{
    public int row, col;
    public Texture animTex;
}

[System.Serializable]
public class TextureAnimClip
{
    public string clipName;
    public bool loop;
    public bool playOnce;

    public int PlayOnceSt, PlayOnceEd;
    public int LoopSt, LoopEd;

    public float PlayOnceSpeed = 0.05f, LoopSpeed = 0.05f;

    [Header("PlayTexIndex")]
    public int index = 0;

    [Header("For Blink eye animation")]
    public float loopInterval = 0f;
}

public class TextureAnimator : MonoBehaviour
{
    public string defaultClipName = "";
    public bool PlayOnAwake = false;

    [SerializeField] protected List<TextureAnimInfo> textures;
    [SerializeField] protected List<TextureAnimClip> clips;
    [SerializeField] protected TextureAnimation controller;

    protected Dictionary<string, TextureAnimClip> anims = null;

    private IEnumerator WaitPlayEndCoroutine = null;
    private string waitPlayClip = "";
    private int curAnimTexInx = -1;

    public List<TextureAnimClip> Clips
    {
        get
        {
            return clips;
        }
        set
        {
            clips = value;
        }
    }

    public bool IsPlaying
    {
        get { return controller.isPlaying; }
    }

    // Use this for initialization
    void Awake()
    {
        if (controller == null)
        {
            controller = GetComponent<TextureAnimation>();
        }
        InitAnimator();
    }

    protected virtual void InitAnimator()
    {
        anims = new Dictionary<string, TextureAnimClip>();

        foreach (var clip in clips)
        {
            anims.Add(clip.clipName, clip);
        }

        if (defaultClipName != "" && PlayOnAwake)
        {
            Play(defaultClipName);
        }
    }

    public virtual void Play(string clipName, bool cleanWaitEvent = true)
    {
        if (cleanWaitEvent && WaitPlayEndCoroutine != null)
        {
            StopCoroutine(WaitPlayEndCoroutine);
            WaitPlayEndCoroutine = null;
        }

        TextureAnimClip clip;
        if (anims.TryGetValue(clipName, out clip))
        {
            if (curAnimTexInx != clip.index)
            {
                controller.UpdateAnimTexInfo(textures[clip.index]);
                curAnimTexInx = clip.index;
            }
            controller.UpdateClipPlayInfo(clip);
            controller.Play();
        }
        else
        {
            Debug.LogError("[TextureAnimationController]: Can't find clip in dictionary, ObjectName: [" + gameObject.name + "], ClipName: [" + clipName + "]");
        }
    }

    public virtual void WaitEndAndContinuePlay(string clipName)
    {
        if (WaitPlayEndCoroutine != null)
        {
            StopCoroutine(WaitPlayEndCoroutine);
            WaitPlayEndCoroutine = null;
        }

        controller.loop = false;

        waitPlayClip = clipName;
        WaitPlayEndCoroutine = WaitEndOfAnimator();
        StartCoroutine(WaitPlayEndCoroutine);
    }

    public virtual void Stop(bool _reset)
    {
        controller.Stop(_reset);
        if (WaitPlayEndCoroutine != null)
        {
            StopCoroutine(WaitPlayEndCoroutine);
        }
    }

    public virtual void OverridePlaySpeed(float PlayOnceSpeed = 0.05f, float LoopSpeed = 0.05f)
    {
        controller.PlayOnceSpeed = PlayOnceSpeed;
        controller.LoopSpeed = LoopSpeed;
    }

    private IEnumerator WaitEndOfAnimator()
    {
        while (controller.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }

        Play(waitPlayClip);
    }
}
