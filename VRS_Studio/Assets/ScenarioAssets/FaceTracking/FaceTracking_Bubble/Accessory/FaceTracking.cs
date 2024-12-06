using System.Collections.Generic;
using UnityEngine;

namespace HTC.FaceTracking.Interaction
{
    public class FT
    {
        private static List<string> logs = new List<string>();

        public static void Log(string message, string tag = "")
        {
            string tagString = string.IsNullOrEmpty(tag) ? string.Empty : $"[{tag}]";
            string log = $"[FTI]{tagString} : {message}" + "\n";
            string fullLog = string.Empty;
            logs.Insert(0, log);
            Debug.Log(log);
            if (logs.Count > 10)
                logs.RemoveAt(logs.Count - 1);

            for (int i = 0; i < logs.Count; i++)
                fullLog += logs[i];

            if (BubbleManager.Instance.debugText)
                BubbleManager.Instance.debugText.text = fullLog;
        }
    }
}