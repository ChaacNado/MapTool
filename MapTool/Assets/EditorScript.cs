using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //foreach (GameObject tile in Board.GetComponent<BoardManager>().tiles)
        //{
        //    if (tile.tag != "Road" && tile.tag != "Junction" && tile.GetComponent<Collider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        //    {
        //        foreach (GameObject building in Board.GetComponent<BoardManager>().buildings)
        //        {
        //            foreach (Tuple<int,int> buildingTile in building.GetComponent<BuildingScript>().GetTiles())
        //            {
        //                if (tile.GetComponent<TileScript>().position.Item1 == buildingTile.Item1)
        //                {
        //                    if (tile.GetComponent<TileScript>().position.Item2 == buildingTile.Item2)
        //                    {
        //                        Debug.Log("building ID: " + building.GetComponent<BuildingScript>().GetID());
        //                        Debug.Log("My Neighbours: ");
        //                        foreach(int neighbour in building.GetComponent<BuildingScript>().GetNeighbours())
        //                        {
        //                            Debug.Log(neighbour);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        //foreach (GameObject junction in Board.GetComponent<BoardManager>().junctions)
        //{
        //    int x = junction.GetComponent<JunctionScript>().GetTile().Item1;
        //    int y = junction.GetComponent<JunctionScript>().GetTile().Item2;
        //    if (Board.GetComponent<BoardManager>().tiles[x,y].GetComponent<Collider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        //    {
        //        Debug.Log("Connected roads are: ");
        //        foreach (int road in junction.GetComponent<JunctionScript>().GetRoads())
        //        {
        //            Debug.Log("Road ID: " + road);
        //        }
        //    }
        //}
        //foreach (GameObject road in Board.GetComponent<BoardManager>().roads)
        //{
        //    foreach (Tuple<int, int> tile in road.GetComponent<RoadScript>().GetTiles())
        //    {
        //        int x = tile.Item1;
        //        int y = tile.Item2;
        //        if (Board.GetComponent<BoardManager>().tiles[x, y].GetComponent<Collider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        //        {
        //            Debug.Log("This Road's ID: " + road.GetComponent<RoadScript>().GetID());
        //        }
        //    }
        //}
    }


}
