using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private ChampionsManager _champiopManager;

    void Start()
    {
        _champiopManager = ChampionsManager.Instance;
    }

    public void ChooseChampion(int index)
    {
        Debug.Log(index);
        _champiopManager.SetChampionToSpawn(_champiopManager.ChampionsPrefabs[index]);
    }
}
