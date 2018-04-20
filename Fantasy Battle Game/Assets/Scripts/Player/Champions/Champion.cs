using UnityEngine;
using Player;

namespace Champions
{
    public class Champion : MonoBehaviour
    {
        public Player.Player Owner { get; set; } 
        public float CurrentX { set; get; }
        public float CurrentY { set; get; }
        public float CurrentZ { set; get; }
        public int CurrentHp { set; get; }
        public int Damage;
   
        public void SetPosition(float x, float y, float z)
        {
            CurrentX = x;
            CurrentY = y;
            CurrentZ = z;
        }
    }
}
