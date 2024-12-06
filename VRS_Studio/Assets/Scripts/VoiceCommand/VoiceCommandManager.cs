using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Intent;
using Microsoft.CognitiveServices.Speech.Translation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#if PLATFORM_ANDROID
using UnityEngine.Android; //Permission
#endif

namespace VRSStudio.VoiceCommand
{
    [DisallowMultipleComponent]
    public sealed class VoiceCommandManager : MonoBehaviour
    {
        private static string LOG_TAG = "VoiceCommandManager";

#if !VRSSTUDIO_INTERNAL
        //CognitiveServices identity
        private const string m_SUBSCRIPTION_KEY = "";
        private const string m_REGION = "";

        public bool IsCognitiveServicesInfoValid()
        {
            return (!String.IsNullOrEmpty(m_SUBSCRIPTION_KEY) && !String.IsNullOrEmpty(m_REGION));
        }
#endif

        private static string SUBSCRIPTION_KEY
        {
            get
            {
#if VRSSTUDIO_INTERNAL
				return VRSStudio.Internal.VoiceComand.VoiceCommandResources.SUBSCRIPTION_KEY;
#else
                return m_SUBSCRIPTION_KEY;
#endif
            }
        }
        private static string REGION
        {
            get
            {
#if VRSSTUDIO_INTERNAL
				return VRSStudio.Internal.VoiceComand.VoiceCommandResources.REGION;
#else
                return m_REGION;
#endif
            }
        }

#if !VRSSTUDIO_INTERNAL
        // LUIS identity
        private const string m_LUIS_SUBSCRIPTION_KEY = "";
        private const string m_LUIS_REGION = "";
        private const string m_LUIS_APP_ID = "";

        public bool IsLUISInfoValid()
        {
            return (!String.IsNullOrEmpty(m_LUIS_SUBSCRIPTION_KEY) && !String.IsNullOrEmpty(m_LUIS_REGION) && !String.IsNullOrEmpty(m_LUIS_APP_ID));
        }
#endif

        private static string LUIS_SUBSCRIPTION_KEY
        {
            get
            {
#if VRSSTUDIO_INTERNAL
				return VRSStudio.Internal.VoiceComand.VoiceCommandResources.LUIS_SUBSCRIPTION_KEY;
#else
                return m_LUIS_SUBSCRIPTION_KEY;
#endif
            }
        }
        private static string LUIS_REGION
        {
            get
            {
#if VRSSTUDIO_INTERNAL
				return VRSStudio.Internal.VoiceComand.VoiceCommandResources.LUIS_REGION;
#else
                return m_LUIS_REGION;
#endif
            }
        }

        private static string LUIS_APP_ID
        {
            get
            {
#if VRSSTUDIO_INTERNAL
				return VRSStudio.Internal.VoiceComand.VoiceCommandResources.LUIS_APP_ID;
#else
                return m_LUIS_APP_ID;
#endif
            }
        }

        private static Dictionary<string, string> LUIS_Intents =
            new Dictionary<string, string> { { "0", "Adjust Volume" },
                                         { "1", "Exit App" },
                                         { "2", "Launch App" },
                                         { "3", "Manage Hand Tracking" },
                                         { "4", "Manage Passthrough" },
                                         { "5", "Media Control" },
                                         { "6", "Power Off" },
                                         { "7", "Switch Voice Command" },
                                         { "8", "Visit Library" },
                                         { "9", "Visit Profile" },
                                         { "10", "Visit Settings" },
                                         { "11", "Visit Store" },
                                         { "12", "Wake Up" },
                                         // Weather
                                         { "13", "Weather.CheckWeatherTime" },
                                         { "14", "Weather.CheckWeatherValue" },
                                         { "15", "Weather.GetWeatherAdvisory" },
                                         { "16", "Weather.QueryWeather" },
                                         { "17", "Weather.ChangeTemperatureUnit" },
                                         // Web
                                         { "18", "Web.WebSearch" } };


        //SpeechRecognize and TranslateRecognize use.
        public enum RecognitionHandlerStat
        {
            None,  // StopRecognition or StopTranslation.
            Recognizing,
            Recognized,
            Canceled
        }
        public RecognitionHandlerStat SpeechRecognitionHandlerStat
        {
            get { return m_SpeechRecognizerComponent.HandlerStat; }
        }
        public RecognitionHandlerStat TranslationRecognitionHandlerStat
        {
            get { return m_TranslationRecognizerComponent.HandlerStat; }
        }
        public RecognitionHandlerStat IntentRecognitionHandlerStat
        {
            get { return m_IntentRecognizerComponent.HandlerStat; }
        }

        public bool IsStartedRecognition
        {
            get
            {
                return m_SpeechRecognizerComponent.IsStartedRecognition ||
                       m_TranslationRecognizerComponent.IsStartedTranslation;
            }
        }
        public bool IsStartedSpeechRecognition
        {
            get { return m_SpeechRecognizerComponent.IsStartedRecognition; }
        }
        public bool IsStartedTranslation
        {
            get { return m_TranslationRecognizerComponent.IsStartedTranslation; }
        }
        public bool IsStartedIntentRecognition
        {
            get { return m_IntentRecognizerComponent.IsStartedRecognition; }
        }

        private bool m_IsReinitProcessing = false;
        public bool IsReinitProcessing
        {
            get { return m_IsReinitProcessing; }
        }
        private readonly object ReinitProcessingLocker = new object();

        #region MicrophoneRecordAudioPermission
        // Required to manifest microphone permission, cf.
        // https://docs.unity3d.com/Manual/android-manifest.html
        private Microphone mic;
        public static bool CheckMicrophoneRecordAudioPermission()
        {
            return m_MicrophoneRecordAudioPermission.CheckMicrophoneRecordAudioPermission();
        }

        [Serializable]
        class MicrophoneRecordAudioPermission
        {
            private static bool m_IsWaitPermissionCallBack = false;

