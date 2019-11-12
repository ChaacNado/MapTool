using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public Tuple<int, int> position;
    public List<GameObject> walls;
    public int numberOfWalls = 0;
    public int wallDirection = 0; //1 = L, 2 = R, 4 = U, 8 = D, 3 = LR, 5 = LU, 6 = RU, 7 = LRU, 9 = LD, 10 = RD, 11 = LRD, 12 = UD, 13 = LUD, 14 = RUD, 15 = LRUD
    public bool hasDoor;

    public GameObject wallPrefab;
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
            walls[numberOfWalls].GetComponent<SpriteRenderer>().size = new Vector3(0.1f, 1, 0);
            walls[numberOfWalls].transform.position = new Vector2(transform.position.x - 0.5f, transform.position.y);
        }
        else if (binaryDirection == 2)
        {
            walls[numberOfWalls].GetComponent<SpriteRenderer>().size = new Vector3(0.1f, 1, 0);
            walls[numberOfWalls].transform.position = new Vector2(transform.position.x + 0.5f, transform.position.y);
        }
        else if (binaryDirection == 4)
        {
            walls[numberOfWalls].GetComponent<SpriteRenderer>().size = new Vector3(1, 0.1f, 0);
            walls[numberOfWalls].transform.position = new Vector2(transform.position.x, transform.position.y + 0.5f);
        }
        else if (binaryDirection == 8)
        {
            walls[numberOfWalls].GetComponent<SpriteRenderer>().size = new Vector3(1, 0.1f, 0);
            walls[numberOfWalls].transform.position = new Vector2(transform.position.x, transform.position.y - 0.5f);
        }
        walls[numberOfWalls].GetComponent<SpriteRenderer>().sortingOrder = 1;
        numberOfWalls++;
    }

    public void SetDoor()
    {
        if (!hasDoor)
        {
            walls[UnityEngine.Random.Range(0, numberOfWalls)].GetComponent<SpriteRenderer>().color = Color.black;
            hasDoor = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
