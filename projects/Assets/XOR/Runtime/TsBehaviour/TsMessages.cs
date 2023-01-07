using System;
using UnityEngine;

namespace XOR
{
    public class TsMessages : MonoBehaviour
    {
        public ReceiveMessagesCallback callback;

        public Action emptyCallback;


        public void ReceiveMessage(string eventName)
        {
            if (this.callback != null)
                this.callback(eventName);
        }
        public void ReceiveMultiparameterMessage(string eventName, params string[] args)
        {
            if (this.callback != null)
                this.callback(eventName, args);
        }
        public void ReceiveEmptyMessage()
        {
            if (this.emptyCallback != null)
                this.emptyCallback();
        }
    }

    public delegate void ReceiveMessagesCallback(string eventName, params string[] args);
}
