using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public int boardRows;
    public int boardColumns;
    private int junctionIndex;
    private int roadIndex;
    private int buildingIndex;

    public GameObject[,] tiles;
    public List<GameObject> buildings;
    public List<GameObject> roads;
    public List<GameObject> junctions;

    public GameObject tilePrefab;
    public GameObject buildingPrefab;
    public GameObject roadPrefab;
    public GameObject junctionPrefab;
    public Sprite wallSprite;
    // Start is called before the first frame update
    void Awake()
    {
        tiles = new GameObject[boardRows, boardColumns];
        buildings = new List<GameObject>();
        roads = new List<GameObject>();
        junctions = new List<GameObject>();
        for (int i = 0; i < boardRows; i++)
        {
            for (int j = 0; j < boardColumns; j++)
            {
                tiles[i, j] = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity, transform);
                tiles[i, j].GetComponent<TileScript>().SetPosition(i, j);
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
        foreach (GameObject road in roads)
        {
            Destroy(road);
        }
        roads.Clear();
        foreach (GameObject junction in junctions)
        {
            Destroy(junction);
        }
        junctions.Clear();

        foreach (GameObject building in buildings)
        {
            Destroy(building);
        }
        buildings.Clear();

        roadIndex = 0;
        junctionIndex = 0;
        buildingIndex = 0;
        boardRows = GridSize;
        boardColumns = GridSize;
        tiles = new GameObject[GridSize, GridSize];
        roads = new List<GameObject>();
        junctions = new List<GameObject>();
        for (int i = 0; i < boardRows; i++)
        {
            for (int j = 0; j < boardColumns; j++)
            {
                tiles[i, j] = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity, transform);
                tiles[i, j].GetComponent<TileScript>().SetPosition(i, j);
            }
        }
    }

    public void AddRoad()
    {
        roads.Add(Instantiate(roadPrefab, transform));
        roads[roadIndex].GetComponent<RoadScript>().SetID(roadIndex);
    }

    public void GenerateRoads()
    {
        for (int x = 0; x < boardRows; x++)
        {
            if ((tiles[x, 0].tag == "Road" || tiles[x, 0].tag == "Junction") && (tiles[x, 1].tag == "Road" || tiles[x, 1].tag == "Junction"))
            {
                int y = 0;
                while (y < boardColumns)
                {
                    if (tiles[x, y].tag == "Road")
                    {
                        AddRoad();
                        while (y < boardColumns && tiles[x, y].tag == "Road")
                        {
                            roads[roadIndex].GetComponent<RoadScript>().AddTile(x, y);
                            y++;
                        }
                        roadIndex++;
                    }
                    y++;
                }
            }
        }
        for (int y = 0; y < boardColumns; y++)
        {
            if ((tiles[0, y].tag == "Road" || tiles[0, y].tag == "Junction") && (tiles[1, y].tag == "Road" || tiles[1, y].tag == "Junction"))
            {
                int x = 0;
                while (x < boardRows)
                {
                    if (tiles[x, y].tag == "Road")
                    {
                        AddRoad();
                        while (x < boardRows && tiles[x, y].tag == "Road")
                        {
                            roads[roadIndex].GetComponent<RoadScript>().AddTile(x, y);
                            x++;
                        }
                        roadIndex++;
                    }
                    x++;
                }
            }
        }
    }

    public void AddJunction(int x, int y)
    {
        junctions.Add(Instantiate(junctionPrefab, transform));
        junctions[junctionIndex].GetComponent<JunctionScript>().SetTile(x, y);
        junctionIndex++;
    }

    public void GenerateJunctions()
    {
        Tuple<int, int> tileToCheck;
        foreach (GameObject junction in junctions)
        {
            int x = junction.GetComponent<JunctionScript>().GetTile().Item1;
            int y = junction.GetComponent<JunctionScript>().GetTile().Item2;
            tileToCheck = new Tuple<int, int>(x - 1, y); //Left
            CheckRoad(junction, tileToCheck.Item1, tileToCheck.Item2);
            tileToCheck = new Tuple<int, int>(x + 1, y); //Right
            CheckRoad(junction, tileToCheck.Item1, tileToCheck.Item2);
            tileToCheck = new Tuple<int, int>(x, y - 1); //Down
            CheckRoad(junction, tileToCheck.Item1, tileToCheck.Item2);
            tileToCheck = new Tuple<int, int>(x, y + 1); //Up
            CheckRoad(junction, tileToCheck.Item1, tileToCheck.Item2);
        }
    }

    private void CheckRoad(GameObject junction, int x, int y)
    {
        foreach (GameObject road in roads)
        {
            if (road.GetComponent<RoadScript>().GetFirstTile().Item1 == x && road.GetComponent<RoadScript>().GetFirstTile().Item2 == y)
            {
                junction.GetComponent<JunctionScript>().AddRoad(road.GetComponent<RoadScript>().GetID());
            }
            else if (road.GetComponent<RoadScript>().GetLastTile().Item1 == x && road.GetComponent<RoadScript>().GetLastTile().Item2 == y)
            {
                junction.GetComponent<JunctionScript>().AddRoad(road.GetComponent<RoadScript>().GetID());
            }
        }
    }

    public void RemoveRoad(GameObject road, int roadID)
    {
        foreach (Tuple<int, int> tile in road.GetComponent<RoadScript>().GetTiles())
        {
            FreeTile(tile.Item1, tile.Item2);
        }
        for (int i = 0; i < junctions.Count; i++)
        {
            if (junctions[i].GetComponent<JunctionScript>().GetRoads().Contains(roadID))
            {
                junctions[i].GetComponent<JunctionScript>().RemoveRoad(roadID);
            }
        }
        roads.Remove(road);
        roadIndex--; //Dunno if will be used, but w/e
    }

    public void FixJunctions()
    {
        for (int i = 0; i < junctions.Count; i++)
        {
            if (junctions[i].GetComponent<JunctionScript>().GetRoads().Count < 1)
            {
                RemoveEmptyJunction(i);
                i--;
            }
            else if (junctions[i].GetComponent<JunctionScript>().GetRoads().Count == 1)
            {
                RemoveDeadEnd(i);
                i--;
            }
        }
    }

    private void RemoveEmptyJunction(int i)
    {
        int x = junctions[i].GetComponent<JunctionScript>().GetTile().Item1;
        int y = junctions[i].GetComponent<JunctionScript>().GetTile().Item2;
        FreeTile(x, y);
        junctions.RemoveAt(i);
    }

    private void RemoveDeadEnd(int i)
    {
        int roadID = junctions[i].GetComponent<JunctionScript>().GetRoads()[0];
        foreach (GameObject road in roads)
        {
            if (road.GetComponent<RoadScript>().GetID() == roadID)
            {
                RemoveRoad(road, roadID);
                break;
            }
        }
        RemoveEmptyJunction(i);
    }

    public void FreeTile(int x, int y)
    {
        tiles[x, y].tag = "Untagged";
        tiles[x, y].GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void PaintBuildings()
    {
        foreach (GameObject building in buildings)
        {
            foreach (Tuple<int, int> tile in building.GetComponent<BuildingScript>().GetTiles())
            {
                int x = tile.Item1;
                int y = tile.Item2;
                tiles[x, y].GetComponent<SpriteRenderer>().color = building.GetComponent<BuildingScript>().GetColor();
            }
        }
    }

    public void GenerateBuildings()
    {
        foreach (GameObject tile in tiles)
        {
            if (tile.tag == "Untagged")
            {
                buildings.Add(Instantiate(buildingPrefab, transform));
                buildings[buildingIndex].GetComponent<BuildingScript>().SetID(buildingIndex);
                buildings[buildingIndex].GetComponent<BuildingScript>().SetColor(new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)));
                int x = tile.GetComponent<TileScript>().position.Item1;
                int y = tile.GetComponent<TileScript>().position.Item2;
                HouseBuilderAI(x, y, buildings[buildingIndex]);
                buildingIndex++;
            }
        }
    }

    public int SplitBuilding(int i, int splitPoint, string direction)
    {
        buildings.Add(Instantiate(buildingPrefab, transform));
        buildings[buildingIndex].GetComponent<BuildingScript>().SetID(buildingIndex);
        buildings[buildingIndex].GetComponent<BuildingScript>().SetColor(new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)));
        List<Tuple<int, int>> removeList = new List<Tuple<int, int>>();
        if (direction == "row")
        {
            foreach (Tuple<int, int> tile in buildings[i].GetComponent<BuildingScript>().GetTiles())
            {
                if (tile.Item1 <= splitPoint)
                {
                    buildings[buildingIndex].GetComponent<BuildingScript>().AddTile(tile.Item1, tile.Item2);
                    removeList.Add(new Tuple<int, int>(tile.Item1, tile.Item2));
                }
            }
        }
        else if (direction == "column")
        {
            foreach (Tuple<int, int> tile in buildings[i].GetComponent<BuildingScript>().GetTiles())
            {
                if (tile.Item2 <= splitPoint)
                {
                    buildings[buildingIndex].GetComponent<BuildingScript>().AddTile(tile.Item1, tile.Item2);
                    removeList.Add(new Tuple<int, int>(tile.Item1, tile.Item2));
                }
            }
        }
        foreach (Tuple<int, int> tile in removeList)
        {
            buildings[i].GetComponent<BuildingScript>().RemoveTile(tile.Item1, tile.Item2);
        }
        buildingIndex++;
        return (buildingIndex-1);
    }

    public void MakeWalls()
    {
        foreach (GameObject building in buildings)
        {
            foreach (Tuple<int, int> tile in building.GetComponent<BuildingScript>().GetTiles())
            {
                if (tiles[tile.Item1, tile.Item2].tag != "Road" && tiles[tile.Item1, tile.Item2].tag != "Junction")
                {
                    if (tile.Item1 != boardRows - 1)
                    {
                        if (!building.GetComponent<BuildingScript>().GetTiles().Contains(new Tuple<int, int>(tile.Item1 + 1, tile.Item2)))
                        {
                            tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().AddWall(wallSprite, 2);
                        }
                    }
                    else
                    {
                        tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().AddWall(wallSprite, 2);
                    }

                    if (tile.Item1 != 0)
                    {
                        if (!building.GetComponent<BuildingScript>().GetTiles().Contains(new Tuple<int, int>(tile.Item1 - 1, tile.Item2)))
                        {
                            tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().AddWall(wallSprite, 1);
                        }
                    }
                    else
                    {
                        tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().AddWall(wallSprite, 1);
                    }

                    if (tile.Item2 != boardColumns - 1)
                    {
                        if (!building.GetComponent<BuildingScript>().GetTiles().Contains(new Tuple<int, int>(tile.Item1, tile.Item2 + 1)))
                        {
                            tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().AddWall(wallSprite, 4);
                        }
                    }
                    else
                    {
                        tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().AddWall(wallSprite, 4);
                    }

                    if (tile.Item2 != 0)
                    {
                        if (!building.GetComponent<BuildingScript>().GetTiles().Contains(new Tuple<int, int>(tile.Item1, tile.Item2 - 1)))
                        {
                            tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().AddWall(wallSprite, 8);
                        }
                    }
                    else
                    {
                        tiles[tile.Item1, tile.Item2].GetComponent<TileScript>().AddWall(wallSprite, 8);
                    }
                }
            }
        }
    }
    public void HouseBuilderAI(int x, int y, GameObject building)
    {
        Tuple<int, int> position = new Tuple<int, int>(x, y);
        bool freeTileFound = true;
        while (freeTileFound)
        {
            freeTileFound = false;
            Tuple<int, int> move = null;
            building.GetComponent<BuildingScript>().AddTile(position.Item1, position.Item2);
            tiles[position.Item1, position.Item2].tag = "Occupado";
            tiles[position.Item1, position.Item2].GetComponent<TileScript>().MakeBuildingRoomParent();

            if (position.Item1 + 1 < boardRows)
            {
                if (tiles[position.Item1 + 1, position.Item2].tag == "Untagged")
                {
                    //Right
                    move = new Tuple<int, int>(position.Item1 + 1, position.Item2);
                    freeTileFound = true;

                }
            }

            if (position.Item1 - 1 >= 0)
            {
                if (tiles[position.Item1 - 1, position.Item2].tag == "Untagged")
                {
                    //Left

                    if (freeTileFound)
                    {
                        HouseBuilderAI(position.Item1 - 1, position.Item2, building);
                    }
                    else
                    {
                        move = new Tuple<int, int>(position.Item1 - 1, position.Item2);
                        freeTileFound = true;
                    }

                }
            }

            if (position.Item2 + 1 < boardColumns)
            {
                if (tiles[position.Item1, position.Item2 + 1].tag == "Untagged")
                {
                    //Up

                    if (freeTileFound)
                    {
                        HouseBuilderAI(position.Item1, position.Item2 + 1, building);
                    }
                    else
                    {
                        move = new Tuple<int, int>(position.Item1, position.Item2 + 1);
                        freeTileFound = true;
                    }

                }
            }

            if (position.Item2 - 1 >= 0)
            {
                if (tiles[position.Item1, position.Item2 - 1].tag == "Untagged")
                {
                    //Down
                    if (freeTileFound)
                    {
                        HouseBuilderAI(position.Item1, position.Item2 - 1, building);
                    }
                    else
                    {
                        move = new Tuple<int, int>(position.Item1, position.Item2 - 1);
                        freeTileFound = true;
                    }

                }
            }
            if (move != null)
            {
                position = move;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    //------------------------------------------------------- Deprecated shit below here -------------------------------------------------------||

    public void GenerateSpaces() //Deprecated version
    {
        foreach (GameObject junction in GameObject.FindGameObjectsWithTag("Junction"))
        {
            //junctions.Add(junction.GetComponent<TileScript>().position);
        }
        foreach (GameObject building in buildings)
            Destroy(building);
        buildings.Clear();
        int currentList = 0;
        bool itHasHappened = false;
        int buildingsOnAColumn = 0;
        for (int x = 0; x < boardRows; x++)
        {
            for (int y = 0; y < boardColumns; y++)
            {
                if (x < boardRows && y < boardColumns && tiles[x, y].tag != "Road" && tiles[x, y].tag != "Junction" && tiles[x, y].tag != "Occupado")
                {
                    int currentX = x;
                    buildings.Add(Instantiate(buildingPrefab, transform));
                    buildings[currentList].GetComponent<BuildingScript>().SetID(currentList);
                    while (y < boardColumns && tiles[x, y].tag != "Road" && tiles[x, y].tag != "Junction" && tiles[x, y].tag != "Occupado")
                    {
                        buildings[currentList].GetComponent<BuildingScript>().AddTile(x, y);
                        tiles[x, y].tag = "Occupado";
                        x++;
                        while (x < boardRows && tiles[x, y].tag != "Road" && tiles[x, y].tag != "Junction" && tiles[x, y].tag != "Occupado")
                        {
                            buildings[currentList].GetComponent<BuildingScript>().AddTile(x, y);
                            tiles[x, y].tag = "Occupado";
                            x++;
                        }
                        x = currentX;
                        y++;
                    }
                    currentList++;
                }
            }
            if (!itHasHappened)
            {
                if (buildings.Count > 0)
                {
                    itHasHappened = true;
                    buildingsOnAColumn = buildings.Count;
                    Debug.Log(buildingsOnAColumn);
                }
            }
        }
        float r = 1f;
        float g = 0f;
        float b = 0f;
        foreach (GameObject building in buildings)
        {
            building.GetComponent<BuildingScript>().AddNeighbours(buildingsOnAColumn, buildings.Count);
            r = UnityEngine.Random.Range(0f, 1f);
            g = UnityEngine.Random.Range(0f, 1f);
            b = UnityEngine.Random.Range(0f, 1f);
            building.GetComponent<BuildingScript>().SetColor(new Color(r, g, b));
            foreach (Tuple<int, int> tile in building.GetComponent<BuildingScript>().GetTiles())
                tiles[tile.Item1, tile.Item2].GetComponent<SpriteRenderer>().color = new Color(r, g, b);
        }
        //Shows that neighbours work.
        //foreach (int i in buildings[6].GetComponent<BuildingScript>().GetNeighbours())
        //{
        //    foreach (Tuple<int, int> tile in buildings[i].GetComponent<BuildingScript>().GetTiles())
        //        tiles[tile.Item1, tile.Item2].GetComponent<SpriteRenderer>().color = new Color(r, g, b);
        //}
    }

    public void Combine() //Deprecated
    {
        //DONE Tell second building's neigbours that the second building's id in their neigbour-lists should be the first building's id.
        //DONE Put all tiles from second building into first building.
        //Search right and down(up?) to see where the road is
        //When the road is found, add those tiles, and make them Occupado.
        //DONE Delete second building.
        List<int> toRemove = new List<int>();
        List<int> noTouchy = new List<int>();
        Debug.Log("Buildings.Count Before = " + buildings.Count);
        for (int i = 0; i < buildings.Count; i++) //Look through all buildings.
        {
            Debug.Log("Iteration: " + i);
            int buildingID = buildings[i].GetComponent<BuildingScript>().GetID();
            Debug.Log("Building ID: " + buildingID);
            if (!toRemove.Contains(buildingID)) //If a building has already been merged into another building this run, then don't bother.
            {
                Debug.Log("MERGE: Building has not been merged yet.");
                //Get a neighbour building ID from the building. This neighbour will be merged into the building.
                int neighbourID = buildings[i].GetComponent<BuildingScript>().GetNeighbours()[UnityEngine.Random.Range(0, buildings[i].GetComponent<BuildingScript>().GetNeighbours().Count)];
                Debug.Log("Neighbour ID: " + neighbourID);
                int neighbourIndex = 0;
                foreach (GameObject building in buildings) //Get the neighbour building Index in the buildings list.
                {
                    if (building.GetComponent<BuildingScript>().GetID() == neighbourID)
                    {
                        break;
                    }
                    neighbourIndex++;
                }
                Debug.Log("Neighbour Index: " + neighbourIndex);
                if (!noTouchy.Contains(neighbourID)) //If the neighbour has already had one of its neighbours been merged into it, don't bother.
                {
                    Debug.Log("MERGE: Neighbour hasn't been merged yet.");
                    Debug.Log("Start changing neighbour's neighbour's neighbours!");
                    //For each neighbour of the neighbours in the neighbour.
                    foreach (int neighbourneighbourID in buildings[neighbourIndex].GetComponent<BuildingScript>().GetNeighbours())
                    {
                        int neighbourneighbourIndex = 0;
                        //Get the neighour's neighbour's Index in the list of buildings.
                        foreach (GameObject building in buildings)
                        {
                            if (building.GetComponent<BuildingScript>().GetID() == neighbourneighbourID)
                            {
                                break;
                            }
                            neighbourneighbourIndex++;
                        }
                        if (neighbourIndex >= buildings.Count)
                        {
                            break;
                        }
                        Debug.Log("Neighbourneighbour ID: " + neighbourneighbourID);
                        Debug.Log("Neighbourneighbour Index: " + neighbourneighbourIndex);
                        Debug.Log("Buildings max index: " + (buildings.Count - 1));
                        //If that neighbour's neighbour isn't the building we're merging into...
                        if (neighbourneighbourID != buildingID)
                        {
                            //Switch the neighbour's neighbour's neighbours from the neighbour to the building we're merging into.
                            buildings[neighbourneighbourIndex].GetComponent<BuildingScript>().SwitchNeighbour(neighbourID, buildingID);
                            Debug.Log("Building ID " + neighbourneighbourID + " Index " + neighbourneighbourIndex + " has switched neighbour ID" + neighbourID + " Index " + neighbourIndex + " to building ID " + buildingID + " Index " + i);
                            buildings[i].GetComponent<BuildingScript>().AddNeighbour(neighbourneighbourID);
                            Debug.Log("Building ID " + buildingID + " Index " + i + " has added neighbour ID " + neighbourneighbourID + " Index " + neighbourneighbourIndex);
                        }
                    }
                    foreach (int neighbour in buildings[i].GetComponent<BuildingScript>().GetNeighbours())
                    {
                        if (neighbour == neighbourID)
                        {
                            buildings[i].GetComponent<BuildingScript>().RemoveNeighbour(neighbourID);
                            Debug.Log("Removed neighbour ID " + neighbourID + " Index " + neighbourIndex + " from building ID " + buildingID + " Index " + i);
                            break;
                        }
                    }
                    foreach (Tuple<int, int> tile in buildings[neighbourID].GetComponent<BuildingScript>().GetTiles()) //For each tile in the neighbour...
                    {
                        buildings[i].GetComponent<BuildingScript>().AddTile(tile.Item1, tile.Item2); //In the building add the tile with x, y from neighbour.
                    }
                    foreach (Tuple<int, int> tile in buildings[i].GetComponent<BuildingScript>().GetTiles()) //Set the color of all tiles.
                    {
                        tiles[tile.Item1, tile.Item2].GetComponent<SpriteRenderer>().color = buildings[i].GetComponent<BuildingScript>().GetColor();
                    }
                    //Add the neighbour's ID to the list of items that should be removed.
                    toRemove.Add(neighbourID);
                    //Add the building's ID to the list of items that should not be further merged in this run.
                    noTouchy.Add(buildingID);
                    noTouchy.Add(neighbourID);
                }
            }
        }
        //For each ID to remove...
        foreach (int id in toRemove)
        {
            foreach (GameObject building in buildings)
            {
                //Check if that ID is equal to any ID in the buildings list...
                if (building.GetComponent<BuildingScript>().GetID() == id)
                {
                    //And then obliterate it.
                    buildings.Remove(building);
                    Destroy(building);
                    break;
                }
            }
        }
        Debug.Log("Buildings.Count After = " + buildings.Count);
        Debug.Log("EOL ---------------------------------------------------");
    }
}
