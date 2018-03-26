using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChampionsManager : MonoBehaviour
{
    public static ChampionsManager Instance;
    public List<GameObject> ChampionsPrefabs;
    private GameObject championToSpawn_;
    private assets.scripts.map.Tile selectedTile_;


    public void SelectTile(assets.scripts.map.Tile tile)
    {
        selectedTile_ = tile;
        championToSpawn_ = null;
    }

    public GameObject GetSelectedChampion()
    {
            return selectedTile_.Champion;
    }
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one ChampionsManager.");
            return;
        }

        Instance = this;
    }

    public GameObject GetChampionToSpawn()
    {
        return championToSpawn_;
    }

    public void SetChampionToSpawn(GameObject champion)
    {
        championToSpawn_ = champion;
    }

}