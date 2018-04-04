using System.Collections.Generic;
using Champions.Scripts;
using CharacterUtilities.Interfaces;
using Map;
using UnityEditor.UI;
using UnityEngine;
using Grid = Map.Grid;

namespace CharacterUtilities.Movements
{
    public class HexMovement:MonoBehaviour, IMovement
    {
        public float RotationSpeed = 5;
        public float Speed = 10;
        
        private ChampionsManager manager_;
        private Map.Grid map_;
        private List<Tile> tilesVectors_;
        private GameObject SelectedChampion { get; set; }
        private Tile SelectedTile { get; set; }
        private Tile TempTile { get; set; }
        
        private void Start()
        {
            manager_ = ChampionsManager.Instance;
            map_ = Grid.Instance;
            tilesVectors_ = new List<Tile>();
        }

        private void Update()
        {
            if (CheckCurrentChampion())
            {
                SetTargetPosition();
            }
            else
            {
                SetCurrentChampion();
            }
        }

        public void SetTargetPosition()
        {
            if (CalculateVectors())
            {
                foreach (var tile in tilesVectors_)
                {
                    var lookAtTarget = new Vector3(tile.Position.x - transform.position.x,
                        transform.position.y, tile.Position.z - transform.position.z);
                    if (lookAtTarget != Vector3.zero)
                    {
                        var playerRotation = Quaternion.LookRotation(lookAtTarget);    
                        transform.rotation = Quaternion.Slerp(transform.rotation, 
                            playerRotation, RotationSpeed * Time.deltaTime);
                    }
                   
                    transform.position = Vector3.MoveTowards(transform.position, tile.Position,
                        Speed * Time.deltaTime);
                }   
            }
        }

        public void Move()
        {
            throw new System.NotImplementedException();
        }

        private void SetCurrentChampion()
        {
            SelectedChampion = manager_.SelectedChampion;
            SelectedTile = manager_.SelectedTile;

        }
        
        private bool CheckCurrentChampion()
        {
            if (SelectedChampion != null)
            {
                return true;
            }
            else
            {
                Debug.Log("Please, make sure you selected any champion");

                return false;
            }
        }
        private bool CalculateVectors()
        {
            int count = 0;
            
            if (SelectedChampion && manager_.SelectedTile != SelectedTile)
            {
                TempTile = SelectedTile;
                var neighbours = map_.AvailableNeighbours(SelectedTile);
                
                while (TempTile != manager_.SelectedTile)
                {
                    Tile shortestLength = SelectedTile;
                    float distance = Vector3.Distance(shortestLength.Position, manager_.SelectedTile.Position);
                    
                    foreach (var tile in neighbours)
                    {
                        var tempDist = Vector3.Distance(tile.Position, manager_.SelectedTile.Position);
                        if (tempDist < distance)
                        {
                            distance = tempDist;
                            shortestLength = tile;
                        }
                    }
                    
                    tilesVectors_.Add(shortestLength);
                    neighbours = map_.AvailableNeighbours(shortestLength);
                    TempTile = shortestLength;
                    count++;
                    if (count > 100)
                    {
                        foreach (var tile in tilesVectors_)
                        {
                            Debug.Log(tile.Coordinate);
                        }
                        Debug.Log(manager_.SelectedTile.Coordinate);
                        
                        Debug.Log("Cannot find path by specified algorithm.");
                        return false;
                    };
                }

                return true;
            }
            else
            {
                return false;
                Debug.Log("Please, select any champion or select different tile than your champion tile.");
            }
        }
    }
}