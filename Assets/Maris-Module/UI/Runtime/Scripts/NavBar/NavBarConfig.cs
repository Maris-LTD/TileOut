using UnityEngine;

namespace GameModules.UI.NavBar
{
    [CreateAssetMenu(fileName = "NavBarConfig", menuName = "Maris Module/UI/NavBar Config")]
    public class NavBarConfig : ScriptableObject
    {
        [Header("Default Settings")]
        [SerializeField] private int defaultPageIndex;

        [Header("Animation Duration")]
        [SerializeField] private float buttonTransitionDuration = 0.3f;
        [SerializeField] private float pageTransitionDuration = 0.3f;

        [Header("Button Active State")]
        [SerializeField] private float activeIconScale = 1f;
        [SerializeField] private float activeIconPositionY = 60f;
        [SerializeField] private bool activeTextVisible = true;

        [Header("Button Inactive State")]
        [SerializeField] private float inactiveIconScale = 0.8f;
        [SerializeField] private float inactiveIconPositionY = 10f;
        [SerializeField] private bool inactiveTextVisible = false;

        public int DefaultPageIndex => defaultPageIndex;
        public float ButtonTransitionDuration => buttonTransitionDuration;
        public float PageTransitionDuration => pageTransitionDuration;
        public float ActiveIconScale => activeIconScale;
        public float ActiveIconPositionY => activeIconPositionY;
        public bool ActiveTextVisible => activeTextVisible;
        public float InactiveIconScale => inactiveIconScale;
        public float InactiveIconPositionY => inactiveIconPositionY;
        public bool InactiveTextVisible => inactiveTextVisible;
    }
}



