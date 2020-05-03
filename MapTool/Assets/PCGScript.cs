using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PCGScript : MonoBehaviour
{
    public BoardManager board;
    public Sprite wallSprite;
    public GameObject status;
    int gridSizeX;
    int gridSizeY;
    int minBuildingWidth;
    int minBuildingHeight;
    int maxBuildingWidth;
    int maxBuildingHeight;
    int splitBuildingChance;
    float RemoveBuildingChance;
    bool[,] grid;
    int rows;
    int columns;
    string fileName;


    // Start is called before the first frame update
    void Start()
    {
        //GameObject.FindWithTag("RoadChance").GetComponent<InputField>().onEndEdit.AddListener(delegate
        //{
        //    RoadChance = int.Parse(GameObject.FindWithTag("RoadChance").GetComponent<InputField>().text);
        //});
        //GameObject.FindWithTag("GridSize").GetComponent<InputField>().onEndEdit.AddListener(delegate
        //{
        //    GridSize = int.Parse(GameObject.FindWithTag("GridSize").GetComponent<InputField>().text);
        //});

        //GameObject.FindWithTag("MinBuildingWidth").GetComponent<InputField>().onEndEdit.AddListener(delegate
        //{
        //    minBuildingWidth = int.Parse(GameObject.FindWithTag("MinBuildingWidth").GetComponent<InputField>().text);
        //});
        //GameObject.FindWithTag("MinBuildingHeight").GetComponent<InputField>().onEndEdit.AddListener(delegate
        //{
        //    minBuildingHeight = int.Parse(GameObject.FindWithTag("MinBuildingHeight").GetComponent<InputField>().text);
        //});

        //GameObject.FindWithTag("MaxBuildingWidth").GetComponent<InputField>().onEndEdit.AddListener(delegate
        //{
        //    maxBuildingWidth = int.Parse(GameObject.FindWithTag("MaxBuildingWidth").GetComponent<InputField>().text);
        //});
        //GameObject.FindWithTag("MaxBuildingHeight").GetComponent<InputField>().onEndEdit.AddListener(delegate
        //{
        //    maxBuildingHeight = int.Parse(GameObject.FindWithTag("MaxBuildingHeight").GetComponent<InputField>().text);
        //});
        //GameObject.FindWithTag("FileName").GetComponent<InputField>().onEndEdit.AddListener(delegate
        //{
        //    fileName = GameObject.FindWithTag("FileName").GetComponent<InputField>().text;
        //});
    }

    public void Generate()
    {
        gridSizeX = int.Parse(GameObject.FindWithTag("GridSizeX").GetComponent<InputField>().text);
        gridSizeY = int.Parse(GameObject.FindWithTag("GridSizeY").GetComponent<InputField>().text);
        minBuildingWidth = int.Parse(GameObject.FindWithTag("MinBuildingWidth").GetComponent<InputField>().text);
        minBuildingHeight = int.Parse(GameObject.FindWithTag("MinBuildingHeight").GetComponent<InputField>().text);
        maxBuildingWidth = int.Parse(GameObject.FindWithTag("MaxBuildingWidth").GetComponent<InputField>().text);
        maxBuildingHeight = int.Parse(GameObject.FindWithTag("MaxBuildingHeight").GetComponent<InputField>().text);
        splitBuildingChance = int.Parse(GameObject.FindWithTag("SplitBuildingChance").GetComponent<InputField>().text);
        RemoveBuildingChance = float.Parse(GameObject.FindWithTag("RemoveBuildingChance").GetComponent<InputField>().text);

        fileName = GameObject.FindWithTag("FileName").GetComponent<InputField>().text;

        board.Prepare(gridSizeX, gridSizeY);
        rows = board.boardRows;
        columns = board.boardColumns;

        if (GameObject.FindWithTag("GenerateRoads").GetComponent<Toggle>().isOn)
        {
            int buildingSize = UnityEngine.Random.Range(minBuildingWidth + 1, maxBuildingWidth + 2);
            for (int i = buildingSize; i < rows - minBuildingWidth + 1; i += buildingSize)
            {
                FillHer(i, "row");
                buildingSize = UnityEngine.Random.Range(minBuildingWidth + 1, maxBuildingWidth + 2);
            }

            buildingSize = UnityEngine.Random.Range(minBuildingHeight, maxBuildingHeight);
            for (int j = buildingSize; j < columns - minBuildingHeight + 1; j += buildingSize)
            {
                FillHer(j, "column");
                buildingSize = UnityEngine.Random.Range(minBuildingHeight + 1, maxBuildingHeight + 2);
            }
            board.GenerateRoads();
            board.GenerateJunctions();
        }

        if (GameObject.FindWithTag("GenerateBuildings").GetComponent<Toggle>().isOn)
        {
            board.GenerateBuildings();
            board.GiveRoadNeighbours();
        }
        if (GameObject.FindWithTag("RemoveOuterBuildings").GetComponent<Toggle>().isOn)
        {
            board.RemoveBuildings(RemoveBuildingChance / 100f);
        }

        if (GameObject.FindWithTag("RemoveRoads").GetComponent<Toggle>().isOn)
        {
            RemoveRoads();
            board.FixJunctions();
        }

        if (GameObject.FindWithTag("GenerateSubBuildings").GetComponent<Toggle>().isOn)
        {
            SplitBuildings();
        }
        if (GameObject.FindWithTag("PaintBuildings").GetComponent<Toggle>().isOn)
        {
            board.PaintBuildings();
        }
        if (GameObject.FindWithTag("GenerateOuterWalls").GetComponent<Toggle>().isOn)
        {
            board.MakeWalls();
        }
        if (GameObject.FindWithTag("GenerateInnerWalls").GetComponent<Toggle>().isOn)
        {
            GenerateInnerWalls();
        }
        //board.GenerateSpaces();
    }

    void FillHer(int i, string direction)
    {
        switch (direction)
        {
            case "row":
                for (int y = 0; y < columns; y++)
                {
                    board.tiles[i, y].GetComponent<SpriteRenderer>().color = Color.black;
                    if (board.tiles[i, y].tag == "Road")
                    {
                        board.tiles[i, y].tag = "Junction";
                        board.AddJunction(i, y);
                    }
                    else
                    {
                        board.tiles[i, y].tag = "Road";
                    }
                }
                break;

            case "column":
                for (int x = 0; x < rows; x++)
                {
                    board.tiles[x, i].GetComponent<SpriteRenderer>().color = Color.black;
                    if (board.tiles[x, i].tag == "Road")
                    {
                        board.tiles[x, i].tag = "Junction";
                        board.AddJunction(x, i);
                    }
                    else
                    {
                        board.tiles[x, i].tag = "Road";
                    }
                }
                break;
        }
    }

    public bool ConfirmRoadConnectivity(Tuple<int, int> dontCheck1, Tuple<int, int> dontCheck2)
    {
        grid = new bool[rows, columns];
        int startX = board.roads[0].GetComponent<RoadScript>().GetTiles()[0].Item1;
        int startY = board.roads[0].GetComponent<RoadScript>().GetTiles()[0].Item2;
        RoadWalkerAI(startX, startY, dontCheck1, dontCheck2);
        foreach (GameObject road in board.roads)
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
                if (board.tiles[position.Item1 + 1, position.Item2].tag == "Road"
                    || board.tiles[position.Item1 + 1, position.Item2].tag == "Junction")
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
                if (board.tiles[position.Item1 - 1, position.Item2].tag == "Road"
                || board.tiles[position.Item1 - 1, position.Item2].tag == "Junction")
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
                if (board.tiles[position.Item1, position.Item2 + 1].tag == "Road"
                || board.tiles[position.Item1, position.Item2 + 1].tag == "Junction")
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
                if (board.tiles[position.Item1, position.Item2 - 1].tag == "Road"
                || board.tiles[position.Item1, position.Item2 - 1].tag == "Junction")
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
        foreach (GameObject junction in board.junctions)
        {
            if (junction.GetComponent<JunctionScript>().GetRoads().Count > 0)
            {
                int roadID = junction.GetComponent<JunctionScript>().GetRoads()[UnityEngine.Random.Range(0, junction.GetComponent<JunctionScript>().GetRoads().Count)];
                foreach (GameObject road in board.roads)
                {
                    if (road.GetComponent<RoadScript>().GetID() == roadID)
                    {
                        if (ConfirmRoadConnectivity(road.GetComponent<RoadScript>().GetFirstTile(), road.GetComponent<RoadScript>().GetLastTile()))
                        {
                            board.RemoveRoad(road, roadID);
                        }
                        break;
                    }
                }
            }
        }
    }

    public void SplitBuildings()
    {
        int buildingCount = board.buildings.Count;
        List<int> newBuildings = new List<int>();
        int index = 0;
        do
        {
            for (int i = index; i < buildingCount; i++)
            {
                while (true)
                {
                    //Set lowX values to highest possible, to ensure that we get lower values in the algorithm.
                    int lowRow = rows;
                    int lowColumn = columns;
                    //Set maxX values to lowest possible, to ensure that we get higher values in the algorithm.
                    int maxRow = 0;
                    int maxColumn = 0;
                    //Splitpoint is an enigma.
                    int splitPoint = 0;
                    //Direction is a mystery.
                    string direction = "";
                    //Check all tiles in the building
                    foreach (Tuple<int, int> tile in board.buildings[i].GetComponent<BuildingScript>().GetTiles())
                    {
                        //If the tile is further to the right than any of the other tiles, then save its value.
                        if (maxRow < tile.Item1)
                        {
                            maxRow = tile.Item1;
                        }
                        //If the tile is further to the left than any of the other tiles, then save its value.
                        else if (lowRow > tile.Item1)
                        {
                            lowRow = tile.Item1;
                        }
                        //If the tile is further to the up than any of the other tiles, then save its value.
                        if (maxColumn < tile.Item2)
                        {
                            maxColumn = tile.Item2;
                        }
                        //If the tile is further to the down than any of the other tiles, then save its value.
                        else if (lowColumn > tile.Item2)
                        {
                            lowColumn = tile.Item2;
                        }
                    }
                    //Get the building's width and height based on these values (maybe we should just save that in the building when it's created...)
                    int width = maxRow - lowRow;
                    int height = maxColumn - lowColumn;



                    if (UnityEngine.Random.Range(0, 100) <= splitBuildingChance && (width >= minBuildingWidth * 2 || height >= minBuildingHeight * 2))
                    {                 
                        //If the building is wide enough to be split vertically.
                        if (width >= minBuildingWidth * 2)
                        {
                            //Make the decision to split it vertically, and get a random value fitting the limits.
                            direction = "row";
                            splitPoint = UnityEngine.Random.Range(lowRow + minBuildingWidth, maxRow - minBuildingWidth);
                        }
                        //If the building is long enough to be split horizontally.
                        if (height >= minBuildingHeight * 2)
                        {
                            //Make the decision to split it horizontally, and get a random value fitting the limits.
                            direction = "column";
                            splitPoint = UnityEngine.Random.Range(lowColumn + minBuildingHeight, maxColumn - minBuildingHeight);
                        }
                    }
                    //If it's not large enough, let's not split this building.
                    else
                    {
                        newBuildings.Remove(i);
                        break;
                    }
                    //Run the SplitBuilding method in the board script!
                    newBuildings.Add(board.SplitBuilding(i, splitPoint, direction));
                }
            }
            //Start the next iteration of the algorithm from the last building of the last iteration. #nicesentence
            index = buildingCount;
            //Increase the buidling count to accomodate the number of buildings we just added.
            buildingCount += newBuildings.Count;
            //Keep iterating untill we haven't added any more sub-buildings.
        } while (newBuildings.Count > 0);
    }

    public void GenerateInnerWalls()
    {
        foreach (GameObject building in board.buildings)
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
                                    board.tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().tiles[x, y].GetComponent<TileScript>().AddWall(wallSprite, 1);
                                else
                                    board.tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().tiles[x, y].GetComponent<TileScript>().AddWall(wallSprite, 2);
                            }
                        }
                    }
                    else
                    {
                        foreach (int randomColumn in randomColumns)
                        {
                            if (tile.Item2 == randomColumn)
                            {
                                board.tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().tiles[1, 0].GetComponent<TileScript>().AddWall(wallSprite, 4);
                                board.tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().tiles[0, 0].GetComponent<TileScript>().AddWall(wallSprite, 4);
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
                                    board.tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().tiles[x, y].GetComponent<TileScript>().AddWall(wallSprite, 4);
                                else
                                    board.tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().tiles[x, y].GetComponent<TileScript>().AddWall(wallSprite, 8);
                            }
                        }
                    }
                    else
                    {
                        foreach (int randomRow in randomRows)
                        {
                            if (tile.Item1 == randomRow)
                            {
                                board.tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().tiles[1, 0].GetComponent<TileScript>().AddWall(wallSprite, 2);
                                board.tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().tiles[1, 1].GetComponent<TileScript>().AddWall(wallSprite, 2);
                            }
                        }
                    }
                }
            }
        }
    }
    public void Merge()
    {
        board.Combine();
    }

    public void Export()
    {
        ExportScript.OpenFolderPanel(fileName);
        foreach (GameObject building in board.buildings)
        {
            ExportScript.WriteBuildingToFile(building.GetComponent<BuildingScript>().GetID());
            foreach (Tuple<int, int> tile in building.GetComponent<BuildingScript>().GetTiles())
            {
                foreach (GameObject wall in board.tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().walls)
                {
                    ExportScript.WriteWallsToFile(wall.transform.position.x, wall.transform.position.y, wall.GetComponent<SpriteRenderer>().size.x, wall.GetComponent<SpriteRenderer>().size.y);
                }
                foreach (GameObject innerTile in board.tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().tiles)
                {
                    foreach (GameObject wall in innerTile.GetComponent<TileScript>().walls)
                    {
                        ExportScript.WriteWallsToFile(wall.transform.position.x, wall.transform.position.y, wall.GetComponent<SpriteRenderer>().size.x, wall.GetComponent<SpriteRenderer>().size.y);
                    }
                }
            }
        }

        foreach (GameObject road in board.roads)
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
                ExportScript.WriteRoadToFile(board.tiles[tile.Item1, tile.Item2].transform.position.x, board.tiles[tile.Item1, tile.Item2].transform.position.y, alignment);
            }
        }

        foreach (GameObject junction in board.junctions)
        {
            Tuple<int, int> tile = junction.GetComponent<JunctionScript>().GetTile();
            ExportScript.WriteJunctionToFile(board.tiles[tile.Item1, tile.Item2].transform.position.x, board.tiles[tile.Item1, tile.Item2].transform.position.y);
        }
        if (ExportScript.path.Length > 40)
            status.GetComponent<Text>().text = "Map exported to: ..." + ExportScript.path.Substring(ExportScript.path.Length - 40);
        else
            status.GetComponent<Text>().text = "Map exported to: " + ExportScript.path;
        //TestImportScript.ReadString();
    }
}
