using System.Collections.Generic;
using Game.Gameplay.Data;
using Game.Gameplay.Entities;
using GameModules.Spawner.Core;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Views
{
    public class TileMapSpawner
    {
        private readonly Dictionary<Tile, TileView> _tileViewMap = new Dictionary<Tile, TileView>();
        private readonly List<TileView> _spawnedTileViews = new List<TileView>();
        
        private Transform _tilesParent;
        private GameObject _tileViewPrefab;
        private GameplayConfig _config;
        private float _tileSize;
        private float _spacing;
        private IObjectResolver _resolver;
        private ISpawner _spawner;
        private const string TILE_VIEW_PREFAB_PATH = "Prefabs/TileView";
        private const string GAMEPLAY_CONFIG_PATH = "GameplayConfig";
        private const string TILES_PARENT_NAME = "TilesParent";

        public void Initialize(IObjectResolver resolver, ISpawner spawner)
        {
            _resolver = resolver;
            _spawner = spawner;
            
            LoadConfig();
            LoadPrefab();
            CreateParent();
        }

        private void LoadConfig()
        {
            _config = Resources.Load<GameplayConfig>(GAMEPLAY_CONFIG_PATH);
            if (_config == null)
            {
                Debug.LogError($"TileMapSpawner: Failed to load GameplayConfig from path: {GAMEPLAY_CONFIG_PATH}");
                _tileSize = 1f;
                _spacing = 0.1f;
                return;
            }

            _tileSize = _config.tileSize.x;
            _spacing = _config.spacing.x;
        }

        private void LoadPrefab()
        {
            _tileViewPrefab = Resources.Load<GameObject>(TILE_VIEW_PREFAB_PATH);
            if (_tileViewPrefab == null)
            {
                Debug.LogError($"TileMapSpawner: Failed to load TileView prefab from path: {TILE_VIEW_PREFAB_PATH}");
            }
        }

        private void CreateParent()
        {
            GameObject parentObj = new GameObject(TILES_PARENT_NAME);
            _tilesParent = parentObj.transform;
            _tilesParent.position = Vector3.zero;
        }

        public void SpawnTileViews(TileGrid tileGrid)
        {
            if (tileGrid == null)
            {
                Debug.LogError("TileMapSpawner: TileGrid is null");
                return;
            }

            if (_tileViewPrefab == null)
            {
                Debug.LogError("TileMapSpawner: TileView prefab is null. Cannot spawn tiles.");
                return;
            }

            if (_tilesParent == null)
            {
                CreateParent();
            }

            foreach (var tileView in _spawnedTileViews)
            {
                if (tileView != null)
                {
                    _spawner?.Return(tileView.gameObject);
                }
            }

            _spawnedTileViews.Clear();
            _tileViewMap.Clear();

            foreach (Tile tile in tileGrid.GetAllTiles())
            {
                SpawnTileView(tile);
            }
        }

        private void SpawnTileView(Tile tile)
        {
            if (tile == null)
            {
                Debug.LogError("Tile is null");
                return;
            }

            GameObject tileObj = _spawner.Rent(_tileViewPrefab, _tilesParent);
            TileView tileView = tileObj.GetComponent<TileView>();

            if (tileView == null)
            {
                Debug.LogError("TileMapSpawner: TileView component not found on prefab!");
                _spawner.Return(tileObj);
                return;
            }

            _resolver?.Inject(tileView);
            tileView.SetUp(tile, _tileSize, _spacing);
            
            _spawnedTileViews.Add(tileView);
            _tileViewMap[tile] = tileView;
        }

        private TileView GetTileView(Tile tile)
        {
            if (tile == null)
                return null;

            return _tileViewMap.GetValueOrDefault(tile);
        }

        public void UpdateTilePosition(Tile tile, Vector2Int newGridPosition)
        {
            TileView tileView = GetTileView(tile);
            if (tileView != null)
            {
                Vector3 targetWorldPosition = GridToWorldPosition(newGridPosition, _tileSize, _spacing);
                tileView.PlayMoveAnimation(targetWorldPosition);
            }
        }

        public void PlayBlockedFeedback(Tile tile)
        {
            TileView tileView = GetTileView(tile);
            if (tileView != null)
            {
                tileView.PlayBlockedFeedback();
            }
        }

        public void PlayFadeOutForAll()
        {
            foreach (var tileView in _spawnedTileViews)
            {
                if (tileView != null && tileView.gameObject.activeInHierarchy)
                {
                    tileView.PlayFadeOutAnimation();
                }
            }
        }

        public void RemoveTileView(Tile tile)
        {
            if (tile == null)
                return;

            if (_tileViewMap.Remove(tile, out var view))
            {
                _spawnedTileViews.Remove(view);
                
                if (view != null)
                {
                    _spawner?.Return(view.gameObject);
                }
            }
        }

        public void Cleanup()
        {
            foreach (var tileView in _spawnedTileViews)
            {
                if (tileView != null)
                {
                    _spawner?.Return(tileView.gameObject);
                }
            }

            _spawnedTileViews.Clear();
            _tileViewMap.Clear();

            if (_tilesParent != null)
            {
                Object.Destroy(_tilesParent.gameObject);
                _tilesParent = null;
            }
        }

        private Vector3 GridToWorldPosition(Vector2Int gridPosition, float tileSize, float spacing)
        {
            float cellSize = tileSize + spacing;
            return new Vector3(
                gridPosition.x * cellSize,
                gridPosition.y * cellSize,
                0f
            );
        }
    }
}

