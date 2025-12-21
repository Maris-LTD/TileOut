using System.Collections.Generic;
using System.Globalization;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameModules.UI.Base;
using GameModules.UI.DTOs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class LoadingPopupData : IPopupData
    {
        public string LoadingText { get; set; } = "Loading...";
    }

    public class LoadingPopupResult
    {
        public bool Completed { get; set; }
    }

    public class LoadingPopup : BasePopup<LoadingPopupData, LoadingPopupResult>
    {
        [SerializeField] private TMP_Text loadingText;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform contentPanel;
        
        private readonly float _defaultLoadingTime = 1;

        protected override void OnInitialize(LoadingPopupData data)
        {
            if (data == null) return;

            if (loadingText != null)
                loadingText.text = data.LoadingText;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            progressSlider.value = 0;
            progressText.text = _defaultLoadingTime.ToString(CultureInfo.InvariantCulture) + "%";
        }

        protected override void OnWillPopExit() { SetResult(new LoadingPopupResult { Completed = true }); }

        protected override UniTask OnWillPopExitAsync() { return PlayHideAnimationAsync(); }

        private UniTask PlayHideAnimationAsync()
        {
            var animationTasks = new List<UniTask>();
            if (progressSlider != null)
            {
                animationTasks.Add(
                    progressSlider.DOValue(1f, _defaultLoadingTime)
                        .SetEase(Ease.OutQuad)
                        .OnUpdate(() =>
                            progressText.text = ((int)(progressSlider.value * 100)).ToString(CultureInfo.InvariantCulture) + "%")
                        .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy())
                );
            }
            return UniTask.WhenAll(animationTasks);
        }

        protected override void OnCleanup() { DOTween.Kill(canvasGroup); }
    }
}