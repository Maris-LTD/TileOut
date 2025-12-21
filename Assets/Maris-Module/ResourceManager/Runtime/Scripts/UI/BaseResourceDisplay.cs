using GameModules.Core;
using GameModules.Systems.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace GameModules.ResourceManager.UI
{
    public abstract class BaseResourceDisplay : MonoBehaviour, IDependencyInjectable
    {
        [Header("Resource")]
        [SerializeField] protected ResourceDefinition definition;
        
        [Header("UI References")]
        [SerializeField] protected Image iconImage;
        [SerializeField] protected TMP_Text amountText;
        [SerializeField] protected Button plusButton;
        
        protected IResourceService ResourceService;
        protected IGlobalEventBus EventBus;
        
        protected long CurrentAmount;
        private bool _eventsSubscribed;
        
        public virtual void InjectDependencies(IObjectResolver resolver)
        {
            EventBus = resolver.Resolve<IGlobalEventBus>();
            ResourceService = resolver.Resolve<IResourceService>();
            
            if (!_eventsSubscribed && EventBus != null)
            {
                SubscribeEvents();
                _eventsSubscribed = true;
            }
            
            RefreshDisplay();
        }
        
        protected virtual void Start()
        {
            if (definition == null) return;
            SetupIcon();
            SetupPlusButton();
            
            if (!_eventsSubscribed && EventBus != null)
            {
                SubscribeEvents();
                _eventsSubscribed = true;
            }
            
            RefreshDisplay();
        }
        
        protected virtual void OnDestroy()
        {
            UnsubscribeEvents();
        }
        
        protected virtual void SetupIcon()
        {
            if (iconImage != null && definition.Icon != null)
            {
                iconImage.sprite = definition.Icon;
            }
        }
        
        protected virtual void SetupPlusButton()
        {
            if (plusButton != null)
            {
                plusButton.onClick.AddListener(OnPlusButtonClicked);
            }
        }
        
        protected virtual void SubscribeEvents()
        {
            EventBus?.Subscribe<ResourceChangedEvent>(OnResourceChanged);
            EventBus?.Subscribe<ResourceInitializedEvent>(OnResourceInitialized);
        }
        
        protected virtual void UnsubscribeEvents()
        {
            if (_eventsSubscribed && EventBus != null)
            {
                EventBus.Unsubscribe<ResourceChangedEvent>(OnResourceChanged);
                EventBus.Unsubscribe<ResourceInitializedEvent>(OnResourceInitialized);
                _eventsSubscribed = false;
            }
        }
        
        protected virtual void OnResourceChanged(ResourceChangedEvent evt)
        {
            if (definition == null || evt.ResourceId != definition.Id) return;
            
            CurrentAmount = evt.NewAmount;
            UpdateDisplay();
        }
        
        protected virtual void OnResourceInitialized(ResourceInitializedEvent evt)
        {
            RefreshDisplay();
        }
        
        protected virtual void RefreshDisplay()
        {
            if (definition == null || ResourceService == null)
            {
                return;
            }
            
            CurrentAmount = ResourceService.Get(definition.Id);
            UpdateDisplay();
        }
        
        protected virtual void UpdateDisplay()
        {
            if (amountText != null)
            {
                amountText.text = FormatAmount(CurrentAmount);
            }
        }
        
        protected abstract string FormatAmount(long amount);
        
        protected virtual void OnPlusButtonClicked()
        {
        }
        
        public void SetDefinition(ResourceDefinition newDefinition)
        {
            definition = newDefinition;
            SetupIcon();
            RefreshDisplay();
        }
        
        public ResourceDefinition GetDefinition() => definition;
        public long GetCurrentAmount() => CurrentAmount;
        
    }
}

