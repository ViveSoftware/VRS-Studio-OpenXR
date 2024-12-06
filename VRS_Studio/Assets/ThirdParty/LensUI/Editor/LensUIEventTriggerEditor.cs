using HTC.UnityPlugin.Utility;
using UnityEditor;

namespace HTC.Triton.LensUI
{
    [CustomEditor(typeof(LensUIEventTrigger), true)]
    [CanEditMultipleObjects]
    public class LensUIEventTriggerEditor : EventTriggerEditorBase<LensUIEventTrigger.EventType>
    {
        protected override string EntriesProp { get { return "entries"; } }

        protected override string EntryEventProp { get { return "eventID"; } }

        protected override string EntryCallbackProp { get { return "callbacks"; } }

        protected override void OnTriggerAdded()
        {
            LensUIEventTrigger lensUIEventTrigger = target as LensUIEventTrigger;
            if (lensUIEventTrigger)
            {
                lensUIEventTrigger.SortEntries();
            }
        }
    }
}