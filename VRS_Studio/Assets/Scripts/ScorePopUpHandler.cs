using UnityEngine;
using static RobotAssistantAudioSource.SpeakToken;

public class ScorePopUpHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject scoreTextGO;
    [SerializeField]
    private AudioClip clip_cheer;
    private bool allowSpeak = true;

    private void OnTriggerEnter(Collider collider)
    {
        if (allowSpeak)
        {
            scoreTextGO.SetActive(false);
            scoreTextGO.SetActive(true);
            if (RobotAssistantManager.Instance)
                RobotAssistantManager.Instance.robotAssistantAudioSource.Speak(clip_cheer, false, null, OnAudioComplete);
            allowSpeak = false;
        }
    }

    private void OnAudioComplete(object sender, SpeakEventArgs e)
    {
        allowSpeak = true;
    }
}
