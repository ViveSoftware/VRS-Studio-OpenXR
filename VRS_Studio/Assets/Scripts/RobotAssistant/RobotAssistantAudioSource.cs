using System.Collections;
using UnityEngine;
using System.Threading.Tasks;

using Microsoft.CognitiveServices.Speech;

using System;
using System.Collections.Generic;
using UnityEngine.Android;

public class RobotAssistantAudioSource : MonoBehaviour
{
    private static string LOG_TAG = "RobotAssistantAudioSource";

    // The AudioSource implied 3DSP
    [SerializeField] private AudioSource audioSource;

    private const string VoiceName_enUS = "en-US-GuyNeural";
    private const string VoiceName_zhTW = "zh-TW-YunJheNeural";
    private const int SampleRate = 24000;
    private bool audioSourceNeedStop;
    private enum SpeakStatus
    {
        Started,
        Completed,
        WaitForAudioSourceStop
    }
    SpeakStatus CurrentSpeakStatus = SpeakStatus.Completed;

    private SpeakToken CurrentSpeakToken = null;

    #region SpeakToken
    public class SpeakToken
    {
        public enum SpeakType
        {
            Text,
            AudioClip
        }
        public SpeakType m_SpeakType = SpeakType.Text;

        public delegate void SpeakStartedHandler(object sender, SpeakEventArgs e);
        public delegate void SpeakCompletedHandler(object sender, SpeakEventArgs e);

        private Action<AudioClip, string, string> m_Action = null;
        private AudioClip m_SpeakAudioClip = null;
        private string m_SpeakText = String.Empty;
        private string m_SpeakLanguage = String.Empty;
        public bool m_Interruptible = false;
        public SpeakEventArgs m_SpeakEventArgs = null;

        public SpeakToken(Action<AudioClip, string, string> Task, AudioClip speakAudioClip, string speakText, string speakLanguage, bool interruptible
            , SpeakStartedHandler speakStartedHandler, SpeakCompletedHandler speakCompletedHandler)
        {
            m_Action = Task;
            m_SpeakAudioClip = speakAudioClip;
            m_SpeakText = speakText;
            m_SpeakLanguage = speakLanguage;
            m_Interruptible = interruptible;
            m_SpeakType = (m_SpeakAudioClip == null) ? SpeakType.Text : SpeakType.AudioClip;
            if (speakStartedHandler == null)
            {
                SpeakStarted += new EventHandler<SpeakEventArgs>((object sender, SpeakEventArgs e) => { });
            }
            else
            {
                SpeakStarted += new EventHandler<SpeakEventArgs>(speakStartedHandler);
            }
            if (speakCompletedHandler == null)
            {
                SpeakCompleted += new EventHandler<SpeakEventArgs>((object sender, SpeakEventArgs e) => { });
            }
            else
            {
                SpeakCompleted += new EventHandler<SpeakEventArgs>(speakCompletedHandler);
            }
            m_SpeakEventArgs = new SpeakEventArgs(speakAudioClip, speakText, speakLanguage);
        }
        public void Run() { m_Action(m_SpeakAudioClip, m_SpeakText, m_SpeakLanguage); }

        public class SpeakEventArgs : EventArgs
        {
            public AudioClip SpeakAudioClip { get; }
            public string SpeakText { get; }
            public string SpeakLanguage { get; }

            public SpeakEventArgs(AudioClip speakAudioClip, string speakText, string speakLanguage)
            {
                SpeakAudioClip = speakAudioClip;
                SpeakText = speakText;
                SpeakLanguage = speakLanguage;
            }
        }

        private event EventHandler<SpeakEventArgs> _SpeakStarted;
        private event EventHandler<SpeakEventArgs> _SpeakCompleted;
        public event EventHandler<SpeakEventArgs> SpeakStarted
        {
            add
            {
                if (_SpeakStarted == null)
                {
                    //Do something
                }
                _SpeakStarted += value;
            }
            remove
            {
                _SpeakStarted -= value;
                if (_SpeakStarted == null)
                {
                    //Do something
                }
            }
        }
        public event EventHandler<SpeakEventArgs> SpeakCompleted
        {
            add
            {
                if (_SpeakCompleted == null)
                {
                    //Do something
                }
                _SpeakCompleted += value;

            }
            remove
            {
                _SpeakCompleted -= value;
                if (_SpeakCompleted == null)
                {
                    //Do something
                }
            }
        }
        public void InvokeSpeakStarted()
        {
            _SpeakStarted?.Invoke(this, m_SpeakEventArgs);
        }
        public void InvokeSpeakCompleted()
        {
            _SpeakCompleted?.Invoke(this, m_SpeakEventArgs);
        }
    }
    Queue<SpeakToken> m_SpeakTokenQueue = new Queue<SpeakToken>();
    private void EnqueueSpeakToken(Action<AudioClip, string, string> Task, AudioClip audioClip, string SpeakText, string Language, bool Interruptible
                    , SpeakToken.SpeakStartedHandler speakStartedHandler, SpeakToken.SpeakCompletedHandler speakCompletedHandler)
    {
        lock (m_SpeakTokenQueue)
        {
            SpeakToken Handler = new SpeakToken(Task, audioClip, SpeakText, Language, Interruptible
                , speakStartedHandler, speakCompletedHandler);
            m_SpeakTokenQueue.Enqueue(Handler);
        }
    }
    #endregion

