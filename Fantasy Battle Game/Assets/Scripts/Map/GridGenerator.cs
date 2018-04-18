using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.Map
{
    /// <summary>
    /// Class used to generate map, it should be attached to GameObject
    /// </summary>
    public class GridGenerator : GridMetrics
    {
        //Internal variables
        private static Dictionary<TileMetrics.HexCoordinate, Tile> tiles_ = new Dictionary<TileMetrics.HexCoordinate, Tile>();
        private static Random rand_;

        #region Public_Methods
        public void Awake()
        {
            if (Seed == 0)
                rand_ = new Random();
            else
                rand_ = new Random(Seed);
            GenerateGrid();
        }

        public void GenerateGrid()
        {
            ClearGrid();

            if (GridMetrics.Instance.Prefabs.Count == 0)
            {
                Debug.Log("Prefabs.Count should be greater than 0");
                return;
            }

            //Generate the grid shape
            switch (SelectedMapShape)
            {
                case MapShape.Hexagon:
                    GenerateHexagonShape();
                    break;

                case MapShape.Rectangle:
                    GenerateRectangleShape();
                    break;

                case MapShape.Parallelogram:
                    GenerateParallelogramShape();
                    break;

                case MapShape.Triangle:
                    GenerateTriangleShape();
                    break;
            }

            Map map = new Map();

            foreach (var tile in tiles_.Values)
            {
                for (var i = 0; i < 6; i++)
                {
                    var index = tile.Coordinate + GridMetrics.Directions[i];
                    if (tiles_.ContainsKey(index))
                    {
                        tile.GetNeighbours().Add(tiles_[index]);
                    }
                }
            }
        }

        public void ClearGrid()
        {
            Debug.Log("Clearing grid...");
            GameObject[] allTileAtScene = GameObject.FindGameObjectsWithTag("Tile");
            foreach (var tile in allTileAtScene)
            {
                DestroyImmediate(tile, false);
            }
        }

        #endregion

        #region Private_Methods

        private void GenerateTriangleShape()
        {
            throw new NotImplementedException();
        }

        private void GenerateParallelogramShape()
        {
            throw new NotImplementedException();
        }

        private void GenerateRectangleShape()
        {
            throw new NotImplementedException();
        }

        private void GenerateHexagonShape()
        {
            Debug.Log("Generating hexagonal shaped grid...");
            var position = Vector3.zero;
            var mapSize = Mathf.Max(MapWidth, MapHeight);

            for (var firstCoord = -mapSize; firstCoord <= mapSize; firstCoord++)
            {
                var r1 = Mathf.Max(-mapSize, -firstCoord - mapSize);
                var r2 = Mathf.Min(mapSize, -firstCoord + mapSize);
                for (var secondCoord = r1; secondCoord <= r2; secondCoord++)
                {
                    switch (SelectedHexOrientation)
                    {
                        case HexOrientation.Flat:
                            position.x = HexRadius * 3.0f / 2.0f * firstCoord;
                            position.z = HexRadius * Mathf.Sqrt(3.0f) * (secondCoord + firstCoord / 2.0f);
                            break;

                        case HexOrientation.Pointy:
                            position.x = HexRadius * Mathf.Sqrt(3.0f) * (firstCoord + secondCoord / 2.0f);
                            position.z = HexRadius * 3.0f / 2.0f * secondCoord;
                            break;
                    }

                    int typeOfHex = rand_.Next(GridMetrics.Instance.Prefabs.Count);
                    createHexGameObject(position, firstCoord, secondCoord, typeOfHex);
                }
            }
        }

        private Tile createHexGameObject(Vector3 position, int firstCoord, int secondCoord, int typeOfHex)
        {
            var go = Instantiate(GridMetrics.Instance.Prefabs[typeOfHex]);

            var tileGO = go.GetComponent<Tile>();
            tileGO.name = "Hex["+firstCoord+";"+secondCoord+"]";
            tileGO.Position = position;
            tileGO.transform.position = position;
            tileGO.transform.parent = transform;
            tileGO.Coordinate = new TileMetrics.HexCoordinate(firstCoord, secondCoord);

            go.transform.Rotate(Vector3.up*rand_.Next(5)*60);
            tiles_.Add(tileGO.Coordinate, tileGO);
            return tileGO;
        }

        #endregion
    }
}