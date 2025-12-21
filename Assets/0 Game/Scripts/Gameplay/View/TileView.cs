using DG.Tweening;
using Game.Gameplay.Data;
using Game.Gameplay.Entities;
using Game.Gameplay.Event;
using GameModules.Systems.Events;
using uPools;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Views
{
    public class TileView : MonoBehaviour, IPoolCallbackReceiver
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private SpriteRenderer _arrowRenderer;
        [SerializeField] private Sprite[] _arrowSprites;
        [SerializeField] private BoxCollider2D _boxCollider;

        private Tile _tile;
        private IGlobalEventBus _bus;
        private bool _isInteractable = true;
        private Sequence _fadeOutSeq;
        private Sequence _blockSeq;

        public Tile Tile => _tile;

        [Inject] public void Construct(IGlobalEventBus bus) { _bus = bus; }

        private void ResetView()
        {
            this.transform.DOKill();
            _fadeOutSeq?.Kill();
            _blockSeq?.Kill();
            
            this.transform.localScale = Vector3.one;
            
            if (_spriteRenderer != null)
            {
                var color = _spriteRenderer.color;
                color.a = 1f;
                _spriteRenderer.color = color;
            }
            
            if (_arrowRenderer != null)
            {
                var color = _arrowRenderer.color;
                color.a = 1f;
                _arrowRenderer.color = color;
            }
            
            _isInteractable = true;
            if (_boxCollider != null)
            {
                _boxCollider.enabled = true;
            }
            
            _tile = null;
            
            gameObject.SetActive(true);
        }
        
        public void SetUp(Tile tile, float tileSize = 1f, float spacing = 0f)
        {
            _tile = tile;

            if (_boxCollider != null)
            {
                _boxCollider.size = new Vector2(tileSize, tileSize);
            }

            UpdateVisual(tileSize, spacing);
        }

        private void UpdateVisual(float tileSize = 1f, float spacing = 0f)
        {
            if (_tile == null) return;
            UpdateArrowDirection();
            UpdatePosition(tileSize, spacing);
        }

        private void UpdateArrowDirection()
        {
            if (_arrowSprites == null || _arrowSprites == null || _arrowSprites.Length < 4)
                return;

            int spriteIndex = _tile.Direction switch
            {
                DirectionType.Up => 0, DirectionType.Down => 1, DirectionType.Left => 2, DirectionType.Right => 3
                , _ => 0
            };

            if (spriteIndex < _arrowSprites.Length && _arrowSprites[spriteIndex] != null)
            {
                _arrowRenderer.sprite = _arrowSprites[spriteIndex];
            }
        }

        private void UpdatePosition(float tileSize, float spacing)
        {
            if (_tile == null)
                return;

            float cellSize = tileSize + spacing;
            Vector3 worldPosition = new Vector3(
                _tile.GridPosition.x * cellSize,
                _tile.GridPosition.y * cellSize,
                0f
            );

            transform.position = worldPosition;
        }

        private void OnMouseDown() { HandleTap(); }

        private void HandleTap()
        {
            if (!_isInteractable || _tile == null || _tile.IsMoved) return;

            _bus?.Publish(new TileTappedEvent(_tile));
        }

        public void SetInteractable(bool value)
        {
            _isInteractable = value;
            if (_boxCollider != null)
            {
                _boxCollider.enabled = value;
            }
        }

        public void PlayMoveAnimation(Vector3 targetWorldPosition, float duration = 10f)
        {
            if (!gameObject.activeInHierarchy) return;

            this.transform.DOKill();
            this.transform.DOMove(targetWorldPosition, duration).SetEase(Ease.OutQuad);
        }


        public void PlayFadeOutAnimation(float duration = 1f)
        {
            if (!gameObject.activeInHierarchy) return;
            _fadeOutSeq?.Kill();
            _fadeOutSeq = DOTween.Sequence();
            if (_spriteRenderer != null)
            {
                _fadeOutSeq.Insert(0, _spriteRenderer.DOFade(0f, duration)
                    .SetEase(Ease.InQuad));
            }

            if (_arrowRenderer != null)
            {
                _fadeOutSeq.Insert(0, _arrowRenderer.DOFade(0f, duration)
                    .SetEase(Ease.InQuad));
            }

            _fadeOutSeq.Insert(0, this.transform.DOScale(Vector3.zero, duration)
                .SetEase(Ease.InBack)
                .OnComplete(() => gameObject.SetActive(false)));
        }

        public void PlayBlockedFeedback()
        {
            if (!gameObject.activeInHierarchy) return;
            this.transform.localScale = Vector3.one;
            _blockSeq?.Kill();
            _blockSeq = DOTween.Sequence();
            _blockSeq.Insert(0, this.transform.DOScale(Vector3.one * 1.2f, 0.5f).SetLoops(2, LoopType.Yoyo));
        }
        
        public void OnRent()
        {
            ResetView();
        }

        public void OnReturn()
        {
            this.transform.DOKill();
            _fadeOutSeq?.Kill();
            _blockSeq?.Kill();
        }
        
        private void OnDestroy()
        {
            this.transform.DOKill();
            _blockSeq?.Kill();
            _fadeOutSeq?.Kill();
        }
    }
}