    Queue<Action> m_TasksRunOnMainThread = new Queue<Action>();
    public void RunOnMainThread(Action task)
    {
        lock (m_TasksRunOnMainThread) { m_TasksRunOnMainThread.Enqueue(task); }
    }

    public bool IsSpeaking
    {
        get
        {
            return !(CurrentSpeakStatus == SpeakStatus.Completed) || audioSource.isPlaying;
        }
    }

    private bool isInternetAndMicrophonePermissionAvailable
    {
        get { return false; }
    }

    /// <summary>
    /// Speak AudioClip from RobotAssistant 3DSP AudioSource.
    /// </summary>
    /// <param name="audioClip"></param>
    /// <param name="interruptible">Could be interrupted by ForceStopAudioSource</param>
    /// <param name="speakStartedHandler">Callback invoked when AudioSouce start to play</param>
    /// <param name="speakCompletedHandler">Callback invoked when AudioSouce stop playing</param>
    public void Speak(AudioClip audioClip, bool interruptible = false, SpeakToken.SpeakStartedHandler speakStartedHandler = null, SpeakToken.SpeakCompletedHandler speakCompletedHandler = null)
    {
        Debug.Log(LOG_TAG + " : " + "Speak AudioClip, CurrentSpeakStatus:" + CurrentSpeakStatus + ", audioSource.isPlaying:" + audioSource.isPlaying);
        if (audioClip != null)
        {
            if (audioClip.length > 0f)
            {
                if (audioClip.samples > 0)
                {
                    EnqueueSpeakToken(SpeakInternal, audioClip, String.Empty, String.Empty, interruptible, speakStartedHandler, speakCompletedHandler);
                }
                else
                {
                    Debug.LogWarning(LOG_TAG + " : " + "No audio data found in audioClip");
                }
            }
            else
            {
                Debug.LogWarning(LOG_TAG + " : " + "audioClip.length is zero");
            }
        }
        else
        {
            Debug.LogWarning(LOG_TAG + " : " + "audioClip is null");
        }
    }

    private void SpeakInternal(AudioClip audioClip, string speakText, string language)
    {
        if (audioClip != null) //For AudioClip only.
        {
            Debug.Log(LOG_TAG + " : " + "SpeakInternal for AudioClip");
            CurrentSpeakToken.InvokeSpeakStarted();
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning(LOG_TAG + " : " + "audioClip(null) and speakText(String.Empty) must not be true at the same time");
        }
    }

