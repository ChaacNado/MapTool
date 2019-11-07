using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JunctionScript : MonoBehaviour
{
    Tuple<int, int> tile;
    List<int> connectedRoads;
    // Start is called before the first frame update
    void Awake()
    {
        connectedRoads = new List<int>();
    }

    public void SetTile(int x, int y)
    {
        tile = new Tuple<int, int>(x, y);
    }

    public Tuple<int,int> GetTile()
    {
        return tile;
    }
    public void AddRoad(int id)
    {
        if(connectedRoads.Count < 4)
            connectedRoads.Add(id);
    }

    public List<int> GetRoads()
    {
        return connectedRoads;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
