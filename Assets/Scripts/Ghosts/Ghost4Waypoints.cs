using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Ghost4Waypoints : MonoBehaviour
{
    public GameObject nextObj;
    LayerMask wall;
    void Start()
    {
        wall = 1 << 10;
        nextWall();
    }

    // Update is called once per frame
   void nextWall()
    {
        RaycastHit2D hit = nextPosWall(Vector2.up, 1);
        if (hit && !hit.collider.gameObject.GetComponent<Ghost4Waypoints>())
        {
            nextObj = hit.collider.gameObject;
            hit.collider.gameObject.AddComponent<Ghost4Waypoints>();
            return;
        }    
        hit = nextPosWall(Vector2.down, 1);
        if (hit && !hit.collider.gameObject.GetComponent<Ghost4Waypoints>())
        {
            nextObj = hit.collider.gameObject;
            hit.collider.gameObject.AddComponent<Ghost4Waypoints>();
            return;
        }
        hit = nextPosWall(Vector2.left, 1);
        if (hit && !hit.collider.gameObject.GetComponent<Ghost4Waypoints>())
        {
            nextObj = hit.collider.gameObject;
            hit.collider.gameObject.AddComponent<Ghost4Waypoints>();
            return;
        }
        hit = nextPosWall(Vector2.right, 1);
        if (hit && !hit.collider.gameObject.GetComponent<Ghost4Waypoints>())
        {
            nextObj = hit.collider.gameObject;
            hit.collider.gameObject.AddComponent<Ghost4Waypoints>();
            return;
        }
   



        //2 distance
        hit = nextPosWall(Vector2.up, 2);
        if (hit && !hit.collider.gameObject.GetComponent<Ghost4Waypoints>())
        {
            nextObj = hit.collider.gameObject;
            hit.collider.gameObject.AddComponent<Ghost4Waypoints>();
            return;
        } 
        hit = nextPosWall(Vector2.down, 2);
        if (hit && !hit.collider.gameObject.GetComponent<Ghost4Waypoints>())
        {
            nextObj = hit.collider.gameObject;
            hit.collider.gameObject.AddComponent<Ghost4Waypoints>();
            return;
        }
        hit = nextPosWall(Vector2.left, 2);
        if (hit && !hit.collider.gameObject.GetComponent<Ghost4Waypoints>())
        {
            nextObj = hit.collider.gameObject;
            hit.collider.gameObject.AddComponent<Ghost4Waypoints>();
            return;
        }
        hit = nextPosWall(Vector2.right, 2);
        if (hit && !hit.collider.gameObject.GetComponent<Ghost4Waypoints>())
        {
            nextObj = hit.collider.gameObject;
            hit.collider.gameObject.AddComponent<Ghost4Waypoints>();
            return;
        }
        hit = nextPosWall(Vector2.up, 2); //if fails to be able to hit any other object then must be at end so get the first element again
        if (hit)
        {
            nextObj = hit.collider.gameObject;
            return;
        }




    }
    RaycastHit2D nextPosWall(Vector2 nextPos, int dist) //checks if the next position the player is moving too is a wall
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, nextPos, dist, wall);
        return hit;
    }
}




//////////////////if down check right is wall
////////////////        //if right check up is wall
////////////////        //if left check down is wall
////////////////        //if up check left is wall
////////////////        if (currDir == (int) CurrDir.down && !nextPosWall(directions[(int)CurrDir.down]) && nextPosWall(directions[(int)CurrDir.right]))
////////////////            nextPos.Add(addDir((int) CurrDir.down, currPos));
////////////////        else if (currDir == (int) CurrDir.right && !nextPosWall(directions[(int)CurrDir.right]) && nextPosWall(directions[(int)CurrDir.up]))
////////////////            nextPos.Add(addDir((int) CurrDir.right, currPos));
////////////////        else if (currDir == (int) CurrDir.left && !nextPosWall(directions[(int)CurrDir.left]) && nextPosWall(directions[(int)CurrDir.down]))
////////////////            nextPos.Add(addDir((int) CurrDir.left, currPos));
////////////////        else if (currDir == (int) CurrDir.up && !nextPosWall(directions[(int)CurrDir.up]) && nextPosWall(directions[(int)CurrDir.left]))
////////////////            nextPos.Add(addDir((int) CurrDir.up, currPos));


////////////////        else if (nextPosWall(directions[(int)CurrDir.right]) && nextPosWall(directions[(int)CurrDir.up]) && nextPosWall(directions[(int)CurrDir.down]))
////////////////            nextPos.Add(addDir((int) CurrDir.left, currPos));
////////////////        else if (nextPosWall(directions[(int)CurrDir.left]) && nextPosWall(directions[(int)CurrDir.up]) && nextPosWall(directions[(int)CurrDir.down]))
////////////////            nextPos.Add(addDir((int) CurrDir.right, currPos));
////////////////        else if (currDir == (int) CurrDir.down)// && !nextPosWall(directions[(int)CurrDir.down]) && nextPosWall(directions[(int)CurrDir.right]))
////////////////{
////////////////    if (nextPosWall(directions[(int)CurrDir.right]))
////////////////        nextPos.Add(addDir((int)CurrDir.left, currPos));
////////////////    else
////////////////        nextPos.Add(addDir((int)CurrDir.right, currPos));
////////////////}
////////////////        else if (currDir == (int) CurrDir.right)// && !nextPosWall(directions[(int)CurrDir.right]) && nextPosWall(directions[(int)CurrDir.up]))
////////////////{
////////////////    if (nextPosWall(directions[(int)CurrDir.up]))
////////////////        nextPos.Add(addDir((int)CurrDir.down, currPos));
////////////////    else
////////////////        nextPos.Add(addDir((int)CurrDir.up, currPos));
////////////////}
////////////////        else if (currDir == (int) CurrDir.left)// && !nextPosWall(directions[(int)CurrDir.left]) && nextPosWall(directions[(int)CurrDir.down]))
////////////////{
////////////////    if (nextPosWall(directions[(int)CurrDir.down]))
////////////////        nextPos.Add(addDir((int)CurrDir.up, currPos));
////////////////    else
////////////////        nextPos.Add(addDir((int)CurrDir.down, currPos));
////////////////}
////////////////        else if (currDir == (int) CurrDir.up)// && !nextPosWall(directions[(int)CurrDir.up]) && nextPosWall(directions[(int)CurrDir.left]))
////////////////{
////////////////    if (nextPosWall(directions[(int)CurrDir.left]))
////////////////        nextPos.Add(addDir((int)CurrDir.right, currPos));
////////////////    else
////////////////        nextPos.Add(addDir((int)CurrDir.left, currPos));
////////////////}

