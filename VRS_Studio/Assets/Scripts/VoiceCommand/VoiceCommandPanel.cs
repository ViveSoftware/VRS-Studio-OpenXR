#define WAVE_NATIVE
using HTC.ViveSoftware.ExpLab.HandInteractionDemo;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Intent;
using Microsoft.CognitiveServices.Speech.Translation;
using System;  // String
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VIVE.OpenXR.CompositionLayer;
using VIVE.OpenXR.Passthrough;
using Task = System.Threading.Tasks.Task;

// using HTC.Triton.LensUI; //LensUIButton

namespace VRSStudio.VoiceCommand.Typing
{
    public class VoiceCommandPanel : MonoBehaviour
    {
        private static string LOG_TAG = "VoiceCommandPanel";

        public Text HintList;
        public Text RecognitionContent;
        public GameObject RecognitionPrompt;
        public CustomInputField panelInputField;

        private const string HintText =
            "<size=30><b>Try saying:</b></size>\n";
        private List<string> FeatureListStr = null;

        // From event handler.
        string RecognizedResultStr = String.Empty;  //Update to InputField.
        string RecognitionContentStr = String.Empty;//Update to prompt.
        string IntentStr = String.Empty;
        string EntityStr = String.Empty;

        string Intent = String.Empty;
        double IntentScore = 0f;
        // key: entity category, value: entity text.
        // Because the entity text may start with capical letter, this case should be hangled by yourself or IntentManager.
        // IntentManager may transfer text to lower case before identitying. 
        public Dictionary<string, string> EntityDic = new Dictionary<string, string>();
        // private readonly object RecognitionContentLock = new object();

        private enum WaitFor
        {
            None,
            WakeUp,    //Wait for wake up intent while looking toward Jelbee.
            Intent,
            Recognized //When Recognizing or Translating languages.
        }
        WaitFor CurrentWaitFor = WaitFor.None;

        bool isWaitingForWakeUpPhrase = false;
        bool isWaitingForIntentPhrase = false;
        bool isWaitingForIntentCompletion = false;

        float intentCompletionThreshold = 1f;
        float intentCompletionTimer = 0f;
        bool timerNeeded = false;
        bool isTimerStarted = false;
        bool resetTimer = false;

        string SynthesisRecognitionLanguage = String.Empty;
        string SynthesisRecognitionMessage = String.Empty;

        #region RunOnMainThread
        Queue<Action> m_TasksRunOnMainThread = new Queue<Action>();
        public void RunOnMainThread(Action task)
        {
            lock (m_TasksRunOnMainThread) { m_TasksRunOnMainThread.Enqueue(task); }
        }
        private class SynthesResultOnPanelHandler
        {
            private Action<string, string> m_Action = null;
            private string m_Language = String.Empty;
            private string m_Recognitionresult = String.Empty;

            public SynthesResultOnPanelHandler(Action<string, string> Task, string Language,
                                               string RecognitionResults)
            {
                m_Action = Task;
                m_Language = Language;
                m_Recognitionresult = RecognitionResults;
            }
            public void Run() { m_Action(m_Language, m_Recognitionresult); }
        }
        Queue<SynthesResultOnPanelHandler> m_TasksRunOnMainThreadTwoParams =
            new Queue<SynthesResultOnPanelHandler>();
        private void RunOnMainThreadTwoParams(Action<string, string> Task, string Language,
                                              string RecognitionResults)
        {
            lock (m_TasksRunOnMainThreadTwoParams)
            {
                SynthesResultOnPanelHandler Handler =
                    new SynthesResultOnPanelHandler(Task, Language, RecognitionResults);
                m_TasksRunOnMainThreadTwoParams.Enqueue(Handler);
            }
        }
        #endregion

