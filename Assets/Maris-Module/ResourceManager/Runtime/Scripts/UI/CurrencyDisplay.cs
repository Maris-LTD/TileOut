using GameModules.Core;

namespace GameModules.ResourceManager.UI
{
    [AutoInject]
    public class CurrencyDisplay : BaseResourceDisplay
    {
        protected override string FormatAmount(long amount)
        {
            if (amount >= 1_000_000_000_000)
                return $"{amount / 1_000_000_000_000f:0.#}T";
            
            if (amount >= 1_000_000_000)
                return $"{amount / 1_000_000_000f:0.#}B";
            
            if (amount >= 1_000_000)
                return $"{amount / 1_000_000f:0.#}M";
            
            if (amount >= 1_000)
                return $"{amount / 1_000f:0.#}K";
            
            return amount.ToString("N0");
        }
    }
}

