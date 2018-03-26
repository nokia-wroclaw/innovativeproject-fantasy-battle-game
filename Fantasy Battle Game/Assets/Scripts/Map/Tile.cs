using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = UnityEngine.Debug;

namespace assets.scripts.map
{
    public class Tile : MonoBehaviour
    {
        public double DistanceFromStart { get; internal set; }
        public Vector3 Position;
        public double Drag;
        public bool Available = false;
        public TileMetrics.HexCoordinate Coordinate;
        public GameObject TileGameObject;
        public GameObject Champion { set; get; }
        private GameObject labelGameObject_;
        private GameObject projectorGameObject_;
        private readonly List<Tile> neighbours_ = new List<Tile>();
        private ChampionsManager championsManager_;

        void Start()
        {
            championsManager_ = ChampionsManager.Instance;
        }

        public void DeleteChildsGO()
        {
            if (labelGameObject_)
                Destroy(labelGameObject_);
            if (projectorGameObject_)
                Destroy(projectorGameObject_);
        }

        public List<Tile> GetNeighbours()
        {
            return neighbours_;
        }

        #region Interaction_with_user

        private void OnMouseDown()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                var grid = Grid.Instance;
                var gridHexRadius = grid.HexRadius;

                foreach (var tiles in grid.TilesInRangeDictionary)
                    tiles.Value.DeleteChildsGO();

                var tilesInRange = grid.TilesInRange(this, 20);

                foreach (Tile tile in tilesInRange.Values)
                {
                    AddProjector(tile, grid);
                    AddLabel(tile, grid);
                }


                if (championsManager_.GetChampionToSpawn() != null && Available == true)
                {
                    if (Champion == null)
                    {
                        GameObject championToSpawn = championsManager_.GetChampionToSpawn();
                        Champion = (GameObject)Instantiate(championToSpawn, transform.position, transform.rotation);
                        championsManager_.SetChampionToSpawn(null);
                    }
                }

                if (Champion)
                {
                    championsManager_.SelectTile(this);
                }
            }
        }

        private void AddLabel(Tile tile, Grid grid)
        {
            GameObject labelGameObject = new GameObject("Label");
            labelGameObject.transform.parent = tile.TileGameObject.transform;
            labelGameObject.transform.position = tile.Position;
            labelGameObject.transform.Rotate(new Vector3(90, 0, 0));
            tile.labelGameObject_ = labelGameObject;
            var text = labelGameObject.AddComponent<TextMesh>();
            text.text = Convert.ToString(Math.Round(tile.DistanceFromStart, 1));
            text.fontSize = (int)(grid.HexRadius * 9);
            text.anchor = TextAnchor.MiddleCenter;
            text.color = Color.white;
        }

        private void AddProjector(Tile tile, Grid grid)
        {
            GameObject go = new GameObject("Shadow");
            go.transform.parent = tile.TileGameObject.transform;
            go.transform.position = tile.Position + new Vector3(0, grid.HexRadius * 2, 0);
            go.transform.Rotate(new Vector3(90, 0, 0));
            tile.projectorGameObject_ = go;
            go.AddComponent<Projector>();
            var projector = go.GetComponent<Projector>();
            projector.material = grid.ProjectorsMaterial;
        }

        #endregion
    }
}
