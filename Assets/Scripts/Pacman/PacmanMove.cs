using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanMove : MonoBehaviour
{

    private Tween tween;
    private float duration = 1;
    private Animator animator;
    int dir = 2;// 1 = up, 2 = left, 3 = down, 4 = right;
    void Start()
    {
        animator = GetComponent<Animator>();
        tween = new Tween(transform.position, new Vector2(transform.position.x - 1, transform.position.y), Time.time, duration);
    }

    // Update is called once per frame
    void Update()
    {
        if(new Vector2(transform.position.x, transform.position.y) == tween.EndPos)
            determinePos();
        move();

    }

    private void move()
    {
        float timeFraction = (Time.time - tween.StartTime) / tween.Duration;
        transform.position = Vector2.Lerp(tween.StartPos, tween.EndPos, timeFraction);
    }

    private void determinePos()
    {
        if (!emptyDirection() && !IsDirection(getDir(dir))){
            newPos();
        }
        else
        {
            Debug.Log("change direction");
            if (dir != 4)
                dir++;
            else
                dir = 1;
            setAnimation();
            newPos();
        }
    }
    private void newPos()
    {
        switch (dir)
        {
            case 1: upDownMove(1); break;
            case 2: lefRighMove(-1); break;
            case 3: upDownMove(-1); break;
            default: lefRighMove(1); break;
        }
    }

    private void lefRighMove(int newPos)
    {
        tween = new Tween(transform.position, new Vector2(transform.position.x + newPos, transform.position.y), Time.time, duration);
    }

    private void upDownMove(int newPos)
    {
        tween = new Tween(transform.position, new Vector2(transform.position.x, transform.position.y + newPos), Time.time, duration);
    }

    private Vector2 getDir(int direction)
    {
        switch (direction)
        {
            case 1: return Vector2.up;
            case 2: return Vector2.left;
            case 3: return Vector2.down;
            default: return Vector2.right;
        }
    }
    void setAnimation()
    {
        switch (dir)
        {
            case 1: animator.SetTrigger("Up"); break;
            case 2: animator.SetTrigger("Left"); break;
            case 3: animator.SetTrigger("Down"); break;
            default: animator.SetTrigger("Right"); break;
        }
    }
    bool IsDirection(Vector2 dir)
    {
        return Physics2D.Raycast(transform.position, dir, .6f);
    }

    bool emptyDirection()
    {
        switch (dir)
        {
            case 3: return !IsDirection(getDir(4));
            case 4: return !IsDirection(getDir(1));
            default: return false;
        }
    }
}