            /// <summary>
            /// Request the permission of Microphone if the permission is not granted. Use this function before using Speech-To-Text feature.
            /// If you don't want to request the permission, you can check it by Permission.HasUserAuthorizedPermission(Permission.Microphone).
            /// </summary>
            /// <returns>
            /// Return if the permission of Microphone is granted or not. Same as calling Permission.HasUserAuthorizedPermission(Permission.Microphone).
            /// </returns>
            public bool CheckMicrophoneRecordAudioPermission()
            {
                // https://docs.unity3d.com/Manual/android-RequestingPermissions.html
#if PLATFORM_ANDROID
                if (isPermissionGranted) { return true; }

                isPermissionGranted = Permission.HasUserAuthorizedPermission(Permission.Microphone);
                if (!isPermissionGranted)
                {
                    if (Instance != null)
                    {
                        Instance.StartCoroutine(GrantRecordAudioPermission());
                    }
                    else
                    {
                        Debug.LogError(LOG_TAG + " : " + "Instace is null so it cannot GrantRecordAudioPermission.");
                    }
                }
                else
                {
                    Debug.Log(LOG_TAG + " : " + "MicPermission is already granted.");
                }
#endif
                return isPermissionGranted;
            }
            private IEnumerator GrantRecordAudioPermission()
            {
#if PLATFORM_ANDROID
                while (!Permission.HasUserAuthorizedPermission(Permission.Microphone) && !m_IsWaitPermissionCallBack)
                {
                    var callbacks = new PermissionCallbacks();
                    callbacks.PermissionGranted += (permission) => { m_IsWaitPermissionCallBack = true; };
                    Permission.RequestUserPermission(Permission.Microphone, callbacks);
                    isPermissionGranted = true;
                    yield return null;
                }
#else
                yield break;
#endif
            }
            private bool isPermissionGranted = false;

        }
        private static MicrophoneRecordAudioPermission m_MicrophoneRecordAudioPermission = new MicrophoneRecordAudioPermission();
        #endregion

        #region SpeechRecognizerComponent
        [System.Serializable]
        public class SpeechRecognizerComponentClass
        {
            private List<string> SupportedLanguages = new List<string> { "en-US", "zh-TW" };

            private RecognitionHandlerStat m_HandlerStat = RecognitionHandlerStat.None;
            public RecognitionHandlerStat HandlerStat
            {
                get { return m_HandlerStat; }
            }
            // TODO
            private string m_MessageFromHandler = String.Empty;
            public string MessageFromHandler
            {
                get { return m_MessageFromHandler; }
            }

            private bool m_IsStartedRecognition = false;
            public bool IsStartedRecognition
            {
                get { return m_IsStartedRecognition; }
            }

            private string m_RecognizedLanguage = String.Empty;
            public string RecognizedLanguage
            {
                get { return m_RecognizedLanguage; }
            }

            private bool m_IsStartRecognitionProcessing = false;
            private bool m_IsStopRecognitionProcessing = false;
            private readonly object m_StartRecognitionProcessingLocker = new object();
            private readonly object m_StopRecognitionProcessingLocker = new object();

            SpeechConfig m_Config = null;
            AudioConfig m_AudioConfig = null;
            private static SpeechRecognizer m_SpeechRecognizer = null;
            private bool IsInitialized
            {
                get
                {
                    return (Microphone.devices.Length.Equals(0) || m_SpeechRecognizer == null) ? false : true;
                }
            }

            public void Initialize(string language = "en-US")
            {
                if (Microphone.devices.Length.Equals(0)) return;

                m_RecognizedLanguage = language;
                m_Config = SpeechConfig.FromSubscription(SUBSCRIPTION_KEY, REGION);
                m_AudioConfig = AudioConfig.FromDefaultMicrophoneInput();
                if (SupportedLanguages.Contains(language))
                {
                    m_SpeechRecognizer = new SpeechRecognizer(m_Config, language, m_AudioConfig);
                }
                else
                {
                    m_SpeechRecognizer = new SpeechRecognizer(m_Config, m_AudioConfig); //Default language en-US.
                }
                m_SpeechRecognizer.Recognizing += RecognizingHandler;
                m_SpeechRecognizer.Recognized += RecognizedHandler;
                m_SpeechRecognizer.Canceled += CanceledHandler;
                m_SpeechRecognizer.SessionStarted += SessionStartedHandler;
                m_SpeechRecognizer.SessionStopped += SessionStoppedHandler;
                m_SpeechRecognizer.SpeechStartDetected += SpeechStartDetectedHandler;
                m_SpeechRecognizer.SpeechEndDetected += SpeechEndDetectedHandler;
            }

            public async Task ReInitialize(string language = "en-US")
            {
                if (m_IsStartedRecognition)
                {
                    await StopRecognition();
                }
                Initialize(language);
            }

            private void Destroy()
            {
            }

            public async Task StartRecognition()
            {
                if (!IsInitialized) return;

                lock (m_StartRecognitionProcessingLocker)
                {
                    if (m_IsStartRecognitionProcessing)
                    {
                        Log.d(LOG_TAG, "m_IsStartRecognitionProcessing is true, skip it");
                        return;
                    }
                    else
                    {
                        Log.d(LOG_TAG, "StartRecognition");
                    }
                    m_IsStartRecognitionProcessing = true;
                }

                Log.d(LOG_TAG, "SpeechRecognizerComponent StartRecognition++");
                float timer = Time.realtimeSinceStartup;
                await m_SpeechRecognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
                float timeInterval = Time.realtimeSinceStartup - timer;
                Log.d(LOG_TAG, "SpeechRecognizerComponent StartRecognition--the interval is:" + timeInterval);

                m_IsStartedRecognition = true;

                lock (m_StartRecognitionProcessingLocker)
                {
                    m_IsStartRecognitionProcessing = false;
                }
            }

