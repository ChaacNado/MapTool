using System;
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
            if (UnityEngine.Random.Range(0, 100) > RoadChance)
            {
                FillHer(i, "row");
                i++;
            }
        }

        for (int j = 0; j < board.GetComponent<BoardManager>().boardColumns; j++)
        {
            if (UnityEngine.Random.Range(0, 100) > RoadChance)
            {
                FillHer(j, "column");
                j++;
            }
        }
        board.GetComponent<BoardManager>().GenerateRoads();
        board.GetComponent<BoardManager>().GenerateJunctions();
        RemoveRoads();
        board.GetComponent<BoardManager>().FixJunctions();
        //board.GetComponent<BoardManager>().GenerateSpaces();
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
                for (int y = 0; y < board.GetComponent<BoardManager>().boardColumns; y++)
                {
                    board.GetComponent<BoardManager>().tiles[i, y].GetComponent<SpriteRenderer>().color = Color.black;
                    if(board.GetComponent<BoardManager>().tiles[i, y].tag == "Road")
                    {
                        board.GetComponent<BoardManager>().tiles[i, y].tag = "Junction";
                        board.GetComponent<BoardManager>().AddJunction(i, y);
                    }
                    else
                    {
                        board.GetComponent<BoardManager>().tiles[i, y].tag = "Road";
                    }
                }
                break;

            case "column":
                for (int x = 0; x < board.GetComponent<BoardManager>().boardRows; x++)
                {
                    board.GetComponent<BoardManager>().tiles[x, i].GetComponent<SpriteRenderer>().color = Color.black;
                    if (board.GetComponent<BoardManager>().tiles[x, i].tag == "Road")
                    {
                        board.GetComponent<BoardManager>().tiles[x, i].tag = "Junction";
                        board.GetComponent<BoardManager>().AddJunction(x, i);
                    }
                    else
                    {
                        board.GetComponent<BoardManager>().tiles[x, i].tag = "Road";
                    }
                }
                break;
        }
    }

    public void RemoveRoads()
    {
        foreach(GameObject junction in board.GetComponent<BoardManager>().junctions)
        {
            if (junction.GetComponent<JunctionScript>().GetRoads().Count > 0)
            {
                int roadID = junction.GetComponent<JunctionScript>().GetRoads()[UnityEngine.Random.Range(0, junction.GetComponent<JunctionScript>().GetRoads().Count)];
                foreach (GameObject road in board.GetComponent<BoardManager>().roads)
                {
                    if (road.GetComponent<RoadScript>().GetID() == roadID)
                    {
                        board.GetComponent<BoardManager>().RemoveRoad(road, roadID);
                        break;
                    }
                }
            }
        }
    }

    public void Merge()
    {
        board.GetComponent<BoardManager>().Combine();
    }

}
