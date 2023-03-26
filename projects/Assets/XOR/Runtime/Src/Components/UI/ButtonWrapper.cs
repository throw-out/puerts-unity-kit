using System;
using UnityEngine;
using UnityEngine.UI;

namespace XOR.UI
{
    [AddComponentMenu("UI/[★]Wrapper/Button")]
    public class ButtonWrapper : UnityEngine.UI.Button
    {
        [SerializeField]
        private ButtonWrapperEvent m_OnClickWrapper = new ButtonWrapperEvent();

        protected override void Awake()
        {
            base.Awake();
            this.onClick.AddListener(Press);
        }
        protected virtual void Press()
        {
            if (m_OnClickWrapper == null)
                return;
            m_OnClickWrapper.Invoke();
        }

        public virtual int GetWrapperEventCount()
        {
            return m_OnClickWrapper != null ? m_OnClickWrapper.GetEventCount() : 0;
        }

        [System.Serializable]
        protected class ButtonWrapperEvent : XOR.Events.EventBase
        {
        }
    }
}