            public async Task StopRecognition()
            {
                if (!IsInitialized) return;

                lock (m_StopRecognitionProcessingLocker)
                {
                    if (m_IsStopRecognitionProcessing)
                    {
                        Log.d(LOG_TAG, "IsStopRecognitionProcessing is true, skip it");
                        return;
                    }
                    else
                    {
                        Log.d(LOG_TAG, "StopRecognition");
                    }
                    m_IsStopRecognitionProcessing = true;
                }

                Log.d(LOG_TAG, "SpeechRecognizerComponent StopRecognition++");
                // float timer = Time.realtimeSinceStartup;
                await m_SpeechRecognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
                // float timeInterval = Time.realtimeSinceStartup - timer;
                // Log.d(LOG_TAG, "SpeechRecognizerComponent StopRecognition-- the interval is:" +
                // timeInterval);

                m_HandlerStat = RecognitionHandlerStat.None;
                m_MessageFromHandler = String.Empty;

                m_IsStartedRecognition = false;
                Log.d(LOG_TAG, "StopRecognition IsStartedRecognition:" + m_IsStartedRecognition.ToString());

                lock (m_StopRecognitionProcessingLocker)
                {
                    m_IsStopRecognitionProcessing = false;
                }
            }
            // StartContinuousRecognitionAsync done ~=> SessionStarted ==> SpeechStartDetected ~=> Recognizing ~=> Recognized ==> SpeechEndDetected ~=> SessionStopped ~=> StopContinuousRecognitionAsync
            #region Handlers of SpeechRecognizer
            private void RecognizingHandler(object sender, SpeechRecognitionEventArgs e)
            {
                m_HandlerStat = RecognitionHandlerStat.Recognizing;
                m_MessageFromHandler = e.Result.Text;
                Log.d(LOG_TAG, "SpeechRecognizerComponent RecognizingHandler:" + m_MessageFromHandler);
            }
            private void RecognizedHandler(object sender, SpeechRecognitionEventArgs e)
            {
                m_HandlerStat = RecognitionHandlerStat.Recognized;
                m_MessageFromHandler = e.Result.Text;
                Log.d(LOG_TAG, "SpeechRecognizerComponent RecognizedHandler:" + m_MessageFromHandler);
            }
            private void CanceledHandler(object sender, SpeechRecognitionCanceledEventArgs e)
            {
                m_HandlerStat = RecognitionHandlerStat.Canceled;
                m_MessageFromHandler = e.ErrorDetails.ToString();
                Log.d(LOG_TAG, "SpeechRecognizerComponent CanceledHandler:" + m_MessageFromHandler);
            }
            private void SessionStartedHandler(object sender, SessionEventArgs e)
            {
                //At the time recognition is started.
                Log.d(LOG_TAG, "SpeechRecognizerComponent SessionStartedHandler");
            }
            private void SessionStoppedHandler(object sender, SessionEventArgs e)
            {
                Log.d(LOG_TAG, "SpeechRecognizerComponent SessionStoppedHandler");
            }
            private void SpeechStartDetectedHandler(object sender, RecognitionEventArgs e)
            {
                //At the time detect voice input.
                Log.d(LOG_TAG, "SpeechRecognizerComponent SpeechStartDetectedHandler");
            }
            private void SpeechEndDetectedHandler(object sender, RecognitionEventArgs e)
            {
                Log.d(LOG_TAG, "SpeechRecognizerComponent SpeechEndDetectedHandler");
            }

            public event EventHandler<SpeechRecognitionEventArgs> Recognizing
            {
                add
                {
                    if (!IsInitialized) return;
                    m_SpeechRecognizer.Recognizing += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_SpeechRecognizer.Recognizing -= value;
                }
            }
            public event EventHandler<SpeechRecognitionEventArgs> Recognized
            {
                add
                {
                    if (!IsInitialized) return;
                    m_SpeechRecognizer.Recognized += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_SpeechRecognizer.Recognized -= value;
                }
            }
            public event EventHandler<SpeechRecognitionCanceledEventArgs> Canceled
            {
                add
                {
                    if (!IsInitialized) return;
                    m_SpeechRecognizer.Canceled += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_SpeechRecognizer.Canceled -= value;
                }
            }
            public event EventHandler<SessionEventArgs> SessionStarted
            {
                add
                {
                    if (!IsInitialized) return;
                    m_SpeechRecognizer.SessionStarted += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_SpeechRecognizer.SessionStarted -= value;
                }
            }
            public event EventHandler<SessionEventArgs> SessionStopped
            {
                add
                {
                    if (!IsInitialized) return;
                    m_SpeechRecognizer.SessionStopped += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_SpeechRecognizer.SessionStopped -= value;
                }
            }
            public event EventHandler<RecognitionEventArgs> SpeechStartDetected
            {
                add
                {
                    if (!IsInitialized) return;
                    m_SpeechRecognizer.SpeechStartDetected += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_SpeechRecognizer.SpeechStartDetected -= value;
                }
            }
            public event EventHandler<RecognitionEventArgs> SpeechEndDetected
            {
                add
                {
                    if (!IsInitialized) return;
                    m_SpeechRecognizer.SpeechEndDetected += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_SpeechRecognizer.SpeechEndDetected -= value;
                }
            }
            #endregion
        }
        #endregion
        private SpeechRecognizerComponentClass m_SpeechRecognizerComponent =
            new SpeechRecognizerComponentClass();
        public SpeechRecognizerComponentClass SpeechRecognizerComponent
        {
            get { return m_SpeechRecognizerComponent; }
        }

        #region TranslationRecognizerComponent
        [System.Serializable]
        public class TranslationRecognizerComponentClass
        {
            private RecognitionHandlerStat m_HandlerStat = RecognitionHandlerStat.None;
            public RecognitionHandlerStat HandlerStat
            {
                get { return m_HandlerStat; }
            }

            IReadOnlyDictionary<string, string> m_MessageDictionaryFromHandler = null;
            public IReadOnlyDictionary<string, string> MessageDictionaryFromHandler
            {
                get { return m_MessageDictionaryFromHandler; }
            }

            private string m_MessageFromHandler = String.Empty;
            public string MessageFromHandler
            {
                get { return m_MessageFromHandler; }
            }

            private bool m_IsStartedTranslation = false;
            public bool IsStartedTranslation
            {
                get { return m_IsStartedTranslation; }
            }

            private string m_RecognizedLanguage = String.Empty;
            public string RecognizedLanguage
            {
                get { return m_RecognizedLanguage; }
            }

            private List<string> m_TargetLanguages = null;
            public List<string> TargetLanguages
            {
                get { return m_TargetLanguages; }
            }

            #region Locker
            private bool m_IsStartTranslationProcessing = false;
            private bool m_IsStopTranslationProcessing = false;
            private readonly object m_StartTranslationProcessingLocker = new object();
            private readonly object m_StopTranslationProcessingLocker = new object();
            #endregion

            SpeechTranslationConfig m_Config = null;
            AudioConfig m_AudioConfig = null;
            private static TranslationRecognizer m_TranslationRecognizer = null;
            private bool IsInitialized
            {
                get
                {
                    return (Microphone.devices.Length.Equals(0) || m_TranslationRecognizer == null) ? false : true;
                }
            }

