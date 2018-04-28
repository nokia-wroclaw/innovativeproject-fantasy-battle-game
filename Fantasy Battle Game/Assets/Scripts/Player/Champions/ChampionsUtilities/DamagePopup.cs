using UnityEngine;

namespace Champions.ChampionsUtilities
{
    public class DamagePopup : MonoBehaviour
    {
        public float DestroyTime = 2.0f;
        public Vector3 Offset = new Vector3(0, 0, 0);
        public Vector3 Rotation = new Vector3(30, 0, 0);

        void Start()
        {
            Destroy(gameObject, DestroyTime);
            transform.position += Offset;
        }
    }
}