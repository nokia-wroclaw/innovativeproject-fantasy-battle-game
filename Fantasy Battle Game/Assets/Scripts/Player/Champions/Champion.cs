using Map;
using Champions.CharacterUtilities.Movements;
using UnityEngine;
using Player;
using UnityEngine.EventSystems;

namespace Champions
{
    public class Champion : HexMovement
    {
        public GameObject GameObject { get; set; }
        private Player.Player owner_;

        //stats
        public int MaxHp { set; get; }
        public int CurrentHp { set; get; }
        public int Damage;
        public int Range = 10;


        public Tile CurrentPossition
        {
            get { return currentTile; }
            set
            {
                if (currentTile)
                {
                    currentTile.Champion = null;
                    currentTile.Available = true;
                }
                currentTile = value;
                value.Champion = this;
                value.Available = false;
            }
        }


        public Player.Player Owner
        {
            get { return owner_; }
            set
            {
                owner_ = value;
                value.Champions.Add(this);
            }
        }

        public void Destroy()
        {
            Debug.Log("destroy champion");
            owner_.Champions.Remove(this);
            Destroy(GameObject);
            CurrentPossition.Champion = null;
            CurrentPossition.Available = true;
        }
        
    }
}
