using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class Grid
    {
        public float HexRadius { get; set; }
        public Material ProjectorsMaterial { get; set; }
        public static Grid Instance { get; private set; }
        private static TileMetrics.HexCoordinate[] directions_ =
        {
            new TileMetrics.HexCoordinate(1, -1),
            new TileMetrics.HexCoordinate(1, 0),
            new TileMetrics.HexCoordinate(0, 1),
            new TileMetrics.HexCoordinate(-1, 1),
            new TileMetrics.HexCoordinate(-1, 0),
            new TileMetrics.HexCoordinate(0, -1)
        };

        private Dictionary<TileMetrics.HexCoordinate, Tile> tilesInRange_ = new Dictionary<TileMetrics.HexCoordinate, Tile>();

        private Dictionary<TileMetrics.HexCoordinate, Tile> tiles_ = new Dictionary<TileMetrics.HexCoordinate, Tile>();

        #region Public_Methods

        public Grid()
        {
            if (Instance == null)
            {
                Instance = this;
            }
                
        }

        public Dictionary<TileMetrics.HexCoordinate, Tile> TilesInRangeDictionary
        {
            get { return tilesInRange_; }
            set { tilesInRange_ = value; }
        }

        public Dictionary<TileMetrics.HexCoordinate, Tile> TilesInRange(Tile center, double range)
        {
            TilesInRangeDictionary.Clear();

            var visited = new Dictionary<TileMetrics.HexCoordinate, bool>();
            visited.Add(center.Coordinate, center);

            var queue = new Queue<Tile>();

            var neighbours = AvailableNeighbours(center);
            foreach (var tile in neighbours)
                if (center.Drag / 2 + tile.Drag / 2 <= range)
                {
                    TilesInRangeDictionary.Add(tile.Coordinate, tile);
                    tile.DistanceFromStart = center.Drag / 2 + tile.Drag / 2;
                    queue.Enqueue(tile);
                }

            while (queue.Count != 0)
            {
                var currentTile = queue.Dequeue();
                neighbours = AvailableNeighbours(currentTile);
                foreach (var tile in neighbours)
                    if (TilesInRangeDictionary.ContainsKey(tile.Coordinate))
                    {
                        if (currentTile.DistanceFromStart + currentTile.Drag / 2 + tile.Drag / 2 <
                            tile.DistanceFromStart)
                        {
                            tile.DistanceFromStart =
                                currentTile.DistanceFromStart + currentTile.Drag / 2 + tile.Drag / 2;
                            queue.Enqueue(tile);
                        }
                    }
                    else if (currentTile.DistanceFromStart + currentTile.Drag / 2 + tile.Drag / 2 <= range)
                    {
                        TilesInRangeDictionary.Add(tile.Coordinate, tile);
                        tile.DistanceFromStart = currentTile.DistanceFromStart + currentTile.Drag / 2 + tile.Drag / 2;
                        queue.Enqueue(tile);
                    }
            }

            TilesInRangeDictionary.Remove(center.Coordinate);

            return TilesInRangeDictionary;
        }


        public List<Tile> AvailableNeighbours(Tile center)
        {
            var listToReturn = new List<Tile>();
            var neighbours = center.GetNeighbours();
            foreach (var tile in neighbours)
                if (tile.Available)
                    listToReturn.Add(tile);
            return listToReturn;
        }

        #endregion
    }
}