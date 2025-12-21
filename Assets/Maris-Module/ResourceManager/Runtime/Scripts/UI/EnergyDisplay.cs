using System;
using Cysharp.Threading.Tasks;
using GameModules.Core;
using TMPro;
using UnityEngine;

namespace GameModules.ResourceManager.UI
{
    [AutoInject]
    public class EnergyDisplay : BaseResourceDisplay
    {
        [Header("Energy Specific")]
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private string timerFormat = "{0:mm\\:ss}";
        [SerializeField] private string fullText = "FULL";
        
        private RegenerativeResourceDefinition _regenDefinition;
        private bool _isTimerRunning;
        
        protected override void Start()
        {
            _regenDefinition = definition as RegenerativeResourceDefinition;
            base.Start();
            StartTimerUpdate().Forget();
        }
        
        protected override void OnDestroy()
        {
            _isTimerRunning = false;
            base.OnDestroy();
        }
        
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            EventBus?.Subscribe<EnergyRegeneratedEvent>(OnEnergyRegenerated);
        }
        
        protected override void UnsubscribeEvents()
        {
            EventBus?.Unsubscribe<EnergyRegeneratedEvent>(OnEnergyRegenerated);
            base.UnsubscribeEvents();
        }
        
        private void OnEnergyRegenerated(EnergyRegeneratedEvent evt)
        {
            if (definition == null || evt.ResourceId != definition.Id) return;
            UpdateTimerDisplay();
        }
        
        protected override string FormatAmount(long amount)
        {
            if (_regenDefinition == null)
                return amount.ToString();
            
            return $"{amount}/{_regenDefinition.MaxCapacity}";
        }
        
        protected override void UpdateDisplay()
        {
            base.UpdateDisplay();
            UpdateTimerDisplay();
        }
        
        private void UpdateTimerDisplay()
        {
            if (timerText == null || _regenDefinition == null) return;
            
            if (CurrentAmount >= _regenDefinition.MaxCapacity)
            {
                timerText.text = fullText;
                return;
            }
            
            var secondsUntilRegen = ResourceService?.GetSecondsUntilNextRegen(definition.Id) ?? 0;
            var timeSpan = TimeSpan.FromSeconds(secondsUntilRegen);
            timerText.text = string.Format(timerFormat, timeSpan);
        }
        
        private async UniTaskVoid StartTimerUpdate()
        {
            _isTimerRunning = true;
            
            while (_isTimerRunning && this != null)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: destroyCancellationToken);
                
                if (_isTimerRunning && CurrentAmount < _regenDefinition?.MaxCapacity)
                {
                    UpdateTimerDisplay();
                }
            }
        }
    }
}

