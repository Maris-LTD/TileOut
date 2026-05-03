using System;
using Cysharp.Threading.Tasks;
using GameModules.Core;
using GameModules.UI.DTOs;
using GameModules.UI.Results;
using VContainer;
using ZBase.UnityScreenNavigator.Core.Sheets;

namespace GameModules.UI.Base
{
    public abstract class BaseSheet<TData, TResult> : Sheet, IDependencyInjectable
        where TData : class, ISheetData
        where TResult : class
    {
        protected IObjectResolver Resolver { get; private set; }
        public TData Data { get; set; }
        private TResult _result;

        public void InjectDependencies(IObjectResolver resolver)
        {
            Resolver = resolver;
            OnDependenciesInjected();
        }

        protected virtual void OnDependenciesInjected()
        {
        }

        protected virtual void OnInitialize(TData data)
        {
        }

        protected virtual void OnWillEnter()
        {
        }

        protected virtual void OnDidEnter()
        {
        }

        protected virtual void OnWillExit()
        {
        }

        protected virtual void OnDidExit()
        {
        }

        protected virtual void OnCleanup()
        {
        }

        protected void SetResult(TResult result)
        {
            _result = result;
        }

        protected void SetResult(UIResult<TResult> result)
        {
            if (result.IsSuccess)
            {
                _result = result.Value;
            }
            else
            {
                _result = default(TResult);
            }
        }

        public TResult GetResult()
        {
            return _result;
        }

        public sealed override UniTask Initialize(Memory<object> args)
        {
            if (args.Length > 0 && args.Span[0] is TData data)
            {
                Data = data;
                OnInitialize(data);
            }
            else
            {
                OnInitialize(null);
            }
            return UniTask.CompletedTask;
        }

        public sealed override UniTask WillEnter(Memory<object> args)
        {
            OnWillEnter();
            return UniTask.CompletedTask;
        }

        public sealed override void DidEnter(Memory<object> args)
        {
            OnDidEnter();
        }

        public sealed override UniTask WillExit(Memory<object> args)
        {
            OnWillExit();
            return UniTask.CompletedTask;
        }

        public sealed override void DidExit(Memory<object> args)
        {
            OnDidExit();
        }

        public sealed override UniTask Cleanup(Memory<object> args)
        {
            OnCleanup();
            return UniTask.CompletedTask;
        }
    }
}
