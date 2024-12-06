using System.Collections;
using UnityEngine;

namespace HTC.FaceTracking.Interaction
{
    public class RobotBehaviour : SimpleSingleton<RobotBehaviour>
    {
        [SerializeField] private Transform[] RobotEscapePoint;

        private bool ignoreHit = false;
        private Transform lastEscapePoint;
        private Coroutine escapeCoroutine = null;
        private Coroutine ignoreHitCoroutine = null;

        private void OnEnable()
        {
            //RobotAssistantManager.robotAssistantManagerInstance.robotAssistantSpeechBubble.TextBoardShowup(false);
        }

        private void OnDisable()
        {
            StartRobotEscape(false);
            ignoreHit = false;
            ignoreHitCoroutine = null;
        }

        public void StartRobotEscape(bool active)
        {
            if (ignoreHit && active)
            {
                if (ignoreHitCoroutine == null)
                    ignoreHitCoroutine = StartCoroutine(IgnoreHit());
                return;
            }
            if (escapeCoroutine != null)
            {
                StopCoroutine(escapeCoroutine);
            }
            /*if (active)
            {
                escapeCoroutine = StartCoroutine(RobotEscape());
            }*/
        }

        private IEnumerator IgnoreHit()
        {
            ignoreHit = true;
            yield return new WaitForSeconds(4);
            ignoreHitCoroutine = null;
            ignoreHit = false;
            yield return null;
        }

        /*private IEnumerator RobotEscape()
        {
            if (!ignoreHit)
            {
                RobotAssistantManager robotAssistantManager = null;
                if (RobotAssistantManager.robotAssistantManagerInstance != null)
                {
                    ignoreHit = true;
                    robotAssistantManager = RobotAssistantManager.robotAssistantManagerInstance;
                    robotAssistantManager.moveSpeed = 2;

                    Transform targetPoint = RobotEscapePoint[Random.Range(0, RobotEscapePoint.Length - 1)];

                    if (lastEscapePoint != null)
                    {
                        while (targetPoint == lastEscapePoint)
                        {
                            targetPoint = RobotEscapePoint[Random.Range(0, RobotEscapePoint.Length - 1)];
                        }
                    }

                    lastEscapePoint = targetPoint;

                    Vector3 targetPosition = targetPoint.position;

                    robotAssistantManager.TriggerReaction(RobotAssistantEnums.ReactionAnimationIndex.Clap);

                    yield return new WaitForSeconds(1f);
                    robotAssistantManager.ForceStopReaction();
                    robotAssistantManager.SetRobotPosition(targetPosition);

                    yield return new WaitForSeconds(2f);
                    robotAssistantManager.ForceStopReaction();
                    robotAssistantManager = RobotAssistantManager.robotAssistantManagerInstance;
                    robotAssistantManager.moveSpeed = 1;
                    robotAssistantManager.TriggerLeisure();
                    ignoreHit = false;
                }
            }
            yield return null;
        }*/
    }
}