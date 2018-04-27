using System;
using System.Collections.Generic;
using Champions;
using Champions.CharacterUtilities.Movements;
using CharacterUtilities.Movements;
using Map.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Map
{
    /// <summary>
    /// Represent basic element in map- hex
    /// </summary>
    public class Tile : MonoBehaviour, ITile
    {
        public GameObject TileGameObject;
        public Champion Champion { set; get; }
        
        public Tile Parent { get; set; }
        public double Cost { get; set; }
        public double MovementCost { get; set; }
        public double DistanceToTarget { get; set; }

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
        private GameObject projectorOnMouseGameObject_;
        private GameObject projectorUnitInRange_;
        private readonly List<Tile> neighbours_ = new List<Tile>();
        private ChampionsManager championsManager_;


        void Awake()
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
            if (projectorUnitInRange_)
            {
                Destroy(projectorUnitInRange_);
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


        void OnMouseEnter()
        {
            var projector = Instantiate(GridMetrics.Instance.Projector);
            projector.transform.parent = TileGameObject.transform;
            projector.transform.position = position_ + new Vector3(0, GridMetrics.HexRadius * 2, 0);
            projectorOnMouseGameObject_ = projector;
        }
        void OnMouseExit()
        {
            if (projectorOnMouseGameObject_)
            {
                Destroy(projectorOnMouseGameObject_);
            }
        }

        private void OnMouseDown()
        {

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                var map = Map.Instance;
                map.SelectedTile = this;
                if (Champion && Champion != championsManager_.SelectedChampion)
                {
                    Champion.Destroy();
                    return;
                }
                    

                if (championsManager_.SelectedChampion)
                {
                    if (map.TilesInRangeDictionary.ContainsKey(this.coordinate_))
                    {
                        if (!championsManager_.SelectedChampion.Moving)
                        {
                            Debug.Log("SetDestinationPoint");
                            championsManager_.SelectedChampion.DestinationTile = this;
                            championsManager_.SelectedChampion.GoToDestination();
                            championsManager_.SelectedChampion.CurrentPossition = this;
                        }
                    }
                }
                else
                {
                    Debug.Log("Champion not selected");
                }
            }
        }

        #endregion


        /// <summary>
        /// Adds game object to selected tile to show distance from start
        /// </summary>
        /// <param name="tile">selected tile</param>
        public static void AddLabel(Tile tile)
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
        public static void AddProjector(Tile tile)
        {
            var projector = Instantiate(GridMetrics.Instance.Projector);
            projector.transform.parent = tile.TileGameObject.transform;
            projector.transform.position = tile.position_ + new Vector3(0, GridMetrics.HexRadius * 2, 0);
            tile.projectorGameObject_ = projector;
        }

        public static void AddProjectorUnitInRange(Tile tile)
        {
            var projector = Instantiate(GridMetrics.Instance.ProjectorUnitsInRange);
            projector.transform.parent = tile.TileGameObject.transform;
            projector.transform.position = tile.position_ + new Vector3(0, GridMetrics.HexRadius * 2, 0);
            tile.projectorUnitInRange_= projector;
        }


        public bool SpawnChampion()
        {
            if (championsManager_.GetChampionToSpawn() != null && Available == true)
            {
                if (Champion == null)
                {
                    GameObject championToSpawn = championsManager_.GetChampionToSpawn();
                    var newChampion = (GameObject)Instantiate(championToSpawn, transform.position, transform.rotation);
                    
                    var champion = newChampion.GetComponent<Champion>();
                    champion.GameObject = newChampion;
                    champion.CurrentPossition = this;
                    championsManager_.SetChampionToSpawn(null);

                    return true;
                }
            }
            return false;
        }
    }
}