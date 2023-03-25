using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XOR
{
    public enum EventBaseParameter
    {
        None,
    }
    [System.Serializable]
    public class EventBaseData
    {
        public UnityEngine.Object target;
        public string method;

        public EventBaseParameter parameter = EventBaseParameter.None;

        public string stringValue;
        public string floatValue;
        public string boolValue;
        public UnityEngine.Object objectValue;
    }

    public class EventBase
    {
        [SerializeField]
        private EventBaseData[] m_Events = new EventBaseData[0];

        public void Invoke()
        {
            if (m_Events == null || m_Events.Length == 0)
                return;
            foreach (var @event in this.m_Events)
            {
                if (@event == null || @event.target == null || !(@event.target is TsComponent component))
                    continue;
                component.InvokeMethod(@event.method);
            }
        }
        public int GetEventCount()
        {
            return m_Events != null ? m_Events.Length : 0;
        }
    }
    public class EventBase<T>
    {
        [SerializeField]
        private EventBaseData[] m_Events = new EventBaseData[0];

        public void Invoke(T value)
        {
            if (m_Events == null || m_Events.Length == 0)
                return;
            foreach (var @event in this.m_Events)
            {
                if (@event == null || @event.target == null || !(@event.target is TsComponent component))
                    continue;
                component.InvokeMethod(@event.method, value);
            }
        }
        public int GetEventCount()
        {
            return m_Events != null ? m_Events.Length : 0;
        }
    }
}
