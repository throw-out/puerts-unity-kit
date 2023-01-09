using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XOR
{
    public class Subscription
    {
        public bool Closed { get; protected set; }

        public virtual void Unsubscribe()
        {
            this.Closed = true;
        }
    }

    public class Subscriber : Subscription
    {
        private Action _close;
        private Action _callback;

        public Subscriber(Action callback)
        {
            this._callback = callback;
        }
        public override void Unsubscribe()
        {
            if (this.Closed) return;
            base.Unsubscribe();

            Action fn = this._close;
            this._close = null;
            this._callback = null;

            fn?.Invoke();
        }
        public void OnClose(Action handler)
        {
            this._close = handler;
        }

        public void Next()
        {
            if (this.Closed) return;
            this._callback?.Invoke();
        }
    }
    public class Subscriber<T> : Subscription
    {
        private Action _close;
        private Action<T> _callback;

        public Subscriber(Action<T> callback)
        {
            this._callback = callback;
        }

        public override void Unsubscribe()
        {
            if (this.Closed) return;
            base.Unsubscribe();

            Action fn = this._close;
            this._close = null;
            this._callback = null;

            fn?.Invoke();
        }
        public void OnClose(Action handler)
        {
            this._close = handler;
        }

        public void Next(T value)
        {
            if (this.Closed) return;
            this._callback?.Invoke(value);
        }
    }

    public class Observable
    {
        public bool Closed { get; protected set; }
        public int Count { get => this.subscribers != null ? this.subscribers.Count : 0; }

        protected Action<Subscriber> subscribe;
        protected HashSet<Subscriber> subscribers;

        public Observable() : this(null) { }
        public Observable(Action<Subscriber> subscribe)
        {
            this.subscribe = subscribe;
            this.subscribers = new HashSet<Subscriber>();
        }

        public virtual Subscription Subscribe(Action callback, bool immediately = true)
        {
            if (this.Closed)
                return null;
            Subscriber subscriber = new Subscriber(callback);
            this.subscribers.Add(subscriber);
            if (immediately)
            {
                this.subscribe?.Invoke(subscriber);
            }
            return subscriber;
        }
        public void Next()
        {
            if (this.Closed) return;

            int closed = 0;
            foreach (var subscriber in this.subscribers)
            {
                if (subscriber.Closed)
                {
                    closed++;
                    continue;
                }
                subscriber.Next();
            }

            if (closed > 0)
            {
                foreach (var subscriber in this.subscribers.Where(s => s.Closed))
                {
                    this.subscribers.Remove(subscriber);
                }
            }
        }
        public void Unsubscribe()
        {
            if (this.Closed) return;
            foreach (var subscriber in this.subscribers)
            {
                subscriber.Unsubscribe();
            }
            this.subscribers.Clear();
        }
        public void Dispose()
        {
            if (this.Closed) return;
            this.Closed = true;
            this.Unsubscribe();
        }
    }
    public class Observable<T>
    {
        public bool Closed { get; protected set; }
        public int Count { get => this.subscribers != null ? this.subscribers.Count : 0; }

        protected Action<Subscriber<T>> subscribe;
        protected HashSet<Subscriber<T>> subscribers;

        public Observable() : this(null) { }
        public Observable(Action<Subscriber<T>> subscribe)
        {
            this.subscribe = subscribe;
            this.subscribers = new HashSet<Subscriber<T>>();
        }

        public virtual Subscription Subscribe(Action<T> callback, bool immediately = true)
        {
            if (this.Closed)
                return null;
            Subscriber<T> subscriber = new Subscriber<T>(callback);
            this.subscribers.Add(subscriber);
            if (immediately)
            {
                this.subscribe?.Invoke(subscriber);
            }
            return subscriber;
        }
        public void Next(T value)
        {
            if (this.Closed) return;

            int closed = 0;
            foreach (var subscriber in this.subscribers)
            {
                if (subscriber.Closed)
                {
                    closed++;
                    continue;
                }
                subscriber.Next(value);
            }

            if (closed > 0)
            {
                foreach (var subscriber in this.subscribers.Where(s => s.Closed))
                {
                    this.subscribers.Remove(subscriber);
                }
            }
        }
        public void Unsubscribe()
        {
            if (this.Closed) return;
            foreach (var subscriber in this.subscribers)
            {
                subscriber.Unsubscribe();
            }
            this.subscribers.Clear();
        }
        public void Dispose()
        {
            if (this.Closed) return;
            this.Closed = true;
            this.Unsubscribe();
        }
    }
    public class Observable<T, TState> : Observable<T>
    {
        protected Action<Subscriber<T>, TState> subscribe2;
        protected TState state;

        public Observable(Action<Subscriber<T>, TState> subscribe)
        {
            this.subscribe2 = subscribe;
        }
        public override Subscription Subscribe(Action<T> callback, bool immediately = true)
        {
            if (this.Closed)
                return null;
            if (this.subscribe2 != null && immediately)
            {
                Subscriber<T> subscriber = new Subscriber<T>(callback);
                this.subscribers.Add(subscriber);
                this.subscribe2(subscriber, state);
                return subscriber;
            }
            else
            {
                return base.Subscribe(callback, immediately);
            }
        }
        public void SetDefault(TState state)
        {
            this.state = state;
        }
    }
}
