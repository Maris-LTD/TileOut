using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameModules.UI.NavBar
{
    public interface IPageTransitionService
    {
        void RegisterPage(int index, BasePage page);
        void UnregisterPage(int index);
        UniTask TransitionToPage(int fromIndex, int toIndex);
        void SetInteractable(bool interactable);
    }

    public class PageTransitionService : IPageTransitionService
    {
        private readonly NavBarConfig _config;
        private readonly CanvasGroup _canvasGroup;
        private readonly Dictionary<int, BasePage> _pages = new();
        
        private Sequence _transitionSequence;
        private BasePage _previousPage;
        private BasePage _currentPage;
        private UniTaskCompletionSource _transitionCompletionSource;

        public PageTransitionService(NavBarConfig config, CanvasGroup canvasGroup = null)
        {
            _config = config;
            _canvasGroup = canvasGroup;
        }

        public void RegisterPage(int index, BasePage page)
        {
            if (_pages.ContainsKey(index))
            {
                _pages[index] = page;
            }
            else
            {
                _pages.Add(index, page);
            }
        }

        public void UnregisterPage(int index)
        {
            _pages.Remove(index);
        }

        public void SetInteractable(bool interactable)
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.interactable = interactable;
            }
        }

        public async UniTask TransitionToPage(int fromIndex, int toIndex)
        {
            _transitionSequence?.Kill();
            _transitionCompletionSource?.TrySetCanceled();

            if (!_pages.TryGetValue(toIndex, out var newPage))
            {
                return;
            }

            _pages.TryGetValue(fromIndex, out var prePage);

            if (fromIndex == toIndex || fromIndex < 0)
            {
                if (_previousPage != null && _previousPage != newPage)
                {
                    _previousPage.Sleep();
                }

                _currentPage = newPage;
                newPage.WakeUp();
                newPage.RectTransform.anchoredPosition = Vector2.zero;
                return;
            }

            _transitionCompletionSource = new UniTaskCompletionSource();
            _transitionSequence = DOTween.Sequence();
            SetInteractable(false);

            if (_previousPage != null && _previousPage != prePage)
            {
                _previousPage.Sleep();
            }

            _previousPage = _currentPage;
            _currentPage = newPage;

            int factor = fromIndex < toIndex ? -1 : 1;
            float duration = _config?.PageTransitionDuration ?? 0.25f;

            if (_previousPage != null)
            {
                var prePageAnchoredDesPosX = _previousPage.RectTransform.rect.width * factor;
                _transitionSequence.Insert(0, 
                    _previousPage.RectTransform
                        .DOAnchorPosX(prePageAnchoredDesPosX, duration)
                        .SetEase(Ease.InOutSine));
            }

            newPage.WakeUp();
            var newPageAnchoredStartPosX = newPage.RectTransform.rect.width * (-factor);
            newPage.RectTransform.anchoredPosition = new Vector2(newPageAnchoredStartPosX, 0);

            var completionSource = _transitionCompletionSource;

            _transitionSequence
                .Insert(0, 
                    newPage.RectTransform
                        .DOAnchorPosX(0, duration)
                        .SetEase(Ease.InOutSine))
                .OnComplete(() =>
                {
                    if (_previousPage != null)
                    {
                        _previousPage.Sleep();
                    }
                    SetInteractable(true);
                    newPage.OnTransitionComplete();
                    completionSource.TrySetResult();
                });

            await completionSource.Task;
        }
    }
}

