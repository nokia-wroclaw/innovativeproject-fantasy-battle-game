using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.Map
{
    public class GridGenerator : GridMetrics
    {
        //Internal variables
        private static Dictionary<TileMetrics.Index, Tile> _tiles = new Dictionary<TileMetrics.Index, Tile>();
        private static Mesh _hexMesh;
        private static Random _rand;
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

        public void Awake()
        {
            if (Seed == 0)
                _rand = new Random();
            else
                _rand = new Random(Seed);
            GenerateGrid();
        }

        public Grid GenerateGrid()
        {
            ClearGrid();
            getMesh();

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
                    generateHexagonShape();
                    break;

                case MapShape.Rectangle:
                    generateRectangleShape();
                    break;

                case MapShape.Parrallelogram:
                    generateParrallelogramShape();
                    break;

                case MapShape.Triangle:
                    generateTriangleShape();
                    break;
            }

            Grid grid = new Grid
            {
                Tiles = _tiles,
                ProjectorsMaterial = ProjectorMaterial,
                HexRadius = HexRadius
            };

            foreach (var tile in _tiles.Values)
            {
                for (var i = 0; i < 6; i++)
                {
                    var index = tile.Coordinate + _directions[i];
                    if (_tiles.ContainsKey(index))
                        tile.GetNeighbours().Add(_tiles[index]);
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

        #region Private Methods
        private void getMesh()
        {
            _hexMesh = null;
            TileMetrics.GetHexMesh(HexRadius, SelectedHexOrientation, ref _hexMesh);
        }

        private void generateTriangleShape()
        {
            throw new NotImplementedException();
        }

        private void generateParrallelogramShape()
        {
            throw new NotImplementedException();
        }

        private void generateRectangleShape()
        {
            throw new NotImplementedException();
        }

        private void generateHexagonShape()
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
                    switch (base.SelectedHexOrientation)
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

                    int typeOfHex = _rand.Next(HexParametrs.Count);
                    var tile = createHexGameObject(position, "Hex[" + q + "," + r + "]", typeOfHex);
                    tile.Coordinate = new TileMetrics.Index(q, r);
                    tile.Position = position;
                    tile.Drag = HexParametrs[typeOfHex].drag;
                    tile.Available = HexParametrs[typeOfHex].available;
                    _tiles.Add(tile.Coordinate, tile);
                }
            }
        }

        private Tile createHexGameObject(Vector3 position, string name, int typeOfHex)
        {
            var go = new GameObject(name, typeof(Material), typeof(MeshFilter), typeof(LineRenderer),
                          typeof(MeshRenderer), typeof(Tile), typeof(MeshCollider));
            go.transform.position = position;
            go.transform.parent = transform;

            var tile = go.GetComponent<Tile>();
            var meshRenderer = go.GetComponent<MeshRenderer>();
            var renderer = go.GetComponent<Renderer>();
            renderer.material.shader = Shader.Find("Specular");
            renderer.material.SetColor("_SpecColor", Color.red);
            go.GetComponent<MeshCollider>().sharedMesh = _hexMesh;
            go.GetComponent<MeshFilter>().sharedMesh = _hexMesh;
            go.GetComponent<MeshRenderer>().material = HexParametrs[typeOfHex].material
                ? HexParametrs[typeOfHex].material
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
                    lines.SetPosition(vert, TileMetrics.Corner(tile.transform.position, HexRadius, vert, SelectedHexOrientation));
            }

            tile.TileGameObject = go;
            return tile;
        }
        #endregion


    }
}
