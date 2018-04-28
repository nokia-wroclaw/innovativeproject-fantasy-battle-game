using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Champions;
using Map;
using UnityEngine;

namespace BattleManagement
{
    class MouseManagement : MonoBehaviour
    {
        public static MouseManagement Instance;
        Map.Map map_;
        ChampionsManager championsManager_;
        TurnManagement turnManagement_;

        public MouseManagement()
        {
            Instance = Instance == null ? this : null;
        }

        void Awake()
        {
            map_ = Map.Map.Instance;
            championsManager_ = ChampionsManager.Instance;
            turnManagement_ = TurnManagement.Instance;
        }

        public void MouseDown(Tile tile)
        {
            map_.SelectedTile = tile;
            if (championsManager_.SelectedChampion)
            {
                switch (turnManagement_.turnPhase_)
                {
                    case TurnPhase.FirstPhase:
                        Debug.Log("FirstPhase");
                        if (map_.TilesInRangeDictionary.ContainsKey(tile.Coordinate))
                        {
                            championsManager_.SelectedChampion.DestinationTile = tile;
                            championsManager_.SelectedChampion.GoToDestination();
                            championsManager_.SelectedChampion.CurrentPossition = tile;
                            turnManagement_.turnPhase_ = TurnPhase.MovingPhase;
                            Debug.Log("goToDestFinished");
                        }
                        else if (tile.Champion && map_.TilesWithChampions.ContainsKey(tile.Coordinate))
                        {
                            MouseCursorManagement.Instance.SetMeelAtackIcon();
                            championsManager_.SelectedEnemyChampion = tile.Champion;
                            turnManagement_.turnPhase_ = TurnPhase.AttackDetailsPhase;
                            map_.ClearMarkTilesInRange();
                            map_.MarkTilesNextToEnemyInRange(tile);
                        }
                        break;

                    case TurnPhase.AttackDetailsPhase:
                        if (map_.TilesNextToEnemyInRange.ContainsKey(tile.Coordinate))
                        {
                            championsManager_.SelectedChampion.PhysicalAttack(championsManager_.SelectedEnemyChampion, tile);
                            championsManager_.SelectedChampion.CurrentPossition = tile;
                        }
                        break;

                    case TurnPhase.MovingPhase:
                        break;

                    case TurnPhase.AttackPhase:
                        break;

                    case TurnPhase.LastPhase:
                        break;
                }
            }
            else
            {
                Debug.LogError("no selected champion");
            }
        }

        public void MouseUp(Tile tile)
        {
            if (tile.Champion && map_.TilesWithChampions.ContainsKey(tile.Coordinate))
            { }
            else
            {

                MouseCursorManagement.Instance.DefaultCursor();
            }
        }


        public void UpdateSelectedChampion(Champion champion)
        {
            map_.SelectedTile = null;
            map_.TilesInRange(champion.CurrentPossition, champion.Range);
            map_.MarkTilesInRange();
            map_.MarkUnitInRange(turnManagement_.CurrentPlayer);
        }
    }
}
