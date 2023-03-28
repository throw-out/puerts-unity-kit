using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XOR.Events
{
    public enum ParameterType
    {
        None,
        String = 1,
        Double = 2,
        Bool = 4,
        Long = 8,
        Object = 16,
        Vector2 = 32,
        Vector3 = 64,
        Color = 128,
        Color32 = 256,
        Post = 4096,
    }

    [System.Serializable]
    public class EventBaseData
    {
        public UnityEngine.Object target;
        public string method;
        public ParameterType parameter = ParameterType.None;

        public string stringValue;
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
                case ParameterType.String:
                    component.InvokeMethod(@event.method, @event.stringValue);
                    break;
                case ParameterType.Double:
                    component.InvokeMethod(@event.method, Serializer.ToData<double>(@event.stringValue));
                    break;
                case ParameterType.Bool:
                    component.InvokeMethod(@event.method, Serializer.ToData<bool>(@event.stringValue));
                    break;
                case ParameterType.Long:
                    component.InvokeMethod(@event.method, Serializer.ToData<long>(@event.stringValue));
                    break;
                case ParameterType.Object:
                    component.InvokeMethod(@event.method, @event.objectValue);
                    break;
                case ParameterType.Vector2:
                    component.InvokeMethod(@event.method, Serializer.ToData<Vector2>(@event.stringValue));
                    break;
                case ParameterType.Vector3:
                    component.InvokeMethod(@event.method, Serializer.ToData<Vector3>(@event.stringValue));
                    break;
                case ParameterType.Color:
                    component.InvokeMethod(@event.method, Serializer.ToData<Color>(@event.stringValue));
                    break;
                case ParameterType.Color32:
                    component.InvokeMethod(@event.method, Serializer.ToData<Color32>(@event.stringValue));
                    break;
                case ParameterType.Post:
                    component.InvokeMethod(@event.method, value);
                    break;
                default:
                    component.InvokeMethod(@event.method);
                    break;
            }
        }
    }
}