            //TODO: TargetLanguages                                                              
            public void Initialize(string FromLanguage = "en-US", List<string> TargetLanguates = null)
            {
                if (Microphone.devices.Length.Equals(0)) return;

                m_RecognizedLanguage = FromLanguage;
                m_TargetLanguages = TargetLanguates;
                Log.d(LOG_TAG, "TranslationRecognizerComponent Initialize : " + FromLanguage);
                m_Config = SpeechTranslationConfig.FromSubscription(SUBSCRIPTION_KEY, REGION);
                m_Config.SpeechRecognitionLanguage = FromLanguage;

                //TODO: TargetLanguages
                if (FromLanguage == "en-US")
                {
                    Log.d(LOG_TAG, "TranslationRecognizerComponent detect en-US, set target zh-TW");
                    // https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support#text-languages
                    m_Config.AddTargetLanguage("zh-Hant");
                    //translationConfig.VoiceName = "zh-TW-HsiaoChenNeural";
                }
                else if (FromLanguage == "zh-TW")
                {
                    Log.d(LOG_TAG, "TranslationRecognizerComponent detect zh-TW, set target en-US");
                    m_Config.AddTargetLanguage("en");
                    //translationConfig.VoiceName = "en-US-JennyNeural";
                }

                m_AudioConfig = AudioConfig.FromDefaultMicrophoneInput();
                m_TranslationRecognizer = new TranslationRecognizer(m_Config, m_AudioConfig);

                m_TranslationRecognizer.Recognizing += RecognizingHandler;
                m_TranslationRecognizer.Recognized += RecognizedHandler;
                m_TranslationRecognizer.Canceled += CanceledHandler;

                m_TranslationRecognizer.SessionStarted += SessionStartedHandler;
                m_TranslationRecognizer.SessionStopped += SessionStoppedHandler;
                m_TranslationRecognizer.SpeechStartDetected += SpeechStartDetectedHandler;
                m_TranslationRecognizer.SpeechEndDetected += SpeechEndDetectedHandler;
            }
            public async Task ReInitialize(string FromLanguage = "en-US", List<string> TargetLanguates = null)
            {
                if (m_IsStartedTranslation)
                {
                    await StopTranslation();
                }
                Initialize(FromLanguage, TargetLanguates);
            }
            //TODO:
            private void Destroy()
            {
            }
            public async Task StartTranslation()
            {
                if (!IsInitialized) return;

                lock (m_StartTranslationProcessingLocker)
                {
                    if (m_IsStartTranslationProcessing)
                    {
                        Log.d(LOG_TAG, "IsStartTranslationProcessing is true, skip it");
                        return;
                    }
                    else
                    {
                        Log.d(LOG_TAG, "StartTranslation");
                    }
                    m_IsStartTranslationProcessing = true;
                }

                Log.d(LOG_TAG, "TranslationRecognizerComponent StartTranslation++");
                // float timer = Time.realtimeSinceStartup;
                await m_TranslationRecognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
                // float timeInterval = Time.realtimeSinceStartup - timer;
                // Log.d(LOG_TAG, "TranslationRecognizerComponent StartTranslation--the interval is:" +
                // timeInterval);

                //TODO: Assign in Event Handler or not.
                m_IsStartedTranslation = true;
                Log.d(LOG_TAG, "IsStartedTranslation:" + m_IsStartedTranslation.ToString());

                lock (m_StartTranslationProcessingLocker)
                {
                    m_IsStartTranslationProcessing = false;
                }
            }
            public async Task StopTranslation()
            {
                if (!IsInitialized) return;

                lock (m_StopTranslationProcessingLocker)
                {
                    if (m_IsStopTranslationProcessing)
                    {
                        Log.d(LOG_TAG, "IsStopTranslationProcessing is true, skip it");
                        return;
                    }
                    else
                    {
                        Log.d(LOG_TAG, "StopTranslation");
                    }
                    m_IsStopTranslationProcessing = true;
                }

                Log.d(LOG_TAG, "TranslationRecognizerComponent StopTranslation++");
                // float timer = Time.realtimeSinceStartup;
                await m_TranslationRecognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
                // float timeInterval = Time.realtimeSinceStartup - timer;
                // Log.d(LOG_TAG, "TranslationRecognizerComponent StopTranslation--the interval is:" +
                // timeInterval);

                m_HandlerStat = RecognitionHandlerStat.None;
                m_MessageFromHandler = String.Empty;

                m_IsStartedTranslation = false;
                Log.d(LOG_TAG, "IsStartedTranslation:" + m_IsStartedTranslation.ToString());

                lock (m_StopTranslationProcessingLocker)
                {
                    m_IsStopTranslationProcessing = false;
                }
            }

            // StartContinuousRecognitionAsync done ~=> SessionStarted ==> SpeechStartDetected ~=> Recognizing ~=> Recognized ==> SpeechEndDetected ~=> SessionStopped ~=> StopContinuousRecognitionAsync
            #region Handlers of translation
            private void RecognizingHandler(object sender, TranslationRecognitionEventArgs e)
            {
                m_HandlerStat = RecognitionHandlerStat.Recognizing;
                //TODO
                m_MessageDictionaryFromHandler = e.Result.Translations;
                m_MessageFromHandler = e.Result.Text;

                string message = "[From]" + m_MessageFromHandler;
                foreach (var element in m_MessageDictionaryFromHandler)
                {
                    message += System.Environment.NewLine + "[To " + element.Key + "]" + element.Value;
                }
                Log.d(LOG_TAG, "TranslationRecognizerComponent RecognizingHandler:" + message);

                //var AutoDetectResult = e.Result.Properties.GetProperty(PropertyId.SpeechServiceConnection_AutoDetectSourceLanguageResult);
            }
            private void RecognizedHandler(object sender, TranslationRecognitionEventArgs e)
            {
                m_HandlerStat = RecognitionHandlerStat.Recognized;
                m_MessageDictionaryFromHandler = e.Result.Translations;
                m_MessageFromHandler = e.Result.Text;

                string message = "[From]" + m_MessageFromHandler;
                foreach (var element in m_MessageDictionaryFromHandler)
                {
                    message += System.Environment.NewLine + "[To " + element.Key + "]" + element.Value;
                }
                Log.d(LOG_TAG, "TranslationRecognizerComponent RecognizingHandler:" + message);
            }
            private void CanceledHandler(object sender, TranslationRecognitionCanceledEventArgs e)
            {
                m_HandlerStat = RecognitionHandlerStat.Canceled;
                m_MessageFromHandler = e.ErrorDetails.ToString();
                Log.d(LOG_TAG, "TranslationRecognizerComponent CanceledHandler:" + m_MessageFromHandler);
            }
            private void SessionStartedHandler(object sender, SessionEventArgs e)
            {
                //At the time recognition is started.
                Log.d(LOG_TAG, "TranslationRecognizerComponent SessionStartedHandler");
            }
            private void SessionStoppedHandler(object sender, SessionEventArgs e)
            {
                Log.d(LOG_TAG, "TranslationRecognizerComponent SessionStoppedHandler");
            }
            private void SpeechStartDetectedHandler(object sender, RecognitionEventArgs e)
            {
                //At the time detect voice input.
                Log.d(LOG_TAG, "TranslationRecognizerComponent SpeechStartDetectedHandler");
            }
            private void SpeechEndDetectedHandler(object sender, RecognitionEventArgs e)
            {
                Log.d(LOG_TAG, "TranslationRecognizerComponent SpeechEndDetectedHandler");
            }

