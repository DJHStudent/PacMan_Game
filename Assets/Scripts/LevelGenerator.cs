using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] levelSection;
    GameObject instance;
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
        for (int i = 0; i < levelMap.GetLength(0); i++)
        {
            for (int j = 0; j < levelMap.GetLength(1); j++)
            {
                currElement = levelMap[i, j];
                instance = Instantiate(levelSection[currElement], new Vector2(i, j), Quaternion.identity);
                if (currElement != 7)
                    instance.transform.rotation = determineRote(i, j);
            }
        }
    }
    //final idea
    //use raycasts(if flat do both have same rote)
    //if corner determine side hit from then rotate accordingly

    Quaternion determineRote(int i, int j)
    {
        return inCornRote(i, j);
        //   switch (currElement)
        //    {
        //        case 1: return inCornRote(i, j);
        //        case 2:
        //        case 3:
        //        case 4:
        //        default: return Quaternion.identity;
        //    }
        //}
        //Quaternion wallRote(int i, int j)
        //{

        }
    Quaternion inCornRote(int i, int j)
    {

        int currRote = 0;
        RaycastHit2D leftHit = Physics2D.Raycast(new Vector2(i, j), -instance.transform.up, 1);

        for (int k = 0; k < 4; k++)
        {
            if (j == levelMap.GetLength(1) - 1 && i == 7)
                return Quaternion.Euler(0, 0, -90);
            if (i == 9 && j == 8)
                return Quaternion.Euler(0, 0, 90);
            if (i == 10 && j == 8)
                return Quaternion.Euler(0, 0, 0);
            RaycastHit2D hit;
            if (leftHit && leftHit.collider.CompareTag("Wall") && currElement != 2 && currElement != 4)
            {
                hit = Physics2D.Raycast(new Vector2(i, j), instance.transform.right, 1);
            }
            else
                hit = Physics2D.Raycast(new Vector2(i, j), instance.transform.up, 1);
            if (hit && hit.collider.CompareTag("Wall"))
            {
                Debug.Log("Hit");
                if (currElement == 4 && hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite == instance.GetComponent<SpriteRenderer>().sprite)
                    return hit.collider.gameObject.transform.rotation * Quaternion.Euler(0, 0, 0);
                else
                    return Quaternion.Euler(0, 0, currRote);
            }
            currRote += 90;
            instance.transform.rotation = Quaternion.Euler(0, 0, currRote);
        }
        Debug.Log("Fail");
        return Quaternion.identity;
    }

  
}
