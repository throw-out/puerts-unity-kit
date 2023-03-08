using System;
using System.Collections.Generic;

namespace CDP
{
    public delegate void PromiseExecutor(Action resolve, Action<Exception> reject);
    public delegate void PromiseExecutor<T>(Action<T> resolve, Action<Exception> reject);
    public delegate void PromiseResolve(Action resolve);
    public delegate void PromiseResolve<T>(Action<T> resolve);

    public class Promise
    {
        private Awaiter _awaiter;

        public Promise()
        {
            this._awaiter = new Awaiter();
        }
        public Promise(PromiseResolve executor) : this()
        {
            executor(Resolve);
        }
        public Promise(PromiseExecutor executor) : this()
        {
            executor(Resolve, Reject);
        }

        public Awaiter GetAwaiter()
        {
            return this._awaiter;
        }

        public void Resolve()
        {
            this._awaiter.Resolve();
        }
        public void Reject(Exception e)
        {
            this._awaiter.Reject(e);
        }

        public class Awaiter :
           //System.Runtime.CompilerServices.ICriticalNotifyCompletion,
           System.Runtime.CompilerServices.INotifyCompletion
        {
            private bool _completed = false;
            private Exception _exception;
            private List<Action> _continuations = new List<Action>();

            public bool IsCompleted { get => this._completed; }
            public void GetResult()
            {
                if (this._exception != null)
                {
                    throw this._exception;
                }
            }
            public void OnCompleted(Action continuation)
            {
                if (this._completed)
                {
                    continuation();
                }
                else
                {
                    this._continuations.Add(continuation);
                }
            }
            public void UnsafeOnCompleted(Action continuation)
            {
                throw new NotImplementedException();
            }
            internal void Reject(Exception e)
            {
                if (this._completed)
                    throw new InvalidOperationException("the promise is completed");
                this._completed = true;
                this._exception = e;
                CompletedContinuations();
            }
            internal void Resolve()
            {
                if (this._completed)
                    throw new InvalidOperationException("the promise is completed");
                this._completed = true;
                CompletedContinuations();
            }
            void CompletedContinuations()
            {
                foreach (var continuation in this._continuations)
                {
                    try
                    {
                        continuation();
                    }
                    catch (Exception e) { UnityEngine.Debug.LogError(e); }
                }
                this._continuations.Clear();
            }
        }
    }
    public class Promise<TResult>
    {
        private Awaiter<TResult> _awaiter;
        public Promise()
        {
            this._awaiter = new Awaiter<TResult>();
        }
        public Promise(PromiseResolve<TResult> executor) : this()
        {
            executor(Resolve);
        }
        public Promise(PromiseExecutor<TResult> executor) : this()
        {
            executor(Resolve, Reject);
        }

        public Awaiter<TResult> GetAwaiter()
        {
            return this._awaiter;
        }
        public void Resolve(TResult result)
        {
            this._awaiter.Resolve(result);
        }
        public void Reject(Exception e)
        {
            this._awaiter.Reject(e);
        }
        public class Awaiter<T> :
            //System.Runtime.CompilerServices.ICriticalNotifyCompletion,
            System.Runtime.CompilerServices.INotifyCompletion
        {
            private bool _completed = false;
            private T _result;
            private Exception _exception;
            private List<Action> _continuations = new List<Action>();

            public bool IsCompleted { get => this._completed; }
            public T GetResult()
            {
                if (this._exception != null)
                {
                    throw this._exception;
                }
                return this._result;
            }

            public void OnCompleted(Action continuation)
            {
                if (this._completed)
                {
                    continuation();
                }
                else
                {
                    this._continuations.Add(continuation);
                }
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                throw new NotImplementedException();
            }

            internal void Reject(Exception e)
            {
                if (this._completed)
                    throw new InvalidOperationException("the promise is completed");
                this._completed = true;
                this._exception = e;
                CompletedContinuations();
            }
            internal void Resolve(T result)
            {
                if (this._completed)
                    throw new InvalidOperationException("the promise is completed");
                this._completed = true;
                this._result = result;
                CompletedContinuations();
            }
            void CompletedContinuations()
            {
                foreach (var continuation in this._continuations)
                {
                    try
                    {
                        continuation();
                    }
                    catch (Exception e) { UnityEngine.Debug.LogError(e); }
                }
                this._continuations.Clear();
            }
        }
    }
}