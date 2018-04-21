using Map;
using Champions.CharacterUtilities.Movements;
using UnityEngine;
using Player;

namespace Champions
{
    public class Champion : HexMovement
    {
        public Player.Player Owner { get; set; }
        public GameObject GameObject { get; set; }
        //stats
        public int MaxHp { set; get; }
        public int CurrentHp { set; get; }
        public int Damage;
        public int Range = 10;

        public Tile CurrentPossition
        {
            get
            {
                return currentTile;
            }
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
                //transform.localPosition = value.Position;

            }
        }
    }
}
