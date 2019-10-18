using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCGScript : MonoBehaviour
{
    public GameObject board;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < board.GetComponent<BoardManager>().boardRows; i++)
        {
            if (Random.Range(0, 100) > 40)
            {
                
            }
        }

        for (int j = 0; j < board.GetComponent<BoardManager>().boardColumns; j++)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FillHer(int i, string direction)
    {
        switch (direction)
        {
            case "row":
                for (int j = 0; j < board.GetComponent<BoardManager>().boardColumns; j++)
                {
                    //board.GetComponent<BoardManager>().tiles[i, j].GetComponent<Sprite>.
                }
                break;

            case "column":
                for (int j = 0; j < board.GetComponent<BoardManager>().boardRows; j++)
                {

                }
                break;

        }
        
    }
}
