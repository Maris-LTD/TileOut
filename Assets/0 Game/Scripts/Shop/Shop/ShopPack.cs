using IAPModule.Adapters.Shop.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;

namespace IAPModule.Adapters.Shop
{
    public class ShopPack : MonoBehaviour
    {
        [SerializeField] private Button _buyButton;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _amountText;
        [SerializeField] private bool _setIconNativeSize = true;

        private ShopPackData _packData;
        private Product _product;
        private IAPGameAdapter _iapAdapter;
        private IShopPurchaseService _purchaseService;

        public void Setup(ShopPackData packData, IAPGameAdapter iapAdapter, IShopPurchaseService purchaseService)
        {
            _packData = packData;
            _iapAdapter = iapAdapter;
            _purchaseService = purchaseService;

            if (_nameText != null)
            {
                _nameText.text = packData.DisplayName;
            }

            if (_descriptionText != null)
                _descriptionText.text = packData.Description;

            if (_amountText != null && !string.IsNullOrEmpty(packData.RewardAmountText))
            {
                _amountText.text = FormatPrice(packData.RewardAmountText);
            }
            else if (_amountText != null)
            {
                _amountText.text = "";
            }

            if (packData.Icon != null)
            {
                _icon.sprite = packData.Icon;
                if (_setIconNativeSize)
                {
                    _icon.SetNativeSize();
                }
            }
            
            _buyButton.onClick.RemoveAllListeners();
            _buyButton.onClick.AddListener(OnBuyButtonClicked);

            if (iapAdapter != null && (iapAdapter.IsInitialized || iapAdapter.IsDevMode))
            {
                _product = iapAdapter.GetProduct(packData.ProductId);
                UpdatePrice();
                UpdatePurchaseAvailability();
            }
            else
            {
                _priceText.text = "Loading...";
                _buyButton.interactable = false;
            }
        }

        private void UpdatePrice()
        {
            if (_iapAdapter != null && _iapAdapter.IsDevMode)
            {
                _priceText.text = $"{_packData.DefaultPrice}$";
            }
            else
            {
                _priceText.text = _product is { metadata: not null } ? _product.metadata.localizedPriceString : "N/A";
            }
        }

        private void UpdatePurchaseAvailability()
        {
            if (_packData == null || _iapAdapter == null) return;

            var canPurchase = true;

            if (_purchaseService != null)
            {
                canPurchase = _purchaseService.CanPurchase(_packData);
            }

            if (_iapAdapter.IsDevMode)
            {
                _buyButton.interactable = canPurchase;
            }
            else
            {
                _buyButton.interactable = canPurchase && _iapAdapter.IsInitialized && _product != null;
            }
        }

        private void OnBuyButtonClicked()
        {
            if (_iapAdapter == null || _packData == null) return;

            if (_purchaseService != null)
            {
                if (!_purchaseService.CanPurchase(_packData))
                {
                    Debug.LogWarning($"Cannot purchase {_packData.ProductId}: Max purchase count reached");
                    return;
                }
            }

            _iapAdapter.PurchaseProduct(_packData.ProductId);
        }

        public void OnIAPInitialized()
        {
            if (_iapAdapter != null && _packData != null)
            {
                if (_iapAdapter.IsDevMode || _iapAdapter.IsInitialized)
                {
                    _product = _iapAdapter.GetProduct(_packData.ProductId);
                    UpdatePrice();
                    UpdatePurchaseAvailability();
                }
            }
        }

        public void OnPurchaseComplete(string productId, bool success)
        {
            if (productId == _packData?.ProductId && success)
            {
                if (_purchaseService != null)
                {
                    _purchaseService.RecordPurchase(productId);
                }

                UpdatePurchaseAvailability();
            }
        }

        private string FormatPrice(string price)
        {
            return int.Parse(price).ToString("N0");
        }
    }
}