        // Update is called once per frame
        void Update()
        {
            // Update text from event handler
            lock (m_TasksRunOnMainThread)
            {
                if (m_TasksRunOnMainThread.Count > 0)
                {
                    var task = m_TasksRunOnMainThread.Dequeue();
                    task();
                }
            }
            lock (m_TasksRunOnMainThreadTwoParams)
            {
                if (m_TasksRunOnMainThreadTwoParams.Count > 0)
                {
                    SynthesResultOnPanelHandler Handler = m_TasksRunOnMainThreadTwoParams.Dequeue();
                    Handler.Run();
                }
            }

            //Handle Wake up intent listening
            if (CurrentWaitFor == WaitFor.None && RobotAssistantLoSCaster.isAlreadyInLoS)
            {
                CurrentWaitFor = WaitFor.WakeUp;
                StartListeningForWakeUpIntent();
            }
            else if (CurrentWaitFor == WaitFor.WakeUp && !RobotAssistantLoSCaster.isAlreadyInLoS)
            {
                CurrentWaitFor = WaitFor.None;
                StopListeningForWakeUpIntent();
            }

            // Handle intent action
            if (Intent != "")
            {
                Debug.Log(LOG_TAG + " : " + "Intent Handling: " + Intent);
                var WakeUpStr = IntentManager.GetIntentStr(IntentManager.IntentType.WakeUp);
                Debug.Log(LOG_TAG + " : " + "CurrentWaitFor: " + (int)CurrentWaitFor);
                if ((CurrentWaitFor == WaitFor.WakeUp && Intent == WakeUpStr) || (CurrentWaitFor == WaitFor.Intent && Intent != WakeUpStr))
                {
                    IntentManager.Instance.IntentInvoking(EntityDic, Intent, IntentScore);
                }
                Intent = "";
            }

            if (CurrentWaitFor == WaitFor.Recognized) //For recognize/translate only
            {
                if (timerNeeded)
                {
                    if (isTimerStarted)
                    {
                        if (resetTimer)
                        {
                            intentCompletionTimer = 0f;
                            resetTimer = false;
                        }
                        else
                        {
                            intentCompletionTimer += Time.deltaTime;
                        }
                    }
                    else
                    {
                        intentCompletionTimer = 0f;
                        isTimerStarted = true;
                    }

                    if (intentCompletionTimer >= intentCompletionThreshold)
                    {
                        timerNeeded = false;
                        resetTimer = false;
                        isTimerStarted = false;
                        intentCompletionTimer = 0f;
                        SpeechIntentCompletionAction();
                    }
                }
                else
                {
                    resetTimer = false;
                    isTimerStarted = false;
                    intentCompletionTimer = 0f;
                }
            }
        }

        private void OnEnable()
        {
            Debug.Log(LOG_TAG + " : " + "OnEnable");
            //m_ChooseStatus = ChooseStatus.Choosing;
            RecognitionContent.text = RecognitionContentStr = String.Empty;

            VoiceCommandManager VM = VoiceCommandManager.Instance;
            VoiceCommandManager.CheckMicrophoneRecordAudioPermission();

            VM.StopAll();
            currentActiveLanguage = ActiveLanguage.EN; //Always start from EN for now.

            CurrentWaitFor = WaitFor.None;
            UpdateHintText();

            IntentManager IM = IntentManager.Instance;

            if (IM == null)
            {
                IM = gameObject.AddComponent<IntentManager>();
            }

            IM.RecognizeEnglish.AddListener(RecognizeEnglish);
            IM.RecognizeChinese.AddListener(RecognizeChinese);
            IM.TranslateEnglish.AddListener(TranslateEnglish);
            IM.TranslateChinese.AddListener(TranslateChinese);
            IM.StopVoiceCommand.AddListener(SetupToChoosingStatus);
            IM.WakeUpEvent.AddListener(OnReceiveWakeUpIntent);
            IM.ExitAppEvent.AddListener(ExitApp);
            IM.TurnOnPassthroughEvent.AddListener(TurnOnPassthrough);
            IM.TurnOffPassthroughEvent.AddListener(TurnOffPassthrough);
            IM.UnknownIntent.AddListener(UnknownIntentAction);
        }
        private void OnDisable()
        {
            Debug.Log(LOG_TAG + " : " + "OnDisable");
            //m_ChooseStatus = ChooseStatus.Choosing;
            RecognitionContent.text = RecognitionContentStr = String.Empty;
            VoiceCommandManager VM = VoiceCommandManager.Instance;
            VM.StopAll();

            IntentManager IM = IntentManager.Instance;
            IM.RecognizeEnglish.RemoveListener(RecognizeEnglish);
            IM.RecognizeChinese.RemoveListener(RecognizeChinese);
            IM.TranslateEnglish.RemoveListener(TranslateEnglish);
            IM.TranslateChinese.RemoveListener(TranslateChinese);
            IM.StopVoiceCommand.RemoveListener(SetupToChoosingStatus);
            IM.WakeUpEvent.RemoveListener(OnReceiveWakeUpIntent);
            IM.ExitAppEvent.RemoveListener(ExitApp);
            IM.TurnOnPassthroughEvent.RemoveListener(TurnOnPassthrough);
            IM.TurnOffPassthroughEvent.RemoveListener(TurnOffPassthrough);
            IM.UnknownIntent.RemoveListener(UnknownIntentAction);
        }