            public event EventHandler<TranslationRecognitionEventArgs> Recognizing
            {
                add
                {
                    if (!IsInitialized) return;
                    m_TranslationRecognizer.Recognizing += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_TranslationRecognizer.Recognizing -= value;
                }
            }
            public event EventHandler<TranslationRecognitionEventArgs> Recognized
            {
                add
                {
                    if (!IsInitialized) return;
                    m_TranslationRecognizer.Recognized += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_TranslationRecognizer.Recognized -= value;
                }
            }
            public event EventHandler<TranslationRecognitionCanceledEventArgs> Canceled
            {
                add
                {
                    if (!IsInitialized) return;
                    m_TranslationRecognizer.Canceled += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_TranslationRecognizer.Canceled -= value;
                }
            }
            public event EventHandler<SessionEventArgs> SessionStarted
            {
                add
                {
                    if (!IsInitialized) return;
                    m_TranslationRecognizer.SessionStarted += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_TranslationRecognizer.SessionStarted -= value;
                }
            }
            public event EventHandler<SessionEventArgs> SessionStopped
            {
                add
                {
                    if (!IsInitialized) return;
                    m_TranslationRecognizer.SessionStopped += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_TranslationRecognizer.SessionStopped -= value;
                }
            }
            public event EventHandler<RecognitionEventArgs> SpeechStartDetected
            {
                add
                {
                    if (!IsInitialized) return;
                    m_TranslationRecognizer.SpeechStartDetected += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_TranslationRecognizer.SpeechStartDetected -= value;
                }
            }
            public event EventHandler<RecognitionEventArgs> SpeechEndDetected
            {
                add
                {
                    if (!IsInitialized) return;
                    m_TranslationRecognizer.SpeechEndDetected += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_TranslationRecognizer.SpeechEndDetected -= value;
                }
            }
            #endregion
        }
        #endregion
        private TranslationRecognizerComponentClass m_TranslationRecognizerComponent = new TranslationRecognizerComponentClass();
        public TranslationRecognizerComponentClass TranslationRecognizerComponent { get { return m_TranslationRecognizerComponent; } }

        #region SpeechSynthesizerComponent
        [System.Serializable]
        public class SpeechSynthesizerComponentClass
        {
            // Setup supported character voice. Ref: https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support#text-to-speech
            private string SpeakAvatar_en_US = "en-US-JennyNeural";
            private string SpeakAvatar_zh_TW = "zh-TW-HsiaoChenNeural";
            //Female: zh-TW-HsiaoChenNeural, zh-TW-HsiaoYuNeural, zh-TW-HanHanRUS, zh-TW-Yating
            //Male: zh-TW-YunJheNeural, zh-TW-Zhiwei

            private bool m_IsStartedSynthesis = false;
            public bool IsStartedSynthesis
            {
                get { return m_IsStartedSynthesis; }
            }

            private string m_RecognizedLanguage = String.Empty;
            public string RecognizedLanguage
            {
                get { return m_RecognizedLanguage; }
            }

            SpeechConfig m_Config = null;
            //AudioConfig m_AudioConfig = null;
            private static SpeechSynthesizer m_SpeechSynthesizer = null;

            private bool IsInitialized
            {
                get { return (m_SpeechSynthesizer == null) ? false : true; }
            }

