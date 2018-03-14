using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace Assets.Map
{
    public class Grid : MonoBehaviour
    {
        public static Grid Inst;

        //Generation Options
        public bool AddColliders = true;
        public bool DrawOutlines = true;

        //Map settings
        public MapShape MapShape = MapShape.Rectangle;
        public int MapWidth;
        public int MapHeight;

        //Hex Settings
        public HexOrientation HexOrientation = HexOrientation.Flat;
        public float HexRadius = 1;
        public Material LineMaterial;

        //Parameters of random hex
        public List<Material> HexMaterialsList = new List<Material>();
        public List<bool> AvailableList = new List<bool>();
        public List<double> DragList = new List<double>();

        //Internal variables
        private readonly Dictionary<string, Tile> _grid = new Dictionary<string, Tile>();
        private Mesh _hexMesh;
        private readonly CubeIndex[] _directions =
        {
            new CubeIndex(1, -1, 0),
            new CubeIndex(1, 0, -1),
            new CubeIndex(0, 1, -1),
            new CubeIndex(-1, 1, 0),
            new CubeIndex(-1, 0, 1),
            new CubeIndex(0, -1, 1)
        };
        private readonly Random _rand = new Random();


        #region Getters and Setters

        public Dictionary<string, Tile> Tiles
        {
            get { return _grid; }
        }

        #endregion

        #region Public Methods

        public void GenerateGrid()
        {
            if (HexMaterialsList.Count != DragList.Count || DragList.Count == 0 ||
                AvailableList.Count != HexMaterialsList.Count)
            {
                Debug.Log("hexMaterialsList.size != dragList.size != availableList.size or size==0");

                return;
            }

            GetMesh();

            //Generate the grid shape
            switch (MapShape)
            {
                case MapShape.Hexagon:
                    GenHexShape();
                    break;

                case MapShape.Rectangle:
                    GenRectShape();
                    break;

                case MapShape.Parrallelogram:
                    GenParrallShape();
                    break;

                case MapShape.Triangle:
                    GenTriShape();
                    break;
            }
        }

        /// <summary>
        ///     Method destory each hex in grid
        /// </summary>
        public void ClearGrid()
        {
            // TODO doesnt destroy after press play in unity
            Debug.Log("Clearing grid...");
            foreach (var tile in _grid)
                DestroyImmediate(tile.Value.gameObject, false);

            _grid.Clear();
        }

        /// <summary>
        ///    Find tile in grid
        /// </summary>
        /// <param name="index">CubeIndex coordinate</param>
        /// <returns>Tile located at coordinate. If tile not exist return null</returns>
        public Tile TileAt(CubeIndex index)
        {
            if (_grid.ContainsKey(index.ToString()))
                return _grid[index.ToString()];
            return null;
        }

        /// <summary>
        ///     Find tile in grid
        /// </summary>
        /// <param name="q">first coord</param>
        /// <param name="r">second coord</param>
        /// <param name="s">third coord</param>
        /// <returns>Tile located at coordinate. If tile not exist return null</returns>
        public Tile TileAt(int q, int r, int s)
        {
            return TileAt(new CubeIndex(q, r, s));
        }

        /// <summary>
        ///     Find tile in grid
        /// </summary>
        /// <param name="q">first coord</param>
        /// <param name="s">third coord</param>
        /// <returns>Tile located at coordinate. If tile not exist return null</returns>
        public Tile TileAt(int q, int s)
        {
            return TileAt(new CubeIndex(q, s));
        }

        /// <summary>
        ///     Method find list of available neighbours selected tile
        /// </summary>
        /// <param name="tile">selected tile</param>
        /// <returns>list of available neighbours</returns>
        public List<Tile> Neighbours(Tile tile)
        {
            var listToReturn = new List<Tile>();

            for (var i = 0; i < 6; i++)
            {
                CubeIndex o = tile.Index + _directions[i];
                if (_grid.ContainsKey(o.ToString()))
                    if (_grid[o.ToString()].Available)
                        listToReturn.Add(_grid[o.ToString()]);
            }

            return listToReturn;
        }

        /// <summary>
        ///     Method find list of available neighbours next to selected cubeindex
        /// </summary>
        /// <param name="index">cubeindex</param>
        /// <returns>list of available neighbours</returns>
        public List<Tile> Neighbours(CubeIndex index)
        {
            return Neighbours(TileAt(index));
        }

        /// <summary>
        ///     Method find list of available neighbours next to selected coordinates
        /// </summary>
        /// <param name="q">first coord</param>
        /// <param name="r">second coord</param>
        /// <param name="s">third coord</param>
        /// <returns>list of available neighbours</returns>
        public List<Tile> Neighbours(int q, int r, int s)
        {
            return Neighbours(TileAt(q, r, s));
        }

        /// <summary>
        ///     Method find list of available neighbours next to selected coordinates
        /// </summary>
        /// <param name="q">first coord</param>
        /// <param name="s">third coord</param>
        /// <returns>list of available neighbours</returns>
        public List<Tile> Neighbours(int q, int s)
        {
            return Neighbours(TileAt(q, s));
        }

        /// <summary>
        ///     Method calculate which tile is in range from selected tile
        ///     Beta version- Each tile is available, drag is const
        /// </summary>
        /// <param name="center">start tile</param>
        /// <param name="range">range</param>
        /// <returns>List of tiles in range</returns>
        public List<Tile> TilesInRange(Tile center, int range)
        {
            // TODO Dijkstra's algorithm with drag support 
            var ret = new List<Tile>();
            CubeIndex o;

            for (var dx = -range; dx <= range; dx++)
            for (var dy = Mathf.Max(-range, -dx - range); dy <= Mathf.Min(range, -dx + range); dy++)
            {
                o = new CubeIndex(dx, dy, -dx - dy) + center.Index;
                if (_grid.ContainsKey(o.ToString()))
                    ret.Add(_grid[o.ToString()]);
            }

            return ret;
        }

        /*
	public List<Tile> TilesInRange(CubeIndex index, int range){
		return TilesInRange(TileAt(index), range);
	}

	public List<Tile> TilesInRange(int x, int y, int z, int range){
		return TilesInRange(TileAt(x,y,z), range);
	}

	public List<Tile> TilesInRange(int x, int z, int range){
		return TilesInRange(TileAt(x,z), range);
	}
    */

        // Distance beetwen two cubeindex, drag is const
        public int Distance(CubeIndex a, CubeIndex b)
        {
            return Mathf.Abs(a.q - b.q) + Mathf.Abs(a.r - b.r) + Mathf.Abs(a.s - b.s);
        }

        // Distance beetwen two tile, drag is const
        public int Distance(Tile a, Tile b)
        {
            return Distance(a.Index, b.Index);
        }

        #endregion

        #region Private Methods

        private void Awake()
        {
            if (!Inst)
                Inst = this;
        }


        private void GetMesh()
        {
            _hexMesh = null;
            Tile.GetHexMesh(HexRadius, HexOrientation, ref _hexMesh);
        }

        private void GenHexShape()
        {
            Debug.Log("Generating hexagonal shaped grid...");

            Tile tile;
            var pos = Vector3.zero;

            var mapSize = Mathf.Max(MapWidth, MapHeight);

            for (var q = -mapSize; q <= mapSize; q++)
            {
                var r1 = Mathf.Max(-mapSize, -q - mapSize);
                var r2 = Mathf.Min(mapSize, -q + mapSize);
                for (var r = r1; r <= r2; r++)
                {
                    switch (HexOrientation)
                    {
                        case HexOrientation.Flat:
                            pos.x = HexRadius * 3.0f / 2.0f * q;
                            pos.z = HexRadius * Mathf.Sqrt(3.0f) * (r + q / 2.0f);
                            break;

                        case HexOrientation.Pointy:
                            pos.x = HexRadius * Mathf.Sqrt(3.0f) * (q + r / 2.0f);
                            pos.z = HexRadius * 3.0f / 2.0f * r;
                            break;
                    }

                    var index = _rand.Next(HexMaterialsList.Count);
                    tile = CreateHexGameObject(pos, "Hex[" + q + "," + r + "," + (-q - r) + "]", index);
                    tile.Index = new CubeIndex(q, r, -q - r);
                    tile.Drag = DragList[index];
                    tile.Position = pos;
                    tile.Available = AvailableList[index];
                    _grid.Add(tile.Index.ToString(), tile);
                }
            }
        }

        private void GenRectShape()
        {
            Debug.Log("Generating rectangular shaped grid...");

            Tile tile;
            var pos = Vector3.zero;

            switch (HexOrientation)
            {
                case HexOrientation.Flat:
                    for (var q = 0; q < MapWidth; q++)
                    {
                        var qOff = q >> 1;
                        for (var r = -qOff; r < MapHeight - qOff; r++)
                        {
                            pos.x = HexRadius * 3.0f / 2.0f * q;
                            pos.z = HexRadius * Mathf.Sqrt(3.0f) * (r + q / 2.0f);

                            var index = _rand.Next(HexMaterialsList.Count);
                            tile = CreateHexGameObject(pos, "Hex[" + q + "," + r + "," + (-q - r) + "]", index);
                            tile.Index = new CubeIndex(q, r, -q - r);
                            tile.Drag = DragList[index];
                            tile.Available = AvailableList[index];
                            tile.Position = pos;
                            _grid.Add(tile.Index.ToString(), tile);
                        }
                    }

                    break;

                case HexOrientation.Pointy:
                    for (var r = 0; r < MapHeight; r++)
                    {
                        var rOff = r >> 1;
                        for (var q = -rOff; q < MapWidth - rOff; q++)
                        {
                            pos.x = HexRadius * Mathf.Sqrt(3.0f) * (q + r / 2.0f);
                            pos.z = HexRadius * 3.0f / 2.0f * r;

                            var index = _rand.Next(HexMaterialsList.Count);
                            tile = CreateHexGameObject(pos, "Hex[" + q + "," + r + "," + (-q - r) + "]", index);
                            tile.Index = new CubeIndex(q, r, -q - r);
                            tile.Drag = DragList[index];
                            tile.Position = pos;
                            tile.Available = AvailableList[index];
                            _grid.Add(tile.Index.ToString(), tile);
                        }
                    }

                    break;
            }
        }

        private void GenParrallShape()
        {
            Debug.Log("Generating parrellelogram shaped grid...");

            Tile tile;
            var pos = Vector3.zero;

            for (var q = 0; q <= MapWidth; q++)
            for (var r = 0; r <= MapHeight; r++)
            {
                switch (HexOrientation)
                {
                    case HexOrientation.Flat:
                        pos.x = HexRadius * 3.0f / 2.0f * q;
                        pos.z = HexRadius * Mathf.Sqrt(3.0f) * (r + q / 2.0f);
                        break;

                    case HexOrientation.Pointy:
                        pos.x = HexRadius * Mathf.Sqrt(3.0f) * (q + r / 2.0f);
                        pos.z = HexRadius * 3.0f / 2.0f * r;
                        break;
                }

                var index = _rand.Next(HexMaterialsList.Count);
                tile = CreateHexGameObject(pos, "Hex[" + q + "," + r + "," + (-q - r) + "]", index);
                tile.Index = new CubeIndex(q, r, -q - r);
                tile.Drag = DragList[index];
                tile.Available = AvailableList[index];
                tile.Position = pos;
                _grid.Add(tile.Index.ToString(), tile);
            }
        }

        private void GenTriShape()
        {
            Debug.Log("Generating triangular shaped grid...");

            Tile tile;
            var pos = Vector3.zero;

            var mapSize = Mathf.Max(MapWidth, MapHeight);

            for (var q = 0; q <= mapSize; q++)
            for (var r = 0; r <= mapSize - q; r++)
            {
                switch (HexOrientation)
                {
                    case HexOrientation.Flat:
                        pos.x = HexRadius * 3.0f / 2.0f * q;
                        pos.z = HexRadius * Mathf.Sqrt(3.0f) * (r + q / 2.0f);
                        break;

                    case HexOrientation.Pointy:
                        pos.x = HexRadius * Mathf.Sqrt(3.0f) * (q + r / 2.0f);
                        pos.z = HexRadius * 3.0f / 2.0f * r;
                        break;
                }

                var index = _rand.Next(HexMaterialsList.Count);
                tile = CreateHexGameObject(pos, "Hex[" + q + "," + r + "," + (-q - r) + "]", index);
                tile.Index = new CubeIndex(q, r, -q - r);
                tile.Drag = DragList[index];
                tile.Available = AvailableList[index];
                tile.Position = pos;
                _grid.Add(tile.Index.ToString(), tile);
            }
        }

        /// <summary>
        ///     Method generate game objects- hexes. To generate hex use material from hexMaterialsList,
        ///     drag from hexMaterialsList, available from availableList
        /// </summary>
        /// <param name="position">position in space</param>
        /// <param name="name">coords [q,r,s]</param>
        /// <param name="type">type of hex to generate (position in hexMaterialsList, hexMaterialsList, available)</param>
        /// <returns>Generated Tile</returns>
        private Tile CreateHexGameObject(Vector3 position, string name, int type)
        {
            var go = new GameObject(name, typeof(Material), typeof(MeshFilter), typeof(MeshRenderer), typeof(Tile));

            if (AddColliders)
                go.AddComponent<MeshCollider>();

            if (DrawOutlines)
                go.AddComponent<LineRenderer>();

            go.transform.position = position;
            go.transform.parent = transform;

            var tile = go.GetComponent<Tile>();
            var fil = go.GetComponent<MeshFilter>();
            var ren = go.GetComponent<MeshRenderer>();
            var rend = go.GetComponent<Renderer>();
            rend.material.shader = Shader.Find("Specular");
            rend.material.SetColor("_SpecColor", Color.red);

            fil.sharedMesh = _hexMesh;

            ren.material = HexMaterialsList[type]
                ? HexMaterialsList[type]
                : AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");

            if (AddColliders)
            {
                var col = go.GetComponent<MeshCollider>();
                col.sharedMesh = _hexMesh;
            }

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
                    lines.SetPosition(vert, Tile.Corner(tile.transform.position, HexRadius, vert, HexOrientation));
            }

            return tile;
        }

        #endregion
    }

    [Serializable]
    public enum MapShape
    {
        Rectangle,
        Hexagon,
        Parrallelogram,
        Triangle
    }

    [Serializable]
    public enum HexOrientation
    {
        Pointy,
        Flat
    }
}