        private const string FeatureList_Recognition_US_EN = "Recognize English.";
        private const string FeatureList_Recognition_US_CN = "識別英文。";
        private const string FeatureList_Recognition_CH_EN = "Recognize Chinese.";
        private const string FeatureList_Recognition_CH_CN = "識別中文。";
        private const string FeatureList_Translation_US_EN = "Translate from English to Chinese.";
        private const string FeatureList_Translation_US_CN = "翻譯英文到中文。";
        private const string FeatureList_Translation_CH_EN = "Translate from Chinese to English.";
        private const string FeatureList_Translation_CH_CN = "翻譯中文到英文。";
        private const string FeatureList_BackToChoosing_US = "Try another voice feature.";
        private const string FeatureList_BackToChoosing_CH = "嘗試其它語音功能。";
        private const string FeatureList_Dismiss_US = "Bye-Bye. See you.";
        private const string FeatureList_Dismiss_CH = "再見。";

        #region SetupFeatureListStatus
        private ActiveLanguage currentActiveLanguage = ActiveLanguage.EN;
        private enum ActiveLanguage
        {
            EN,
            CN,
        }

        private void SetupToChoosingStatus()
        {
            FeatureListStr = new List<string> {
            FeatureList_Recognition_US_EN,       FeatureList_Recognition_CH_EN,
            FeatureList_Translation_US_EN,       FeatureList_Translation_CH_EN,
            };
            //VoiceCommandManager VM = VoiceCommandManager.Instance;
            //VM.ReInitializeAll("en-US");

            CurrentWaitFor = WaitFor.Intent;
            UpdateHintText();
        }
        /*
        // Means chosen FeatureList_Recognition_US or FeatureList_Translation_US
        private void SetupFeatureListEnglishChoosedStatus()
        {
            currentActiveLanguage = ActiveLanguage.EN;
            FeatureListStr = new List<string> {
            FeatureList_Recognition_US_EN,       FeatureList_Recognition_CH_EN,
            FeatureList_Translation_US_EN,       FeatureList_Translation_CH_EN,
            FeatureList_BackToChoosing_US,       FeatureList_Dismiss_US };
        }
        // Means chosen FeatureList_Recognition_CH or FeatureList_Translation_CH
        private void SetupFeatureListChineseChoosedStatus()
        {
            currentActiveLanguage = ActiveLanguage.CN;
            FeatureListStr = new List<string> {
            FeatureList_Recognition_US_CN,       FeatureList_Recognition_CH_CN,
            FeatureList_Translation_US_CN,       FeatureList_Translation_CH_CN,
            FeatureList_BackToChoosing_CH,       FeatureList_Dismiss_CH };
        }
        */
        #endregion

