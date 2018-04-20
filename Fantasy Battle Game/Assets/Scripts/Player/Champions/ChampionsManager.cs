﻿using System.Collections.Generic;
using Assets.Scripts.Map;
using UnityEngine;

namespace Champions
{
    public class ChampionsManager:MonoBehaviour
    {
        public static ChampionsManager Instance;
        public List<GameObject> ChampionsPrefabs;
        private GameObject championToSpawn_;
        private Champion selectedChampion_;

        public Champion SelectedChampion
        {
            set
            {
                selectedChampion_=value;
                Map.Instance.TilesInRange(selectedChampion_.CurrentPossition, 8);
            }
            get { return selectedChampion_; }
        }

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
