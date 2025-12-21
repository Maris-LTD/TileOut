using System.Collections.Generic;
using System.Linq;
using Game.Gameplay.Entities;
using UnityEngine;

namespace Game.Gameplay.System
{
    public class TileMovementSystem
    {
        public bool CanMove(Tile tile, TileGrid grid)
        {
            if (tile == null || grid == null) return false;
            if (tile.IsMoved) return false;

            var targetPosition = tile.GetTargetPosition();
            if (!grid.IsDirectionBlockedByOtherTile(targetPosition, tile))
            {
                return true;
            }

            return false;
        }

        public bool MoveTile(Tile tile, TileGrid grid)
        {
            if(!CanMove(tile, grid)) return false;

            Vector2Int oldPosition = tile.GridPosition;
            Vector2Int newPosition = tile.GetTargetPosition();
            
            grid.RemoveTile(oldPosition);
            tile.SetPosition(newPosition);
            tile.MarkAsMoved();

            return true;
        }

        public List<Tile> GetAllMovableTiles(TileGrid grid)
        {
            if (grid == null) return new List<Tile>();
            
            return grid.GetAllTiles().Where(tile => CanMove(tile, grid)).ToList();
        }
        
        public bool HasAnyMovableTile(TileGrid grid)
        {
            if (grid == null)
                return false;

            return grid.GetAllTiles().Any(tile => CanMove(tile, grid));
        }
    }
}