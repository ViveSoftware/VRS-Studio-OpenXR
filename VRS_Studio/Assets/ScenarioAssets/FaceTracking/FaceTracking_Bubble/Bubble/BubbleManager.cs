using System.Collections;
using TMPro;
using UnityEngine;

namespace HTC.FaceTracking.Interaction
{
    [System.Serializable]
    public class BubbleParticleSetup
    {
        public ParticleSystem mainParticle;
        public BubbleEvent bubbleEvent;
    }

    public class BubbleManager : SimpleSingleton<BubbleManager>
    {
        public GameObject root;
        public GameObject textHint;
        public TextMeshProUGUI debugText;
        public BubbleParticleSetup rightHandBubbleSetup;
        public BubbleParticleSetup leftHandBubbleSetup;
        public BubbleParticleSetup lipBubbleSetup;
        public BubbleParticleSetup bubbleFallSetup;

        [Header("Scale Effect Parameter")]
        [SerializeField] private Component fovCollider;
        [SerializeField] private float maxScale;
        [SerializeField] private ParticleSystem.MinMaxCurve scaleCurve;
        public Color normalColor;
        public Color highlightedColor;
        public bool isParticleTriggerable
        {
            get
            {
                return isRightHandBubbleTriggerSetupDone && isLeftHandBubbleTriggerSetupDone;
            }
        }
        private bool isRightHandBubbleTriggerSetupDone = false;
        private bool isLeftHandBubbleTriggerSetupDone = false;
        private bool isLipBubbleTriggerSetupDone = false;

        private float fiveSecTimer = 5f;

        private void Awake()
        {
            StartCoroutine(FindMeshCollider());
        }

        public void AddHandBubbleColliderEvent(bool isLeftHand)
        {
            if (isLeftHand)
            {
                if (isLeftHandBubbleTriggerSetupDone) return;
                isLeftHandBubbleTriggerSetupDone = true;
                leftHandBubbleSetup.mainParticle.trigger.SetCollider(0, fovCollider);
            }
            else
            {
                if (isRightHandBubbleTriggerSetupDone) return;
                isRightHandBubbleTriggerSetupDone = true;
                rightHandBubbleSetup.mainParticle.trigger.SetCollider(0, fovCollider);

            }
        }

        public void RemoveHandBubbleColliderEvent(bool isLeftHand)
        {
            if (isLeftHand)
            {
                if (!isLeftHandBubbleTriggerSetupDone) return;
                isLeftHandBubbleTriggerSetupDone = false;
                leftHandBubbleSetup.mainParticle.trigger.SetCollider(0, null);

            }
            else
            {
                if (!isRightHandBubbleTriggerSetupDone) return;
                isRightHandBubbleTriggerSetupDone = false;
                rightHandBubbleSetup.mainParticle.trigger.SetCollider(0, null);
            }
        }

        public void AddLipBubbleColliderEvent()
        {
            if (isLipBubbleTriggerSetupDone) return;
            isLipBubbleTriggerSetupDone = true;
            lipBubbleSetup.mainParticle.trigger.SetCollider(0, fovCollider);
        }

        public void RemoveLipBubbleColliderEvent()
        {
            if (!isLipBubbleTriggerSetupDone) return;
            isLipBubbleTriggerSetupDone = false;
            lipBubbleSetup.mainParticle.trigger.SetCollider(0, null);
        }

        private void OnEnable()
        {
            UpdateParticleSettings();
        }

        private void UpdateParticleSettings()
        {
            rightHandBubbleSetup.bubbleEvent.scaleCurve = scaleCurve;
            leftHandBubbleSetup.bubbleEvent.scaleCurve = scaleCurve;
            bubbleFallSetup.bubbleEvent.scaleCurve = scaleCurve;
            lipBubbleSetup.bubbleEvent.scaleCurve = scaleCurve;

            rightHandBubbleSetup.bubbleEvent.maxScale = maxScale;
            leftHandBubbleSetup.bubbleEvent.maxScale = maxScale;
            bubbleFallSetup.bubbleEvent.maxScale = maxScale;
            lipBubbleSetup.bubbleEvent.maxScale = maxScale;
        }

        private void Update()
        {
#if UNITY_EDITOR
            UpdateParticleSettings();
#endif
            var isValid = default(bool);

            if (EyeManager.Instance != null && !root.activeSelf)
            {
                isValid = EyeManager.Instance.IsTracked();

                if (root.activeSelf != isValid) root.SetActive(isValid);

                if (fiveSecTimer > 0f)
                {
                    fiveSecTimer -= Time.deltaTime;
                }
                else
                {
                    fiveSecTimer = 5f;
                    textHint.SetActive(!isValid);
                }
            }
        }

        IEnumerator FindMeshCollider()
        {
            yield return new WaitUntil(() => GetMeshCollider());
            bubbleFallSetup.mainParticle.trigger.SetCollider(0, fovCollider);
            leftHandBubbleSetup.mainParticle.trigger.SetCollider(0, fovCollider);
            rightHandBubbleSetup.mainParticle.trigger.SetCollider(0, fovCollider);
            lipBubbleSetup.mainParticle.trigger.SetCollider(0, fovCollider);
        }

        private bool GetMeshCollider()
        {
            var found = GameObject.Find("EyeTrackingCollider");

            if (found != null)
            {
                fovCollider = found.transform;
                return true;
            }

            return false;
        }
    }
}
