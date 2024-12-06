using UnityEngine;
using VIVE.OpenXR.FacialTracking;

namespace HTC.FaceTracking.Interaction
{
    public class BubbleLauncherLip : MonoBehaviour
    {
        public static bool isLipBubbleLaunch = false;

        [SerializeField] private ParticleSystem bubbleParticle;
        [SerializeField] private bool launchInEditor = false;
        private ParticleSystem.EmissionModule emissionModule;
        private Transform cam;

        void Update()
        {
            if (cam == null) cam = Camera.main.transform;

            emissionModule = bubbleParticle.emission;
            bubbleParticle.transform.position = LipShapeUpdater.lipPosition;
            bubbleParticle.transform.rotation = Quaternion.LookRotation(cam.forward);

            bool enableToLaunch = LipShapeUpdater.CurrentLipShape == XrLipExpressionHTC.XR_LIP_EXPRESSION_MOUTH_POUT_HTC;
            if (enableToLaunch || launchInEditor)
            {
                emissionModule.enabled = true;
                isLipBubbleLaunch = true;
            }
            else
            {
                isLipBubbleLaunch = false;
                emissionModule.enabled = false;
            }
        }
    }
}