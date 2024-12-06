using System.Collections;
using UnityEngine;

namespace HTC.FaceTracking.Interaction
{
    public class BubbleSoundController : SimpleSingleton<BubbleSoundController>
    {
        [SerializeField] private float volumeLaunch;
        [SerializeField] private AudioSource audioSourceLaunch;

        [SerializeField] private float volumeBurstBackground;
        [SerializeField] private AudioSource[] audioSourceBurstBackgroundSetup;

        [SerializeField] private float volumeBurstFocus;
        [SerializeField] private AudioSource[] audioSourceBurstFocusSetup;

        [SerializeField] private float pinchMax = 1.5f;
        [SerializeField] private float pinchStep = 0.1f;
        private bool comboStarted = false;
        private bool comboSoundEffect = false;
        private int comboCount = 0;

        private void Start()
        {
            StartCoroutine(CheckComboLoop());
            StartCoroutine(UpdateComboEffect());
        }

        private void Update()
        {
            if (BubbleLauncher.isLeftHandBubbleLaunch || BubbleLauncher.isRightHandBubbleLaunch || BubbleLauncherLip.isLipBubbleLaunch)
            {
                PlayLaunchSound();
            }
            else
            {
                StopLaunchSound();
            }
        }

        IEnumerator CheckComboLoop()
        {
            while (true)
            {
                if (comboStarted)
                {
                    comboStarted = false;
                    comboSoundEffect = true;
                }
                else
                {
                    comboSoundEffect = false;
                    comboCount = 0;
                }
                yield return new WaitForSeconds(1);
            }
        }

        IEnumerator UpdateComboEffect()
        {
            while (true)
            {
                foreach (AudioSource audioSource in audioSourceBurstFocusSetup)
                {
                    if (comboSoundEffect)
                    {
                        if (audioSource.pitch <= pinchMax)
                        {
                            audioSource.pitch = 1 + (comboCount * pinchStep);
                        }
                    }
                    else
                    {
                        audioSource.pitch = 1;
                    }
                }
                yield return null;
            }
        }

        public void PlayLaunchSound()
        {
            if (!audioSourceLaunch.isPlaying) audioSourceLaunch.Play();
        }

        public void StopLaunchSound()
        {
            if (audioSourceLaunch.isPlaying) audioSourceLaunch.Stop();
        }

        public void PlayBurstSound(Vector3 pos)
        {
            AudioSource[] sources = audioSourceBurstFocusSetup;
            float volume = volumeBurstFocus;

            if (pos == Vector3.zero)
            {
                sources = audioSourceBurstBackgroundSetup;
                volume = volumeBurstBackground;
            }
            else
            {
                comboStarted = true;
                comboCount++;
            }

            foreach (AudioSource audioSource in sources)
            {
                if (!audioSource.isPlaying)
                {
                    if (pos != Vector3.zero) audioSource.transform.position = pos;
                    audioSource.volume = volume;
                    audioSource.Play();
                    break;
                }
            }
        }
    }
}