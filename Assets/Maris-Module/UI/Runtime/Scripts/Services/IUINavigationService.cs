using System.Threading;
using Cysharp.Threading.Tasks;
using GameModules.UI.DTOs;
using GameModules.UI.Results;

namespace GameModules.UI.Services
{
    public interface IUINavigationService
    {
        UniTask<TResult> PushScreenAsync<TData, TResult>(string resourceKey, TData data = null, CancellationToken cancellationToken = default)
            where TData : class, IScreenData
            where TResult : class;

        UniTask PopScreenAsync(CancellationToken cancellationToken = default);

        UniTask<TResult> ShowPopupAsync<TData, TResult>(string resourceKey, TData data = null, CancellationToken cancellationToken = default)
            where TData : class, IPopupData
            where TResult : class;

        UniTask<TResult> ShowPopupAsync<TData, TResult>(string resourceKey, TData data, bool playAnimation, CancellationToken cancellationToken = default)
            where TData : class, IPopupData
            where TResult : class;

        UniTask ClosePopupAsync(CancellationToken cancellationToken = default);

        UniTask ClosePopupAsync(bool playAnimation, CancellationToken cancellationToken = default);

        UniTask<TResult> ShowSheetAsync<TData, TResult>(string resourceKey, TData data = null, CancellationToken cancellationToken = default)
            where TData : class, ISheetData
            where TResult : class;

        UniTask CloseSheetAsync(CancellationToken cancellationToken = default);
    }
}

