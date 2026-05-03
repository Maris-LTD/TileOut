using GameModules.DataManager;

namespace Game.Gameplay.Data.Provider
{
    public class GameProgressProvider : IDataProvider
    {
        private readonly IGameProgressStorageStrategy _storageStrategy;
        private GameProgress _cachedProgress;

        public GameProgressProvider()
        {
            _storageStrategy = new PlayerPrefsProgressStorage();
        }

        public GameProgressProvider(IGameProgressStorageStrategy storageStrategy)
        {
            _storageStrategy = storageStrategy;
        }

        public bool LoadData(object data)
        {
            try
            {
                _cachedProgress = _storageStrategy.Load();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public object GetData() { return _cachedProgress ??= _storageStrategy.Load(); }

        public object SaveData()
        {
            if (_cachedProgress != null)
            {
                _storageStrategy.Save(_cachedProgress);
            }
            return _cachedProgress;
        }

        public bool HasData()
        {
            return _storageStrategy.HasData() || _cachedProgress != null;
        }

        public void ClearData()
        {
            _cachedProgress = null;
            _storageStrategy.Clear();
        }
        
        // Helper method for easy access
        private GameProgress GetProgress()
        {
            return (GameProgress)GetData();
        }
        
        public void SaveHighestLevel(int level)
        {
            var progress = GetProgress();
            if (level > progress.HighestLevel)
            {
                progress.HighestLevel = level;
                SaveData();
            }
        }
    }
}
