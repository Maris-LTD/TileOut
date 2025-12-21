using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace GameModules.UI.NavBar
{
    public class NavBarController : MonoBehaviour
    {
        [Header("Config")] [SerializeField] private NavBarConfig config;

        [Header("Pages Container")] [SerializeField]
        private RectTransform pagesContainer;

        [SerializeField] private CanvasGroup pagesCanvasGroup;

        [Header("Pages (In Order)")] [SerializeField]
        private List<BasePage> pages = new();

        [Header("Buttons")] [SerializeField] private List<NavBarButton> buttons = new();

        [Header("Settings")] [SerializeField] private int defaultPageIndex = 1;

        private IPageTransitionService _transitionService;
        private int _currentPageIndex = -1;
        private bool _isInitialized;
        private bool _isTransitioning;

        public int CurrentPageIndex => _currentPageIndex;
        public event Action<int, int> OnPageChanged;

        [Inject] public void Construct() { }

        private void Start() { Initialize(); }

        private void Initialize()
        {
            if (_isInitialized) return;

            _transitionService = new PageTransitionService(config, pagesCanvasGroup);

            InitializePages();
            InitializeButtons();

            SelectPage(defaultPageIndex, false);

            _isInitialized = true;
        }

        private void OnDestroy()
        {
            foreach (var button in buttons)
            {
                if (button != null)
                {
                    button.OnButtonClicked -= HandleButtonClicked;
                }
            }
        }

        private void InitializePages()
        {
            if (pages == null || pages.Count == 0)
            {
                if (pagesContainer != null)
                {
                    pages = new List<BasePage>();
                    foreach (Transform child in pagesContainer)
                    {
                        var page = child.GetComponent<BasePage>();
                        if (page != null)
                        {
                            pages.Add(page);
                        }
                    }
                }
            }

            for (int i = 0; i < pages.Count; i++)
            {
                var page = pages[i];
                if (page != null)
                {
                    _transitionService.RegisterPage(i, page);
                    if (i != defaultPageIndex)
                    {
                        page.Sleep();
                    }
                }
            }
        }

        private void InitializeButtons()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                var button = buttons[i];
                if (button == null) continue;

                button.Initialize(config, i);
                button.OnButtonClicked += HandleButtonClicked;
            }
        }

        private void HandleButtonClicked(int index) { SelectPage(index); }

        public void SelectPage(int index, bool animate = true)
        {
            if (_isTransitioning) return;
            if (_currentPageIndex == index) return;
            if (index < 0 || index >= pages.Count) return;

            SelectPageAsync(index, animate).Forget();
        }

        private async UniTaskVoid SelectPageAsync(int index, bool animate)
        {
            _isTransitioning = true;

            try
            {
                int previousIndex = _currentPageIndex;
                
                UpdateButtonStates(index, animate);

                if (animate && previousIndex >= 0)
                {
                    await _transitionService.TransitionToPage(previousIndex, index);
                }
                else
                {
                    await _transitionService.TransitionToPage(-1, index);
                }

                _currentPageIndex = index;
                OnPageChanged?.Invoke(previousIndex, index);
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        private void UpdateButtonStates(int activeIndex, bool animate)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                var button = buttons[i];
                if (button == null) continue;
                button.SetActive(i == activeIndex, animate);
            }
        }

        public void AddPage(BasePage page)
        {
            int index = pages.Count;
            pages.Add(page);
            _transitionService?.RegisterPage(index, page);
            page.Sleep();
        }

        public void GoToPage(int index) { SelectPage(index); }
    }
}