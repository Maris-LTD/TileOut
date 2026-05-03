namespace Game.Gameplay.Data.Provider
{
    public interface IGameProgressStorageStrategy
    {
        GameProgress Load();
        void Save(GameProgress progress);
        bool HasData();
        void Clear();
    }
}
