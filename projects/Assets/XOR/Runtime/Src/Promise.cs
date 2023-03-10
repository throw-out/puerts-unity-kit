using System;
using System.Collections.Generic;

namespace XOR
{
    public delegate void PromiseExecutor(Action resolve, Action<Exception> reject);
    public delegate void PromiseExecutor<T>(Action<T> resolve, Action<Exception> reject);
    public delegate void PromiseResolve(Action resolve);
    public delegate void PromiseResolve<T>(Action<T> resolve);

    public interface IPromiseAwaiter :
        //System.Runtime.CompilerServices.ICriticalNotifyCompletion,
        System.Runtime.CompilerServices.INotifyCompletion
    {
        bool IsCompleted { get; }
        void GetResult();
    }
    public interface IPromiseAwaiter<TResult> :
           //System.Runtime.CompilerServices.ICriticalNotifyCompletion,
           System.Runtime.CompilerServices.INotifyCompletion
    {
        bool IsCompleted { get; }
        TResult GetResult();
    }
    public interface IPromiseAwaitable
    {
        IPromiseAwaiter GetAwaiter();
        IPromiseAwaitable Then(Action onfulfilled);
        IPromiseAwaitable Catch(Action<Exception> onrejected);
        IPromiseAwaitable Finally(Action onfinally);
    }
    public interface IPromiseAwaitable<TResult>
    {
        IPromiseAwaiter<TResult> GetAwaiter();
        IPromiseAwaitable<TResult> Then(Action<TResult> onfulfilled);
        IPromiseAwaitable<TResult> Catch(Action<Exception> onrejected);
        IPromiseAwaitable<TResult> Finally(Action onfinally);
    }

    public sealed class Promise : IPromiseAwaitable
    {
        private Awaiter _awaiter;
        private ConfiguredTaskAwaitable _configuredTaskAwaitable;

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

        public IPromiseAwaiter GetAwaiter()
        {
            return this._awaiter;
        }
        public IPromiseAwaitable ConfigureAwait(bool continueOnCapturedContext)
        {
            if (continueOnCapturedContext)
            {
                TryCreateConfiguredTaskAwaitable();
                return this._configuredTaskAwaitable;
            }
            return this;
        }
        public IPromiseAwaitable Then(Action onfulfilled)
        {
            if (onfulfilled == null)
                return this;
            this._awaiter.OnCompleted(() =>
            {
                if (this._awaiter.Exception != null)
                    return;
                onfulfilled();
            });
            return this;
        }
        public IPromiseAwaitable Catch(Action<Exception> onrejected)
        {
            if (onrejected == null)
                return this;
            this._awaiter.OnCompleted(() =>
            {
                if (this._awaiter.Exception == null)
                    return;
                onrejected(this._awaiter.Exception);
            });
            return this;
        }
        public IPromiseAwaitable Finally(Action onfinally)
        {
            if (onfinally == null)
                return this;
            this._awaiter.OnCompleted(onfinally);
            return this;
        }

        public void Resolve()
        {
            this._awaiter.Resolve();
        }
        public void Reject(Exception e)
        {
            this._awaiter.Reject(e);
        }
        void TryCreateConfiguredTaskAwaitable()
        {
            if (this._configuredTaskAwaitable != null)
                return;
            this._configuredTaskAwaitable = new ConfiguredTaskAwaitable(this._awaiter);
        }
        private class Awaiter : IPromiseAwaiter
        {
            private bool _completed = false;
            private Exception _exception;
            private List<Action> _continuations = new List<Action>();

            public bool IsCompleted { get => this._completed; }
            public Exception Exception { get => this._exception; }
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
                    return;
                }
                bool completed = false;
                lock (this._continuations)
                {
                    if (this._completed) completed = true;
                    else this._continuations.Add(continuation);
                }
                if (completed)
                {
                    continuation();
                }
            }
            public void UnsafeOnCompleted(Action continuation)
            {
                throw new NotImplementedException();
            }
            public void Reject(Exception e)
            {
                if (this._completed)
                    throw NewPromiseCompletedException();
                lock (this._continuations)
                {
                    if (this._completed)
                        throw NewPromiseCompletedException();
                    this._completed = true;
                }
                this._exception = e;
                CompletedContinuations();
            }
            public void Resolve()
            {
                if (this._completed)
                    throw NewPromiseCompletedException();
                lock (this._continuations)
                {
                    if (this._completed)
                        throw NewPromiseCompletedException();
                    this._completed = true;
                }
                CompletedContinuations();
            }
            void CompletedContinuations()
            {
                Action[] continuations;
                lock (this._continuations)
                {
                    continuations = new Action[this._continuations.Count];
                    for (int i = 0; i < this._continuations.Count; i++)
                    {
                        continuations[i] = this._continuations[i];
                    }
                    this._continuations.Clear();
                }
                for (int i = 0; i < continuations.Length; i++)
                {
                    try
                    {
                        continuations[i]();
                    }
                    catch (Exception e) { UnityEngine.Debug.LogError(e); }
                }
            }

