using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MateInN.UI.Reward
{
    public class RewardItemView : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text amountText;

        public void Setup(RewardData data)
        {
            if (data == null)
            {
                gameObject.SetActive(false);
                return;
            }

            if (iconImage != null)
            {
                if (data.Icon != null)
                {
                    iconImage.sprite = data.Icon;
                    iconImage.enabled = true;
                }
                else
                {
                    iconImage.enabled = false;
                }
            }

            if (amountText != null)
            {
                amountText.text = data.Amount;
            }

            gameObject.SetActive(true);
        }
    }
}