        private async void OnReceiveWakeUpIntent()
        {
            await SynthesizeVoiceAssistantSpeech("I'm here. How may I help you?");

            isWaitingForWakeUpPhrase = false;
            isWaitingForIntentPhrase = true;
            UpdateHintText();

            if (currentActiveLanguage == ActiveLanguage.EN)
            {
                await RecognizeIntentEnglish();
            }
            else if (currentActiveLanguage == ActiveLanguage.CN)
            {
                await RecognizeIntentChinese();
            }
        }

        private async void StartListeningForWakeUpIntent()
        {
            Debug.Log(LOG_TAG + " : " + "StartListeningForWakeUpIntent");
            if (currentActiveLanguage == ActiveLanguage.EN)
            {
                await RecognizeIntentEnglish();
            }
            else if (currentActiveLanguage == ActiveLanguage.CN)
            {
                await RecognizeIntentChinese();
            }
        }
        private async void StopListeningForWakeUpIntent()
        {
            Debug.Log(LOG_TAG + " : " + "StopListeningForWakeUpIntent");
            await VoiceCommandManager.Instance.StopIntent();
        }

        private void UpdateHintText()
        {
            if (CurrentWaitFor == WaitFor.Intent) //Show intent hint list
            {
                string TextContent = HintText;
                for (int i = 0; i < FeatureListStr.Count; i++)
                {
                    TextContent += System.Environment.NewLine + FeatureListStr[i];
                }

                TextContent += System.Environment.NewLine + "Enable Passthrough.";
                TextContent += System.Environment.NewLine + "Disable Passthrough.";
                TextContent += System.Environment.NewLine + "Exit App.";

                HintList.text = TextContent;
            }
            else //Show wake up hint text
            {
                string TextContent = "Look at the robot and try saying \"Hi\"!";

                HintList.text = TextContent;
            }
        }

        #region Intent Actions

        private async void SpeechIntentCompletionAction()
        {
            Log.d(LOG_TAG, "SpeechIntentCompletionAction");

            await SynthesizeVoiceAssistantSpeech("Done, call me again when you need me.");

            SetupToChoosingStatus();
        }

        private async void UnknownIntentAction()
        {
            await SynthesizeVoiceAssistantSpeech("Sorry, I don't understand. Could you please say that again?");

            isWaitingForIntentPhrase = true;
            isWaitingForIntentCompletion = false;
        }

        private async void ExitApp()
        {
            await SynthesizeVoiceAssistantSpeech("Exiting application now. See you next time.");

            Log.d(LOG_TAG, "Exit App");
            Application.Quit();
        }

        private async void TurnOnPassthrough()
        {
            await SynthesizeVoiceAssistantSpeech("OK, turning on passthrough.");

            Log.d(LOG_TAG, "Turn on passthrough");
            EnablePassthrough();
            isWaitingForIntentPhrase = true;
            isWaitingForIntentCompletion = false;

            if (currentActiveLanguage == ActiveLanguage.EN)
            {
                await RecognizeIntentEnglish();
            }
            else if (currentActiveLanguage == ActiveLanguage.CN)
            {
                await RecognizeIntentChinese();
            }
        }

        private async void TurnOffPassthrough()
        {
            await SynthesizeVoiceAssistantSpeech("OK, turning off passthrough.");

            Log.d(LOG_TAG, "Turn off passthrough");
            DisablePassthrough();
            isWaitingForIntentCompletion = false;
        }

        private void UpdatePanelInputField()
        {
            RunOnMainThread(DisableRecognitionPrompt);

            if (panelInputField.text != "") panelInputField.text = panelInputField.text + "\n";

            if (RecognizedResultStr != "") panelInputField.text = panelInputField.text + RecognizedResultStr;

            Debug.Log(LOG_TAG + " : " + "UpdatePanelInputField RecognizedResultStr is " + RecognizedResultStr);
        }
        /*
                private void NotifyPaneltoSynthesisResults(string Language, string RecognitionContentStr)
		{
                        RunOnMainThread(DisableRecognitionPrompt);

                        if (panelInputField.text != "") panelInputField.text = panelInputField.text + "\n";

			if (RecognitionContentStr != "") panelInputField.text = panelInputField.text + RecognitionContentStr;

                        Debug.Log(LOG_TAG + " : " + "NotifyPaneltoSynthesisResults, Language is " + Language +
							   " and RecognitionContentStr is " + RecognitionContentStr);
                }
		*/
        private void UpdateRecognitionContent()
        {
            // Update UI text
            if (RecognitionContent != null)
            {
                if (!RecognitionPrompt.activeSelf) RecognitionPrompt.SetActive(true);

                RecognitionContent.text = RecognitionContentStr;
            }
        }

