using HTC.UnityPlugin.Utility;
using UnityEngine;

namespace vrsstudio.SceneCapability
{
    public class SceneCapabilityProperties : MonoBehaviour
    {
        [SerializeField]
        [FlagsFromEnum(typeof(SceneCapabilityTypes))]
        private uint supportedInteractionModes = 1u << (int)SceneCapabilityTypes.Hand;

        public uint SupportedInteractionModes { get { return supportedInteractionModes; } private set { supportedInteractionModes = value; } }

        private void Start()
        {
            SceneCapabilityManager.Instance.NotifySceneCapabilityChange(this);
        }
    }

    public enum SceneCapabilityTypes
    {
        None = -1,
        Controller = 1,
        Hand = 2,
    }
}