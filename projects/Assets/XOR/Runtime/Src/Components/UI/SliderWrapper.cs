using System;
using UnityEngine;
using UnityEngine.UI;

namespace XOR.UI
{
    [AddComponentMenu("UI/[★]Wrapper/Slider")]
    public class SliderWrapper : UnityEngine.UI.Slider
    {
        [SerializeField]
        private SliderWrapperEvent m_OnValueChangedWrapper = new SliderWrapperEvent();

        protected override void Awake()
        {
            base.Awake();
            this.onValueChanged.AddListener(OnValueChanged);
        }
        protected virtual void OnValueChanged(float value)
        {
            if (m_OnValueChangedWrapper == null)
                return;
            m_OnValueChangedWrapper.Invoke(value);
        }
        public virtual int GetOnValueChangedEventCount()
        {
            return m_OnValueChangedWrapper != null ? m_OnValueChangedWrapper.GetEventCount() : 0;
        }

        [System.Serializable]
        protected class SliderWrapperEvent : XOR.Events.EventBase<float>
        {
        }
    }
}