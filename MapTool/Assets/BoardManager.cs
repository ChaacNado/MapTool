using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int boardRows;
    public int boardColumns;

    public GameObject[,] tiles;

    public GameObject tilePrefabIGuess;

    // Start is called before the first frame update
    void Awake()
    {
        tiles = new GameObject[boardRows, boardColumns];
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

    // Update is called once per frame
    void Update()
    {

    }
}
