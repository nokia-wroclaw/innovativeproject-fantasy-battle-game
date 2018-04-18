using System;
using System.Collections.Generic;
using Assets.Scripts.Map.Interfaces;
using Champions.Scripts;
using CharacterUtilities.Movements;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Map
{
    /// <summary>
    /// Represent basic element in map- hex
    /// </summary>
    public class Tile : MonoBehaviour, ITile
    {
        public GameObject TileGameObject;
        public GameObject Champion { set; get; }

        [SerializeField]
        private double distanceFromStart_ = 0;
        [SerializeField]
        private Vector3 position_;
        [SerializeField]
        private double drag_;
        [SerializeField]
        private bool available_ = false;
        [SerializeField]
        private TileMetrics.HexCoordinate coordinate_;
        
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
            {
                Destroy(labelGameObject_);
            }
            if (projectorGameObject_)
            {
                Destroy(projectorGameObject_);
            }
        }
        
        public List<Tile> GetNeighbours()
        {
            return neighbours_;
        }

        #region Properties
        public double DistanceFromStart
        {
            get { return distanceFromStart_; }
            set { this.distanceFromStart_ = value; }
        }

        public Vector3 Position
        {
            get { return position_; }
            set { this.position_ = value; }
        }

        public double Drag
        {
            get { return drag_; }
            set { this.drag_ = value; }
        }

        public bool Available
        {
            get { return available_; }
            set { available_ = value; }
        }

        public TileMetrics.HexCoordinate Coordinate
        {
            get { return coordinate_; }
            set { coordinate_ = value; }
        }
        #endregion

        #region Interaction_with_user

        private void OnMouseDown()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                var map = Map.Instance;

                foreach (var tiles in map.TilesInRangeDictionary)
                    tiles.Value.DeleteChildsGO();

                var tilesInRange = map.TilesInRange(this, 5);

                foreach (Tile tile in tilesInRange.Values)
                {
                    AddProjector(tile);
                    AddLabel(tile);
                }


                if (championsManager_.GetChampionToSpawn() != null && Available == true)
                {
                    if (Champion == null)
                    {
                        GameObject championToSpawn = championsManager_.GetChampionToSpawn();
                        Champion = (GameObject)Instantiate(championToSpawn, transform.position, transform.rotation);
                        Champion.AddComponent<HexMovement>();
                        championsManager_.SetChampionToSpawn(null);
                    }
                }
                championsManager_.SelectTile(this);
            }
        }


        /// <summary>
        /// Adds game object to selected tile to show distance from start
        /// </summary>
        /// <param name="tile">selected tile</param>
        private void AddLabel(Tile tile)
        {
            var label = Instantiate(GridMetrics.Instance.Label);
            label.transform.parent = tile.TileGameObject.transform;
            label.transform.position = tile.position_;
            var text = label.GetComponent<TextMesh>();
            text.text = Convert.ToString(Math.Round(tile.distanceFromStart_, 1));
            tile.labelGameObject_ = label;
        }

        /// <summary>
        /// Adds the projector to selected tile
        /// </summary>
        /// <param name="tile">The tile.</param>
        private void AddProjector(Tile tile)
        {
            var projector = Instantiate(GridMetrics.Instance.Projector);

            projector.transform.parent = tile.TileGameObject.transform;
            projector.transform.position = tile.position_ + new Vector3(0, GridMetrics.HexRadius * 2, 0);
            

            tile.projectorGameObject_ = projector;
        }
        #endregion
       
    }
}