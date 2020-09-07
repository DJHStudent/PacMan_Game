using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanMove : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    Vector2 direction, nextDirection, newPos;
    void Start()
    {       
        direction = Vector2.left;
        nextDirection = Vector2.down;
        newPos = new Vector2(transform.position.x - 1, transform.position.y);
       // DetermineNewPos();
    }
    //first check direction dowable
    //move to next spot 1 unit along
    //check if can change and need to
    //repeat



    // Update is called once per frame
    void Update()
    {
        Move();
    }
    void Move()
    {
        if (!IsDirection(direction))
        {
            Vector2 currPos = new Vector2(transform.position.x, transform.position.y);
            if (currPos != newPos)//direction == Vector2.up && currPos.y <= newPos.y || direction == Vector2.right && currPos.x <= newPos.x
                //|| direction == Vector2.left && currPos.x >= newPos.x || direction == Vector2.down && currPos.y >= newPos.y)
            {
                transform.position = Vector2.MoveTowards(transform.position, newPos, Time.deltaTime);
            }
            else
            {
                DetermineNewPos();
            }
        }
        else
        {
            DetermineDirection();
        }
    }

    bool IsDirection(Vector2 dir)
    {
        return Physics2D.Raycast(transform.position, dir, .6f);
    }
    void DetermineNewPos()
    {
        if (IsDirection(nextDirection))
        {
            getNewPos();
        }
        else
            DetermineDirection();
    }
    void getNewPos()
    {
        if (direction == Vector2.up)
        {
            newPos = new Vector2(transform.position.x, transform.position.y + 1);
        }
        else if (direction == Vector2.left)
        {
            newPos = new Vector2(transform.position.x - 1, transform.position.y);
        }
        else if (direction == Vector2.down)
        {
            newPos = new Vector2(transform.position.x, transform.position.y - 1);
        }
        else if (direction == Vector2.right)
        {
            newPos = new Vector2(transform.position.x + 1, transform.position.y);
        }
    }
    void DetermineDirection()
    {
        if (direction == Vector2.up)
        {
            direction = Vector2.left;
            nextDirection = Vector2.down;
        }
        else if (direction == Vector2.left)
        {
            direction = Vector2.down;
            nextDirection = Vector2.right;
        }
        else if (direction == Vector2.down)
        {
            direction = Vector2.right;
            nextDirection = Vector2.up;
        }
        else if (direction == Vector2.right)
        {
            direction = Vector2.up;
            nextDirection = Vector2.left;
        }
        int xPos = Mathf.RoundToInt(transform.position.x);
        int yPos = Mathf.RoundToInt(transform.position.y);
        transform.position = new Vector2((int)xPos, (int)yPos);
        getNewPos();
    }

    void CheckDirection()
    {
       // if(!Physics2D.Raycast(transform.position, nextDirection, .5f))
    }
    void Movse(Vector2 dir)
    {
        if (Physics2D.Raycast(transform.position, nextDirection, .5f))// && Physics2D.Raycast(transform.position, nextDirection, .5f))
            transform.Translate(direction * Time.deltaTime);
        else
        {
            if (direction == Vector2.up)
            {
                direction = Vector2.left;
                nextDirection = Vector2.down;
            }
            else if (direction == Vector2.left)
            {
                direction = Vector2.down;
                nextDirection = Vector2.right;
            }
            else if (direction == Vector2.down)
            {
                direction = Vector2.right;
                nextDirection = Vector2.up;
            }
            else if (direction == Vector2.right)
            {
                direction = Vector2.up;
                nextDirection = Vector2.left;
            }
        }
    }
}

