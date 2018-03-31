using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Map;
using UnityEditor;
using UnityEngine;

public class ChampionsManager : MonoBehaviour
{
    public static ChampionsManager Instance;
    public List<GameObject> ChampionsPrefabs;
    private GameObject championToSpawn_;
    private Tile selectedTile_;
    public GameObject SelectedChampion { set; get; }

    public void SelectTile(Tile tile)
    {
        selectedTile_ = tile;
        championToSpawn_ = null;
        if (selectedTile_.Champion)
        { SelectedChampion = selectedTile_.Champion; }
        else
        {
                SelectedChampion = null;
        }     
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