            public void Initialize(string language = "en-US", string customAvatar = null)
            {
                m_RecognizedLanguage = language;
                m_Config = SpeechConfig.FromSubscription(SUBSCRIPTION_KEY, REGION);
                string SpeakAvatar = SpeakAvatar_en_US;
                if (String.Equals(language, "en-US"))
                {
                    Log.d(LOG_TAG, "SpeechSynthesizerComponent language is en-US");
                    SpeakAvatar = SpeakAvatar_en_US;
                    if (customAvatar != null)
                    {
                        SpeakAvatar = customAvatar;
                    }
                }
                else if (String.Equals(language, "zh-TW"))
                {
                    Log.d(LOG_TAG, "SpeechSynthesizerComponent language is zh-TW");
                    SpeakAvatar = SpeakAvatar_zh_TW;
                    if (customAvatar != null)
                    {
                        SpeakAvatar = customAvatar;
                    }
                }
                else
                {
                    Log.e(LOG_TAG, "SpeechSynthesizerComponent language should not be here");
                }
                m_Config.SpeechSynthesisVoiceName = SpeakAvatar;
                m_SpeechSynthesizer = new SpeechSynthesizer(m_Config);

                m_SpeechSynthesizer.SynthesisStarted += SynthesisStartedHandler;
                m_SpeechSynthesizer.Synthesizing += SynthesizingHandler;
                m_SpeechSynthesizer.SynthesisCompleted += SynthesisCompletedHandler;
                m_SpeechSynthesizer.SynthesisCanceled += SynthesisCanceledHandler;
            }
            public async Task ReInitialize(string language = "en-US", string customAvatar = null)
            {
                if (m_IsStartedSynthesis)
                {
                    await StopSynthesis();
                }
                Initialize(language, customAvatar);
            }
            // TODO:
            private void Destroy() { }
            public async Task StartSynthesis(string Message = "")
            {
                if (!IsInitialized) return;

                if (String.IsNullOrEmpty(Message))
                {
                    Log.w(LOG_TAG, "FireOnceSpeakTextAsync without Message");
                    return;
                }
                await m_SpeechSynthesizer.SpeakTextAsync(Message);
            }
            public async Task StopSynthesis()
            {
                if (!IsInitialized) return;

                Log.d(LOG_TAG, "SpeechSynthesizerComponent StopSynthesis++");
                await m_SpeechSynthesizer.StopSpeakingAsync();
                Log.d(LOG_TAG, "SpeechSynthesizerComponent StopSynthesis--");
            }
            #region Handlers of synthesis
            private void SynthesisStartedHandler(object sender, SpeechSynthesisEventArgs e)
            {
                Log.d(LOG_TAG, "SpeechSynthesizerComponent SynthesisStartedHandler");
            }
            private void SynthesizingHandler(object sender, SpeechSynthesisEventArgs e)
            {
                Log.d(LOG_TAG, "SpeechSynthesizerComponent SynthesizingHandler");
            }
            private void SynthesisCompletedHandler(object sender, SpeechSynthesisEventArgs e)
            {
                Log.d(LOG_TAG, "SpeechSynthesizerComponent SynthesisCompletedHandler");
            }
            private void SynthesisCanceledHandler(object sender, SpeechSynthesisEventArgs e)
            {
                Log.d(LOG_TAG, "SpeechSynthesizerComponent SynthesisCanceledHandler");
            }
            public event EventHandler<SpeechSynthesisEventArgs> SynthesisStarted
            {
                add
                {
                    if (!IsInitialized) return;
                    m_SpeechSynthesizer.SynthesisStarted += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_SpeechSynthesizer.SynthesisStarted -= value;
                }
            }
            public event EventHandler<SpeechSynthesisEventArgs> Synthesizing
            {
                add
                {
                    if (!IsInitialized) return;
                    m_SpeechSynthesizer.Synthesizing += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_SpeechSynthesizer.Synthesizing -= value;
                }
            }
            public event EventHandler<SpeechSynthesisEventArgs> SynthesisCompleted
            {
                add
                {
                    if (!IsInitialized) return;
                    m_SpeechSynthesizer.SynthesisCompleted += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_SpeechSynthesizer.SynthesisCompleted -= value;
                }
            }
            public event EventHandler<SpeechSynthesisEventArgs> SynthesisCanceled
            {
                add
                {
                    if (!IsInitialized) return;
                    m_SpeechSynthesizer.SynthesisCanceled += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_SpeechSynthesizer.SynthesisCanceled -= value;
                }
            }
            #endregion
            //Use this If you don't want to force stop the SpeakText and you don't need to receive any other handler.
            async public Task FireOnceSpeakTextAsync(string language = "en-US", string Message = "")
            {
                if (String.IsNullOrEmpty(Message))
                {
                    Log.w(LOG_TAG, "FireOnceSpeakTextAsync without Message");
                    return;
                }

                var LocalConfig = SpeechConfig.FromSubscription(SUBSCRIPTION_KEY, REGION);
                string SpeakAvatar = SpeakAvatar_en_US;
                if (String.Equals(language, "en-US"))
                {
                    SpeakAvatar = SpeakAvatar_en_US;
                }
                else if (String.Equals(language, "zh-TW"))
                {
                    SpeakAvatar = SpeakAvatar_zh_TW;
                }
                LocalConfig.SpeechSynthesisVoiceName = SpeakAvatar;
                var synthesizerLocal = new SpeechSynthesizer(LocalConfig);
                if (synthesizerLocal != null)
                {
                    await synthesizerLocal.SpeakTextAsync(Message);
                }
            }
        }
        #endregion
        private SpeechSynthesizerComponentClass m_SpeechSynthesizerComponent =
            new SpeechSynthesizerComponentClass();
        public SpeechSynthesizerComponentClass SpeechSynthesizerComponent
        {
            get { return m_SpeechSynthesizerComponent; }
        }

        #region IntentRecognizerComponent
        [System.Serializable]
        public class IntentRecognizerComponentClass
        {
            private List<string> SupportedLanguages = new List<string> { "en-US", "zh-TW" };

            private RecognitionHandlerStat m_HandlerStat = RecognitionHandlerStat.None;
            public RecognitionHandlerStat HandlerStat
            {
                get { return m_HandlerStat; }
            }
            // TODO
            private string m_MessageFromHandler = String.Empty;
            public string MessageFromHandler
            {
                get { return m_MessageFromHandler; }
            }

            private bool m_IsStartedRecognition = false;
            public bool IsStartedRecognition
            {
                get { return m_IsStartedRecognition; }
            }

            private string m_RecognizedLanguage = String.Empty;
            public string RecognizedLanguage
            {
                get { return m_RecognizedLanguage; }
            }

            #region Locker
            private bool m_IsStartIntentProcessing = false;
            private bool m_IsStopIntentProcessing = false;
            private readonly object m_StartIntentProcessingLocker = new object();
            private readonly object m_StopIntentProcessingLocker = new object();
            #endregion

            SpeechConfig m_Config = null;
            // AudioConfig m_AudioConfig = null;
            private static IntentRecognizer m_IntentRecognizer = null;
            private bool IsInitialized
            {
                get
                {
                    return (Microphone.devices.Length.Equals(0) || m_IntentRecognizer == null) ? false : true;
                }
            }

            public void Initialize(string language = "en-US")
            {
                if (Microphone.devices.Length.Equals(0)) return;

                m_RecognizedLanguage = language;
                m_Config = SpeechConfig.FromSubscription(LUIS_SUBSCRIPTION_KEY, LUIS_REGION);
                // m_AudioConfig = AudioConfig.FromDefaultMicrophoneInput();
                if (SupportedLanguages.Contains(language))
                {
                    m_Config.SpeechRecognitionLanguage = language;
                }
                m_IntentRecognizer = new IntentRecognizer(m_Config);  // Default language en-US.
                                                                      // Creates a Language Understanding model using the app id, and adds specific intents from
                                                                      // your model
                var model = LanguageUnderstandingModel.FromAppId(LUIS_APP_ID);
                // Add intent name-id pairs
                foreach (var intent in LUIS_Intents)
                {
                    m_IntentRecognizer.AddIntent(model, intent.Value, intent.Key);
                }

                m_IntentRecognizer.Recognizing += RecognizingHandler;
                m_IntentRecognizer.Recognized += RecognizedHandler;
                m_IntentRecognizer.Canceled += CanceledHandler;

                m_IntentRecognizer.SessionStarted += SessionStartedHandler;
                m_IntentRecognizer.SessionStopped += SessionStoppedHandler;
                m_IntentRecognizer.SpeechStartDetected += SpeechStartDetectedHandler;
                m_IntentRecognizer.SpeechEndDetected += SpeechEndDetectedHandler;
            }

