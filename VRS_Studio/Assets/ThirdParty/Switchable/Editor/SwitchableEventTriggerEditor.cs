using UnityEditor;

namespace HTC.UnityPlugin.Utility.Switchable
{
    [CustomEditor(typeof(SwitchableEventTrigger), true)]
    [CanEditMultipleObjects]
    public class SwitchableEventTriggerEditor : EventTriggerEditorBase<SwitchableEventTrigger.EventType>
    {
        protected override string EntriesProp { get { return "entries"; } }

        protected override string EntryEventProp { get { return "eventID"; } }

        protected override string EntryCallbackProp { get { return "callbacks"; } }

        protected override void OnTriggerAdded()
        {
            SwitchableEventTrigger switchableEventTrigger = target as SwitchableEventTrigger;
            if (switchableEventTrigger != null)
            {
                switchableEventTrigger.SortEntries();
            }
        }
    }
}