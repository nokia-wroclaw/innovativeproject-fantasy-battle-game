using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Champions;
using UnityEngine;
using System.Threading;

namespace BattleManagement
{
    class TurnManagement : MonoBehaviour
    {
        private static TurnManagement instance_;

        private Player.Player firstPlayer_;
        private Player.Player secondPlayer_;
        private Player.Player winnerPlayer_;
        private List<Champion> firstPlayerTurnList_;
        private List<Champion> secondPlayerTurnList_;
        private ChampionsManager championsManager_;
        private bool lastMoveFirstPlayer_ = false;
        private bool endGame_ = false;
        
        void Start()
        {
            NextTurn();
        }

        public void SetPlayers(Player.Player firstPlayer, Player.Player secondPlayer)
        {
            firstPlayer_ = firstPlayer;
            secondPlayer_ = secondPlayer;
        }

        public void Winner()
        {
            if (endGame_)
            {
                if (lastMoveFirstPlayer_) { Debug.Log("Win " + firstPlayer_.Name); }
                else { Debug.Log("Win " + secondPlayer_.Name); }
            }
        }

        public void StartBattle()
        {
            if (firstPlayer_== null || secondPlayer_ == null)
            {
                Debug.LogError("no first or second player");
                return;
            }

            championsManager_ = ChampionsManager.Instance;
            firstPlayerTurnList_ = new List<Champion>();
            secondPlayerTurnList_ = new List<Champion>();
        }

        public bool NextTurn()
        {
            if (firstPlayer_.Champions.Count == 0 || secondPlayer_.Champions.Count == 0)
            {
                Winner();
                return false;
            }
            if (lastMoveFirstPlayer_)
            {
                lastMoveFirstPlayer_ = false;
                endGame_ = secondPlayerTurn();
            }
            else
            {
                lastMoveFirstPlayer_ = true;
                endGame_ = firstPlayerTurn();
            }
            return !endGame_;
        }

        private bool firstPlayerTurn()
        {
            Champion selectedChampion;
            do
            {
                if (firstPlayerTurnList_.Count == 0)
                {
                    if (firstPlayer_.Champions.Count == 0) { return false; }
                    foreach (var champion in firstPlayer_.Champions)
                    {
                        firstPlayerTurnList_.Add(champion);
                    }
                }
                selectedChampion = firstPlayerTurnList_[0];
                firstPlayerTurnList_.RemoveAt(0);
            } while (!firstPlayer_.Champions.Contains(selectedChampion));

            Debug.Log("first player turn");
            championsManager_.SelectedChampion = selectedChampion;
            return true;
        }

        private bool secondPlayerTurn()
        {
            Champion selectedChampion;
            do
            {
                if (secondPlayerTurnList_.Count == 0)
                {
                    if (secondPlayer_.Champions.Count == 0) { return false; }
                    foreach (var champion in secondPlayer_.Champions)
                    {
                        secondPlayerTurnList_.Add(champion);
                    }
                }
                selectedChampion = secondPlayerTurnList_[0];
                secondPlayerTurnList_.RemoveAt(0);
            } while (!secondPlayer_.Champions.Contains(selectedChampion));

            Debug.Log("second player turn");
            championsManager_.SelectedChampion = selectedChampion;
            return true;
        }

        public TurnManagement()
        {
            instance_ = Instance == null ? this : null;
        }
        
        public static TurnManagement Instance
        {
            get { return instance_; }
            set { instance_ = value; }
        }

    }
}
