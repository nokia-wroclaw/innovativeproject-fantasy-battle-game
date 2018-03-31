﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Map;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;

public class ChampionsManager : MonoBehaviour
{
    public static ChampionsManager Instance;
    public List<GameObject> ChampionsPrefabs;
    public Tile SelectedTile { get; set; }
    public GameObject SelectedChampion { set; get; }

    private GameObject championToSpawn_;
    

    public void SelectTile(Tile tile)
    {
        SelectedTile = tile;
        if (SelectedTile == null)
        {
            SelectedChampion = null;
            return;
        }

        if (SelectedTile.Champion)
        {
            SelectedChampion = SelectedTile.Champion;
        }
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