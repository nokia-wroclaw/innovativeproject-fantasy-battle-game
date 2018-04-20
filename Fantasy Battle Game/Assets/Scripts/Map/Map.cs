using System.Collections.Generic;
using Assets.Scripts.Map.Interfaces;
using UnityEngine;
using System;

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
        private Tile selectedTile_;
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

        public Tile SelectedTile
        {
            get { return selectedTile_; }
            set { selectedTile_ = value; }
        }

        #endregion

        public List<KeyValuePair<TileMetrics.HexCoordinate, int>> RandomPositions(int amountofCreaturesFirstPlayer, int amountofCreaturesSecondPlayer)
        {
            List<KeyValuePair<TileMetrics.HexCoordinate, int>> list = new List<KeyValuePair<TileMetrics.HexCoordinate, int>>();

            var mapSize = Mathf.Max(GridMetrics.Instance.MapHeight, GridMetrics.Instance.MapWidth);
            System.Random random = new System.Random();

            for (int i = 0; i < amountofCreaturesFirstPlayer;)
            {
                TileMetrics.HexCoordinate coordinate;
                coordinate.FirstCoord = random.Next(0, 2)-mapSize;
                coordinate.SecondCoord = random.Next(0, mapSize);
                if (tiles_.ContainsKey(coordinate))
                {
                    if (!list.Contains(new KeyValuePair<TileMetrics.HexCoordinate, int>(coordinate, 0)))
                    {
                        var randTile = tiles_[coordinate];
                        if (randTile.Available)
                        {
                            i++;
                            list.Add(new KeyValuePair<TileMetrics.HexCoordinate, int>(coordinate, 0));
                            //Debug.Log("first player:" + coordinate.FirstCoord + " " + coordinate.SecondCoord);
                        }
                    }
                }
            }
            for (int i = 0; i < amountofCreaturesSecondPlayer;)
            {
                TileMetrics.HexCoordinate coordinate;
                coordinate.FirstCoord = mapSize - random.Next(0, 2);
                coordinate.SecondCoord = -random.Next(0, mapSize);
                if (tiles_.ContainsKey(coordinate) && !list.Contains(new KeyValuePair<TileMetrics.HexCoordinate, int>(coordinate, 1)))
                {
                    
                    var randTile = tiles_[coordinate];
                    if (randTile.Available)
                    {
                        i++;
                        list.Add(new KeyValuePair<TileMetrics.HexCoordinate, int>(coordinate, 1));
                        //Debug.Log("second player:" + coordinate.FirstCoord + " " + coordinate.SecondCoord);
                    }
                }
            }
            return list;
        }

        public Tile GetTile(TileMetrics.HexCoordinate hexCoordinate)
        {
            if (tiles_.ContainsKey(hexCoordinate))
            {
                    return tiles_[hexCoordinate];
            }
            return null;
        }
        
        public Dictionary<TileMetrics.HexCoordinate, Tile> TilesInRange(Tile center, double range)
        {
            ClearMarkTilesInRange();
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
            markTilesInRange();
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

        public void ClearMarkTilesInRange()
        {
            foreach (var tiles in TilesInRangeDictionary)
            {
                tiles.Value.DeleteChildsGO();
            }
        }

        #endregion

        #region Private_Methods



        private void markTilesInRange()
        {
            foreach (Tile tile in TilesInRangeDictionary.Values)
            {
                Tile.AddProjector(tile);
                Tile.AddLabel(tile);
            }
        }
        #endregion
    }
}