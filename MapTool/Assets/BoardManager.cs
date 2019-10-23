using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public int boardRows;
    public int boardColumns;

    public GameObject[,] tiles;
    public List<GameObject> buildings;

    public GameObject tilePrefabIGuess;
    public GameObject buildingPrefabIGuess;
    // Start is called before the first frame update
    void Awake()
    {
        tiles = new GameObject[boardRows, boardColumns];
        buildings = new List<GameObject>();
        for (int i = 0; i < boardRows; i++)
        {
            for (int j = 0; j < boardColumns; j++)
            {
                tiles[i, j] = Instantiate(tilePrefabIGuess, new Vector3(i, j, 0), Quaternion.identity, transform);
            }
        }
    }

    public void Prepare(int GridSize)
    {
        for (int i = 0; i < boardRows; i++)
        {
            for (int j = 0; j < boardColumns; j++)
            {
                Destroy(tiles[i, j]);
            }
        }
        boardRows = GridSize;
        boardColumns = GridSize;
        tiles = new GameObject[GridSize, GridSize];
        for (int i = 0; i < boardRows; i++)
        {
            for (int j = 0; j < boardColumns; j++)
            {
                tiles[i, j] = Instantiate(tilePrefabIGuess, new Vector3(i, j, 0), Quaternion.identity, transform);
            }
        }
    }

    public void GenerateSpaces()
    {
        foreach (GameObject building in buildings)
            Destroy(building);
        buildings.Clear();
        int currentList = 0;
        bool itHasHappened = false;
        int buildingsOnAColumn = 0;
        for (int x = 0; x < boardRows; x++)
        {
            for (int y = 0; y < boardColumns; y++)
            {
                if (x < boardRows && y < boardColumns && tiles[x, y].tag != "Road" && tiles[x, y].tag != "Occupado")
                {
                    int currentX = x;
                    buildings.Add(Instantiate(buildingPrefabIGuess, transform));
                    buildings[currentList].GetComponent<BuildingScript>().SetID(currentList);
                    while (y < boardColumns && tiles[x, y].tag != "Road" && tiles[x, y].tag != "Occupado")
                    {
                        buildings[currentList].GetComponent<BuildingScript>().AddTile(x, y);
                        tiles[x, y].tag = "Occupado";
                        x++;
                        while (x < boardRows && tiles[x, y].tag != "Road" && tiles[x, y].tag != "Occupado")
                        {
                            buildings[currentList].GetComponent<BuildingScript>().AddTile(x, y);
                            tiles[x, y].tag = "Occupado";
                            x++;
                        }
                        x = currentX;
                        y++;
                    }
                    currentList++;
                }
            }
            if (!itHasHappened)
            {
                if (buildings.Count > 0)
                {
                    itHasHappened = true;
                    buildingsOnAColumn = buildings.Count;
                    Debug.Log(buildingsOnAColumn);
                }
            }
        }
        float r = 1f;
        float g = 0f;
        float b = 0f;
        foreach (GameObject building in buildings)
        {
            building.GetComponent<BuildingScript>().AddNeighbours(buildingsOnAColumn, buildings.Count);
            r = UnityEngine.Random.Range(0f, 1f);
            g = UnityEngine.Random.Range(0f, 1f);
            b = UnityEngine.Random.Range(0f, 1f);
            building.GetComponent<BuildingScript>().SetColor(new Color(r, g, b));
            foreach (Tuple<int, int> tile in building.GetComponent<BuildingScript>().GetTiles())
                tiles[tile.Item1, tile.Item2].GetComponent<SpriteRenderer>().color = new Color(r, g, b);
        }
        //Shows that neighbours work.
        //foreach (int i in buildings[6].GetComponent<BuildingScript>().GetNeighbours())
        //{
        //    foreach (Tuple<int, int> tile in buildings[i].GetComponent<BuildingScript>().GetTiles())
        //        tiles[tile.Item1, tile.Item2].GetComponent<SpriteRenderer>().color = new Color(r, g, b);
        //}
    }

    public void Combine()
    {
        //DONE Tell second building's neigbours that the second building's id in their neigbour-lists should be the first building's id.
        //DONE Put all tiles from second building into first building.
        //Search right and down(up?) to see where the road is
        //When the road is found, add those tiles, and make them Occupado.
        //DONE Delete second building.
        List<int> toRemove = new List<int>();
        List<int> noTouchy = new List<int>();
        for (int i = 0; i < buildings.Count; i++)
        {
            if (!toRemove.Contains(i))
            {
                int neighbour = buildings[i].GetComponent<BuildingScript>().GetNeighbours()[UnityEngine.Random.Range(0, buildings[i].GetComponent<BuildingScript>().GetNeighbours().Count)];
                if (!noTouchy.Contains(neighbour))
                {
                    foreach (int neighbourneighbour in buildings[neighbour].GetComponent<BuildingScript>().GetNeighbours())
                    {
                        buildings[neighbourneighbour].GetComponent<BuildingScript>().SwitchNeighbour(neighbour, buildings[i].GetComponent<BuildingScript>().GetID());
                    }
                    foreach (Tuple<int, int> tile in buildings[neighbour].GetComponent<BuildingScript>().GetTiles())
                    {
                        buildings[i].GetComponent<BuildingScript>().AddTile(tile.Item1, tile.Item2);
                    }
                    foreach (Tuple<int, int> tile in buildings[i].GetComponent<BuildingScript>().GetTiles())
                    {
                        tiles[tile.Item1, tile.Item2].GetComponent<SpriteRenderer>().color = buildings[i].GetComponent<BuildingScript>().GetColor();
                    }
                    toRemove.Add(neighbour);
                    noTouchy.Add(i);
                }
            }
        }
        foreach (int id in toRemove)
        {
            foreach (GameObject building in buildings)
            {
                if (building.GetComponent<BuildingScript>().GetID() == id)
                {
                    Destroy(building);
                    buildings.Remove(building);
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