            public async Task ReInitialize(string language = "en-US")
            {
                if (m_IsStartedRecognition)
                {
                    await StopIntent();
                }
                Initialize(language);
            }

            private void Destroy() { }

            public async Task StartIntent()
            {
                if (!IsInitialized) return;

                lock (m_StartIntentProcessingLocker)
                {
                    if (m_IsStartIntentProcessing)
                    {
                        Log.d(LOG_TAG, "m_IsStartIntentProcessing is true, skip it");
                        return;
                    }
                    else
                    {
                        Log.d(LOG_TAG, "StartIntent");
                    }
                    m_IsStartIntentProcessing = true;
                }

                Log.d(LOG_TAG, "IntentRecognitionComponent StartRecognition++");
                // float timer = Time.realtimeSinceStartup;
                await m_IntentRecognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
                // float timeInterval = Time.realtimeSinceStartup - timer;
                // Log.d(LOG_TAG, "SpeechRecognizerComponent StartRecognition--the interval is:" +
                // timeInterval);

                m_IsStartedRecognition = true;

                lock (m_StartIntentProcessingLocker) { m_IsStartIntentProcessing = false; }
            }

            public async Task StopIntent()
            {
                if (!IsInitialized) return;

                lock (m_StopIntentProcessingLocker)
                {
                    if (m_IsStopIntentProcessing)
                    {
                        Log.d(LOG_TAG, "m_IsStopIntentProcessing is true, skip it");
                        return;
                    }
                    else
                    {
                        Log.d(LOG_TAG, "StopIntent");
                    }
                    m_IsStopIntentProcessing = true;
                }

                Log.d(LOG_TAG, "IntentRecognizerComponent StopRecognition++");
                // float timer = Time.realtimeSinceStartup;
                await m_IntentRecognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
                // float timeInterval = Time.realtimeSinceStartup - timer;
                // Log.d(LOG_TAG, "SpeechRecognizerComponent StopRecognition-- the interval is:" +
                // timeInterval);

                m_HandlerStat = RecognitionHandlerStat.None;
                m_MessageFromHandler = String.Empty;

                m_IsStartedRecognition = false;
                Log.d(LOG_TAG, "StopRecognition IsStartedRecognition:" + m_IsStartedRecognition.ToString());

                lock (m_StopIntentProcessingLocker) { m_IsStopIntentProcessing = false; }
            }
            // StartContinuousRecognitionAsync done ~=> SessionStarted ==> IntentStartDetected ~=> Recognizing
            // ~=> Recognized ==> IntentEndDetected ~=> SessionStopped ~=> StopContinuousRecognitionAsync
            #region Handlers of IntentRecognizer
            private void RecognizingHandler(object sender, IntentRecognitionEventArgs e)
            {
                m_HandlerStat = RecognitionHandlerStat.Recognizing;
                m_MessageFromHandler = e.Result.Text;
                Log.d(LOG_TAG, "IntentRecognizerComponent RecognizingHandler:" + m_MessageFromHandler);
            }
            private void RecognizedHandler(object sender, IntentRecognitionEventArgs e)
            {
                m_HandlerStat = RecognitionHandlerStat.Recognized;
                m_MessageFromHandler = e.Result.Text;
                Log.d(LOG_TAG, "IntentRecognizerComponent RecognizedHandler:" + m_MessageFromHandler);
            }
            private void CanceledHandler(object sender, IntentRecognitionCanceledEventArgs e)
            {
                m_HandlerStat = RecognitionHandlerStat.Canceled;
                m_MessageFromHandler = e.ErrorDetails.ToString();
                Log.d(LOG_TAG, "IntentRecognizerComponent CanceledHandler:" + m_MessageFromHandler);
            }
            private void SessionStartedHandler(object sender, SessionEventArgs e)
            {
                // At the time recognition is started.
                Log.d(LOG_TAG, "IntentRecognizerComponent SessionStartedHandler");
            }
            private void SessionStoppedHandler(object sender, SessionEventArgs e)
            {
                Log.d(LOG_TAG, "IntentRecognizerComponent SessionStoppedHandler");
            }
            private void SpeechStartDetectedHandler(object sender, RecognitionEventArgs e)
            {
                // At the time detect voice input.
                Log.d(LOG_TAG, "IntentRecognizerComponent SpeechStartDetectedHandler");
            }
            private void SpeechEndDetectedHandler(object sender, RecognitionEventArgs e)
            {
                Log.d(LOG_TAG, "IntentRecognizerComponent SpeechEndDetectedHandler");
            }

            public event EventHandler<IntentRecognitionEventArgs> Recognizing
            {
                add
                {
                    if (!IsInitialized) return;
                    m_IntentRecognizer.Recognizing += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_IntentRecognizer.Recognizing -= value;
                }
            }
            public event EventHandler<IntentRecognitionEventArgs> Recognized
            {
                add
                {
                    if (!IsInitialized) return;
                    m_IntentRecognizer.Recognized += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_IntentRecognizer.Recognized -= value;
                }
            }
            public event EventHandler<IntentRecognitionCanceledEventArgs> Canceled
            {
                add
                {
                    if (!IsInitialized) return;
                    m_IntentRecognizer.Canceled += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_IntentRecognizer.Canceled -= value;
                }
            }
            public event EventHandler<SessionEventArgs> SessionStarted
            {
                add
                {
                    if (!IsInitialized) return;
                    m_IntentRecognizer.SessionStarted += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_IntentRecognizer.SessionStarted -= value;
                }
            }
            public event EventHandler<SessionEventArgs> SessionStopped
            {
                add
                {
                    if (!IsInitialized) return;
                    m_IntentRecognizer.SessionStopped += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_IntentRecognizer.SessionStopped -= value;
                }
            }
            public event EventHandler<RecognitionEventArgs> SpeechStartDetected
            {
                add
                {
                    if (!IsInitialized) return;
                    m_IntentRecognizer.SpeechStartDetected += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_IntentRecognizer.SpeechStartDetected -= value;
                }
            }
            public event EventHandler<RecognitionEventArgs> SpeechEndDetected
            {
                add
                {
                    if (!IsInitialized) return;
                    m_IntentRecognizer.SpeechEndDetected += value;
                }
                remove
                {
                    if (!IsInitialized) return;
                    m_IntentRecognizer.SpeechEndDetected -= value;
                }
            }
            #endregion
        }
        #endregion
        private IntentRecognizerComponentClass m_IntentRecognizerComponent =
            new IntentRecognizerComponentClass();
        public IntentRecognizerComponentClass IntentRecognizerComponent
        {
            get { return m_IntentRecognizerComponent; }
        }

