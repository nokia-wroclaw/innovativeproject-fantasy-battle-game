using System.Collections.Generic;
using Assets.Scripts.Map;
using UnityEngine;

namespace Champions
{
    public class ChampionsManager:MonoBehaviour
    {
        public static ChampionsManager Instance;
        public List<GameObject> ChampionsPrefabs;
        public GameObject SelectedChampion { set; get; }
        private GameObject championToSpawn_;

        public ChampionsManager()
        {
            if (Instance == null) Instance = this;
            
        }

        private void Start()
        {
            if (Instance == null) Instance = this;
        }

        public GameObject GetChampionToSpawn()
        {
            return championToSpawn_;
        }

        public void SetChampionToSpawn(GameObject champion)
        {
            championToSpawn_ = champion;
        }

        public bool SpawnChampion(TileMetrics.HexCoordinate coord)
        {
            if (!championToSpawn_)
            {
                return false;
            }
            var tile = Map.Instance.GetTile(coord);
            if (!tile)
            {
                return false;
            }
            return tile.SpawnChampion();
        }
    }
}
