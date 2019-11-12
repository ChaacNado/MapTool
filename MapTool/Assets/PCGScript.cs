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
    int rows;
    int columns;
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
        rows = board.GetComponent<BoardManager>().boardRows;
        columns = board.GetComponent<BoardManager>().boardColumns;
        for (int i = 0; i < rows; i++)
        {
            if (UnityEngine.Random.Range(0, 100) > RoadChance)
            {
                FillHer(i, "row");
                i++;
            }
        }

        for (int j = 0; j < columns; j++)
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
        board.GetComponent<BoardManager>().GenerateBuildings();
        SplitBuildings();
        board.GetComponent<BoardManager>().PaintBuildings();
        board.GetComponent<BoardManager>().MakeWalls();
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
                for (int y = 0; y < columns; y++)
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
                for (int x = 0; x < rows; x++)
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
        grid = new bool[rows, columns];
        int startX = board.GetComponent<BoardManager>().roads[0].GetComponent<RoadScript>().GetTiles()[0].Item1;
        int startY = board.GetComponent<BoardManager>().roads[0].GetComponent<RoadScript>().GetTiles()[0].Item2;
        RoadWalkerAI(startX, startY, dontCheck1, dontCheck2);
        foreach (GameObject road in board.GetComponent<BoardManager>().roads)
        {
            foreach (Tuple<int, int> tile in road.GetComponent<RoadScript>().GetTiles())
            {
                int x = tile.Item1;
                int y = tile.Item2;
                if (x == dontCheck1.Item1 && y == dontCheck1.Item2)
                {
                    break;
                }
                if (grid[x, y] == false)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void RoadWalkerAI(int x, int y, Tuple<int, int> dontCheck1, Tuple<int, int> dontCheck2)
    {
        Tuple<int, int> position = new Tuple<int, int>(x, y);
        bool roadFound = true;
        while (roadFound)
        {
            roadFound = false;
            Tuple<int, int> move = null;
            //Check if position is on the forbidden road, if so: breakdance.
            if ((position.Item1 == dontCheck1.Item1 && position.Item2 == dontCheck1.Item2) || (position.Item1 == dontCheck2.Item1 && position.Item2 == dontCheck2.Item2))
            {
                break;
            }
            grid[position.Item1, position.Item2] = true;

            if (position.Item1 + 1 < rows)
            {
                if (board.GetComponent<BoardManager>().tiles[position.Item1 + 1, position.Item2].tag == "Road"
                    || board.GetComponent<BoardManager>().tiles[position.Item1 + 1, position.Item2].tag == "Junction")
                {
                    //Right
                    if (grid[position.Item1 + 1, position.Item2] == false)
                    {
                        move = new Tuple<int, int>(position.Item1 + 1, position.Item2);
                        roadFound = true;
                    }
                }
            }

            if (position.Item1 - 1 >= 0)
            {
                if (board.GetComponent<BoardManager>().tiles[position.Item1 - 1, position.Item2].tag == "Road"
                || board.GetComponent<BoardManager>().tiles[position.Item1 - 1, position.Item2].tag == "Junction")
                {
                    //Left
                    if (grid[position.Item1 - 1, position.Item2] == false)
                    {
                        if (roadFound)
                        {
                            RoadWalkerAI(position.Item1 - 1, position.Item2, dontCheck1, dontCheck2);
                        }
                        else
                        {
                            move = new Tuple<int, int>(position.Item1 - 1, position.Item2);
                            roadFound = true;
                        }
                    }
                }
            }

            if (position.Item2 + 1 < columns)
            {
                if (board.GetComponent<BoardManager>().tiles[position.Item1, position.Item2 + 1].tag == "Road"
                || board.GetComponent<BoardManager>().tiles[position.Item1, position.Item2 + 1].tag == "Junction")
                {
                    //Up
                    if (grid[position.Item1, position.Item2 + 1] == false)
                    {
                        if (roadFound)
                        {
                            RoadWalkerAI(position.Item1, position.Item2 + 1, dontCheck1, dontCheck2);
                        }
                        else
                        {
                            move = new Tuple<int, int>(position.Item1, position.Item2 + 1);
                            roadFound = true;
                        }
                    }
                }
            }

            if (position.Item2 - 1 >= 0)
            {
                if (board.GetComponent<BoardManager>().tiles[position.Item1, position.Item2 - 1].tag == "Road"
                || board.GetComponent<BoardManager>().tiles[position.Item1, position.Item2 - 1].tag == "Junction")
                {
                    //Down
                    if (grid[position.Item1, position.Item2 - 1] == false)
                    {
                        if (roadFound)
                        {
                            RoadWalkerAI(position.Item1, position.Item2 - 1, dontCheck1, dontCheck2);
                        }
                        else
                        {
                            move = new Tuple<int, int>(position.Item1, position.Item2 - 1);
                            roadFound = true;
                        }
                    }
                }
            }
            if (move != null)
            {
                position = move;
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

    public void SplitBuildings()
    {
        int buildingCount = board.GetComponent<BoardManager>().buildings.Count;
        for (int i = 0; i < buildingCount; i++)
        {
            int lowRow = rows;
            int lowColumn = columns;
            int maxRow = 0;
            int maxColumn = 0;
            int splitPoint = 0;
            string direction = "";
            foreach (Tuple<int, int> tile in board.GetComponent<BoardManager>().buildings[i].GetComponent<BuildingScript>().GetTiles())
            {
                if (maxRow < tile.Item1)
                {
                    maxRow = tile.Item1;
                }
                else if (lowRow > tile.Item1)
                {
                    lowRow = tile.Item1;
                }
                if (maxColumn < tile.Item2)
                {
                    maxColumn = tile.Item2;
                }
                else if (lowColumn > tile.Item2)
                {
                    lowColumn = tile.Item2;
                }
            }
            if (maxRow - lowRow > maxColumn - lowColumn)
            {
                direction = "row";
                splitPoint = UnityEngine.Random.Range(lowRow, maxRow);
            }
            else
            {
                direction = "column";
                splitPoint = UnityEngine.Random.Range(lowColumn, maxColumn);
            }
            board.GetComponent<BoardManager>().SplitBuilding(i, splitPoint, direction);
        }
    }

    public void Merge()
    {
        board.GetComponent<BoardManager>().Combine();
    }
}
