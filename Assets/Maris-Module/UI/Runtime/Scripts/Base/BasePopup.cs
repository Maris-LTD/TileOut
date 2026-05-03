using System;
using Cysharp.Threading.Tasks;
using GameModules.Core;
using GameModules.UI.DTOs;
using GameModules.UI.Results;
using VContainer;
using ZBase.UnityScreenNavigator.Core.Modals;

namespace GameModules.UI.Base
{
    public abstract class BasePopup<TData, TResult> : Modal, IDependencyInjectable
        where TData : class, IPopupData
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

        protected virtual void OnWillPushEnter()
        {
        }

        protected virtual void OnDidPushEnter()
        {
        }

        protected virtual void OnWillPushExit()
        {
        }

        protected virtual void OnDidPushExit()
        {
        }

        protected virtual void OnWillPopEnter()
        {
        }

        protected virtual void OnDidPopEnter()
        {
        }

        protected virtual void OnWillPopExit()
        {
        }

        protected virtual UniTask OnWillPopExitAsync()
        {
            return UniTask.CompletedTask;
        }

        protected virtual void OnDidPopExit()
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
            _result = result.IsSuccess ? result.Value : null;
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

        public sealed override UniTask WillPushEnter(Memory<object> args)
        {
            OnWillPushEnter();
            return UniTask.CompletedTask;
        }

        public sealed override void DidPushEnter(Memory<object> args)
        {
            OnDidPushEnter();
        }

        public sealed override UniTask WillPushExit(Memory<object> args)
        {
            OnWillPushExit();
            return UniTask.CompletedTask;
        }

        public sealed override void DidPushExit(Memory<object> args)
        {
            OnDidPushExit();
        }

        public sealed override UniTask WillPopEnter(Memory<object> args)
        {
            OnWillPopEnter();
            return UniTask.CompletedTask;
        }

        public sealed override void DidPopEnter(Memory<object> args)
        {
            OnDidPopEnter();
        }

        public sealed override UniTask WillPopExit(Memory<object> args)
        {
            OnWillPopExit();
            return OnWillPopExitAsync();
        }

        public sealed override void DidPopExit(Memory<object> args)
        {
            OnDidPopExit();
        }

        public sealed override UniTask Cleanup(Memory<object> args)
        {
            OnCleanup();
            return UniTask.CompletedTask;
        }
    }
}
