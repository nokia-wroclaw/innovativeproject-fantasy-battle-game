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
        private static Mesh hexMesh_;
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

        public Map GenerateGrid()
        {
            ClearGrid();
            GetMesh();

            if (HexParameters.Count == 0)
            {
                Debug.Log(HexParameters.Count);
                Debug.Log("HexParameters.Count should be greater than 0");
                return null;
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

            return map;
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

        private void GetMesh()
        {
            hexMesh_ = null;
            TileMetrics.GetHexMesh(HexRadius, SelectedHexOrientation, ref hexMesh_);
        }

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

            for (var q = -mapSize; q <= mapSize; q++)
            {
                var r1 = Mathf.Max(-mapSize, -q - mapSize);
                var r2 = Mathf.Min(mapSize, -q + mapSize);
                for (var r = r1; r <= r2; r++)
                {
                    switch (SelectedHexOrientation)
                    {
                        case HexOrientation.Flat:
                            position.x = HexRadius * 3.0f / 2.0f * q;
                            position.z = HexRadius * Mathf.Sqrt(3.0f) * (r + q / 2.0f);
                            break;

                        case HexOrientation.Pointy:
                            position.x = HexRadius * Mathf.Sqrt(3.0f) * (q + r / 2.0f);
                            position.z = HexRadius * 3.0f / 2.0f * r;
                            break;
                    }

                    int typeOfHex = rand_.Next(HexParameters.Count);
                    var tile = createHexGameObject(position, "Hex[" + q + "," + r + "]", typeOfHex);
                    tile.Coordinate = new TileMetrics.HexCoordinate(q, r);
                    tile.Position = position;
                    tile.Drag = HexParameters[typeOfHex].Drag;
                    tile.Available = HexParameters[typeOfHex].Available;
                    tiles_.Add(tile.Coordinate, tile);
                }
            }
        }

        private Tile createHexGameObject(Vector3 position, string nameOfGameObject, int typeOfHex)
        {
            var gameObject = new GameObject(
                nameOfGameObject, 
                typeof(MeshFilter), 
                typeof(LineRenderer), 
                typeof(MeshRenderer), 
                typeof(Tile), 
                typeof(MeshCollider));

            gameObject.transform.position = position;
            gameObject.transform.parent = transform;

            var tile = gameObject.GetComponent<Tile>();
            var renderer = gameObject.GetComponent<Renderer>();
            renderer.material.shader = Shader.Find("Specular");
            renderer.material.SetColor("_SpecColor", Color.red);
            gameObject.GetComponent<MeshCollider>().sharedMesh = hexMesh_;
            gameObject.GetComponent<MeshFilter>().sharedMesh = hexMesh_;
            gameObject.GetComponent<MeshRenderer>().material = HexParameters[typeOfHex].Material
                ? HexParameters[typeOfHex].Material
                : AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");

            if (DrawOutlines)
            {
                var lines = gameObject.GetComponent<LineRenderer>();

                lines.useLightProbes = false;
                lines.receiveShadows = false;

                lines.SetWidth(0.1f, 0.1f);
                lines.SetColors(Color.black, Color.black);
                lines.material = LineMaterial;

                lines.SetVertexCount(7);

                for (var iter = 0; iter <= 6; iter++)
                {
                    Vector3 positionOfCorner = TileMetrics.Corner(tile.transform.position, HexRadius, iter, SelectedHexOrientation);
                    lines.SetPosition(iter, positionOfCorner);
                }
            }

            tile.TileGameObject = gameObject;
            return tile;
        }

        #endregion
    }
}