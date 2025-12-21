using Game.Gameplay.Data;
using Game.Gameplay.Entities;
using Game.Gameplay.Event;
using Game.Gameplay.Services;
using GameModules.CameraSystem.Authoring;
using GameModules.CameraSystem.Core;
using GameModules.Core;
using GameModules.Systems.Events;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

namespace Game.Camera
{
    [RequireComponent(typeof(CameraRigAuthoring))]
    [AutoInject(ModuleScope.Scene)]
    public sealed class GameCameraSetup : MonoBehaviour
    {
        [SerializeField]
        private string gameplayConfigPath = "GameplayConfig";

        private IGlobalEventBus _eventBus;
        private IGameplayService _gameplayService;
        private ICameraDirectorService _cameraDirector;
        private CameraRigAuthoring _cameraRig;
        private UnityEngine.Camera _unityCamera;
        private CinemachineBrain _cinemachineBrain;
        private GameplayConfig _config;

        [Inject]
        public void Construct(IGlobalEventBus eventBus, IGameplayService gameplayService, ICameraDirectorService cameraDirector)
        {
            _eventBus = eventBus;
            _gameplayService = gameplayService;
            _cameraDirector = cameraDirector;
        }

        private void Awake()
        {
            _cameraRig = GetComponent<CameraRigAuthoring>();
            _unityCamera = _cameraRig?.VirtualCamera?.GetComponentInChildren<UnityEngine.Camera>();
            if (_unityCamera == null)
            {
                _unityCamera = UnityEngine.Camera.main;
            }
            _cinemachineBrain = _unityCamera?.GetComponent<CinemachineBrain>();
            if (_cinemachineBrain == null && _unityCamera != null)
            {
                _cinemachineBrain = _unityCamera.gameObject.AddComponent<CinemachineBrain>();
            }
            LoadConfig();
        }

        private void Start()
        {
            if (_cameraDirector != null && _cameraRig != null && _unityCamera != null)
            {
                var context = new CameraRuntimeContext(null, _unityCamera, _cinemachineBrain);
                _cameraDirector.SwitchCamera(_cameraRig.CameraId, context);
            }
        }

        private void OnEnable()
        {
            _eventBus?.Subscribe<LevelInitializedEvent>(OnLevelInitialized);
        }

        private void OnDisable()
        {
            _eventBus?.Unsubscribe<LevelInitializedEvent>(OnLevelInitialized);
        }

        private void LoadConfig()
        {
            _config = Resources.Load<GameplayConfig>(gameplayConfigPath);
            if (_config == null)
            {
                Debug.LogError($"GameCameraSetup: Failed to load GameplayConfig from path: {gameplayConfigPath}");
            }
        }

        private void OnLevelInitialized(LevelInitializedEvent evt)
        {
            var tileGrid = _gameplayService?.GetTileGrid();
            if (tileGrid == null)
            {
                Debug.LogWarning("GameCameraSetup: TileGrid is null, cannot setup camera");
                return;
            }

            if (_config == null)
            {
                Debug.LogWarning("GameCameraSetup: GameplayConfig is null, cannot setup camera");
                return;
            }

            SetupCamera(tileGrid);
        }

        private void SetupCamera(TileGrid tileGrid)
        {
            if (_cameraRig?.VirtualCamera == null)
            {
                Debug.LogWarning("GameCameraSetup: CameraRig or VirtualCamera is null");
                return;
            }

            if (_unityCamera == null)
            {
                Debug.LogWarning("GameCameraSetup: Unity Camera is null");
                return;
            }

            var cellSize = _config.tileSize.x + _config.spacing.x;
            var gridBounds = CalculateGridBounds(tileGrid);
            
            if (gridBounds.size.x == 0 && gridBounds.size.y == 0)
            {
                Debug.LogWarning("GameCameraSetup: Grid is empty, cannot setup camera");
                return;
            }

            var gridWidth = gridBounds.size.x * cellSize;
            var gridHeight = gridBounds.size.y * cellSize;

            var centerPosition = CalculateGridCenter(gridBounds, cellSize);
            var orthographicSize = CalculateOrthographicSize(gridWidth, gridHeight);

            var virtualCameraTransform = _cameraRig.VirtualCamera.transform;
            var currentZ = virtualCameraTransform.position.z;
            virtualCameraTransform.position = new Vector3(centerPosition.x, centerPosition.y, currentZ);

            var lens = _cameraRig.VirtualCamera.Lens;
            lens.OrthographicSize = orthographicSize;
            _cameraRig.VirtualCamera.Lens = lens;

            if (_cameraDirector != null)
            {
                var context = new CameraRuntimeContext(null, _unityCamera, _cinemachineBrain);
                _cameraDirector.SwitchCamera(_cameraRig.CameraId, context);
            }
        }

        private BoundsInt CalculateGridBounds(TileGrid tileGrid)
        {
            int minX = int.MaxValue;
            int maxX = int.MinValue;
            int minY = int.MaxValue;
            int maxY = int.MinValue;

            foreach (var tile in tileGrid.GetAllTiles())
            {
                var pos = tile.GridPosition;
                minX = Mathf.Min(minX, pos.x);
                maxX = Mathf.Max(maxX, pos.x);
                minY = Mathf.Min(minY, pos.y);
                maxY = Mathf.Max(maxY, pos.y);
            }

            if (minX == int.MaxValue)
            {
                return new BoundsInt();
            }

            return new BoundsInt(minX, minY, 0, maxX - minX + 1, maxY - minY + 1, 1);
        }

        private Vector2 CalculateGridCenter(BoundsInt gridBounds, float cellSize)
        {
            var centerX = (gridBounds.min.x + gridBounds.max.x - 1) * 0.5f * cellSize;
            var centerY = (gridBounds.min.y + gridBounds.max.y - 1) * 0.5f * cellSize;

            return new Vector2(centerX, centerY);
        }

        private float CalculateOrthographicSize(float gridWidth, float gridHeight)
        {
            var screenHeight = Screen.height;
            var screenWidth = Screen.width;
            var aspect = (float)screenWidth / screenHeight;

            var sizeForHeight = (gridHeight / 2f) / 0.7f;
            var sizeForWidth = (gridWidth / 2f) / 0.95f / aspect;

            return Mathf.Max(sizeForHeight, sizeForWidth);
        }

        private void Update()
        {
            _cameraDirector?.Tick(Time.deltaTime);
        }
    }
}

