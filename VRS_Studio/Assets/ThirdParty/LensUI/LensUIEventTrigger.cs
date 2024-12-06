#pragma warning disable 0649
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.Triton.LensUI
{
    public class LensUIEventTrigger : MonoBehaviour
        , ILensUISelectableStateTransitionHandler
    {
        public enum EventType
        {
            OnNormal = LensUISelectable.State.Normal,
            OnHighlighted = LensUISelectable.State.Highlighted,
            OnPressed = LensUISelectable.State.Pressed,
            OnSelected = LensUISelectable.State.Selected,
            OnDisabled = LensUISelectable.State.Disabled,
        }

        [Serializable]
        private class Entry
        {
            public EventType eventID;
            public UnityEvent callbacks;
        }

        [SerializeField]
        private List<Entry> entries;

        public void SortEntries()
        {
            entries.Sort((a, b) => { return a.eventID < b.eventID ? -1 : a.eventID > b.eventID ? 1 : 0; });
        }

        public void AddListner(EventType eventID, UnityAction listener)
        {
            var callbacks = (UnityEvent)null;
            if (entries == null)
            {
                entries = new List<Entry>()
                {
                    new Entry()
                    {
                        eventID = eventID,
                        callbacks = callbacks = new UnityEvent(),
                    }
                };
            }
            else
            {
                foreach (var entry in entries)
                {
                    if (entry.eventID == eventID)
                    {
                        if (entry.callbacks == null)
                        {
                            entry.callbacks = callbacks = new UnityEvent();
                        }
                        else
                        {
                            callbacks = entry.callbacks;
                        }
                        break;
                    }
                }

                if (callbacks == null)
                {
                    callbacks = new UnityEvent();
                    entries.Add
                    (
                        new Entry()
                        {
                            eventID = eventID,
                            callbacks = callbacks,
                        }
                    );
                }
            }

            if (callbacks != null)
            {
                callbacks.AddListener(listener);
            }
        }

        public void RemoveListner(EventType eventID, UnityAction listener)
        {
            if (entries == null) { return; }

            foreach (var entry in entries)
            {
                if (entry.eventID == eventID)
                {
                    if (entry.callbacks != null)
                    {
                        entry.callbacks.RemoveListener(listener);
                    }
                    return;
                }
            }
        }

        private void Execute(EventType eventID)
        {
            if (entries == null) { return; }

            foreach (var entry in entries)
            {
                if (entry.eventID == eventID)
                {
                    if (entry.callbacks != null)
                    {
                        entry.callbacks.Invoke();
                    }
                    return;
                }
            }
        }

        void ILensUISelectableStateTransitionHandler.OnLensUISelectableStateTransition(LensUISelectable selectable, LensUISelectable.State state, bool snap)
        {
            Execute((EventType)state);
        }
    }
}