        private void DisableRecognitionPrompt()
        {
            if (RecognitionContent != null)
            {
                if (RecognitionPrompt.activeSelf) RecognitionPrompt.SetActive(false);

                RecognitionContent.text = "";
            }
        }

        private void OnVoiceAssistantSpeechBegin(string dialog)
        {
            StartCoroutine(VoiceAssistantSpeechBubbleCoroutine(dialog));
        }

        IEnumerator VoiceAssistantSpeechBubbleCoroutine(string dialog)
        {
            RobotAssistantManager Instance = RobotAssistantManager.Instance;
            Instance.robotAssistantSpeechBubble.ClearSpeechBubble();
            Instance.robotAssistantSpeechBubble.TextBoardShowup(true);
            //yield return new WaitForSecondsRealtime(1.5f);
            Instance.robotAssistantSpeechBubble.RobotLines = dialog;
            Instance.robotAssistantSpeechBubble.typingInterval = 0.05f;
            yield return StartCoroutine(Instance.robotAssistantSpeechBubble.PlayTypingWordAnim());
            yield return new WaitForSecondsRealtime(1f);
            Instance.robotAssistantSpeechBubble.TextBoardShowup(false);
        }

        private async Task SynthesizeVoiceAssistantSpeech(string dialog, string language = "en-US")
        {
            string voice = "en-US-GuyNeural";

            if (language.Equals("en-US"))
            {
                voice = "en-US-GuyNeural";
            }
            else if (language.Equals("zh-TW"))
            {
                voice = "zh-TW-YunJheNeural";
            }

            await VoiceCommandManager.Instance.ReInitializeSpeechSynthesizer(language, voice);

            //VoiceCommandManager.Instance.SpeechSynthesizerComponent.SynthesisStarted += OnVoiceAssistantSpeechBegin;

            OnVoiceAssistantSpeechBegin(dialog);
            await VoiceCommandManager.Instance.StartSynthesis(dialog);
        }
        #endregion

