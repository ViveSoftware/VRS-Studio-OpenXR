#pragma warning disable 0649
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.Utility.Switchable
{
    public class SwitchableEventTrigger : MonoBehaviour
        , ISwitchableInitializeHandler
        , ISwitchableSwitchedHandler
        , ISwitchableOpenStartHandler
        , ISwitchableOpenEndHandler
        , ISwitchableCloseStartHandler
        , ISwitchableCloseEndHandler
    {
        public enum EventType
        {
            OnInitialize,
            OnOpen,
            OnOpenStart,
            OnOpenEnd,
            OnOpenAnimateStart,
            OnOpenAnimateEnd,
            OnOpenSnap,
            OnClose,
            OnCloseStart,
            OnCloseEnd,
            OnCloseAnimateStart,
            OnCloseAnimateEnd,
            OnCloseSnap,
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
                    entries.Add
                    (
                        new Entry()
                        {
                            eventID = eventID,
                            callbacks = callbacks = new UnityEvent(),
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

        void ISwitchableInitializeHandler.OnSwitchableInitialized(Switchable switchable)
        {
            Execute(EventType.OnInitialize);
        }

        void ISwitchableSwitchedHandler.OnSwitchableSwitched(Switchable switchable, bool isOpened, bool snap)
        {
            if (isOpened)
            {
                Execute(EventType.OnOpen);
            }
            else
            {
                Execute(EventType.OnClose);
            }
        }

        void ISwitchableOpenStartHandler.OnSwitchableOpenStart(Switchable switchable, bool snap)
        {
            Execute(EventType.OnOpenStart);
            if (!snap) { Execute(EventType.OnOpenAnimateStart); }
        }

        void ISwitchableOpenEndHandler.OnSwitchableOpenEnd(Switchable switchable, bool snap)
        {
            Execute(EventType.OnOpenEnd);
            if (!snap) { Execute(EventType.OnOpenAnimateEnd); }
            else if (switchable.Status == SwitchableStatus.Opened) { Execute(EventType.OnOpenSnap); }
        }

        void ISwitchableCloseStartHandler.OnSwitchableCloseStart(Switchable switchable, bool snap)
        {
            Execute(EventType.OnCloseStart);
            if (!snap) { Execute(EventType.OnCloseAnimateStart); }
        }

        void ISwitchableCloseEndHandler.OnSwitchableCloseEnd(Switchable switchable, bool snap)
        {
            Execute(EventType.OnCloseEnd);
            if (!snap) { Execute(EventType.OnCloseAnimateEnd); }
            else if (switchable.Status == SwitchableStatus.Closed) { Execute(EventType.OnCloseSnap); }
        }
    }
}