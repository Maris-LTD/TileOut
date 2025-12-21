using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay.Data
{
    [Serializable]
    public class LevelData
    {
        public List<NodeData> nodes;
        public Vector2Int gridSize;
    }

    [Serializable]
    public class NodeData
    {
        public Vector2 position;
        public DirectionType direction;
    }

    public enum DirectionType
    {
        Up,
        Down,
        Left,
        Right,
    }
}