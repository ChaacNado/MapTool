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
    bool[,] grid;
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
        int rows = board.GetComponent<BoardManager>().boardRows;
        int columns = board.GetComponent<BoardManager>().boardColumns;
        grid = new bool[rows, columns];
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
                    if (board.GetComponent<BoardManager>().tiles[i, y].tag == "Road")
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

    public bool ConfirmRoadConnectivity(Tuple<int, int> dontCheck1, Tuple<int, int> dontCheck2)
    {
        int startX = board.GetComponent<BoardManager>().roads[0].GetComponent<RoadScript>().GetTiles()[0].Item1;
        int startY = board.GetComponent<BoardManager>().roads[0].GetComponent<RoadScript>().GetTiles()[0].Item2;
        RunAI(startX, startY, dontCheck1, dontCheck2);
        foreach (GameObject road in board.GetComponent<BoardManager>().roads)
        {
            foreach (Tuple<int, int> tile in road.GetComponent<RoadScript>().GetTiles())
            {
                int x = tile.Item1;
                int y = tile.Item2;
                if (grid[x, y] == false)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void RunAI(int x, int y, Tuple<int, int> dontCheck1, Tuple<int, int> dontCheck2)
    {
        Tuple<int, int> position = new Tuple<int, int>(x, y);
        bool roadFound = false;
        while (roadFound)
        {
            roadFound = false;
            //Check if position is on the forbidden road, if so: breakdance.
            if ((position.Item1 == dontCheck1.Item1 && position.Item2 == dontCheck1.Item2) || (position.Item1 == dontCheck2.Item1 && position.Item2 == dontCheck2.Item2))
            {
                break;
            }
            grid[position.Item1, position.Item2] = true;
            //WARNING: The code can check outside the array now.
            if (board.GetComponent<BoardManager>().tiles[position.Item1 + 1, position.Item2].tag == "Road"
                || board.GetComponent<BoardManager>().tiles[position.Item1 + 1, position.Item2].tag == "Junction")
            {
                //Right
                if (grid[position.Item1 + 1, position.Item2] == false)
                {
                    position = new Tuple<int, int>(position.Item1 + 1, position.Item2);
                    roadFound = true;
                }
            }
            if (board.GetComponent<BoardManager>().tiles[position.Item1 - 1, position.Item2].tag == "Road"
                || board.GetComponent<BoardManager>().tiles[position.Item1 - 1, position.Item2].tag == "Junction")
            {
                //Left
                if (grid[position.Item1 - 1, position.Item2] == false)
                {
                    if (roadFound)
                    {
                        RunAI(position.Item1 - 1, position.Item2, dontCheck1, dontCheck2);
                    }
                    else
                    {
                        position = new Tuple<int, int>(position.Item1 - 1, position.Item2);
                        roadFound = true;
                    }
                }
            }
            if (board.GetComponent<BoardManager>().tiles[position.Item1, position.Item2 + 1].tag == "Road"
                || board.GetComponent<BoardManager>().tiles[position.Item1, position.Item2 + 1].tag == "Junction")
            {
                //Up
                if (grid[position.Item1, position.Item2 + 1] == false)
                {
                    if (roadFound)
                    {
                        RunAI(position.Item1, position.Item2 + 1, dontCheck1, dontCheck2);
                    }
                    else
                    {
                        position = new Tuple<int, int>(position.Item1, position.Item2 + 1);
                        roadFound = true;
                    }
                }
            }
            if (board.GetComponent<BoardManager>().tiles[position.Item1, position.Item2 - 1].tag == "Road"
                || board.GetComponent<BoardManager>().tiles[position.Item1, position.Item2 - 1].tag == "Junction")
            {
                //Down
                if (grid[position.Item1, position.Item2 - 1] == false)
                {
                    if (roadFound)
                    {
                        RunAI(position.Item1, position.Item2 - 1, dontCheck1, dontCheck2);
                    }
                    else
                    {
                        position = new Tuple<int, int>(position.Item1, position.Item2 - 1);
                        roadFound = true;
                    }
                }
            }
        }
    }

    public void RemoveRoads()
    {
        foreach (GameObject junction in board.GetComponent<BoardManager>().junctions)
        {
            if (junction.GetComponent<JunctionScript>().GetRoads().Count > 0)
            {
                int roadID = junction.GetComponent<JunctionScript>().GetRoads()[UnityEngine.Random.Range(0, junction.GetComponent<JunctionScript>().GetRoads().Count)];
                foreach (GameObject road in board.GetComponent<BoardManager>().roads)
                {
                    if (road.GetComponent<RoadScript>().GetID() == roadID)
                    {
                        if (ConfirmRoadConnectivity(road.GetComponent<RoadScript>().GetFirstTile(), road.GetComponent<RoadScript>().GetLastTile()))
                        {
                            board.GetComponent<BoardManager>().RemoveRoad(road, roadID);
                        }
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
