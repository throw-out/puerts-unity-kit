using System;
using UnityEngine;
using UnityEngine.UI;

namespace XOR.UI
{
    [AddComponentMenu("UI/[★]Wrapper/Dropdown")]
    public class DropdownWrapper : UnityEngine.UI.Dropdown
    {
        [SerializeField]
        private DropdownWrapperEvent m_OnValueChangedWrapper = new DropdownWrapperEvent();

        protected override void Awake()
        {
            base.Awake();
            this.onValueChanged.AddListener(OnValueChanged);
        }
        protected virtual void OnValueChanged(int value)
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
        protected class DropdownWrapperEvent : XOR.Events.EventBase<int>
        {
        }
    }
}