using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTester : MonoBehaviour
{
    public GameObject[] levelSection;
    public GameObject mapSection;
    GameObject instance;
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
        GenerateMap(-1, -1,0);
        GenerateMap(1, 1, 180);
       // GenerateMap(-1, 1, 1, -1);
        //GenerateMap(1, -1, -1, 1);
        //      GenerateMap(-1, 1);
        //      GenerateMap(1, -1);
        //      GenerateMap(-1, -1);
        //   mapSection.transform.position = new Vector2(levelMap.GetLength(1) + 2, levelMap.GetLength(0) - 1);
        //   mapSection.transform.localScale = new Vector2(-1, -1);


    }
    void Start()
    {
        Destroy(mapSection.GetComponent<LevelGenerator>());
        //  mapSection = gameObject;
     //   GenerateSections(-levelMap.GetLength(1) + 1, -levelMap.GetLength(0) + 2, 1, 1);
      //  GenerateSections(-levelMap.GetLength(1) + 1, levelMap.GetLength(0) - 1, 1, -1);
      //  GenerateSections(levelMap.GetLength(1) + 2, -levelMap.GetLength(0) + 2, -1, 1);
    }
    void GenerateSections(int xPos, int yPos, int xScale, int yScale)
    {
        GameObject section = Instantiate(mapSection, new Vector2(xPos, yPos), Quaternion.identity);
        section.transform.localScale = new Vector2(xScale, yScale);
    }
    void GenerateMap(int upDir, int rightDir, int startRote)
    {
        for (int i = 0; i < levelMap.GetLength(0); i++)
        {
            for (int j = 0; j < levelMap.GetLength(1); j++)
            {
                currElement = levelMap[i, j];
                instance = Instantiate(levelSection[currElement], new Vector2(rightDir * (levelMap.GetLength(0)-(i+1)), upDir* (levelMap.GetLength(0)-(j+1))), Quaternion.identity, mapSection.transform);
                instance.transform.rotation = determineRote(rightDir* (levelMap.GetLength(0) - (i + 1)), upDir * (levelMap.GetLength(0) - (j + 1)), startRote);
            }
        }
    }
    //final idea
    //use raycasts(if flat do both have same rote)
    //if corner determine side hit from then rotate accordingly

    Quaternion determineRote(int i, int j, int startRote)
    {
        switch (currElement)
        {
            case 1: return cornerRote(i, j, startRote);
            case 2: return wallRote(i, j);
            case 3: return cornerRote(i, j, startRote);
            case 4: return wallRote(i, j);
            default: return Quaternion.identity;
        }
    }
    Quaternion wallRote(int i, int j) //determines rotation of the wall elements
    {        
        int currRote = 0;
        for (int k = 0; k < 4; k++)
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(i, j), instance.transform.up, 1);
            if (hit)
            {
                if (hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite == instance.GetComponent<SpriteRenderer>().sprite)
                {
                    return hit.collider.transform.rotation * Quaternion.identity;
                }
                else
                {
                    return Quaternion.Euler(0, 0, currRote);
                }
            }
            currRote += 90;
            instance.transform.rotation = Quaternion.Euler(0, 0, currRote);
        }
        return Quaternion.identity;
        //if (hit)
        //    if (hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite == instance.GetComponent<SpriteRenderer>().sprite)//checks if left side is another wall
        //        return hit.collider.transform.rotation * Quaternion.identity; //if hiting another wall set rotation to that ones
        //    else
        //        return Quaternion.Euler(0, 0, 90);//if hit map and not another wall must be horizontally rotated
        //else
        //    return Quaternion.identity; //if no wall hit must be vertical
    }
    Quaternion cornerRote(int i, int j, int startRote)//determine the rotation of a corner piece
    {
        int currRote = startRote;
        RaycastHit2D hitUp = Physics2D.Raycast(new Vector2(i, j), instance.transform.up, 1);

        RaycastHit2D hitDown = Physics2D.Raycast(new Vector2(i, j), -instance.transform.up, 1);

        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(i, j), -instance.transform.right, 1);

        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(i, j), instance.transform.right, 1);

        if (hitUp)
            currRote += 0;
        if (hitLeft)
            currRote += 90;
        if (hitDown)
            currRote += 90;
        if (hitRight)
            currRote += 90;
        if (hitDown && hitLeft)
            currRote = 180;
        return Quaternion.Euler(0, 0, currRote);

        //for (int k = 0; k < 4; k++)
        //{
        //    RaycastHit2D hit = Physics2D.Raycast(new Vector2(i, j), instance.transform.right, 1);
        //    if (hit)
        //    {
        //      //  if (hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite == instance.GetComponent<SpriteRenderer>().sprite)
        //        {
        //        //    return hit.collider.transform.rotation * Quaternion.identity;
        //        }
        //       // else
        //        {
        //            return Quaternion.Euler(0, 0, currRote);
        //        }
        //    }
        //    currRote -= 90;
        //    instance.transform.rotation = Quaternion.Euler(0, 0, currRote);
        //}
        //return Quaternion.identity;


        //special cases where raycasts not most efficent way
        //if (j == levelMap.GetLength(1) - 1 && i == 7)
        //    return Quaternion.Euler(0, 0, -90);
        //else if (i == 9 && j == 8)
        //    return Quaternion.Euler(0, 0, 90);
        //else if (i == 10 && j == 8)
        //    return Quaternion.Euler(0, 0, 0);
        ////cases based on raycast findings
        //else
        //{
        //    //detect if another map piece is either to the left or below the current piece as during generation these only two pieces which matter for determing corner rotation
        //  //  RaycastHit2D leftHit = Physics2D.Raycast(new Vector2(i, j), checkRight* -instance.transform.right, 1);
        //   // RaycastHit2D downtHit = Physics2D.Raycast(new Vector2(i, j), checkLeft* -instance.transform.up, 1);
        //    if (leftHit && downtHit)
        //        return Quaternion.Euler(0, 0, 180);
        //    else if (leftHit)
        //        return Quaternion.Euler(0, 0, 90);
        //    else if (downtHit)
        //        return Quaternion.Euler(0, 0, -90);
        //    else return Quaternion.identity;
        //}



        /////////is stuff below better or worse, also can somehow integrate the two versions
        /////overall both seem around same efficenty but up seems possibly better???????


        //int currRote = 0;


        //for (int k = 0; k < 4; k++)
        //{
        //    if (j == levelMap.GetLength(1) - 1 && i == 7)
        //        return Quaternion.Euler(0, 0, -90);
        //    if (i == 9 && j == 8)
        //        return Quaternion.Euler(0, 0, 90);
        //    if (i == 10 && j == 8)
        //        return Quaternion.Euler(0, 0, 0);
        //    RaycastHit2D hit;
        //    if (leftHit && leftHit.collider.CompareTag("Wall"))
        //    {
        //        hit = Physics2D.Raycast(new Vector2(i, j), instance.transform.right, 1);
        //    }
        //    else
        //        hit = Physics2D.Raycast(new Vector2(i, j), instance.transform.up, 1);
        //    if (hit && hit.collider.CompareTag("Wall"))
        //    {
        //        return Quaternion.Euler(0, 0, currRote);
        //    }
        //    currRote += 90;
        //    instance.transform.rotation = Quaternion.Euler(0, 0, currRote);
        //}
        //return Quaternion.identity;
    }


}

