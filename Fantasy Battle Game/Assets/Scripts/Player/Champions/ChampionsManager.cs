using System.Collections.Generic;
using Map;
using UnityEngine;

namespace Champions
{
    public class ChampionsManager:MonoBehaviour
    {
        public static ChampionsManager Instance;
        public List<GameObject> ChampionsPrefabs;
        private GameObject championToSpawn_;
        private Champion selectedChampion_;
        private Champion selectedEnemyChampion_;
        private BattleManagement.MouseManagement mouseManagement_;

        public Champion SelectedChampion
        {
            set
            {
                selectedChampion_=value;
                mouseManagement_.UpdateSelectedChampion(value);
            }
            get { return selectedChampion_; }
        }

        public Champion SelectedEnemyChampion
        {
            get { return selectedEnemyChampion_; }
            set { selectedEnemyChampion_ = value; }
        }

        public ChampionsManager()
        {
            if (Instance == null) Instance = this;
            mouseManagement_ = BattleManagement.MouseManagement.Instance;
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
            var tile = Map.Map.Instance.GetTile(coord);
            
            if (!tile)
            {
                return false;
            }
            return tile.SpawnChampion();
        }
    }
}
