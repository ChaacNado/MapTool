using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public Tuple<int, int> position;
    public List<GameObject> walls;
    public GameObject[,] tiles;
    public int numberOfWalls = 0;
    public int wallDirection = 0; //1 = L, 2 = R, 4 = U, 8 = D, 3 = LR, 5 = LU, 6 = RU, 7 = LRU, 9 = LD, 10 = RD, 11 = LRD, 12 = UD, 13 = LUD, 14 = RUD, 15 = LRUD
    public bool hasDoor;
    public bool Parent;
    public int buildingID;

    public GameObject wallPrefab;
    public GameObject tilePrefab;

    public void SetPosition(int x, int y)
    {
        position = new Tuple<int, int>(x, y);
    }

    public void AddWall(Sprite newSprite, int binaryDirection)
    {
        wallDirection += binaryDirection;
        walls.Add(Instantiate(wallPrefab, transform));
        walls[numberOfWalls].GetComponent<SpriteRenderer>().sprite = newSprite;
        if (binaryDirection == 1)
        {
            walls[numberOfWalls].GetComponent<SpriteRenderer>().size = new Vector3(transform.GetComponent<SpriteRenderer>().size.x / 10, transform.GetComponent<SpriteRenderer>().size.y, 0);
            walls[numberOfWalls].transform.position = new Vector2(transform.position.x - transform.GetComponent<SpriteRenderer>().size.x/2, transform.position.y);
        }
        else if (binaryDirection == 2)
        {
            walls[numberOfWalls].GetComponent<SpriteRenderer>().size = new Vector3(transform.GetComponent<SpriteRenderer>().size.x / 10, transform.GetComponent<SpriteRenderer>().size.y, 0);
            walls[numberOfWalls].transform.position = new Vector2(transform.position.x + transform.GetComponent<SpriteRenderer>().size.x / 2, transform.position.y);
        }
        else if (binaryDirection == 4)
        {
            walls[numberOfWalls].GetComponent<SpriteRenderer>().size = new Vector3(transform.GetComponent<SpriteRenderer>().size.x, transform.GetComponent<SpriteRenderer>().size.y / 10, 0);
            walls[numberOfWalls].transform.position = new Vector2(transform.position.x, transform.position.y + transform.GetComponent<SpriteRenderer>().size.y / 2);
        }
        else if (binaryDirection == 8)
        {
            walls[numberOfWalls].GetComponent<SpriteRenderer>().size = new Vector3(transform.GetComponent<SpriteRenderer>().size.x, transform.GetComponent<SpriteRenderer>().size.y / 10, 0);
            walls[numberOfWalls].transform.position = new Vector2(transform.position.x, transform.position.y - transform.GetComponent<SpriteRenderer>().size.y / 2);
        }
        walls[numberOfWalls].GetComponent<SpriteRenderer>().sortingOrder = transform.GetComponent<SpriteRenderer>().sortingOrder + 1;
        numberOfWalls++;
    }

    public void MakeBuildingRoomParent(int buildingID)
    {
        this.buildingID = buildingID;
        if (!Parent)
        {
            Parent = true;
            tiles = new GameObject[2, 2];
            //Topleft
            tiles[0, 0] = Instantiate(tilePrefab, new Vector3(transform.position.x - transform.GetComponent<SpriteRenderer>().size.x / 4, transform.position.y + transform.GetComponent<SpriteRenderer>().size.y / 4, 0), Quaternion.identity);
            tiles[0, 0].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.2f);
            tiles[0, 0].GetComponent<SpriteRenderer>().size = new Vector2(transform.GetComponent<SpriteRenderer>().size.x / 2, transform.GetComponent<SpriteRenderer>().size.y / 2);
            tiles[0,0].GetComponent<SpriteRenderer>().sortingOrder = transform.GetComponent<SpriteRenderer>().sortingOrder + 1;
            //TopRight
            tiles[1, 0] = Instantiate(tilePrefab, new Vector3(transform.position.x + transform.GetComponent<SpriteRenderer>().size.x / 4, transform.position.y + transform.GetComponent<SpriteRenderer>().size.y / 4, 0), Quaternion.identity);
            tiles[1, 0].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.6f);
            tiles[1, 0].GetComponent<SpriteRenderer>().size = new Vector2(transform.GetComponent<SpriteRenderer>().size.x / 2, transform.GetComponent<SpriteRenderer>().size.y / 2);
            tiles[1, 0].GetComponent<SpriteRenderer>().sortingOrder = transform.GetComponent<SpriteRenderer>().sortingOrder + 1;
            ////BottomLeft
            tiles[0, 1] = Instantiate(tilePrefab, new Vector3(transform.position.x - transform.GetComponent<SpriteRenderer>().size.x / 4, transform.position.y - transform.GetComponent<SpriteRenderer>().size.y / 4, 0), Quaternion.identity);
            tiles[0, 1].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.6f);
            tiles[0, 1].GetComponent<SpriteRenderer>().size = new Vector2(transform.GetComponent<SpriteRenderer>().size.x / 2, transform.GetComponent<SpriteRenderer>().size.y / 2);
            tiles[0, 1].GetComponent<SpriteRenderer>().sortingOrder = transform.GetComponent<SpriteRenderer>().sortingOrder + 1;
            ////BottomRight
            tiles[1, 1] = Instantiate(tilePrefab, new Vector3(transform.position.x + transform.GetComponent<SpriteRenderer>().size.x / 4, transform.position.y - transform.GetComponent<SpriteRenderer>().size.y / 4, 0), Quaternion.identity);
            tiles[1, 1].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.2f);
            tiles[1, 1].GetComponent<SpriteRenderer>().size = new Vector2(transform.GetComponent<SpriteRenderer>().size.x / 2, transform.GetComponent<SpriteRenderer>().size.y / 2);
            tiles[1, 1].GetComponent<SpriteRenderer>().sortingOrder = transform.GetComponent<SpriteRenderer>().sortingOrder + 1;
            
            tiles[1, 0].transform.parent = transform;
            tiles[0, 0].transform.parent = transform;
            tiles[0, 1].transform.parent = transform;
            tiles[1, 1].transform.parent = transform;
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
