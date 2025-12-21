using GameModules.UI.DTOs;

namespace Game.UI.Victory
{
    public class VictoryPopupData : IPopupData
    {
        public int LevelNumber { get; set; }
        public int Score { get; set; }
        public float LoadingProgress { get; set; } = 0f;
    }
}

