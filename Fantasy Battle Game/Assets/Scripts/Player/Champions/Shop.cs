using UnityEngine;

namespace Champions
{
    public class Shop : MonoBehaviour
    {
        private ChampionsManager championsManager_;

        void Start()
        {
            championsManager_ = ChampionsManager.Instance;
        }

        public void ChooseChampion(int index)
        {
            Debug.Log(index);
            championsManager_.SetChampionToSpawn(championsManager_.ChampionsPrefabs[index]);
        }
    }
}
