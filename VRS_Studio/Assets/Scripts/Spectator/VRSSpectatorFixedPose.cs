using System.Collections;
using UnityEngine;

namespace VRSStudio.Spectator
{
    public class VRSSpectatorFixedPose : MonoBehaviour
    {
        bool registered = false;
        IEnumerator Start()
        {
            var time = Time.time;
            while (VRSSpectatorManager.Instance == null)
            {
                yield return null;
                if (Time.time - time > 2)
                    yield break;
            }

            VRSSpectatorManager.Instance.RegisterFixedPose(this);
            registered = true;
        }

        private void OnDisable()
        {
            if (registered)
            {
                if (VRSSpectatorManager.Instance != null)
                {
                    VRSSpectatorManager.Instance.UnregisterFixedPose(this);
                }
            }
            registered = false;
        }
    }
}