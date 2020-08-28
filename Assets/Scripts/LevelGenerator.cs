using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] levelSection;
    bool down, left;
    int currElement;
    int[,] levelMap =
    {
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
        {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
        {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
        {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
        {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
        {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
        {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
        {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
        {0,0,0,0,0,2,5,4,4,0,3,4,4,0},
        {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
        {0,0,0,0,0,0,5,0,0,0,4,0,0,0},
    };
    private void Awake()
    {
        //levelPrefabs = new GameObject[levelMap.GetLength(0), levelMap.GetLength(1)];
    }
    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        for(int i = 0; i < levelMap.GetLength(0); i++)
        {
            for(int j = 0; j < levelMap.GetLength(1); j++)
            {
                currElement = levelMap[i, j];
                Instantiate(levelSection[currElement], new Vector2(i, j), determineRote(i,j));
            }
        }
    }
    //final idea
        //use raycasts(if flat do both have same rote)
        //if corner determine side hit from then rotate accordingly

    Quaternion determineRote(int i, int j)
    {
        RaycastHit2D hitDown = Physics2D.Raycast(new Vector2(i,j), Vector2.down, 1); 
        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(i,j), Vector2.left, 1);
        
        if (hitLeft && hitLeft.collider.CompareTag("Wall"))
        {
            Sprite hitSprite = hitLeft.collider.gameObject.GetComponent<SpriteRenderer>().sprite;
            Sprite currSprite = levelSection[currElement].GetComponent<SpriteRenderer>().sprite;
            switch (currElement)
            {
                case 1: return hitLeft.collider.transform.rotation;
                case 2: return hitLeft.collider.transform.rotation;
                case 3: return hitLeft.collider.transform.rotation * Quaternion.Euler(0, 0, -90);
                case 4: if (currSprite != hitSprite && hitLeft.collider.transform.rotation == Quaternion.identity) return hitLeft.collider.transform.rotation * Quaternion.Euler(0, 0, 90); else return hitLeft.collider.transform.rotation;
                case 7: return hitLeft.collider.transform.rotation;
                default: return Quaternion.identity;
            }
        }
        else if (hitDown && hitDown.collider.CompareTag("Wall"))
            {
                Sprite hitSprite = hitDown.collider.gameObject.GetComponent<SpriteRenderer>().sprite;
                Sprite currSprite = levelSection[currElement].GetComponent<SpriteRenderer>().sprite;
                switch (currElement)
                {
                    case 1: return hitDown.collider.transform.rotation * Quaternion.Euler(0, 0, -90);
                    case 2: return hitDown.collider.transform.rotation;
                    case 3: return hitDown.collider.transform.rotation * Quaternion.Euler(0, 0, -90);
                    case 4: return hitDown.collider.transform.rotation;
                    case 7: return hitDown.collider.transform.rotation;
                    default: return Quaternion.identity;
                }
            }
            else
            return Quaternion.identity;
        }










    //need all 4 around element which exist
    //tag telling if wall element or pellet
    //determine type of element using
    //if before it or left then update rotation
    //Quaternion determineRote(int i, int j)
    //{
    //    int otherWallType = findType(i, j);
    //    int currWallType = levelMap[i, j];
    //    Quaternion prevRote = findRote(i, j);
    //    switch(otherWallType)
    //    {
    //        case 1: if(down)//return Quaternion.Euler(0, 0, 45);
    //        case 2: return Quaternion.Euler(0, 0, 45);
    //        case 3: return Quaternion.Euler(0, 0, 45);
    //        case 4: return Quaternion.Euler(0, 0, 45);
    //        case 7: return Quaternion.Euler(0, 0, 45);
    //        default : return Quaternion.Euler(0, 0, 0);
    //    }
    //}
    //Quaternion wall1(int currWallType, Quaternion other)
    //{
    //    switch (currWallType)
    //    {
    //        case 1: if (down) { return other + Quaternion.Euler(0, 0, 45)};
    //        case 2: return Quaternion.Euler(0, 0, 45);
    //        case 3: return Quaternion.Euler(0, 0, 45);
    //        case 4: return Quaternion.Euler(0, 0, 45);
    //        case 7: return Quaternion.Euler(0, 0, 45);
    //        default: return Quaternion.Euler(0, 0, 0);
    //    }
    //}
    //int findType(int i, int j)
    //{
    //    if (j > 0 && levelSection[levelMap[i, j - 1]].CompareTag("Wall"))
    //        return levelMap[i, j - 1];
    //    else if (i > 0 && levelSection[levelMap[i - 1, j]].CompareTag("Wall"))
    //        return levelMap[i - 1, j];
    //    else
    //        return -1; //no type found
    //}

    //Quaternion findRote(int i, int j)
    //{

    //    RaycastHit2D hitDown = Physics2D.Raycast(new Vector2(i,j), Vector2.down, 1);
    //    RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(i,j), Vector2.left, 1);
    //    if (hitDown && hitDown.collider.CompareTag("Wall"))
    //    {
    //        down = true; left = false;
    //        return hitDown.transform.rotation;
    //    }
    //    else if (hitLeft && hitLeft.collider.CompareTag("Wall"))
    //    {
    //        down = false; left = true;
    //        return hitLeft.transform.rotation;
    //    }
    //    else
    //    {
    //        down = false; left = false;
    //        return Quaternion.identity;
    //    }
    //}
}
