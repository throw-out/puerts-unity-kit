using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace XOR.UI
{
    [AddComponentMenu("UI/[★]Wrapper/EventTrigger")]
    public class EventTriggerWrapper : UnityEngine.EventSystems.EventTrigger
    {
        [SerializeField]
        [FormerlySerializedAs("delegatesWrapper")]
        private List<EntryWrapper> m_DelegatesWrapper = new List<EntryWrapper>();

        protected virtual void Awake()
        {
            RegisterEntries(this, this.m_DelegatesWrapper);
        }
        public virtual int GetEventCount()
        {
            int count = 0;
            if (m_DelegatesWrapper != null)
            {
                foreach (var entry in m_DelegatesWrapper)
                {
                    if (entry == null || entry.callback == null)
                        continue;
                    count += entry.callback.GetEventCount();
                }
            }
            return count;
        }

        static void RegisterEntries(UnityEngine.EventSystems.EventTrigger eventTrigger, IEnumerable<EntryWrapper> entries)
        {
            if (entries == null)
                return;
            foreach (var entry in entries)
            {
                eventTrigger.triggers.Add(CreateEntry(entry.eventID, entry.callback.Invoke));
            }
        }
        static Entry CreateEntry(EventTriggerType eventID, UnityAction<BaseEventData> callback)
        {
            var entry = new Entry();
            entry.eventID = eventID;
            entry.callback.AddListener(callback);
            return entry;
        }

        [System.Serializable]
        protected class EntryWrapper
        {
            public EventTriggerType eventID = EventTriggerType.PointerClick;

            public TriggerWrapperEvent callback = new TriggerWrapperEvent();
        }
        [System.Serializable]
        protected class TriggerWrapperEvent : XOR.EventBase<BaseEventData>
        {
        }
    }
}