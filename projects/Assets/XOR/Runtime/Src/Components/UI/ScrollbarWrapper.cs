using System;
using UnityEngine;
using UnityEngine.UI;

namespace XOR.UI
{
    [AddComponentMenu("UI/[★]Wrapper/Scrollbar")]
    public class ScrollbarWrapper : UnityEngine.UI.Scrollbar
    {
        [SerializeField]
        private ScrollWrapperEvent m_OnValueChangedWrapper = new ScrollWrapperEvent();

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
        protected class ScrollWrapperEvent : XOR.Events.EventBase<float>
        {
        }
    }
}