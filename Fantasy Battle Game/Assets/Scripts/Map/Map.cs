using System.Collections.Generic;
using UnityEngine;
using System;
using BattleManagement;
using Champions;
using Map.Interfaces;

namespace Map
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
        private Dictionary<TileMetrics.HexCoordinate, Tile> tilesWithChampions_ = new Dictionary<TileMetrics.HexCoordinate, Tile>();
        private Dictionary<TileMetrics.HexCoordinate, Tile> tilesNextToEnemyInRange_ = new Dictionary<TileMetrics.HexCoordinate, Tile>();
        private TurnManagement turnManagement_;

        #region Public_Methods
        public Map()
        {
            instance_ = Instance == null ? this : null;
            turnManagement_ = TurnManagement.Instance;
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

        public Dictionary<TileMetrics.HexCoordinate, Tile> TilesWithChampions
        {
            get { return tilesWithChampions_; }
            set { tilesWithChampions_ = value; }
        }

        public Dictionary<TileMetrics.HexCoordinate, Tile> TilesNextToEnemyInRange
        {
            get { return tilesNextToEnemyInRange_; }
            set { tilesNextToEnemyInRange_ = value; }
        }

        public void ClearCosts()
        {
            foreach (var tile in tiles_)
            {
                
            }
        }
        #endregion

        public void MarkTilesNextToEnemyInRange(Tile tileWithEnemy)
        {
            tilesNextToEnemyInRange_.Clear();
            foreach (var tile in tileWithEnemy.GetNeighbours())
            {
                if (tilesInRange_.ContainsKey(tile.Coordinate))
                {
                    tilesNextToEnemyInRange_.Add(tile.Coordinate, tile);
                }
                else if(tile == ChampionsManager.Instance.SelectedChampion.CurrentPossition)
                {
                    tilesNextToEnemyInRange_.Add(tile.Coordinate, tile);
                }
            }
            foreach (Tile tile in tilesNextToEnemyInRange_.Values)
            {
                Tile.AddProjector(tile);
            }

        }


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
            TilesWithChampions.Clear();
            tilesInRange_.Clear();

            var visited = new Dictionary<TileMetrics.HexCoordinate, bool>();
            var queueTiles = new Queue<Tile>();

            visited.Add(center.Coordinate, center);
            queueTiles.Enqueue(center);
            center.DistanceFromStart = 0;

            while (queueTiles.Count != 0)
            {
                var currentTile = queueTiles.Dequeue();
                var neighbours = currentTile.GetNeighbours();
                foreach (var tile in neighbours)
                {
                    if (tile.Available)
                    {
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
                            tile.DistanceFromStart =
                                currentTile.DistanceFromStart + currentTile.Drag / 2 + tile.Drag / 2;
                            queueTiles.Enqueue(tile);
                        }
                    }
                    else
                    {
                        if (tile.Champion && !TilesWithChampions.ContainsKey(tile.Coordinate) && tile.Champion.Owner == turnManagement_.CurrentPlayer)
                        {

                            TilesWithChampions.Add(tile.Coordinate, tile);
                        }
                    }
                }
            }
            tilesInRange_.Remove(center.Coordinate);
            TilesWithChampions.Remove(center.Coordinate);

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
            foreach (var tiles in TilesWithChampions)
            {
                tiles.Value.DeleteChildsGO();
            }
            foreach (var tiles in TilesNextToEnemyInRange)
            {
                tiles.Value.DeleteChildsGO();
            }
        }

        public void MarkUnitInRange(Player.Player currentPlayer)
        {
            foreach (Tile tile in TilesWithChampions.Values)
            {
                Tile.AddProjectorUnitInRange(tile);
            }
        }

        public void MarkTilesInRange()
        {
            //markUnitsInRange();
            foreach (Tile tile in TilesInRangeDictionary.Values)
            {
                Tile.AddProjector(tile);
            }
        }

        #endregion

        #region Private_Methods




        #endregion
    }
}