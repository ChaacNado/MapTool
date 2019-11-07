using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadScript : MonoBehaviour
{
    int id;
    List<Tuple<int, int>> tiles;
    List<int> neighbouringBuildings;
    // Start is called before the first frame update
    void Awake()
    {
        tiles = new List<Tuple<int, int>>();
        neighbouringBuildings = new List<int>();
    }

    public void SetID(int id)
    {
        this.id = id;
    }

    public int GetID()
    {
        return id;
    }

    public void AddTile(int x, int y)
    {
        tiles.Add(new Tuple<int, int>(x, y));
    }

    public Tuple<int,int> GetFirstTile()
    {
        return tiles[0];
    }
    public Tuple<int,int> GetLastTile()
    {
        return tiles[tiles.Count - 1];
    }

    public List<Tuple<int,int>> GetTiles()
    {
        return tiles;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
