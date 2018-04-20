using System;
using System.Collections.Generic;
using Assets.Scripts.Map.Interfaces;
using Champions;
using Champions.CharacterUtilities.Movements;
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
        public Champion Champion { set; get; }

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
                map.SelectedTile = this;

                if (championsManager_.GetChampionToSpawn() != null)
                {
                    SpawnChampion();
                }
                else
                {
                    if (Champion)
                    {
                        Debug.Log("Champion selected");
                        championsManager_.SelectedChampion = Champion;
                        //championsManager_.SelectedChampion.GetComponent<HexMovement>().SetTargetPosition();
                        map.TilesInRange(this, 8);
                    }
                    else
                    {
                        if (championsManager_.SelectedChampion)
                        {
                            if (map.TilesInRangeDictionary.ContainsKey(this.coordinate_))
                            {
                                Debug.Log("SetDestinationPoint");
                                championsManager_.SelectedChampion.DestinationTile = this;
                                championsManager_.SelectedChampion.GoToDestination();
                                championsManager_.SelectedChampion.CurrentPossition = this;
                                //championsManager_.SelectedChampion.GetComponent<HexMovement>().SetDestinationPoint();
                            }
                            else
                            {
                                Debug.Log("Champion not selected");
                            }
                        }
                        
                    }
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

        
        public bool SpawnChampion()
        {
            if (championsManager_.GetChampionToSpawn() != null && Available == true)
            {
                if (Champion == null)
                {
                    GameObject championToSpawn = championsManager_.GetChampionToSpawn();
                    var newChampion = (GameObject)Instantiate(championToSpawn, transform.position, transform.rotation);
                    var champion = newChampion.GetComponent<Champion>();
                    champion.CurrentPossition = this;
                    championsManager_.SetChampionToSpawn(null);
                    championsManager_.SetChampionToSpawn(null);

                    return true;
                }
            }
            return false;
        }
    }
}