            static Exception NewPromiseCompletedException()
            {
                return new InvalidOperationException("the promise is completed");
            }
        }
        private class ConfiguredTaskAwaitable : IPromiseAwaitable
        {
            private ConfiguredTaskAwaiter _awaiter;
            public ConfiguredTaskAwaitable(Awaiter awaiter)
            {
                this._awaiter = new ConfiguredTaskAwaiter(awaiter);
            }
            public IPromiseAwaiter GetAwaiter()
            {
                return this._awaiter;
            }
            public IPromiseAwaitable Then(Action onfulfilled)
            {
                if (onfulfilled == null)
                    return this;
                this._awaiter.OnCompleted(() =>
                {
                    if (this._awaiter.Exception != null)
                        return;
                    onfulfilled();
                });
                return this;
            }
            public IPromiseAwaitable Catch(Action<Exception> onrejected)
            {
                if (onrejected == null)
                    return this;
                this._awaiter.OnCompleted(() =>
                {
                    if (this._awaiter.Exception == null)
                        return;
                    onrejected(this._awaiter.Exception);
                });
                return this;
            }
            public IPromiseAwaitable Finally(Action onfinally)
            {
                if (onfinally == null)
                    return this;
                this._awaiter.OnCompleted(onfinally);
                return this;
            }
        }
        private class ConfiguredTaskAwaiter : IPromiseAwaiter
        {
            private Awaiter _awaiter;
            public bool IsCompleted => this._awaiter.IsCompleted;
            public Exception Exception { get => this._awaiter.Exception; }

            public ConfiguredTaskAwaiter(Awaiter awaiter)
            {
                this._awaiter = awaiter;
            }
            public void GetResult()
            {
                this._awaiter.GetResult();
            }
            public void OnCompleted(Action continuation)
            {
                /*
                var operation = System.ComponentModel.AsyncOperationManager.CreateOperation(null);
                operation.Post(state => continuation(), null);
                this._awaiter.OnCompleted(operation.OperationCompleted);
                //*/
                var syncContext = System.Threading.SynchronizationContext.Current;
                if (syncContext == null)
                {
                    throw new InvalidProgramException();
                }
                this._awaiter.OnCompleted(() =>
                {
                    syncContext.Post(state => continuation(), null);
                });
            }
            public void UnsafeOnCompleted(Action continuation)
            {
                this._awaiter.UnsafeOnCompleted(continuation);
            }
        }
    }
    public sealed class Promise<TResult> : IPromiseAwaitable<TResult>
    {
        private Awaiter<TResult> _awaiter;
        private ConfiguredTaskAwaitable<TResult> _configuredTaskAwaitable;

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

        public IPromiseAwaiter<TResult> GetAwaiter()
        {
            return this._awaiter;
        }
        public IPromiseAwaitable<TResult> ConfigureAwait(bool continueOnCapturedContext)
        {
            if (continueOnCapturedContext)
            {
                TryCreateConfiguredTaskAwaitable();
                return this._configuredTaskAwaitable;
            }
            return this;
        }
        public IPromiseAwaitable<TResult> Then(Action<TResult> onfulfilled)
        {
            if (onfulfilled == null)
                return this;
            this._awaiter.OnCompleted(() =>
            {
                if (this._awaiter.Exception != null)
                    return;
                onfulfilled(this._awaiter.GetResult());
            });
            return this;
        }
        public IPromiseAwaitable<TResult> Catch(Action<Exception> onrejected)
        {
            if (onrejected == null)
                return this;
            this._awaiter.OnCompleted(() =>
            {
                if (this._awaiter.Exception == null)
                    return;
                onrejected(this._awaiter.Exception);
            });
            return this;
        }
        public IPromiseAwaitable<TResult> Finally(Action onfinally)
        {
            if (onfinally == null)
                return this;
            this._awaiter.OnCompleted(onfinally);
            return this;
        }

        public void Resolve(TResult result)
        {
            this._awaiter.Resolve(result);
        }
        public void Reject(Exception e)
        {
            this._awaiter.Reject(e);
        }
        void TryCreateConfiguredTaskAwaitable()
        {
            if (this._configuredTaskAwaitable != null)
                return;
            this._configuredTaskAwaitable = new ConfiguredTaskAwaitable<TResult>(this._awaiter);
        }

        private class Awaiter<T> : IPromiseAwaiter<T>
        {
            private bool _completed = false;
            private T _result;
            private Exception _exception;
            private List<Action> _continuations = new List<Action>();

            public bool IsCompleted { get => this._completed; }
            public Exception Exception { get => this._exception; }

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
                    return;
                }
                bool completed = false;
                lock (this._continuations)
                {
                    if (this._completed) completed = true;
                    else this._continuations.Add(continuation);
                }
                if (completed)
                {
                    continuation();
                }
            }
            public void UnsafeOnCompleted(Action continuation)
            {
                throw new NotImplementedException();
            }

            public void Reject(Exception e)
            {
                if (this._completed)
                    throw NewPromiseCompletedException();
                lock (this._continuations)
                {
                    if (this._completed)
                        throw NewPromiseCompletedException();
                    this._completed = true;
                }
                this._exception = e;
                CompletedContinuations();
            }
            public void Resolve(T result)
            {
                if (this._completed)
                    throw NewPromiseCompletedException();
                lock (this._continuations)
                {
                    if (this._completed)
                        throw NewPromiseCompletedException();
                    this._completed = true;
                }
                this._result = result;
                CompletedContinuations();
            }
            void CompletedContinuations()
            {
                Action[] continuations;
                lock (this._continuations)
                {
                    continuations = new Action[this._continuations.Count];
                    for (int i = 0; i < this._continuations.Count; i++)
                    {
                        continuations[i] = this._continuations[i];
                    }
                    this._continuations.Clear();
                }
                for (int i = 0; i < continuations.Length; i++)
                {
                    try
                    {
                        continuations[i]();
                    }
                    catch (Exception e) { UnityEngine.Debug.LogError(e); }
                }
            }
            static Exception NewPromiseCompletedException()
            {
                return new InvalidOperationException("the promise is completed");
            }
        }
        private class ConfiguredTaskAwaitable<T> : IPromiseAwaitable<T>
        {
            private ConfiguredTaskAwaiter<T> _awaiter;
            public ConfiguredTaskAwaitable(Awaiter<T> awaiter)
            {
                this._awaiter = new ConfiguredTaskAwaiter<T>(awaiter);
            }
            public IPromiseAwaiter<T> GetAwaiter()
            {
                return this._awaiter;
            }
            public IPromiseAwaitable<T> Then(Action<T> onfulfilled)
            {
                if (onfulfilled == null)
                    return this;
                this._awaiter.OnCompleted(() =>
                {
                    if (this._awaiter.Exception != null)
                        return;
                    onfulfilled(this._awaiter.GetResult());
                });
                return this;
            }
            public IPromiseAwaitable<T> Catch(Action<Exception> onrejected)
            {
                if (onrejected == null)
                    return this;
                this._awaiter.OnCompleted(() =>
                {
                    if (this._awaiter.Exception == null)
                        return;
                    onrejected(this._awaiter.Exception);
                });
                return this;
            }
            public IPromiseAwaitable<T> Finally(Action onfinally)
            {
                if (onfinally == null)
                    return this;
                this._awaiter.OnCompleted(onfinally);
                return this;
            }
        }
        private class ConfiguredTaskAwaiter<T> : IPromiseAwaiter<T>
        {
            private Awaiter<T> _awaiter;
            public bool IsCompleted => this._awaiter.IsCompleted;
            public Exception Exception { get => this._awaiter.Exception; }

            public ConfiguredTaskAwaiter(Awaiter<T> awaiter)
            {
                this._awaiter = awaiter;
            }
            public T GetResult()
            {
                return this._awaiter.GetResult();
            }
            public void OnCompleted(Action continuation)
            {
                var syncContext = System.Threading.SynchronizationContext.Current;
                if (syncContext == null)
                {
                    throw new InvalidProgramException();
                }
                this._awaiter.OnCompleted(() =>
                {
                    syncContext.Post(state => continuation(), null);
                });
            }
            public void UnsafeOnCompleted(Action continuation)
            {
                this._awaiter.UnsafeOnCompleted(continuation);
            }
        }
    }
}