        private async void RecognizeEnglish()
        {
            //SetupFeatureListEnglishChoosedStatus();
            VoiceCommandManager VM = VoiceCommandManager.Instance;
            await VM.ReInitializeAll("en-US");
            VM.SpeechRecognizerComponent.Recognizing += RecognizingHandler;
            VM.SpeechRecognizerComponent.Recognized += RecognizedHandler;
            VM.SpeechRecognizerComponent.Canceled += CanceledHandler;
            await SynthesizeVoiceAssistantSpeech("Sure! Please speak English now.");
            await VM.StartRecognition();
        }
        private async void RecognizeChinese()
        {
            //SetupFeatureListChineseChoosedStatus();
            VoiceCommandManager VM = VoiceCommandManager.Instance;
            await VM.ReInitializeAll("zh-TW");
            VM.SpeechRecognizerComponent.Recognizing += RecognizingHandler;
            VM.SpeechRecognizerComponent.Recognized += RecognizedHandler;
            VM.SpeechRecognizerComponent.Canceled += CanceledHandler;
            await SynthesizeVoiceAssistantSpeech("沒問題!請開始講中文吧!", "zh-TW");
            await VM.StartRecognition();
        }
        private async void TranslateEnglish()
        {
            //SetupFeatureListEnglishChoosedStatus();
            VoiceCommandManager VM = VoiceCommandManager.Instance;
            await VM.ReInitializeAll("en-US");
            VM.TranslationRecognizerComponent.Recognizing += TranslationRecognizingHandler;
            VM.TranslationRecognizerComponent.Recognized += TranslationRecognizedHandler;
            VM.TranslationRecognizerComponent.Canceled += TranslationCanceledHandler;
            await SynthesizeVoiceAssistantSpeech("Great! Let's Translate English to Chinese.");
            await VM.StartTranslation();
        }
        private async void TranslateChinese()
        {
            //SetupFeatureListChineseChoosedStatus();
            VoiceCommandManager VM = VoiceCommandManager.Instance;
            await VM.ReInitializeAll("zh-TW");
            VM.TranslationRecognizerComponent.Recognizing += TranslationRecognizingHandler;
            VM.TranslationRecognizerComponent.Recognized += TranslationRecognizedHandler;
            VM.TranslationRecognizerComponent.Canceled += TranslationCanceledHandler;
            await SynthesizeVoiceAssistantSpeech("太棒了!讓我們來翻譯中文到英文。", "zh-TW");
            await VM.StartTranslation();
        }
        private async Task RecognizeIntentEnglish()
        {
            Debug.Log(LOG_TAG + " : " + "RecognizeIntentEnglish");
            VoiceCommandManager VM = VoiceCommandManager.Instance;
            await VM.ReInitializeAll("en-US");
            VM.IntentRecognizerComponent.Recognizing += IntentRecognizingHandler;
            VM.IntentRecognizerComponent.Recognized += IntentRecognizedHandler;
            VM.IntentRecognizerComponent.Canceled += IntentCanceledHandler;
            await VM.StartIntent();
        }
        private async Task RecognizeIntentChinese()
        {
            Debug.Log(LOG_TAG + " : " + "RecognizeIntentChinese");
            VoiceCommandManager VM = VoiceCommandManager.Instance;
            await VM.ReInitializeAll("zh-TW");
            VM.IntentRecognizerComponent.Recognizing += IntentRecognizingHandler;
            VM.IntentRecognizerComponent.Recognized += IntentRecognizedHandler;
            VM.IntentRecognizerComponent.Canceled += IntentCanceledHandler;
            await VM.StartIntent();
        }
        #region Recognition Event Handlers
        private void RecognizingHandler(object sender, SpeechRecognitionEventArgs e)
        {
            RecognitionContentStr = e.Result.Text;
            Debug.Log(LOG_TAG + " : " + "RecognizingHandler:" + RecognitionContentStr);
            RunOnMainThread(UpdateRecognitionContent);

            if (isTimerStarted) resetTimer = true;
        }
        private void RecognizedHandler(object sender, SpeechRecognitionEventArgs e)
        {
            // string TextToSpeechMessage = String.Empty;
            RecognitionContentStr = e.Result.Text;
            Log.d(LOG_TAG, "RecognizedHandler:" + RecognitionContentStr);
            RunOnMainThread(UpdateRecognitionContent);

            timerNeeded = true;
        }
        private void CanceledHandler(object sender, SpeechRecognitionCanceledEventArgs e)
        {
            RecognitionContentStr = e.ErrorDetails.ToString();
            Debug.Log(LOG_TAG + " : " + "CanceledHandler:" + RecognitionContentStr);
            RunOnMainThread(UpdateRecognitionContent);

            RecognizedResultStr = e.ErrorDetails.ToString(); //Log error on keyboard screen.
            RunOnMainThread(UpdatePanelInputField);

            timerNeeded = false;
        }

        #endregion

