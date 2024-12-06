using System.Collections;
using UnityEngine;

public class TextureAnimation : MonoBehaviour
{
    public string ShaderProperty = "_MainTex";
    public bool needTiling = true;
    public int row, col;
    public int PlayOnceSt, PlayOnceEd;
    public int LoopSt, LoopEd;
    public bool loop = true, playOnce = true, playOnStart = false;

    public float PlayOnceSpeed = 0.05f, LoopSpeed = 0.05f;

    // The delay between two loop.
    public float LoopInterval = 0f;

    private float ScaleX = 1, ScaleY = 1;
    public Renderer rend;

    private IEnumerator lastRoutine = null;

    public bool isPlaying
    {
        get { return _isPlaying; }
    }
    private bool _isPlaying = false;

    [Header("Debug")]
    [SerializeField] private bool debug = false;

    //[Range(0, 24)] public int testInx = 0;
    //private bool test;

    private void Awake()
    {
        rend = GetComponent<Renderer>();

        col = (col == 0) ? 1 : col;
        row = (row == 0) ? 1 : row;
        SetTiling();
    }

    private void OnEnable()
    {
        if (playOnStart)
        {
            Play();
        }
    }

    public void PlayFrame(int inx)
    {
        float offX = ((float)inx % (float)col) * ScaleX;
        float offY = ((row - 1) - (inx / col)) * ScaleY;
        rend.material.SetTextureOffset(ShaderProperty, new Vector2(offX, offY));

        if (debug)
        {
            Debug.Log("[PlayFrame:" + inx + "][Time:" + Time.realtimeSinceStartup + "]");
        }
    }

    private void SetTiling()
    {
        if (needTiling)
        {
            rend.material.SetTextureScale(ShaderProperty, new Vector2(1 / (float)col, 1 / (float)row));
        }
        ScaleX = 1 / (float)col; ScaleY = 1 / (float)row;
    }
    float timer = 0; float cycle = 0; int sign = 1;
    private IEnumerator PlayAnimCoroutine()
    {
        bool firstPlay = playOnce;

        if (firstPlay)
        {
            PlayFrame(PlayOnceSt);
        }
        else
        {
            PlayFrame(LoopSt);
        }

        yield return null;

        timer = 0;
        sign = (PlayOnceEd > PlayOnceSt) ? 1 : -1;
        cycle = Mathf.Abs(PlayOnceEd - PlayOnceSt) * PlayOnceSpeed;

        while (firstPlay && PlayOnceSt != PlayOnceEd)
        {
            if (timer > cycle)
            {
                firstPlay = false;
            }
            PlayFrame(PlayOnceSt + Mathf.FloorToInt(timer / PlayOnceSpeed) * sign);
            timer += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        timer = 0;
        sign = (LoopEd > LoopSt) ? 1 : -1;
        cycle = (Mathf.Abs(LoopEd - LoopSt) + 1) * LoopSpeed;
        while (loop)
        {
            while (timer < cycle)
            {
                int inx = LoopSt + Mathf.FloorToInt(timer / LoopSpeed) * sign;

                PlayFrame(inx);
                timer += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            timer = 0;
            yield return new WaitForSeconds(LoopInterval);

        }
        Stop(false);
    }

    public void Play(bool clearPlayingAnim = true)
    {
        if (lastRoutine != null && clearPlayingAnim)
        {
            StopCoroutine(lastRoutine);
            lastRoutine = null;
        }
        if (lastRoutine == null)
        {
            _isPlaying = true;
            lastRoutine = PlayAnimCoroutine();
            StartCoroutine(lastRoutine);
        }
    }

    public void Play(int st, int ed, float speed, bool needLoop)
    {
        if (needLoop)
        {
            loop = true;
            playOnce = false;
            LoopSt = st;
            LoopEd = ed;
            LoopSpeed = speed;
        }
        else
        {
            loop = false;
            playOnce = true;
            PlayOnceSt = st;
            PlayOnceEd = ed;
            PlayOnceSpeed = speed;
        }

        Play();
    }

    public void Stop(bool _reset)
    {
        if (_reset)
        {
            TextureReset();
        }
        if (lastRoutine != null)
        {
            StopCoroutine(lastRoutine);
            lastRoutine = null;
        }
        _isPlaying = false;
    }

    public void TextureReset()
    {
        PlayFrame(0);
    }

    public void UpdateClipPlayInfo(TextureAnimClip clip)
    {
        loop = clip.loop;
        playOnce = clip.playOnce;

        PlayOnceSt = clip.PlayOnceSt;
        PlayOnceEd = clip.PlayOnceEd;
        LoopSt = clip.LoopSt;
        LoopEd = clip.LoopEd;

        LoopInterval = clip.loopInterval;

        PlayOnceSpeed = clip.PlayOnceSpeed; LoopSpeed = clip.LoopSpeed;
    }

    public void UpdateAnimTexInfo(TextureAnimInfo info)
    {
        rend.material.SetTexture(ShaderProperty, info.animTex);
        col = info.col;
        row = info.row;
    }

    public void CallByEditorInit()
    {
        rend = GetComponent<Renderer>();
        col = (col == 0) ? 1 : col;
        row = (row == 0) ? 1 : row;
        SetTiling();
    }
}
