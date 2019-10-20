using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PCGScript : MonoBehaviour
{
    public GameObject board;
    int RoadChance;
    int GridSize;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindWithTag("RoadChance").GetComponent<InputField>().onEndEdit.AddListener(delegate
        {
            RoadChance = int.Parse(GameObject.FindWithTag("RoadChance").GetComponent<InputField>().text);
        });
        GameObject.FindWithTag("GridSize").GetComponent<InputField>().onEndEdit.AddListener(delegate
        {
            GridSize = int.Parse(GameObject.FindWithTag("GridSize").GetComponent<InputField>().text);
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Generate()
    {
        board.GetComponent<BoardManager>().Prepare(GridSize);
        for (int i = 0; i < board.GetComponent<BoardManager>().boardRows; i++)
        {
            if (Random.Range(0, 100) > RoadChance)
            {
                FillHer(i, "row");
                i++;
            }
        }

        for (int j = 0; j < board.GetComponent<BoardManager>().boardColumns; j++)
        {
            if (Random.Range(0, 100) > RoadChance)
            {
                FillHer(j, "column");
                j++;
            }
        }
        board.GetComponent<BoardManager>().GenerateSpaces();
    }

    void SubmitValue(string arg0)
    {
        RoadChance = int.Parse(arg0);
    }

    void FillHer(int i, string direction)
    {
        switch (direction)
        {
            case "row":
                for (int j = 0; j < board.GetComponent<BoardManager>().boardColumns; j++)
                {
                    board.GetComponent<BoardManager>().tiles[i, j].GetComponent<SpriteRenderer>().color = Color.black;
                    board.GetComponent<BoardManager>().tiles[i, j].tag = "Road";
                }
                break;

            case "column":
                for (int j = 0; j < board.GetComponent<BoardManager>().boardRows; j++)
                {
                    board.GetComponent<BoardManager>().tiles[j, i].GetComponent<SpriteRenderer>().color = Color.black;
                    board.GetComponent<BoardManager>().tiles[j, i].tag = "Road";
                }
                break;
        }
    }
}
