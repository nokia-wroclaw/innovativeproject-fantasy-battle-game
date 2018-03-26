using UnityEngine;

public class Champion : MonoBehaviour
{
    public float CurrentX { set; get; }
    public float CurrentY { set; get; }
    public float CurrentZ { set; get; }
    public int CurrentHp { set; get; }
    public bool IsFirstPlayer;
    public int Damage;
   
    public void SetPosition(float x, float y, float z)
    {
        CurrentX = x;
        CurrentY = y;
        CurrentZ = z;
    }
}