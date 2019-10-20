using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int boardRows;
    public int boardColumns;

    public GameObject[,] tiles;
    public List<Tuple<int, int>> idk;

    public GameObject tilePrefabIGuess;

    // Start is called before the first frame update
    void Awake()
    {
        tiles = new GameObject[boardRows, boardColumns];
        idk = new List<Tuple<int, int>>();
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
        int space = 0;
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                if (tiles[i, j].tag != "Road")
                {
                    int x = j;
                    while (tiles[i, j].tag != "Road")
                    {
                        if (tiles[i, j].tag == "Road")
                        {
                            j = x;
                            i++;
                        }
                        else
                        {
                            idk.Add(new Tuple<int, int>(i, j));
                            j++;
                        }
                        if (i >= tiles.GetLength(0) || j >= tiles.GetLength(1))
                        {
                            break;
                        }
                    }
                    j = x;
                    space++;
                }
            }
        }
        foreach (Tuple<int, int> FUCK in idk)
        {
            tiles[FUCK.Item1, FUCK.Item2].GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
