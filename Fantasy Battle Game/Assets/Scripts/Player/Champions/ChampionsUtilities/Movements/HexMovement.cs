using System.Collections.Generic;
using Assets.Scripts.Map;
using Champions.ChampionsUtilities.Interfaces;
using Champions;
using UnityEngine;

namespace CharacterUtilities.Movements
{
    public class HexMovement:MonoBehaviour, IMovement
    {
        public float RotationSpeed = 5;
        public float Speed = 10;
        
        private ChampionsManager manager_;
        private Map map_;
        private GameObject SelectedChampion { get; set; }
        private Tile ChampionsPosition { get; set; }
        private Tile DestinationTile { get; set; }
        private Tile TempDestinationTile { get; set; }
        private Quaternion playerRotation_;
        private List<Tile> route_;
        private bool calculate_;
        private bool move_;
        private bool nextMove_;
        private int iterator_ = 1;

        
        private void Start()
        {
            manager_ = ChampionsManager.Instance;
            map_ = Map.Instance;
            route_ = new List<Tile>();
        }

        private void Update()
        {
            SetTargetPosition();
            SetDestinationPoint();

            if (calculate_)
            {
                CalculateRoute();
            }
            
            if (move_)
            {
                if (nextMove_)
                {
                    iterator_++;
                    nextMove_ = false;
                }
                Move();
            }
            
        }

        public void SetTargetPosition()
        {
            if (manager_ && manager_.SelectedChampion != null)
            {
                SelectedChampion = manager_.SelectedChampion;
                ChampionsPosition = map_.SelectedTile;
                Debug.Log("Champion has been set " + ChampionsPosition.Coordinate);
            }
        }

        public void Move()
        {
            Debug.Log("Moving...");
            var lookAtTarget = new Vector3(route_[iterator_].Position.x - SelectedChampion.transform.position.x,
                transform.position.y, route_[iterator_].Position.z - transform.position.z);
            playerRotation_ = Quaternion.LookRotation(lookAtTarget);

            transform.rotation = Quaternion.Slerp(transform.rotation, playerRotation_, RotationSpeed * Time.deltaTime);
            
            SelectedChampion.transform.position = Vector3.MoveTowards(transform.position, route_[iterator_].Position, Speed*Time.deltaTime);

            if (transform.position == route_[iterator_].Position)
            {
                nextMove_ = true;
            }

            if (transform.position == DestinationTile.Position)
            {
                move_ = false;
                nextMove_ = false;

                ChampionsPosition.Champion = null;
                ChampionsPosition = DestinationTile;
                ChampionsPosition.Champion = SelectedChampion;
                iterator_ = 0;
            }
        }

        private void SetDestinationPoint()
        {
            if (manager_ && manager_.SelectedChampion == null)
            {
                if (map_.SelectedTile != ChampionsPosition)
                {
                    Debug.Log("Destination point has been set - calculating route...");
                    DestinationTile = map_.SelectedTile;
                    calculate_ = true;
                    
                }
            }
        }

        private void CalculateRoute()
        {
            route_.Clear();
            TempDestinationTile = ChampionsPosition;
            route_.Add(TempDestinationTile);
            while (TempDestinationTile != DestinationTile)
            {
                var list = SortList(TempDestinationTile, DestinationTile);

                var position = 0;
                while (route_.Contains(list[position]))
                {
                    position++;
                }
                route_.Add(list[position]);
                TempDestinationTile = list[position];
            }
            calculate_ = false;
            move_ = true;
            Debug.Log("Route calculated");
        }

        private List<Tile> SortList(Tile center, Tile endPoint)
        {
            var list = map_.AvailableNeighbours(center);
            var i = 1;
            while(i < list.Count)
            {
                var j = i;
                while (j > 0 && Vector3.Distance(list[j-1].Position, endPoint.Position) > Vector3.Distance(list[j].Position, endPoint.Position))
                {
                    var temp = list[j];
                    list[j] = list[j - 1];
                    list[j - 1] = temp;

                    j--;
                }
                i++;
            }
            return list;
        }

    }
}