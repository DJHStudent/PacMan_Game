using System.Collections;
using UnityEngine;

public class Ghost4Waypoints : MonoBehaviour //for the border wall set each valid position as a waypoint for ghost 4 to follow
{
    public GameObject nextObj;
    LayerMask wall, innerWall;
    public static Vector2 currDir;//stop backtracking being possible
    public static Sprite straightWall;
    void Start()
    {
        wall = GameObject.Find("SceneManager").GetComponent<GameManager>().wall;
        innerWall = GameObject.Find("SceneManager").GetComponent<GameManager>().innerWall;
        innerWall |= 1 << 11;
        if (straightWall == null)
            straightWall = gameObject.GetComponent<SpriteRenderer>().sprite;
        nextWall(transform.position);
    }
    void posAccessible()
    {
        RaycastHit2D checkUp = nextPosWall(nextObj.transform.position, Vector2.up, 1, innerWall);
        RaycastHit2D checkLeft = nextPosWall(nextObj.transform.position, Vector2.left, 1, innerWall);
        RaycastHit2D checkRight = nextPosWall(nextObj.transform.position, Vector2.right, 1, innerWall);
        RaycastHit2D checkDown = nextPosWall(nextObj.transform.position, Vector2.down, 1, innerWall);
        //continually check until the next pos is a valid one then thats the next pos it will move towards
        if (checkDown || !isStraightWall(nextObj))
            StartCoroutine(waitFrame());
        else if (checkLeft)
            StartCoroutine(waitFrame());
        else if (checkRight)
            StartCoroutine(waitFrame());
        else if (checkUp)
            StartCoroutine(waitFrame());
        else //if next pos does have an accessible path to it then add that as the next one
            nextObj.AddComponent<Ghost4Waypoints>();
    }
    public void nextWall(Vector2 checkFromPos)
{
        RaycastHit2D hit = nextPosWall(checkFromPos,Vector2.up, 1, wall);
        if (Vector2.up != -currDir && hit && !hit.collider.gameObject.GetComponent<Ghost4Waypoints>())
        {
            nextObj = hit.collider.gameObject;
            currDir = Vector2.up;
            posAccessible();
            return;
        }            
        hit = nextPosWall(checkFromPos, Vector2.down, 1, wall);
        if (Vector2.down != -currDir && hit && !hit.collider.gameObject.GetComponent<Ghost4Waypoints>())
        {
            nextObj = hit.collider.gameObject;
            currDir = Vector2.down;
            posAccessible();
            return;
        }
        hit = nextPosWall(checkFromPos, Vector2.right, 1, wall);
        if (Vector2.right != -currDir && hit && !hit.collider.gameObject.GetComponent<Ghost4Waypoints>())
        {
            nextObj = hit.collider.gameObject;
            currDir = Vector2.right;
            posAccessible();
            return;
        }
        hit = nextPosWall(checkFromPos, Vector2.left, 1, wall);
        if (Vector2.left != -currDir && hit && !hit.collider.gameObject.GetComponent<Ghost4Waypoints>())
        {
            nextObj = hit.collider.gameObject;
            currDir = Vector2.left;
            posAccessible();
            return;
        }
        //2 distance for the one wide gap between the two teloporters so it doesn't get stuck here
        hit = nextPosWall(checkFromPos, Vector2.up, 2, wall);
        if (Vector2.up != -currDir && hit && !hit.collider.gameObject.GetComponent<Ghost4Waypoints>())
        {
            nextObj = hit.collider.gameObject;
            currDir = Vector2.left;
            hit.collider.gameObject.AddComponent<Ghost4Waypoints>();
            return;
        } 
        hit = nextPosWall(checkFromPos, Vector2.right, 2, wall);
        if (Vector2.right != -currDir && hit && !hit.collider.gameObject.GetComponent<Ghost4Waypoints>())
        {
            nextObj = hit.collider.gameObject;
            currDir = Vector2.left;
            hit.collider.gameObject.AddComponent<Ghost4Waypoints>();
            return;
        }
        hit = nextPosWall(checkFromPos, Vector2.down, 2, wall);
        if (Vector2.down != -currDir && hit && !hit.collider.gameObject.GetComponent<Ghost4Waypoints>())
        {
            nextObj = hit.collider.gameObject;
            currDir = Vector2.left;
            hit.collider.gameObject.AddComponent<Ghost4Waypoints>();
            return;
        }
        hit = nextPosWall(checkFromPos, Vector2.left, 2, wall);
        if (Vector2.left != -currDir && hit && !hit.collider.gameObject.GetComponent<Ghost4Waypoints>())
        {
            nextObj = hit.collider.gameObject;
            currDir = Vector2.left;
            hit.collider.gameObject.AddComponent<Ghost4Waypoints>();
            return;
        }
        hit = nextPosWall(checkFromPos, Vector2.up, 2, wall); //if fails to be able to hit any other object then must be at end so get the first element again
        if (hit)
        {
            nextObj = hit.collider.gameObject;
            currDir = Vector2.up;
            return;
        }
    }
    bool isStraightWall(GameObject nextObj)//checks if corner piece or not
    {
        return nextObj.GetComponent<SpriteRenderer>().sprite == straightWall;
    }
    RaycastHit2D nextPosWall(Vector2 currPos, Vector2 nextPos, int dist, LayerMask layer) //checks if the next position the player is moving too is a wall
    {
        RaycastHit2D hit = Physics2D.Raycast(currPos, nextPos, dist, layer);
        return hit;
    }

    public IEnumerator waitFrame()//wait for frame here so not stack overflow
    {
        yield return new WaitForEndOfFrame();
            nextWall(nextObj.transform.position);
    }
}

