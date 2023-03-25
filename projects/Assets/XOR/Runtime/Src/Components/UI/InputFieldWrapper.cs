using System;
using UnityEngine;
using UnityEngine.UI;

namespace XOR.UI
{
    [AddComponentMenu("UI/[★]Wrapper/InputField")]
    public class InputFieldWrapper : UnityEngine.UI.InputField
    {
        [SerializeField]
        private OnChangeWrapperEvent m_OnValueChangedWrapper = new OnChangeWrapperEvent();
        [SerializeField]
        private SubmitWrapperEvent m_OnEndEditWrapper = new SubmitWrapperEvent();

        protected override void Awake()
        {
            base.Awake();
            this.onValueChanged.AddListener(OnValueChanged);
            this.onEndEdit.AddListener(OnEndEdit);
        }
        protected virtual void OnValueChanged(string value)
        {
            if (m_OnValueChangedWrapper == null)
                return;
            m_OnValueChangedWrapper.Invoke(value);
        }
        protected virtual void OnEndEdit(string value)
        {
            if (m_OnEndEditWrapper == null)
                return;
            m_OnEndEditWrapper.Invoke(value);
        }
        public virtual int GetOnValueChangedEventCount()
        {
            return m_OnValueChangedWrapper != null ? m_OnValueChangedWrapper.GetEventCount() : 0;
        }
        public virtual int GetOnEndEditEventCount()
        {
            return m_OnEndEditWrapper != null ? m_OnEndEditWrapper.GetEventCount() : 0;
        }

        [System.Serializable]
        protected class SubmitWrapperEvent : XOR.EventBase<string>
        {
        }
        [System.Serializable]
        protected class OnChangeWrapperEvent : XOR.EventBase<string>
        {
        }
    }
}