        #region Translation Event Handlers
        private void TranslationRecognizingHandler(object sender, TranslationRecognitionEventArgs e)
        {
            Debug.Log(LOG_TAG + " : " + "TranslationRecognizingHandler");
            // lock (RecognitionContentLock)
            //{
            RecognitionContentStr = "[From]" + e.Result.Text;
            foreach (var element in e.Result.Translations)
            {
                RecognitionContentStr += System.Environment.NewLine + "[To]" + element.Value;
            }
            Debug.Log(LOG_TAG + " : " + "TranslationRecognizingHandler:" + RecognitionContentStr);
            RunOnMainThread(UpdateRecognitionContent);

            if (isTimerStarted) resetTimer = true;
            //}
        }
        private void TranslationRecognizedHandler(object sender, TranslationRecognitionEventArgs e)
        {
            Debug.Log(LOG_TAG + " : " + "TranslationRecognizedHandler");
            string TextToSpeechMessage = String.Empty;
            // lock (RecognitionContentLock)
            //{
            if (e.Result.Reason == ResultReason.NoMatch)
            {
                Debug.Log(LOG_TAG + " : " + "[Translate]RecognizedHandler skipped due to empty message");
                return;
            }
            TextToSpeechMessage = "[From]" + e.Result.Text;
            string TranslateResultMessage = String.Empty;
            foreach (var element in e.Result.Translations)
            {
                TextToSpeechMessage += System.Environment.NewLine + "[To]" + element.Value;
                TranslateResultMessage = element.Value;
            }
            RecognitionContentStr = RecognizedResultStr = TextToSpeechMessage;
            RunOnMainThread(UpdateRecognitionContent);

            timerNeeded = true;
            //}

            VoiceCommandManager VM = VoiceCommandManager.Instance;
            if (VM.TranslationRecognizerComponent.RecognizedLanguage.Equals("en-US"))
            {
                //RunOnMainThreadTwoParams(NotifyPaneltoSynthesisResults, "zh-TW", TextToSpeechMessage);
                //RunOnMainThread(UpdatePanelInputField);
                // await SynthesizeVoiceAssistantSpeech(, );
                SynthesisRecognitionLanguage = "zh-TW";

            }
            else if (VM.TranslationRecognizerComponent.RecognizedLanguage.Equals("zh-TW"))
            {
                //RunOnMainThreadTwoParams(NotifyPaneltoSynthesisResults, "en-US", TextToSpeechMessage);
                //RunOnMainThread(UpdatePanelInputField);
                // await SynthesizeVoiceAssistantSpeech(TextToSpeechMessage);
                SynthesisRecognitionLanguage = "en-US";
            }
            SynthesisRecognitionMessage = TranslateResultMessage;
        }
        private void TranslationCanceledHandler(object sender,
                                                TranslationRecognitionCanceledEventArgs e)
        {
            // lock(RecognitionContentLock) {
            RecognitionContentStr = e.ErrorDetails.ToString();
            Debug.Log(LOG_TAG + " : " + "TranslationCanceledHandler:" + RecognitionContentStr);
            RunOnMainThread(UpdateRecognitionContent);

            RecognizedResultStr = e.ErrorDetails.ToString(); //Log error on keyboard screen.
            RunOnMainThread(UpdatePanelInputField);

            timerNeeded = false;
            //}
        }
        #endregion

