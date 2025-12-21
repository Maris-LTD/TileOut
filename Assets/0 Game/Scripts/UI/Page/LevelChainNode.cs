using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

namespace MateInN.UI
{
    public class LevelChainNode : MonoBehaviour
    {
        [SerializeField] private Image _highLight;
        [SerializeField] private Transform _node;
        [SerializeField] private Slider _slider;
        [SerializeField] private TMP_Text _levelText;

        private Sequence _seq;
        
        public void SetUp(int levelIndex)
        {
            _levelText.text = levelIndex.ToString();
            _highLight.gameObject.SetActive(false);
            _slider.value = 0;
            _seq?.Kill();
        }

        public void SetNextLevel(float slideDuration)
        {
            _highLight.color = new Color(1, 1, 1, 0);
            _slider.value = 0;
            _highLight.gameObject.SetActive(true);
            _seq?.Kill();
            _seq = DOTween.Sequence();
            _seq.Insert(0, _slider.DOValue(1, slideDuration));
            _seq.Insert(slideDuration, _node.DOScale(1.2f, 0.15f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo));
            _seq.Insert(slideDuration + 0.3f, _highLight.DOFade(1, 0.3f));
        }

        private void OnDestroy()
        {
            _seq?.Kill();
            if (_slider != null)
                DOTween.Kill(_slider);
            if (_node != null)
                DOTween.Kill(_node);
            if (_highLight != null)
                DOTween.Kill(_highLight);
        }
        
        
    }
}