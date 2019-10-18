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
    void Start()
    {
        tiles = new GameObject[boardRows, boardColumns];
        for (int i = 0; i < boardRows; i++)
        {
            for (int j = 0; j < boardColumns; j++)
            {
                Debug.Log(tilePrefabIGuess);
                tiles[i, j] = Instantiate(tilePrefabIGuess, new Vector3(i, j, 0), Quaternion.identity, transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
