using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VRSStudio.VoiceCommand
{
    /// <summary>Class <c>IntentManager</c> Manage Intents/Entities and Event invoke.
    /// Keep Intents/Entities up-to-date with CLU backend project VRSStudioVoiceCommandManager.
    /// Register thoses events when you need to listen the event invoke</summary>
    ///
    public class IntentManager : MonoBehaviour
    {
        public UnityEvent RecognizeEnglish = new UnityEvent();
        public UnityEvent RecognizeChinese = new UnityEvent();
        public UnityEvent TranslateEnglish = new UnityEvent();
        public UnityEvent TranslateChinese = new UnityEvent();
        public UnityEvent StopVoiceCommand = new UnityEvent();
        public UnityEvent ExitAppEvent = new UnityEvent();
        public UnityEvent TurnOnPassthroughEvent = new UnityEvent();
        public UnityEvent TurnOffPassthroughEvent = new UnityEvent();
        public UnityEvent WakeUpEvent = new UnityEvent();
        public UnityEvent UnknownIntent = new UnityEvent();

        private static IntentManager m_Instance = null;
        public static IntentManager Instance
        {
            get { return m_Instance; }
        }

        private static string LOG_TAG = "IntentManager";

        public enum IntentType
        {
            AdjustVolume,                  //0
            ExitApp,
            LaunchApp,
            ManageHandTracking,
            ManagePassthrough,
            MediaControl,                  //5
            PowerOff,
            SwitchVoiceCommand,
            TurnOn,
            TurnOff,
            VisitLibrary,                  //10
            VisitProfile,
            VisitSettings,
            VisitStore,
            WakeUp,
            WeatherCheckWeatherTime,       //15
            WeatherCheckWeatherValue,
            WeatherGetWeatherAdvisory,
            WeatherQueryWeather,
            WeatherChangeTemperatureUnit,
            WebWebSearch                   //20
        }
        private static List<string> IntentStrs = new List<string>
        {
            "AdjustVolume",                //0
			"ExitApp",
            "LaunchApp",
            "ManageHandTracking",
            "ManagePassthrough",
            "MediaControl",                //5
			"PowerOff",
                        "SwitchVoiceCommand",
            "TurnOn",
            "TurnOff",
            "VisitLibrary",                //10
			"VisitProfile",
            "VisitSettings",
            "VisitStore",
            "WakeUp",
                        "Weather.CheckWeatherTime",    //15
			"Weather.CheckWeatherValue",
            "Weather.GetWeatherAdvisory",
            "Weather.QueryWeather",
            "Weather.ChangeTemperatureUnit",
            "Web.WebSearch"                 //20
		};
        public static string GetIntentStr(IntentType Type) { return IntentStrs[(int)Type]; }
        public enum EntityType
        {
            App,                  //0
            Device,
            Language,
            Passthrough,
            VoiceCommand
        }
        private static List<string> EntityStrs = new List<string>
                {
                        "App",                //0
			"Device",
                        "Language",
                        "Passthrough",
                        "VoiceCommand"
                };
        public static string GetEntityStr(EntityType Type) { return EntityStrs[(int)Type]; }

        void Awake() { m_Instance = this; }

        /// <summary>
        /// Invoke the events depends on the Intent and entities. It is better to handle words which start with capital letter in this function.
        /// </summary>
        /// <param name="EntityDic">The category and text of the entities derived from recognition result.</param>
        /// <param name="Intent">The Intent derived from recognition result.</param>
        /// <param name="IntentScore">The confidence score of the intent derived from recognition result.</param>
        /// <returns>
        /// Return True when the intent and entities are handled well and will invoke a known Intent event.
        /// Return False when the intent and entities are not handled well and will invoke UnknownIntent event.
        /// </returns>
        public bool IntentInvoking(Dictionary<string, string> EntityDic, string Intent,
                                 double IntentScore)
        {
            Debug.Log(LOG_TAG + " : " + "IntentInvoking" + "[Intent(" + Intent + "),Score(" + IntentScore + ")]");
            string EntityDicStr = "EntityDicStr:";
            foreach (KeyValuePair<string, string> entity in EntityDic)
            {
                EntityDicStr += "[" + entity.Key + ":" + entity.Value + "]";
            }
            Debug.Log(LOG_TAG + " : " + EntityDicStr);

            // Handle intent action
            // Passthrough
            if (EntityDic.ContainsKey(GetEntityStr(EntityType.Passthrough)))
            {
                if (Intent == GetIntentStr(IntentType.TurnOn))
                {
                    Debug.Log(LOG_TAG + " : " + "TurnOnPassthrough");
                    TurnOnPassthroughEvent.Invoke();
                    return true;
                }
                else if (Intent == GetIntentStr(IntentType.TurnOff))
                {
                    Debug.Log(LOG_TAG + " : " + "TurnOffPassthrough");
                    TurnOffPassthroughEvent.Invoke();
                    return true;
                }
                UnknownIntent.Invoke();
                return false;
            }

            switch (Intent)
            {
                case "WakeUp":
                    if (IntentScore >= 0.9)
                    {
                        Debug.Log(LOG_TAG + " : " + "Invoke WakeUp");
                        WakeUpEvent.Invoke();
                        return true;
                    }
                    break;
                case "SwitchVoiceCommand":
                    if (EntityDic.ContainsKey(GetEntityStr(EntityType.VoiceCommand)) && EntityDic.ContainsKey(GetEntityStr(EntityType.Language)))
                    {
                        Debug.Log(LOG_TAG + " : " + "EntityDic.ContainsKey VoiceCommand and Language");
                        switch (EntityDic[GetEntityStr(EntityType.VoiceCommand)].ToLower()) //might be Recognize/recognize/Translate/translate.
                        {
                            case "recognize":
                                if (EntityDic[GetEntityStr(EntityType.Language)] == "English")
                                {
                                    Debug.Log(LOG_TAG + " : " + "Invoke RecognizeEnglish");
                                    RecognizeEnglish.Invoke();
                                    return true;
                                }
                                else if (EntityDic[GetEntityStr(EntityType.Language)] == "Chinese")
                                {
                                    Debug.Log(LOG_TAG + " : " + "Invoke RecognizeChinese");
                                    RecognizeChinese.Invoke();
                                    return true;
                                }
                                break;
                            case "translate":
                                if (EntityDic[GetEntityStr(EntityType.Language)] == "English")
                                {
                                    Debug.Log(LOG_TAG + " : " + "Invoke TranslateEnglish");
                                    TranslateEnglish.Invoke();
                                    return true;
                                }
                                else if (EntityDic[GetEntityStr(EntityType.Language)] == "Chinese")
                                {
                                    Debug.Log(LOG_TAG + " : " + "Invoke TranslateChinese");
                                    TranslateChinese.Invoke();
                                    return true;
                                }
                                break;
                            default:
                                Debug.Log(LOG_TAG + " : " + "Unknown VoiceCommand");
                                break;
                        }
                    }
                    else
                    {
                        Debug.Log(LOG_TAG + " : " + "SwitchVoiceCommand but does not ContainsKey VoiceCommand and Language at the same time");
                    }
                    break;
                case "TurnOn":
                    break;
                case "TurnOff":
                    if (EntityDic.ContainsKey(GetEntityStr(EntityType.VoiceCommand)))
                    {
                        Debug.Log(LOG_TAG + " : " + "Invoke StopVoiceCommand");
                        StopVoiceCommand.Invoke();
                        return true;
                    }
                    break;
                case "ExitApp":
                    Debug.Log(LOG_TAG + " : " + "Invoke ExitApp");
                    ExitAppEvent.Invoke();
                    return true;
                default:
                    break;
            }
            Debug.Log(LOG_TAG + " : " + "Invoke UnknownIntent.");
            UnknownIntent.Invoke();
            return false;
        }
    }
}