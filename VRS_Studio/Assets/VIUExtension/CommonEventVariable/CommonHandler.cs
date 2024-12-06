using System;
using UnityEngine;

namespace HTC.UnityPlugin.CommonEventVariable
{
    public abstract class CommonHandler
    {
        public static bool EnableAllDebugMessage { get; set; }
        public static bool EnableAllEventDebugMessage { get; set; }
        public static bool EnableAllVariableDebugMessage { get; set; }
        public static bool VariableDebugMessageOnlyOnChange { get; set; }
        public static event Action<string> OnDebugMessage;

        static CommonHandler()
        {
            EnableAllDebugMessage = false;
            EnableAllEventDebugMessage = false;
            EnableAllVariableDebugMessage = false;
            VariableDebugMessageOnlyOnChange = true;

            OnDebugMessage += Debug.Log;
        }

        public bool EnableDebugMessage { get; set; }
        public string Name { get; protected set; }
        protected void SendDebugMessage(string msg) { OnDebugMessage.Invoke(msg); }
    }
}
