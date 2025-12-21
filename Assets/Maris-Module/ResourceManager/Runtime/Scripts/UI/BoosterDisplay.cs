using UnityEngine;

namespace GameModules.ResourceManager.UI
{
    public class BoosterDisplay : BaseResourceDisplay
    {
        [Header("Booster Specific")]
        [SerializeField] private string format = "x{0}";
        [SerializeField] private bool hideWhenZero = true;
        [SerializeField] private GameObject countContainer;
        
        protected override string FormatAmount(long amount)
        {
            return string.Format(format, amount);
        }
        
        protected override void UpdateDisplay()
        {
            base.UpdateDisplay();
            
            if (hideWhenZero && countContainer != null)
            {
                countContainer.SetActive(CurrentAmount > 0);
            }
        }
    }
}

