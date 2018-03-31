using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Grid = UnityEngine.Grid;
using Random = System.Random;

namespace Map
{
    public class GridGenerator : GridMetrics
    {
        //Internal variables
        private static Dictionary<TileMetrics.HexCoordinate, Tile> tiles_ = new Dictionary<TileMetrics.HexCoordinate, Tile>();
        private static Mesh hexMesh_;
        private static Random rand_;

        private static TileMetrics.HexCoordinate[] directions_ =
        {
            new TileMetrics.HexCoordinate(1, -1),
            new TileMetrics.HexCoordinate(1, 0),
            new TileMetrics.HexCoordinate(0, 1),
            new TileMetrics.HexCoordinate(-1, 1),
            new TileMetrics.HexCoordinate(-1, 0),
            new TileMetrics.HexCoordinate(0, -1)
        };

        #region Public_Methods

        public void Awake()
        {
            if (Seed == 0)
                rand_ = new Random();
            else
                rand_ = new Random(Seed);
            GenerateGrid();
        }

        public Grid GenerateGrid()
        {
            ClearGrid();
            GetMesh();

            if (HexParametrs.Count == 0)
            {
                Debug.Log(HexParametrs.Count);
                Debug.Log("HexParametrs.Count should be greater than 0");
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

                case MapShape.Parrallelogram:
                    GenerateParrallelogramShape();
                    break;

                case MapShape.Triangle:
                    GenerateTriangleShape();
                    break;
            }

            Grid grid = new Grid
            {
                TilesInRangeDictionary = tiles_,
                ProjectorsMaterial = ProjectorMaterial,
                HexRadius = HexRadius
            };

            foreach (var tile in tiles_.Values)
            {
                for (var i = 0; i < 6; i++)
                {
                    var index = tile.Coordinate + directions_[i];
                    if (tiles_.ContainsKey(index))
                        tile.GetNeighbours().Add(tiles_[index]);
                }
            }

            return grid;
        }

        public void ClearGrid()
        {
            Debug.Log("Clearing grid...");
            GameObject[] allTileAtScean = GameObject.FindGameObjectsWithTag("Tile");
            foreach (var tile in allTileAtScean)
                DestroyImmediate(tile, false);
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

        private void GenerateParrallelogramShape()
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

                    int typeOfHex = rand_.Next(HexParametrs.Count);
                    var tile = createHexGameObject(position, "Hex[" + q + "," + r + "]", typeOfHex);
                    tile.Coordinate = new TileMetrics.HexCoordinate(q, r);
                    tile.Position = position;
                    tile.Drag = HexParametrs[typeOfHex].Drag;
                    tile.Available = HexParametrs[typeOfHex].Available;
                    tiles_.Add(tile.Coordinate, tile);
                }
            }
        }

        private Tile createHexGameObject(Vector3 position, string nameOfGameObject, int typeOfHex)
        {
            var go = new GameObject(nameOfGameObject, typeof(MeshFilter), typeof(LineRenderer),
                typeof(MeshRenderer), typeof(Tile), typeof(MeshCollider));
            go.transform.position = position;
            go.transform.parent = transform;

            var tile = go.GetComponent<Tile>();
            var renderer = go.GetComponent<Renderer>();
            renderer.material.shader = Shader.Find("Specular");
            renderer.material.SetColor("_SpecColor", Color.red);
            go.GetComponent<MeshCollider>().sharedMesh = hexMesh_;
            go.GetComponent<MeshFilter>().sharedMesh = hexMesh_;
            go.GetComponent<MeshRenderer>().material = HexParametrs[typeOfHex].Material
                ? HexParametrs[typeOfHex].Material
                : AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");

            if (DrawOutlines)
            {
                var lines = go.GetComponent<LineRenderer>();

                lines.useLightProbes = false;
                lines.receiveShadows = false;

                lines.SetWidth(0.1f, 0.1f);
                lines.SetColors(Color.black, Color.black);
                lines.material = LineMaterial;

                lines.SetVertexCount(7);

                for (var vert = 0; vert <= 6; vert++)
                    lines.SetPosition(vert,
                        TileMetrics.Corner(tile.transform.position, HexRadius, vert, SelectedHexOrientation));
            }

            tile.TileGameObject = go;
            return tile;
        }

        #endregion
    }
}