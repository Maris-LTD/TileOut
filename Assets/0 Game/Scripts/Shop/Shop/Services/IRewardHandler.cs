using Cysharp.Threading.Tasks;
using IAPModule.Interfaces;

namespace IAPModule.Adapters.Shop.Services
{
    public interface IRewardHandler
    {
        void AddCoin(int amount);
        UniTask ShowRewardPopupAsync(IBaseRewardPayload rewardPayload, string title = "REWARDS");
    }
}
