using System.Collections.Generic;
using Assets.Scripts.Map.Interfaces;

namespace Assets.Scripts.Map
{
    /// <summary>
    /// Class represent map based at hexagonal tiles.
    /// <para>To get instance of map you should use property Instance.</para>
    /// <para>Tiles are stored at dictionary tiles_. Keys are hexCoordiante
    /// based at axial coordinate system </para>
    /// <para>https://www.redblobgames.com/grids/hexagons/#coordinates-axial </para>
    /// </summary>
    public class Map : IMap
    {
        private static Map instance_;
        private Dictionary<TileMetrics.HexCoordinate, Tile> tilesInRange_ = new Dictionary<TileMetrics.HexCoordinate, Tile>();
        private Dictionary<TileMetrics.HexCoordinate, Tile> tiles_ = new Dictionary<TileMetrics.HexCoordinate, Tile>();

        #region Public_Methods
        public Map()
        {
            if (Instance == null)
            {
                instance_ = this;
            }
        }

        #region Properties
        public static Map Instance
        {
            get { return instance_; }
            set { instance_ = value; }
        }

        public Dictionary<TileMetrics.HexCoordinate, Tile> Tiles
        {
            get { return this.tiles_; }
            set { this.tiles_ = value; }
        }

        public Dictionary<TileMetrics.HexCoordinate, Tile> TilesInRangeDictionary
        {
            get { return tilesInRange_; }
        }
        #endregion
        
        public Dictionary<TileMetrics.HexCoordinate, Tile> TilesInRange(Tile center, double range)
        {
            tilesInRange_.Clear();

            var visited = new Dictionary<TileMetrics.HexCoordinate, bool>();
            visited.Add(center.Coordinate, center);

            var queueTiles = new Queue<Tile>();

            var neighbours = AvailableNeighbours(center);
            foreach (var tile in neighbours)
            {
                if (center.Drag / 2 + tile.Drag / 2 <= range)
                {
                    tilesInRange_.Add(tile.Coordinate, tile);
                    tile.DistanceFromStart = center.Drag / 2 + tile.Drag / 2;
                    queueTiles.Enqueue(tile);
                }
            }

            while (queueTiles.Count != 0)
            {
                var currentTile = queueTiles.Dequeue();
                neighbours = AvailableNeighbours(currentTile);
                foreach (var tile in neighbours)
                    if (tilesInRange_.ContainsKey(tile.Coordinate))
                    {
                        if (currentTile.DistanceFromStart + currentTile.Drag / 2 + tile.Drag / 2 <
                            tile.DistanceFromStart)
                        {
                            tile.DistanceFromStart =
                                currentTile.DistanceFromStart + currentTile.Drag / 2 + tile.Drag / 2;
                            queueTiles.Enqueue(tile);
                        }
                    }
                    else if (currentTile.DistanceFromStart + currentTile.Drag / 2 + tile.Drag / 2 <= range)
                    {
                        tilesInRange_.Add(tile.Coordinate, tile);
                        tile.DistanceFromStart = currentTile.DistanceFromStart + currentTile.Drag / 2 + tile.Drag / 2;
                        queueTiles.Enqueue(tile);
                    }
            }
            tilesInRange_.Remove(center.Coordinate);
            return tilesInRange_;
        }


        public List<Tile> AvailableNeighbours(Tile center)
        {
            var listToReturn = new List<Tile>();
            var neighbours = center.GetNeighbours();
            foreach (var tile in neighbours)
            {
                if (tile.Available)
                {
                    listToReturn.Add(tile);
                }
            }
            return listToReturn;
        }
        #endregion
    }
}