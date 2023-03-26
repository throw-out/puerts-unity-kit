using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XOR.Events
{
    public enum EventBaseParameter
    {
        None,
        String = 1,
        Double = 2,
        Bool = 4,
        Object = 8,
        Post = 4096,
    }
    [System.Serializable]
    public class EventBaseData
    {
        public UnityEngine.Object target;
        public string method;
        public EventBaseParameter parameter = EventBaseParameter.None;

        public string stringValue;
        public double doubleValue;
        public bool boolValue;
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
                EventHelper.Invoke(component, @event);
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
                EventHelper.Invoke<T>(component, @event, value);
            }
        }
        public int GetEventCount()
        {
            return m_Events != null ? m_Events.Length : 0;
        }
    }
    internal static class EventHelper
    {
        public static void Invoke(XOR.TsComponent component, EventBaseData @event)
        {
            Invoke<object>(component, @event, default);
        }
        public static void Invoke<T>(XOR.TsComponent component, EventBaseData @event, T value = default)
        {
            switch (@event.parameter)
            {
                case EventBaseParameter.String:
                    component.InvokeMethod(@event.method, @event.stringValue);
                    break;
                case EventBaseParameter.Double:
                    component.InvokeMethod(@event.method, @event.doubleValue);
                    break;
                case EventBaseParameter.Bool:
                    component.InvokeMethod(@event.method, @event.boolValue);
                    break;
                case EventBaseParameter.Object:
                    component.InvokeMethod(@event.method, @event.objectValue);
                    break;
                case EventBaseParameter.Post:
                    component.InvokeMethod(@event.method, value);
                    break;
                default:
                    component.InvokeMethod(@event.method);
                    break;
            }
        }
    }
}
