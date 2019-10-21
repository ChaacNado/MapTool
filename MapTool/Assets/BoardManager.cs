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
    public List<List<Tuple<int, int>>> idk;

    public GameObject tilePrefabIGuess;

    // Start is called before the first frame update
    void Awake()
    {
        tiles = new GameObject[boardRows, boardColumns];
        idk = new List<List<Tuple<int, int>>>();
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
        idk.Clear();
        int currentList = 0;
        for (int x = 0; x < boardRows; x++)
        {
            for (int y = 0; y < boardColumns; y++)
            {
                if (x < boardRows && y < boardColumns && tiles[x, y].tag != "Road" && tiles[x, y].tag != "Occupado")
                {
                    int currentX = x;
                    idk.Add(new List<Tuple<int, int>>());
                    while (y < boardColumns && tiles[x, y].tag != "Road" && tiles[x, y].tag != "Occupado")
                    {
                        idk[currentList].Add(new Tuple<int, int>(x, y));
                        tiles[x, y].tag = "Occupado";
                        x++;
                        while (x < boardRows && tiles[x, y].tag != "Road" && tiles[x, y].tag != "Occupado")
                        {
                            idk[currentList].Add(new Tuple<int, int>(x, y));
                            tiles[x, y].tag = "Occupado";
                            x++;
                        }
                        x = currentX;
                        y++;
                    }
                    currentList++;
                }
            }
        }

        foreach (List<Tuple<int, int>> shit in idk)
        {
            float r = UnityEngine.Random.Range(0f, 1f);
            float g = UnityEngine.Random.Range(0f, 1f);
            float b = UnityEngine.Random.Range(0f, 1f);
            foreach (Tuple<int, int> FUCK in shit)
            {

                tiles[FUCK.Item1, FUCK.Item2].GetComponent<SpriteRenderer>().color = new Color(r, g, b);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
