using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.Map
{
    public class Tile : MonoBehaviour
    {
        public double DistanceFromStart { get; internal set; }
        public Vector3 Position;
        public double Drag;
        public bool Available = false;
        public TileMetrics.Index Coordinate;
        public GameObject TileGameObject;

        private GameObject _labelGameObject;
        private GameObject _projectorGameObject;
        private List<Tile> _neighbours = new List<Tile>();
        private ChampionsManager _championsManager;
        private GameObject _champion;

        void Start()
        {
            _championsManager = ChampionsManager.Instance;
        }

        public void DeleteChildsGO()
        {
            if (_labelGameObject)
                Destroy(_labelGameObject);
            if (_projectorGameObject)
                Destroy(_projectorGameObject);
        }

        public List<Tile> GetNeighbours()
        {
            return _neighbours;
        }

        #region Interaction with user

        private void OnMouseDown()
        {
            var grid = Grid.Instance;
            Debug.Log(grid.HexRadius);
            var gridHexRadius = grid.HexRadius;
            
            foreach (var tiles in grid.TilesInRangeDictionary)
                tiles.Value.DeleteChildsGO();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var tilesInRange = grid.TilesInRange(this, 20);
            stopwatch.Stop();
            Debug.Log(stopwatch.Elapsed.TotalMilliseconds + "ms");
            foreach (Tile tile in tilesInRange.Values)
            {

                // Add Projector to tile
                GameObject go = new GameObject("Shadow");
                go.transform.parent = tile.TileGameObject.transform;
                go.transform.position = tile.Position + new Vector3(0, gridHexRadius * 2, 0);
                go.transform.Rotate(new Vector3(90, 0, 0));
                tile._projectorGameObject = go;
                go.AddComponent<Projector>();
                var projector = go.GetComponent<Projector>();
                projector.material = grid.ProjectorsMaterial;

                // Add label to tile
                GameObject labelGameObject = new GameObject("Label");
                labelGameObject.transform.parent = tile.TileGameObject.transform;
                labelGameObject.transform.position = tile.Position;
                labelGameObject.transform.Rotate(new Vector3(90, 0, 0));
                tile._labelGameObject = labelGameObject;
                var text = labelGameObject.AddComponent<TextMesh>();
                text.text = Convert.ToString(Math.Round(tile.DistanceFromStart, 1));
                text.fontSize = (int) (grid.HexRadius * 9);
                text.anchor = TextAnchor.MiddleCenter;
                text.color = Color.white;
            }

            if (_championsManager.GetChampionToSpawn() != null)
            {
                if (_champion == null)
                {
                    GameObject championToSpawn = _championsManager.GetChampionToSpawn();
                    _champion = (GameObject) Instantiate(championToSpawn, transform.position, transform.rotation);
                }
            }
        }

        #endregion
    }
}
