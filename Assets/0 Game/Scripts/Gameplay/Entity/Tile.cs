using System;
using Game.Gameplay.Data;
using UnityEngine;

namespace Game.Gameplay.Entities
{
    public class Tile
    {
        public Vector2Int GridPosition { get; private set; }
        public DirectionType Direction { get; private set; }
        public bool IsMoved { get; private set; }
        
        public Tile(Vector2Int gridPosition, DirectionType direction)
        {
            GridPosition = gridPosition;
            Direction = direction;
            IsMoved = false;
        }

        public Vector2Int GetTargetPosition()
        {
            return GridPosition + GetDirectionOffset(Direction);
        }

        public void MarkAsMoved()
        {
            IsMoved = true;
        }

        public void SetPosition(Vector2Int gridPosition)
        {
            GridPosition = gridPosition;
        }

        public void Reset()
        {
            IsMoved = false;
        }

        private Vector2Int GetDirectionOffset(DirectionType direction)
        {
            return direction switch
            {
                DirectionType.Up => new Vector2Int(0, 1) * 100,
                DirectionType.Down => new Vector2Int(0, -1) * 100,
                DirectionType.Right => new Vector2Int(1, 0) * 100,
                DirectionType.Left => new Vector2Int(-1, 0) * 100,
                _ => throw new NotImplementedException()
            };
        }
    }
}