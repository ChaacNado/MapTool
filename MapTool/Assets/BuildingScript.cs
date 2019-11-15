using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingScript : MonoBehaviour
{
    private int id;
    private List<int> neighbours;
    private List<Tuple<int, int>> tiles;
    public bool hasDoor;
    private Color color;

    // Start is called before the first frame update
    void Awake()
    {
        neighbours = new List<int>();
        tiles = new List<Tuple<int, int>>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddTile(int x, int y)
    {
        foreach(Tuple<int,int> tile in tiles)
        {
            if (x == tile.Item1 && y == tile.Item2)
                return;
        }
        tiles.Add(new Tuple<int, int>(x, y));
    }

    public void RemoveTile(int x, int y)
    {
        foreach(Tuple<int,int> tile in tiles)
        {
            if(tile.Item1 == x && tile.Item2 == y)
            {
                tiles.Remove(tile);
                break;
            }
        }
    }

    public List<Tuple<int,int>> GetTiles()
    {
        return tiles;
    }

    public void AddNeighbours(int buildings, int maxbuildings)
    {
        //Debug.Log("ID: " + id + " buildingsOnAColumn: " + buildings + " yes: " + id%buildings);
        if (id - 1 >= 0 && id % buildings != 0)
            neighbours.Add(id - 1);
        if (id + 1 < maxbuildings && (id + 1) % buildings != 0)
            neighbours.Add(id + 1);
        if (id - buildings >= 0)
            neighbours.Add(id - buildings);
        if (id + buildings < maxbuildings)
            neighbours.Add(id + buildings);
    }

    public void AddNeighbour(int neighbourID)
    {
        neighbours.Add(neighbourID);
    }

    public void RemoveNeighbour(int neighbourID)
    {
        neighbours.Remove(neighbourID);
    }

    public List<int> GetNeighbours()
    {
        return neighbours;
    }

    public void SwitchNeighbour(int oldNeighbour, int newNeighbour)
    {
        foreach (int neighbour in neighbours)
        {
            if (neighbour == newNeighbour)
            {
                return;
            }
        }
        int index = 0;
        foreach (int neighbour in neighbours)
        {
            if (neighbour == oldNeighbour)
            {
                neighbours[index] = newNeighbour;
                break;
            }
            index++;
        }
    }

    public int GetID()
    {
        return id;
    }
    public void SetID(int ID)
    {
        id = ID;
    }

    public void SetColor(Color color)
    {
        this.color = color;
    }
    public Color GetColor()
    {
        return color;
    }
}
