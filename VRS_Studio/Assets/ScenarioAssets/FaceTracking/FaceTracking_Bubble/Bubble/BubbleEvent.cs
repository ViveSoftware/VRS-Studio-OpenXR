using System.Collections.Generic;
using UnityEngine;

namespace HTC.FaceTracking.Interaction
{
    public class BubbleEvent : MonoBehaviour
    {
        public static Vector3 lastEnterPosition = Vector3.zero;

        [HideInInspector] public float maxScale = 2;
        [HideInInspector] public ParticleSystem.MinMaxCurve scaleCurve;

        private int numberOfParticles;
        private ParticleSystem bubbleParticle;
        private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        [SerializeField] private float triggerLifeRemainTime = 999;

        private void OnEnable()
        {
            if (bubbleParticle == null) bubbleParticle = gameObject.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule mainModule = bubbleParticle.main;
            mainModule.startColor = BubbleManager.Instance.normalColor;
        }

        private void LateUpdate()
        {
            OnDeadSFX();
        }

        void OnParticleCollision(GameObject other)
        {
            int numCollisionEvents = bubbleParticle.GetCollisionEvents(other, collisionEvents);

            if (other.transform.name.Equals("RobotCollider"))
            {
                RobotBehaviour.Instance.StartRobotEscape(true);
            }
        }

        private void OnDeadSFX()
        {
            var count = bubbleParticle.particleCount;
            if (count < numberOfParticles)
            {
                BubbleSoundController.Instance.PlayBurstSound(Vector3.zero);
            }
            numberOfParticles = count;
        }

        private void OnParticleTrigger()
        {
            OnTriggerParticlesEnter();
            OnTriggerParticlesInside();
            OnTriggerParticlesOutside();
        }

        private void OnTriggerParticlesInside()
        {
            List<ParticleSystem.Particle> insideUnit = new List<ParticleSystem.Particle>();
            int Count = bubbleParticle.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, insideUnit);
            if (Count == 0) lastEnterPosition = Vector3.zero;
            for (int i = 0; i < Count; i++)
            {
                ParticleSystem.Particle p = insideUnit[i];
                if (p.startSize < maxScale && triggerLifeRemainTime > p.remainingLifetime)
                {
                    if (p.startColor == BubbleManager.Instance.normalColor && triggerLifeRemainTime > p.remainingLifetime)
                    {
                        p.startColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
                    }
                    float scale = scaleCurve.Evaluate(p.remainingLifetime / p.startLifetime);
                    p.startSize += scale;
                }
                else if (p.startSize > maxScale)
                {
                    lastEnterPosition = p.position;
                    p.remainingLifetime = 0;
                    BubbleSoundController.Instance.PlayBurstSound(p.position);
                }
                insideUnit[i] = p;
            }
            bubbleParticle.SetTriggerParticles(ParticleSystemTriggerEventType.Inside, insideUnit);
        }

        private void OnTriggerParticlesEnter()
        {
            List<ParticleSystem.Particle> enterUnit = new List<ParticleSystem.Particle>();
            int Count = bubbleParticle.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterUnit);
            for (int i = 0; i < Count; i++)
            {
                ParticleSystem.Particle p = enterUnit[i];
                if (p.startColor == BubbleManager.Instance.normalColor && triggerLifeRemainTime > p.remainingLifetime)
                {
                    p.startColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
                }
                enterUnit[i] = p;
            }
            bubbleParticle.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterUnit);
        }

        private void OnTriggerParticlesOutside()
        {
            if (!BubbleManager.Instance.isParticleTriggerable) return;
            List<ParticleSystem.Particle> outsideUnit = new List<ParticleSystem.Particle>();
            int Count = bubbleParticle.GetTriggerParticles(ParticleSystemTriggerEventType.Outside, outsideUnit);
            for (int i = 0; i < Count; i++)
            {
                ParticleSystem.Particle p = outsideUnit[i];
                float rs = p.remainingLifetime / p.startLifetime;

                if (p.startSize > 0.20f && p.startColor != BubbleManager.Instance.normalColor)
                {
                    float scale = scaleCurve.Evaluate((1 - rs)) * 2;
                    p.startSize -= scale;
                }
                else
                {
                    p.startColor = BubbleManager.Instance.normalColor;
                }
                outsideUnit[i] = p;
            }
            bubbleParticle.SetTriggerParticles(ParticleSystemTriggerEventType.Outside, outsideUnit);
        }
    }
}