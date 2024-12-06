using HTC.UnityPlugin.Vive;
using HTC.UnityPlugin.VRModuleManagement;
using UnityEngine;
using UnityEngine.UI;

namespace vrsstudio.SceneCapability
{
    public class SceneCapabilityManager : MonoBehaviour
    {
        public static SceneCapabilityManager Instance = null;

        public GameObject notificationGO;
        public Text notificationTextField;

        private SceneCapabilityTypes currentRightHandInteractionMode = SceneCapabilityTypes.None;
        private SceneCapabilityTypes currentLeftHandInteractionMode = SceneCapabilityTypes.None;

        private SceneCapabilityTypes activeInteractionMode = SceneCapabilityTypes.None;
        private uint currentSceneInteractionMode = 0u;

        private ViveRoleProperty rightHandRole = ViveRoleProperty.New(HandRole.RightHand);
        private ViveRoleProperty leftHandRole = ViveRoleProperty.New(HandRole.LeftHand);

        private const string notificationTextTemplate = "Supported interaction modes:\n";
        private string LOG_TAG = "SceneCapabilityManager";


        private void Awake()
        {
            Instance = this;
            CapabilityCheck();
        }

        private void Update()
        {
            IVRModuleDeviceState rightHandDeviceState = VRModule.GetCurrentDeviceState(rightHandRole.GetDeviceIndex());
            IVRModuleDeviceState leftHandDeviceState = VRModule.GetCurrentDeviceState(leftHandRole.GetDeviceIndex());

            //Log.d(LOG_TAG, "rightHandDevice Class: " + rightHandDeviceState.deviceClass + " , PoseValid" + rightHandDeviceState.isPoseValid + " , Connected: " + rightHandDeviceState.isConnected);
            //Log.d(LOG_TAG, "leftHandDevice Class: " + leftHandDeviceState.deviceClass + " , PoseValid" + leftHandDeviceState.isPoseValid + " , Connected: " + leftHandDeviceState.isConnected);


            if (rightHandDeviceState.deviceClass == VRModuleDeviceClass.Controller)
            {
                if (!rightHandDeviceState.isPoseValid) //Controller Connected but is idle, consider case as hand lost tracking
                {
                    currentRightHandInteractionMode = SceneCapabilityTypes.Hand;
                }
                else
                {
                    currentRightHandInteractionMode = SceneCapabilityTypes.Controller;
                }
            }
            else if (rightHandDeviceState.deviceClass == VRModuleDeviceClass.TrackedHand)
            {
                currentRightHandInteractionMode = SceneCapabilityTypes.Hand;
            }
            else
            {
                currentRightHandInteractionMode = SceneCapabilityTypes.None;
            }

            if (leftHandDeviceState.deviceClass == VRModuleDeviceClass.Controller)
            {
                if (!leftHandDeviceState.isPoseValid) //Controller Connected but is idle, consider case as hand lost tracking
                {
                    currentLeftHandInteractionMode = SceneCapabilityTypes.Hand;
                }
                else
                {
                    currentLeftHandInteractionMode = SceneCapabilityTypes.Controller;
                }
            }
            else if (leftHandDeviceState.deviceClass == VRModuleDeviceClass.TrackedHand)
            {
                currentLeftHandInteractionMode = SceneCapabilityTypes.Hand;
            }
            else
            {
                currentRightHandInteractionMode = SceneCapabilityTypes.None;
            }


            SceneCapabilityTypes currentActiveInteractionMode = SceneCapabilityTypes.None;
            if (currentRightHandInteractionMode == SceneCapabilityTypes.Hand || currentLeftHandInteractionMode == SceneCapabilityTypes.Hand)
            {
                currentActiveInteractionMode = SceneCapabilityTypes.Hand;
            }
            else if (currentRightHandInteractionMode == SceneCapabilityTypes.Controller || currentLeftHandInteractionMode == SceneCapabilityTypes.Controller)
            {
                currentActiveInteractionMode = SceneCapabilityTypes.Controller;
            }
            else
            {
                currentActiveInteractionMode = SceneCapabilityTypes.None;
            }

            if (currentActiveInteractionMode != activeInteractionMode)
            {
                activeInteractionMode = currentActiveInteractionMode;
                Log.d(LOG_TAG, "activeInteractionMode: " + activeInteractionMode.ToString());
                CapabilityCheck();
            }
        }

        public void NotifySceneCapabilityChange(SceneCapabilityProperties inProps)
        {
            currentSceneInteractionMode = inProps.SupportedInteractionModes;
            Log.d(LOG_TAG, "currentSceneInteractionMode: " + currentSceneInteractionMode.ToString());
            CapabilityCheck();
        }

        private void CapabilityCheck()
        {
            if (currentSceneInteractionMode == 0u)
            {
                Log.d(LOG_TAG, "CapabilityCheck: No specified capabilities.");
                UpdateNotificationState(false);
                return;
            }

            if ((currentSceneInteractionMode & (1u << (int)activeInteractionMode)) == 0) //Check if current active interaction mode is supported in the current content scene
            {
                //Not supported
                //Update text content
                notificationTextField.text = notificationTextTemplate;

                if ((currentSceneInteractionMode & (1u << (int)SceneCapabilityTypes.Hand)) != 0)
                {
                    notificationTextField.text += "Hands\n";
                }
                if ((currentSceneInteractionMode & (1u << (int)SceneCapabilityTypes.Controller)) != 0)
                {
                    notificationTextField.text += "Controllers\n";
                }

                //Show notification
                UpdateNotificationState(true);
            }
            else
            {
                UpdateNotificationState(false);
            }
        }

        private void UpdateNotificationState(bool enable)
        {
            notificationGO.SetActive(enable);
        }
    }
}