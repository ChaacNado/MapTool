using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PCGScript : MonoBehaviour
{
    public GameObject board;
    public Sprite wallSprite;
    int RoadChance;
    int GridSize;
    int minBuildingWidth;
    int minBuildingHeight;
    int maxBuildingWidth;
    int maxBuildingHeight;
    bool[,] grid;
    int rows;
    int columns;
    string fileName;
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

        GameObject.FindWithTag("MinBuildingWidth").GetComponent<InputField>().onEndEdit.AddListener(delegate
        {
            minBuildingWidth = int.Parse(GameObject.FindWithTag("MinBuildingWidth").GetComponent<InputField>().text);
        });
        GameObject.FindWithTag("MinBuildingHeight").GetComponent<InputField>().onEndEdit.AddListener(delegate
        {
            minBuildingHeight = int.Parse(GameObject.FindWithTag("MinBuildingHeight").GetComponent<InputField>().text);
        });

        GameObject.FindWithTag("MaxBuildingWidth").GetComponent<InputField>().onEndEdit.AddListener(delegate
        {
            maxBuildingWidth = int.Parse(GameObject.FindWithTag("MaxBuildingWidth").GetComponent<InputField>().text);
        });
        GameObject.FindWithTag("MaxBuildingHeight").GetComponent<InputField>().onEndEdit.AddListener(delegate
        {
            maxBuildingHeight = int.Parse(GameObject.FindWithTag("MaxBuildingHeight").GetComponent<InputField>().text);
        });
        GameObject.FindWithTag("FileName").GetComponent<InputField>().onEndEdit.AddListener(delegate
        {
            fileName = GameObject.FindWithTag("FileName").GetComponent<InputField>().text;
        });
    }

    public void Generate()
    {
        board.GetComponent<BoardManager>().Prepare(GridSize);
        rows = board.GetComponent<BoardManager>().boardRows;
        columns = board.GetComponent<BoardManager>().boardColumns;
        int stepsSinceLastRoad = minBuildingWidth;
        for (int i = minBuildingWidth; i < rows - minBuildingWidth; i++)
        {
            if (stepsSinceLastRoad >= maxBuildingWidth)
            {
                FillHer(i, "row");
                i += minBuildingWidth;
                stepsSinceLastRoad = minBuildingWidth;
            }
            else if (UnityEngine.Random.Range(0, 100) < RoadChance)
            {
                FillHer(i, "row");
                i += minBuildingWidth;
                stepsSinceLastRoad = minBuildingWidth;
            }
            stepsSinceLastRoad++;
        }
        stepsSinceLastRoad = minBuildingHeight;
        for (int j = minBuildingHeight; j < columns - minBuildingHeight; j++)
        {
            if (stepsSinceLastRoad >= maxBuildingHeight)
            {
                FillHer(j, "column");
                j += minBuildingHeight;
                stepsSinceLastRoad = minBuildingHeight;
            }
            else if (UnityEngine.Random.Range(0, 100) < RoadChance)
            {
                FillHer(j, "column");
                j += minBuildingHeight;
                stepsSinceLastRoad = minBuildingHeight;
            }
            stepsSinceLastRoad++;
        }
        board.GetComponent<BoardManager>().GenerateRoads();
        board.GetComponent<BoardManager>().GenerateJunctions();
        RemoveRoads();
        board.GetComponent<BoardManager>().FixJunctions();
        board.GetComponent<BoardManager>().GenerateBuildings();
        SplitBuildings();
        board.GetComponent<BoardManager>().PaintBuildings();
        board.GetComponent<BoardManager>().MakeWalls();
        GenerateInnerWalls();
        //board.GetComponent<BoardManager>().GenerateSpaces();
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
        List<int> newBuildings = new List<int>();
        int index = 0;
        do
        {
            for (int i = index; i < buildingCount; i++)
            {
                while (true)
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
                    int width = maxRow - lowRow;
                    int height = maxColumn - lowColumn;
                    if (width >= minBuildingWidth * 2)
                    {
                        direction = "row";
                        splitPoint = UnityEngine.Random.Range(lowRow + minBuildingWidth, maxRow - minBuildingWidth);
                    }
                    else if (height >= minBuildingHeight * 2)
                    {
                        direction = "column";
                        splitPoint = UnityEngine.Random.Range(lowColumn + minBuildingHeight, maxColumn - minBuildingHeight);
                    }
                    else
                    {
                        newBuildings.Remove(i);
                        break;
                    }
                    newBuildings.Add(board.GetComponent<BoardManager>().SplitBuilding(i, splitPoint, direction));
                }
            }
            index = buildingCount;
            buildingCount += newBuildings.Count;
        } while (newBuildings.Count > 0);
    }

    public void GenerateInnerWalls()
    {
        foreach (GameObject building in board.GetComponent<BoardManager>().buildings)
        {
            int lowRow = rows;
            int lowColumn = columns;
            int maxRow = 0;
            int maxColumn = 0;
            foreach (Tuple<int, int> tile in building.GetComponent<BuildingScript>().GetTiles())
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
            int width = maxRow - lowRow + 1;
            int height = maxColumn - lowColumn + 1;
            if (height > width)
            {
                List<int> randomColumns = new List<int>();
                for (int i = lowColumn + 1; i < maxColumn - 1; i++)
                {
                    if (UnityEngine.Random.Range(0, 100) <= 20)
                    {
                        randomColumns.Add(i);
                    }
                }
                foreach (Tuple<int, int> tile in building.GetComponent<BuildingScript>().GetTiles())
                {
                    if (tile.Item1 == maxRow - width / 2)
                    {
                        for (int x = 0; x < 2; x++)
                        {
                            for (int y = 0; y < 2; y++)
                            {
                                if (x == 0)
                                    board.GetComponent<BoardManager>().tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().tiles[x, y].GetComponent<TileScript>().AddWall(wallSprite, 1);
                                else
                                    board.GetComponent<BoardManager>().tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().tiles[x, y].GetComponent<TileScript>().AddWall(wallSprite, 2);
                            }
                        }
                    }
                    else
                    {
                        foreach (int randomColumn in randomColumns)
                        {
                            if (tile.Item2 == randomColumn)
                            {
                                board.GetComponent<BoardManager>().tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().tiles[1, 0].GetComponent<TileScript>().AddWall(wallSprite, 4);
                                board.GetComponent<BoardManager>().tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().tiles[0, 0].GetComponent<TileScript>().AddWall(wallSprite, 4);
                            }
                        }
                    }
                }
            }
            else
            {
                List<int> randomRows = new List<int>();
                for (int i = lowRow + 1; i < maxRow - 1; i++)
                {
                    if (UnityEngine.Random.Range(0, 100) <= 20)
                    {
                        randomRows.Add(i);
                    }
                }
                foreach (Tuple<int, int> tile in building.GetComponent<BuildingScript>().GetTiles())
                {
                    if (tile.Item2 == maxColumn - height / 2)
                    {
                        for (int x = 0; x < 2; x++)
                        {
                            for (int y = 0; y < 2; y++)
                            {
                                if (y == 0)
                                    board.GetComponent<BoardManager>().tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().tiles[x, y].GetComponent<TileScript>().AddWall(wallSprite, 4);
                                else
                                    board.GetComponent<BoardManager>().tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().tiles[x, y].GetComponent<TileScript>().AddWall(wallSprite, 8);
                            }
                        }
                    }
                    else
                    {
                        foreach (int randomRow in randomRows)
                        {
                            if (tile.Item1 == randomRow)
                            {
                                board.GetComponent<BoardManager>().tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().tiles[1, 0].GetComponent<TileScript>().AddWall(wallSprite, 2);
                                board.GetComponent<BoardManager>().tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().tiles[1, 1].GetComponent<TileScript>().AddWall(wallSprite, 2);
                            }
                        }
                    }
                }
            }
        }
    }
    public void Merge()
    {
        board.GetComponent<BoardManager>().Combine();
    }

    public void Export()
    {
        ExportScript.OpenFolderPanel(fileName);
        foreach (GameObject building in board.GetComponent<BoardManager>().buildings)
        {
            ExportScript.WriteBuildingToFile(building.GetComponent<BuildingScript>().GetID());
            foreach (Tuple<int, int> tile in building.GetComponent<BuildingScript>().GetTiles())
            {
                foreach (GameObject wall in board.GetComponent<BoardManager>().tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().walls)
                {
                    ExportScript.WriteWallsToFile(wall.transform.position.x, wall.transform.position.y, wall.GetComponent<SpriteRenderer>().size.x, wall.GetComponent<SpriteRenderer>().size.y);
                }
                foreach (GameObject innerTile in board.GetComponent<BoardManager>().tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().tiles)
                {
                    foreach (GameObject wall in innerTile.GetComponent<TileScript>().walls)
                    {
                        ExportScript.WriteWallsToFile(wall.transform.position.x, wall.transform.position.y, wall.GetComponent<SpriteRenderer>().size.x, wall.GetComponent<SpriteRenderer>().size.y);
                    }
                }
            }
        }

        foreach (GameObject road in board.GetComponent<BoardManager>().roads)
        {
            string alignment = "";
            if (road.GetComponent<RoadScript>().GetFirstTile().Item1 == road.GetComponent<RoadScript>().GetLastTile().Item1)
            {
                alignment = "h";
            }
            else
            {
                alignment = "v";
            }
            foreach (Tuple<int, int> tile in road.GetComponent<RoadScript>().GetTiles())
            {
                ExportScript.WriteRoadToFile(board.GetComponent<BoardManager>().tiles[tile.Item1, tile.Item2].transform.position.x, board.GetComponent<BoardManager>().tiles[tile.Item1, tile.Item2].transform.position.y, alignment);
            }
        }

        foreach (GameObject junction in board.GetComponent<BoardManager>().junctions)
        {
            Tuple<int, int> tile = junction.GetComponent<JunctionScript>().GetTile();
            ExportScript.WriteJunctionToFile(board.GetComponent<BoardManager>().tiles[tile.Item1, tile.Item2].transform.position.x, board.GetComponent<BoardManager>().tiles[tile.Item1, tile.Item2].transform.position.y);
        }

        TestImportScript.ReadString();
    }
}
