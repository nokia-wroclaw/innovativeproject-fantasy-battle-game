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

        public Dictionary<TileMetrics.Index, Tile> TilesInRangeDictionary
        {
            get { return _tilesInRange; }
        }

        public Dictionary<TileMetrics.Index, Tile> TilesInRange(Tile center, double range)
        {
            TilesInRangeDictionary.Clear();

            Dictionary<TileMetrics.Index, bool> visited = new Dictionary<TileMetrics.Index, bool>();
            visited.Add(center.Coordinate, center);

            Queue<Tile> queue = new Queue<Tile>();

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
                        if (currentTile.DistanceFromStart + currentTile.Drag / 2 + tile.Drag / 2 < tile.DistanceFromStart)
                        {
                            tile.DistanceFromStart = currentTile.DistanceFromStart + currentTile.Drag / 2 + tile.Drag / 2;
                            queue.Enqueue(tile);
                        }
                    }
                    else if (currentTile.DistanceFromStart + currentTile.Drag / 2 + tile.Drag / 2 <= range)
                    {
                        TilesInRangeDictionary.Add(tile.Coordinate, tile);
                        tile.DistanceFromStart = currentTile.DistanceFromStart + currentTile.Drag / 2 + tile.Drag / 2;                        queue.Enqueue(tile);
                    }
            }
            
            TilesInRangeDictionary.Remove(center.Coordinate);

            return TilesInRangeDictionary;
        }


        public List<Tile> AvailableNeighbours(Tile center)
        {
            var listToReturn = new List<Tile>();
            var neighbours = center.GetNeighbours();
            foreach (Tile tile in neighbours)
                if(tile.Available)
                    listToReturn.Add(tile);
            return listToReturn;
        }

        #endregion
        
    }
}