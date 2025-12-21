using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MateInN.UI
{
    public class LoadingSceneController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text loadingText;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Settings")]
        [SerializeField] private string targetSceneName = "PlayScene";
        [SerializeField] private float minLoadingDuration = 2.5f;
        [SerializeField] private string loadingTextContent = "Loading...";

        [Header("Animation")]
        [SerializeField] private float fadeOutDuration = 0.5f;

        private void Start()
        {
            InitializeUI();
            StartLoading().Forget();
        }

        private void InitializeUI()
        {
            if (loadingText != null)
                loadingText.text = loadingTextContent;

            if (progressSlider != null)
                progressSlider.value = 0f;

            if (progressText != null)
                progressText.text = "0%";

            if (canvasGroup != null)
                canvasGroup.alpha = 1f;
        }

        private async UniTaskVoid StartLoading()
        {
            await SimulateLoading();
            await FadeOutAndLoadScene();
        }

        private async UniTask SimulateLoading()
        {
            float elapsed = 0f;

            while (elapsed < minLoadingDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / minLoadingDuration);
                progress = EaseOutCubic(progress);

                UpdateProgressUI(progress);

                await UniTask.Yield();
            }

            UpdateProgressUI(1f);
        }

        private void UpdateProgressUI(float progress)
        {
            if (progressSlider != null)
                progressSlider.value = progress;

            if (progressText != null)
                progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
        }

        private float EaseOutCubic(float t)
        {
            return 1f - Mathf.Pow(1f - t, 3f);
        }

        private async UniTask FadeOutAndLoadScene()
        {
            if (canvasGroup != null)
            {
                await canvasGroup.DOFade(0f, fadeOutDuration)
                    .SetEase(Ease.InQuad)
                    .AsyncWaitForCompletion();
            }

            await SceneManager.LoadSceneAsync(targetSceneName);
        }
    }
}