    private IEnumerator SpeakTextRoutine(Task<SpeechSynthesisResult> speakTask)
    {
        var startTime = DateTime.Now;

        while (!speakTask.IsCompleted)
        {
            //Debug.Log(LOG_TAG + " : " + "StartSpeakingTextAsync not yet complete");
            yield return null;
        }

        var result = speakTask.Result;
        var audioDataStream = AudioDataStream.FromResult(result);
        while (!audioDataStream.CanReadData(4092 * 2)) // audio clip requires 4096 samples before it's ready to play. 0.292s
        {
            //Debug.Log(LOG_TAG + " : " + "SpeakRoutine CanReadData false.");
            yield return null;
        }
        var isFirstAudioChunk = true;
        bool isAlreadySetStop = false;
        Debug.Log(LOG_TAG + " : " + "Read " + audioDataStream.GetStatus() + " from SpeechSynthesisResult.");
        var audioClip = AudioClip.Create(
                "Speech",
                SampleRate * 600, // Able to speak maximum 10mins audio data
                1,
                SampleRate,
                true,
                (float[] audioChunk) =>
                {
                    var chunkSize = audioChunk.Length;
                    var audioChunkBytes = new byte[chunkSize * 2];
                    var readBytes = audioDataStream.ReadData(audioChunkBytes);
                    //Debug.Log(LOG_TAG + " : " + "chunkSize is " + chunkSize + " readBytes is " + readBytes);
                    if (isFirstAudioChunk && readBytes > 0)
                    {
                        var endTime = DateTime.Now;
                        var latency = endTime.Subtract(startTime).TotalMilliseconds;
                        Debug.Log(LOG_TAG + " : " + "Read synthesized AudioDataStream succeeded! after " + latency + "ms");
                        isFirstAudioChunk = false;
                    }
                    for (int i = 0; i < chunkSize; ++i)
                    {
                        if (i < readBytes / 2)
                        {
                            audioChunk[i] = (short)(audioChunkBytes[i * 2 + 1] << 8 | audioChunkBytes[i * 2]) / 32768.0F;
                        }
                        else
                        {
                            //Once enter here, the readBytes start always be 0 in following callback.
                            audioChunk[i] = 0.0f;
                        }
                    }
                    if (readBytes == 0 && !isAlreadySetStop)
                    {
                        Debug.Log(LOG_TAG + " : " + "Read synthesized AudioDataStream done. Stop audioSource");
                        audioSourceNeedStop = true;
                        isAlreadySetStop = true;
                    }
                });
        var endTime = DateTime.Now;
        var latency = endTime.Subtract(startTime).TotalMilliseconds;
        Debug.Log(LOG_TAG + " : " + "After " + latency + " ms." + "Read enough data to generate AudioClip and ready to play!");
        CurrentSpeakToken.InvokeSpeakStarted();
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    void Start()
    {
        if (audioSource == null)
        {
            Debug.LogError("audioSource property is null! Assign a AudioSource to it.");
        }
    }

    /// <summary>
    /// The AudioSource will be stopped if the current speak was marked as interruptible.
    /// </summary>
    public void ForceStopAudioSource()
    {
        if (audioSource.isPlaying)
        {
            RunOnMainThread(() =>
            {
                //Audio Source could be interrupt but Text type can't.
                if (CurrentSpeakToken != null && CurrentSpeakToken.m_Interruptible)
                {
                    Debug.Log(LOG_TAG + " : " + "ForceStopAudioSource, CurrentSpeakToken.m_Interruptible: " + CurrentSpeakToken.m_Interruptible + " audioSource.isPlaying:" + audioSource.isPlaying);
                    Reset();
                    CurrentSpeakToken?.InvokeSpeakCompleted();
                }
            });
        }
    }

    void Update()
    {
        //Debug.Log(LOG_TAG + " : " + "CurrentSpeakStatus: " + CurrentSpeakStatus + ", audioSource.isPlaying: " + audioSource.isPlaying + ", m_SpeakTokenQueue.Count: " + m_SpeakTokenQueue.Count);

        if (m_TasksRunOnMainThread.Count > 0)
        {
            Debug.Log(LOG_TAG + " : " + "Update  m_TasksRunOnMainThread.Dequeue() ");
            var task = m_TasksRunOnMainThread.Dequeue();
            task();
        }

        if (CurrentSpeakStatus == SpeakStatus.Completed && !audioSource.isPlaying)
        {
            if (m_SpeakTokenQueue.Count > 0)
            {
                SpeakToken Handler = m_SpeakTokenQueue.Dequeue();
                Debug.Log(LOG_TAG + " : " + "SpeakTokenQueue.Dequeue(), Handler.m_SpeakType is " + Handler.m_SpeakType);
                CurrentSpeakToken = Handler;
                Handler.Run();
                if (CurrentSpeakToken.m_SpeakType == SpeakToken.SpeakType.Text)
                {
                    CurrentSpeakStatus = SpeakStatus.Started;
                }
                else if (CurrentSpeakToken.m_SpeakType == SpeakToken.SpeakType.AudioClip)
                {
                    CurrentSpeakStatus = SpeakStatus.WaitForAudioSourceStop;
                }
            }
        }
        else if (CurrentSpeakStatus == SpeakStatus.Started)
        {
            if (audioSourceNeedStop)
            {
                Debug.Log(LOG_TAG + " : " + "audioSource.Stop()");
                audioSource.Stop();
                audioSourceNeedStop = false;
                CurrentSpeakStatus = SpeakStatus.WaitForAudioSourceStop;
            }
        }
        else if (CurrentSpeakStatus == SpeakStatus.WaitForAudioSourceStop && !audioSource.isPlaying)
        {
            Debug.Log(LOG_TAG + " : " + "audioSource is stopped.");
            CurrentSpeakToken.InvokeSpeakCompleted();
            CurrentSpeakStatus = SpeakStatus.Completed;
        }

        if (CurrentSpeakStatus == SpeakStatus.Completed && audioSource.isPlaying)
        {
            Debug.LogWarning(LOG_TAG + " : " + "Play AudioSouce somewhere without API Speak. Stop AudioSource!");
            Reset();
        }
    }

    void OnDestroy()
    {
        Reset();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) Reset();
    }
    private void OnDisable()
    {
        Reset();
    }
    private void Reset()
    {
        Debug.Log(LOG_TAG + " : " + "Reset");
        audioSource.Stop();
        audioSourceNeedStop = false;
        CurrentSpeakStatus = SpeakStatus.Completed;
    }
}