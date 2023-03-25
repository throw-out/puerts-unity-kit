using System;
using UnityEngine;
using UnityEngine.UI;

namespace XOR.UI
{
    [AddComponentMenu("UI/[★]Wrapper/Toggle")]
    public class ToggleWrapper : UnityEngine.UI.Toggle
    {
        [SerializeField]
        private ToggleWrapperEvent m_OnValueChangedWrapper = new ToggleWrapperEvent();

        protected override void Awake()
        {
            base.Awake();
            this.onValueChanged.AddListener(OnValueChanged);
        }
        protected virtual void OnValueChanged(bool value)
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
        protected class ToggleWrapperEvent : XOR.EventBase<bool>
        {
        }
    }
}