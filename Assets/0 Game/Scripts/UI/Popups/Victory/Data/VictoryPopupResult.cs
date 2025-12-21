namespace Game.UI.Victory
{
    public enum VictoryAction
    {
        Home,
        NextLevel
    }

    public class VictoryPopupResult
    {
        public VictoryAction Action { get; set; }
    }
}

