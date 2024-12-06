using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace VRSStudio.VoiceCommand.Typing
{
    public class VoiceAssistantActivator : MonoBehaviour
    {
        private static string LOG_TAG = "VoiceAssistantActivator";

        [SerializeField] private GameObject VoiceCommandPanel;
        [SerializeField] private Image ButtonImage_VC;
        [SerializeField] private AudioClip AudioClip_Hint;

        private bool bNeedButtonBlink = false;
        private const float BlinkPeriod = 0.25f;

        private const string Hint_Str = "Feel free to input words. Also, do you notice the blinking button on the bottom? Try pressing it to wake up the voice assistant.";

        private void Start()
        {
            Debug.Log(LOG_TAG + " : " + "Start");
            if (ButtonImage_VC == null)
            {
                Debug.LogError(LOG_TAG + " : " + "ButtonImage_VC property is null! Assign a ButtonImage to it.");
            }
            else if (VoiceCommandPanel == null)
            {
                Debug.LogError(LOG_TAG + " : " + "VoiceCommandPanel property is null! Assign a VoiceCommandPanel to it.");
            }
            else if (AudioClip_Hint == null)
            {
                Debug.LogError(LOG_TAG + " : " + "AudioClips_Hint property is null! Assign a AudioClip to it.");
            }
        }

        public void ButtonClick()
        {
            Debug.Log(LOG_TAG + " : " + "ButtonClick");
            if (!VoiceCommandManager.CheckMicrophoneRecordAudioPermission()) { return; }

            if (VoiceCommandPanel.activeSelf)
            {
                VoiceCommandPanel.SetActive(false);
            }
            else
            {
                VoiceCommandPanel.SetActive(true);
            }

            bNeedButtonBlink = false;
            UpdateButtonImage();
        }

        private void UpdateButtonImage()
        {
            ButtonImage_VC.color = (VoiceCommandPanel.activeSelf) ? new Color32(255, 255, 0, 255) : new Color32(0, 0, 0, 255); //Yellow, Black
        }

        void BlinkButton()
        {
            StartCoroutine("SwitchColor");
        }

        IEnumerator SwitchColor()
        {
            bNeedButtonBlink = true;

            while (bNeedButtonBlink)
            {
                ButtonImage_VC.color = new Color32(255, 255, 0, 255); //Yellow
                yield return new WaitForSeconds(BlinkPeriod);
                ButtonImage_VC.color = new Color32(0, 0, 0, 255); //Black
                yield return new WaitForSeconds(BlinkPeriod);
            }
            UpdateButtonImage();
        }

        void Awake()
        {
            Debug.Log(LOG_TAG + " : " + "Awake");
        }

        void OnEnable()
        {
            Debug.Log(LOG_TAG + " : " + "OnEnable");
            if (RobotAssistantManager.Instance != null)
            {
                RobotAssistantManager.Instance.robotAssistantAudioSource.ForceStopAudioSource();
                RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(AudioClip_Hint, true, null,
                    (object sender, RobotAssistantAudioSource.SpeakToken.SpeakEventArgs e) =>
                    {
                        bNeedButtonBlink = false;
                    });
            }
            BlinkButton();
        }
    }
}