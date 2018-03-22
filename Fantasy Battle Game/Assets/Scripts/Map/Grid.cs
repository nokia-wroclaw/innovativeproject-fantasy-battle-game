using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class Grid : MonoBehaviour
    {
        //Internal variables
        public float HexRadius { get; set; }
        public Material ProjectorsMaterial { get; set; }
        public static Grid Instance { get; private set; }
        private Dictionary<TileMetrics.Index, Tile> _tiles = new Dictionary<TileMetrics.Index, Tile>();
        private readonly Dictionary<TileMetrics.Index, Tile> _tilesInRange = new Dictionary<TileMetrics.Index, Tile>();
        private static TileMetrics.Index[] _directions =
        {
            new TileMetrics.Index(1, -1),
            new TileMetrics.Index(1, 0),
            new TileMetrics.Index(0, 1),
            new TileMetrics.Index(-1, 1),
            new TileMetrics.Index(-1, 0),
            new TileMetrics.Index(0, -1)
        };

        #region Public Methods

        public Grid()
        {
            if (Instance == null)
                Instance = this;
        }

        public Dictionary<TileMetrics.Index, Tile> Tiles
        {
            get { return _tiles; }
            set { _tiles = value; }
        }

        public Dictionary<TileMetrics.Index, Tile> TilesInRange(Tile center, double range)
        {
            foreach (var tiles in _tilesInRange)
                tiles.Value.DeleteChildsGO();

            _tilesInRange.Clear();
            var neighbours = AvailableNeighbours(center);

            foreach (var tile in neighbours)
                if (center.Drag / 2 + tile.Drag / 2 <= range)
                {
                    _tilesInRange.Add(tile.Coordinate, tile);
                    tile.DistanceFromStart = center.Drag / 2 + tile.Drag / 2;
                }

            foreach (var tile in neighbours)
                if (_tilesInRange.ContainsKey(tile.Coordinate))
                    TilesInRangeRecursion(tile, range);

            _tilesInRange.Remove(center.Coordinate);

            return _tilesInRange;
        }


        public List<Tile> AvailableNeighbours(Tile center)
        {
            var listToReturn = new List<Tile>();

            for (var i = 0; i < 6; i++)
            {
                var index = center.Coordinate + _directions[i];
                if (_tiles.ContainsKey(index))
                    if (_tiles[index].Available)
                        listToReturn.Add(_tiles[index]);
            }
            return listToReturn;
        }

        #endregion

        #region Private Methods

        private void TilesInRangeRecursion(Tile currentTile, double range)
        {
            var neighbours = AvailableNeighbours(currentTile);
            foreach (var tile in neighbours)
                if (_tilesInRange.ContainsKey(tile.Coordinate))
                {
                    if (currentTile.DistanceFromStart + currentTile.Drag / 2 + tile.Drag / 2 < tile.DistanceFromStart)
                    {
                        tile.DistanceFromStart = currentTile.DistanceFromStart + currentTile.Drag / 2 + tile.Drag / 2;
                        TilesInRangeRecursion(tile, range);
                    }
                }
                else if (currentTile.DistanceFromStart + currentTile.Drag / 2 + tile.Drag / 2 <= range)
                {
                    _tilesInRange.Add(tile.Coordinate, tile);
                    tile.DistanceFromStart = currentTile.DistanceFromStart + currentTile.Drag / 2 + tile.Drag / 2;
                    TilesInRangeRecursion(tile, range);
                }
        }

        #endregion
        
    }
}