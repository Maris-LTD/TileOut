using UnityEngine;

namespace Game.Gameplay.Data
{
    [CreateAssetMenu(fileName = "GameplayConfig",menuName = "Game/Gameplay/GameplayConfig")]
    public class GameplayConfig : ScriptableObject
    {
        public float animationSpeed;
        public Vector2 tileSize;
        public Vector2 spacing;
    }
}