        #region Intent Recognition Event Handlers
        private void IntentRecognizingHandler(object sender, IntentRecognitionEventArgs e)
        {
            RecognitionContentStr = e.Result.Text;
            Debug.Log(LOG_TAG + " : " + "IntentRecognizingHandler:" + RecognitionContentStr);
            IntentStr = EntityStr = "";
            Intent = "";
            IntentScore = 0f;
            EntityDic.Clear();
            //RunOnMainThread(UpdateRecognitionContent);
        }
        private void IntentRecognizedHandler(object sender, IntentRecognitionEventArgs e)
        {
            Debug.Log(LOG_TAG + " : " + "IntentRecognizedHandler e.Result.Reason is :" + e.Result.Reason);
            // string TextToSpeechMessage = String.Empty;
            if (e.Result.Reason == ResultReason.RecognizedIntent)
            {
                RecognitionContentStr = e.Result.Text;
                Log.d(LOG_TAG, "IntentRecognizedHandler(text):" + RecognitionContentStr);
                Log.d(LOG_TAG, "IntentRecognizedHandler(intent_id):" + e.Result.IntentId);

                if (RecognitionContentStr.Length > 0)
                {
                    var json = e.Result.Properties.GetProperty(
                        PropertyId.LanguageUnderstandingServiceResponse_JsonResult);
                    if (json.Length > 0)
                    {
                        var jsonRoot = JsonUtility.FromJson<JsonRoot>(json);
                        Intent = jsonRoot.topScoringIntent.intent;
                        IntentScore = jsonRoot.topScoringIntent.score;
                        IntentStr = Intent + ":" + IntentScore;
                        //Log.d(LOG_TAG, "IntentRecognizedHandler(IntentName):" + IntentName +
                        //				   "IntentRecognizedHandler(IntentScore):" + IntentScore);

                        var entities = jsonRoot.entities;
                        for (int i = 0; i < entities.Count; i++)
                        {
                            EntityDic.Add(entities[i].type, entities[i].entity);
                            EntityStr += entities[i].type + ":" + entities[i].entity;
                            EntityStr += "  ";
                        }
                    }
                }
                Log.d(LOG_TAG, "IntentRecognizedHandler(intent name):" + IntentStr);
                Log.d(LOG_TAG, "IntentRecognizedHandler(entity name):" + EntityStr);
            }
            else if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                RecognitionContentStr = e.Result.Text;
                Log.d(LOG_TAG, "IntentRecognizedHandler(text):" + RecognitionContentStr);
                Log.d(LOG_TAG, "Intent not recognized.");
            }
            else if (e.Result.Reason == ResultReason.NoMatch)
            {
                Log.d(LOG_TAG, "NOMATCH: Speech could not be recognized.");
            }
            var VM = VoiceCommandManager.Instance;
            //RunOnMainThread(UpdateRecognitionContent);
        }

        private void IntentCanceledHandler(object sender, IntentRecognitionCanceledEventArgs e)
        {
            var cancellation = CancellationDetails.FromResult(e.Result);
            Debug.Log(LOG_TAG + " : " + "IntentCanceledHandler(canceled reason):" + cancellation.Reason);
            if (cancellation.Reason == CancellationReason.Error)
            {
                IntentStr = cancellation.ErrorCode.ToString() + " " + cancellation.ErrorDetails.ToString();
                Debug.Log(LOG_TAG + " : " + "IntentCanceledHandler(canceled error):" + IntentStr);
                Debug.Log(LOG_TAG + " : " + "Intent canceled: Did you update the subscription info?");
            }
            //RunOnMainThread(UpdateRecognitionContent);

            RecognizedResultStr = cancellation.ErrorCode.ToString() + " " + cancellation.ErrorDetails.ToString(); //Log error on keyboard screen.
            RunOnMainThread(UpdatePanelInputField);
        }
        #endregion

        #region Passthrough
        private VIVE.OpenXR.Passthrough.XrPassthroughHTC activePassthroughID = 0;
        public void EnablePassthrough()
        {
            if (activePassthroughID == 0)
            {
                PassthroughAPI.CreatePlanarPassthrough(out activePassthroughID, LayerType.Overlay, OnDestroyPassthroughFeatureSession);
                PassthroughAPI.SetPassthroughLayerType(activePassthroughID, LayerType.Overlay);
            }
        }

        public void DisablePassthrough()
        {
            if (activePassthroughID != 0)
            {
                OnDestroyPassthroughFeatureSession(activePassthroughID);
            }
        }

        private void OnDestroyPassthroughFeatureSession(VIVE.OpenXR.Passthrough.XrPassthroughHTC passthroughID)
        {
            PassthroughAPI.DestroyPassthrough(passthroughID);
            activePassthroughID = 0;
        }
        #endregion
    }

    // For parsing intent recognition result json string
    [Serializable]
    public class JsonRoot
    {
        public string query;
        public TopScoringIntent topScoringIntent;
        public List<Entities> entities;
    }
    [Serializable]
    public class TopScoringIntent
    {
        public string intent;
        public double score;
    }
    [Serializable]
    public class Entities
    {
        public string entity;
        public string type;
        public int startIndex;
        public int endIndex;
        public double score;
        public Resolution resolution;
    }
    [Serializable]
    public class Resolution
    {
        public List<string> values;
    }
}