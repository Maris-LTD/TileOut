using GameModules.Core;
using UnityEngine;
using VContainer;

namespace GameModules.UI.NavBar
{
    public abstract class BasePage : MonoBehaviour, IDependencyInjectable
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private CanvasGroup canvasGroup;

        private bool _isAwake = true;
        private int _pageIndex;
        protected IObjectResolver _objectResolver;

        public int PageIndex => _pageIndex;
        public RectTransform RectTransform => rectTransform;
        public CanvasGroup PageCanvasGroup => canvasGroup;
        public bool IsAwake => _isAwake;

        protected virtual void Awake()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
        }

        public void InjectDependencies(IObjectResolver resolver)
        {
            _objectResolver = resolver;
            OnDependencyInjected();
            var injectables = GetComponentsInChildren<IDependencyInjectable>(true);
            foreach (var injectable in injectables)
            {
                if (this as IDependencyInjectable == injectable)
                {
                    continue;
                }

                injectable.InjectDependencies(_objectResolver);
            }
        }


        public void SetPageIndex(int index) { _pageIndex = index; }

        public virtual void Sleep()
        {
            _isAwake = false;
            OnPageWillExit();
            gameObject.SetActive(false);
            OnPageDidExit();
        }

        public virtual void WakeUp()
        {
            OnPageWillEnter();
            gameObject.SetActive(true);
            _isAwake = true;
            OnPageDidEnter();
        }

        public virtual void OnTransitionComplete() { OnPageTransitionComplete(); }

        protected virtual void OnPageWillEnter() { }

        protected virtual void OnPageDidEnter() { }

        protected virtual void OnPageWillExit() { }

        protected virtual void OnPageDidExit() { }

        protected virtual void OnPageTransitionComplete() { }
        protected virtual void OnDependencyInjected() { }
    }
}