        private static VoiceCommandManager m_Instance = null;
        public static VoiceCommandManager Instance
        {
            get { return m_Instance; }
        }
        public void InitializeAll(string FromLanguage = "en-US", List<string> TargetLanguates = null)
        {
            m_SpeechRecognizerComponent.Initialize(FromLanguage);
            m_TranslationRecognizerComponent.Initialize(FromLanguage, TargetLanguates);
            m_SpeechSynthesizerComponent.Initialize(FromLanguage);
            m_IntentRecognizerComponent.Initialize(FromLanguage);
        }
        public async Task ReInitializeAll(string FromLanguage = "en-US", List<string> TargetLanguates = null)
        {
            /*
			lock (ReinitProcessingLocker)
			{
					if (m_IsReinitProcessing)
					{
							Log.d(LOG_TAG, "m_IsReinitProcessing is true, skip it");
							return;
					}
					m_IsReinitProcessing = true;
			}
			*/

            m_IsReinitProcessing = true;

            await m_SpeechRecognizerComponent.ReInitialize(FromLanguage);
            await m_TranslationRecognizerComponent.ReInitialize(FromLanguage, TargetLanguates);
            await m_SpeechSynthesizerComponent.ReInitialize(FromLanguage);
            await m_IntentRecognizerComponent.ReInitialize(FromLanguage);

            /*
			lock (ReinitProcessingLocker)
			{
					m_IsReinitProcessing = false;
			}
			*/
            m_IsReinitProcessing = false;
        }
        public async Task ReInitializeSpeechRecognizer(string FromLanguage = "en-US")
        {
            m_IsReinitProcessing = true;
            /*
			lock (ReinitProcessingLocker)
			{
					if (m_IsReinitProcessing)
					{
							Log.d(LOG_TAG, "m_IsReinitProcessing is true, skip it");
							return;
					}
					m_IsReinitProcessing = true;
			}
			*/

            await m_SpeechRecognizerComponent.ReInitialize(FromLanguage);

            /*
			lock (ReinitProcessingLocker)
			{
					m_IsReinitProcessing = false;
			}
			*/
            m_IsReinitProcessing = false;
        }
        public async Task ReInitializeTranslationRecognizer(string FromLanguage = "en-US", List<string> TargetLanguates = null)
        {
            m_IsReinitProcessing = true;
            /*
			lock (ReinitProcessingLocker)
			{
					if (m_IsReinitProcessing)
					{
							Log.d(LOG_TAG, "m_IsReinitProcessing is true, skip it");
							return;
					}
					m_IsReinitProcessing = true;
			}
			*/

            await m_TranslationRecognizerComponent.ReInitialize(FromLanguage, TargetLanguates);

            /*
			lock (ReinitProcessingLocker)
			{
					m_IsReinitProcessing = false;
			}
			*/
            m_IsReinitProcessing = false;
        }
        async public Task ReInitializeSpeechSynthesizer(string FromLanguage = "en-US", string customAvatar = null)
        {
            m_IsReinitProcessing = true;
            /*
			lock (ReinitProcessingLocker)
			{
					if (m_IsReinitProcessing)
					{
							Log.d(LOG_TAG, "m_IsReinitProcessing is true, skip it");
							return;
					}
					m_IsReinitProcessing = true;
			}
			*/

            await m_SpeechSynthesizerComponent.ReInitialize(FromLanguage, customAvatar);

            /*
			lock (ReinitProcessingLocker)
			{
					m_IsReinitProcessing = false;
			}
			*/
            m_IsReinitProcessing = false;
        }
        public async Task ReInitializeIntentRecognizer(string FromLanguage = "en-US")
        {
            m_IsReinitProcessing = true;
            /*
			lock (ReinitProcessingLocker)
			{
					if (m_IsReinitProcessing)
					{
							Log.d(LOG_TAG, "m_IsReinitProcessing is true, skip it");
							return;
					}
					m_IsReinitProcessing = true;
			}
			*/

            await m_IntentRecognizerComponent.ReInitialize(FromLanguage);

            /*
			lock (ReinitProcessingLocker)
			{
					m_IsReinitProcessing = false;
			}
			*/
            m_IsReinitProcessing = false;
        }
        /// <summary>
        /// StopSpeechRecognition, StopTranslation, StopSynthesis and StopIntent.
        /// </summary>
        public void StopAll()
        {
            _ = StopRecognition();
            _ = StopTranslation();
            _ = StopSynthesis();
            _ = StopIntent();
        }
        public async Task StartRecognition() { await m_SpeechRecognizerComponent.StartRecognition(); }
        public async Task StopRecognition() { await m_SpeechRecognizerComponent.StopRecognition(); }
        public async Task StartTranslation() { await m_TranslationRecognizerComponent.StartTranslation(); }
        public async Task StopTranslation() { await m_TranslationRecognizerComponent.StopTranslation(); }
        public async Task StartSynthesis(string Message = "") { await m_SpeechSynthesizerComponent.StartSynthesis(Message); }
        public async Task StopSynthesis() { await m_SpeechSynthesizerComponent.StopSynthesis(); }
        public async Task StartIntent() { await m_IntentRecognizerComponent.StartIntent(); }
        public async Task StopIntent() { await m_IntentRecognizerComponent.StopIntent(); }

        public async Task FireOnceSpeakTextAsync(string language = "en-US", string Message = "") { await m_SpeechSynthesizerComponent.FireOnceSpeakTextAsync(language, Message); }

        void Awake()
        {
            m_Instance = this;
        }
    }
}