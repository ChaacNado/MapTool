using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public float speed = 2f;
    public float edgeScrollSpeed = 0.5f;
    public float zoom = 5;
    public bool edgeScrollEnabled = true;
    private float zoomSpeed = 2f;

    private Camera myCam;
    public GameObject Board;
    // Start is called before the first frame update
    void Start()
    {
        myCam = GetComponent<Camera>();
    }
    // Update is called once per frame
    void Update()
    {
        ScrollZoom();
        //EdgeScroll();
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
        }

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
        foreach (GameObject junction in Board.GetComponent<BoardManager>().junctions)
        {
            int x = junction.GetComponent<JunctionScript>().GetTile().Item1;
            int y = junction.GetComponent<JunctionScript>().GetTile().Item2;
            if (Board.GetComponent<BoardManager>().tiles[x,y].GetComponent<Collider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                Debug.Log("Connected roads are: ");
                foreach (int road in junction.GetComponent<JunctionScript>().GetRoads())
                {
                    Debug.Log("Road ID: " + road);
                }
            }
        }
        foreach (GameObject road in Board.GetComponent<BoardManager>().roads)
        {
            foreach (Tuple<int, int> tile in road.GetComponent<RoadScript>().GetTiles())
            {
                int x = tile.Item1;
                int y = tile.Item2;
                if (Board.GetComponent<BoardManager>().tiles[x, y].GetComponent<Collider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
                {
                    Debug.Log("This Road's ID: " + road.GetComponent<RoadScript>().GetID());
                }
            }
        }
    }
    private void ScrollZoom()
    {
        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            zoom -= zoomSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            zoom += zoomSpeed * Time.deltaTime;
        }
        if (Input.mouseScrollDelta.y > 0)
        {
            zoom -= zoomSpeed * Time.deltaTime * 10;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            zoom += zoomSpeed * Time.deltaTime * 10;
        }

        myCam.orthographicSize = zoom;
    }

    private void EdgeScroll() //if your mouse goes towards the edge of the screen the screen will go that way
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!edgeScrollEnabled)
                edgeScrollEnabled = true;
            else
                edgeScrollEnabled = false;
        }
        if (!edgeScrollEnabled)
        {
            return;
        }
        int edgeSize = 50;
        if (Input.mousePosition.x > Screen.width - edgeSize)
        {
            transform.Translate(new Vector3(speed * Time.deltaTime * edgeScrollSpeed, 0, 0));
        }
        if (Input.mousePosition.x < edgeSize)
        {
            transform.Translate(new Vector3(-speed * Time.deltaTime * edgeScrollSpeed, 0, 0));
        }
        if (Input.mousePosition.y > Screen.height - edgeSize)
        {
            transform.Translate(new Vector3(0, speed * Time.deltaTime * edgeScrollSpeed, 0));
        }
        if (Input.mousePosition.y < edgeSize)
        {
            transform.Translate(new Vector3(0, -speed * Time.deltaTime * edgeScrollSpeed, 0));
        }
    }
}
