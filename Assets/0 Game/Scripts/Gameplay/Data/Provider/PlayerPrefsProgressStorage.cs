using UnityEngine;

namespace Game.Gameplay.Data.Provider
{
    public class PlayerPrefsProgressStorage : IGameProgressStorageStrategy
    {
        private const string SAVE_KEY = "GameProgress_SaveData";

        public GameProgress Load()
        {
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                string json = PlayerPrefs.GetString(SAVE_KEY);
                return JsonUtility.FromJson<GameProgress>(json) ?? new GameProgress();
            }
            return new GameProgress();
        }

        public void Save(GameProgress progress)
        {
            string json = JsonUtility.ToJson(progress);
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();
        }

        public bool HasData()
        {
            return PlayerPrefs.HasKey(SAVE_KEY);
        }

        public void Clear()
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            PlayerPrefs.Save();
        }
    }
}
