using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ChampionsManager : MonoBehaviour
{
    public static ChampionsManager Instance;
    public List<GameObject> ChampionsPrefabs;
    private GameObject _championToSpawn;


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
        return _championToSpawn;
    }

    public void SetChampionToSpawn(GameObject champion)
    {
        _championToSpawn = champion;
    }

}