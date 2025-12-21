using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameModules.UI.NavBar
{
    public class NavBarButton : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button button;
        [SerializeField] private Image background;
        [SerializeField] private RectTransform iconTransform;
        [SerializeField] private TextMeshProUGUI label;

        [Header("Sprites")]
        [SerializeField] private Sprite activeSprite;
        [SerializeField] private Sprite inactiveSprite;

        private NavBarConfig _config;
        private int _index;
        private bool _isActive;
        private Coroutine _transitionCoroutine;

        public int Index => _index;
        public bool IsActive => _isActive;
        public event Action<int> OnButtonClicked;

        public void Initialize(NavBarConfig config, int index)
        {
            _config = config;
            _index = index;
            _isActive = false;

            if (button != null)
            {
                button.onClick.AddListener(HandleClick);
            }

            SetStateImmediate(false);
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleClick);
            }
        }

        private void HandleClick()
        {
            OnButtonClicked?.Invoke(_index);
        }

        public void SetActive(bool active, bool animate = true)
        {
            if (_isActive == active) return;

            _isActive = active;

            if (button != null)
            {
                button.interactable = !active;
            }

            if (animate && _config != null)
            {
                if (_transitionCoroutine != null)
                {
                    StopCoroutine(_transitionCoroutine);
                }
                _transitionCoroutine = StartCoroutine(TransitionCoroutine(active));
            }
            else
            {
                SetStateImmediate(active);
            }
        }

        private void SetStateImmediate(bool active)
        {
            if (_config == null) return;

            float targetScale = active ? _config.ActiveIconScale : _config.InactiveIconScale;
            float targetPosY = active ? _config.ActiveIconPositionY : _config.InactiveIconPositionY;
            bool textVisible = active ? _config.ActiveTextVisible : _config.InactiveTextVisible;
            Sprite targetSprite = active ? activeSprite : inactiveSprite;

            if (iconTransform != null)
            {
                iconTransform.localScale = Vector3.one * targetScale;
                var pos = iconTransform.localPosition;
                pos.y = targetPosY;
                iconTransform.localPosition = pos;
            }

            if (label != null)
            {
                label.gameObject.SetActive(textVisible);
            }

            if (background != null && targetSprite != null)
            {
                background.sprite = targetSprite;
            }
        }

        private IEnumerator TransitionCoroutine(bool active)
        {
            float duration = _config.ButtonTransitionDuration;
            float elapsed = 0f;

            float startScale = iconTransform != null ? iconTransform.localScale.x : 1f;
            float startPosY = iconTransform != null ? iconTransform.localPosition.y : 0f;

            float targetScale = active ? _config.ActiveIconScale : _config.InactiveIconScale;
            float targetPosY = active ? _config.ActiveIconPositionY : _config.InactiveIconPositionY;
            bool textVisible = active ? _config.ActiveTextVisible : _config.InactiveTextVisible;
            Sprite targetSprite = active ? activeSprite : inactiveSprite;

            if (background != null && targetSprite != null)
            {
                background.sprite = targetSprite;
            }

            if (label != null)
            {
                label.gameObject.SetActive(textVisible);
            }

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

                if (iconTransform != null)
                {
                    float currentScale = Mathf.Lerp(startScale, targetScale, t);
                    iconTransform.localScale = Vector3.one * currentScale;

                    float currentPosY = Mathf.Lerp(startPosY, targetPosY, t);
                    var pos = iconTransform.localPosition;
                    pos.y = currentPosY;
                    iconTransform.localPosition = pos;
                }

                yield return null;
            }

            SetStateImmediate(active);
            _transitionCoroutine = null;
        }
    }
}



