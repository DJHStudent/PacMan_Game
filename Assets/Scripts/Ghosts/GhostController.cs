﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class nextPos
{
    public Vector2 pos;
    public int direction;
    public nextPos(int direction, Vector2 pos)
    {
        this.pos = pos;
        this.direction = direction;
    }
}

public class GhostController : MonoBehaviour
{
    //implement A* if time
    public Animator animator;
    Tween tween;
    float duration = .7f;//.9f;
    enum CurrDir { up, left, right, down };
    public enum CurrState { normal, scared, recovery, dead };
    public int currDir = (int)CurrDir.down;
    public int currState = (int)CurrState.normal;
    Vector2 spawnPos, leftBasePos;
    public Vector2[] exitSpawnPos;
    public GameObject playerPos;
    Vector2[] directions = { Vector2.up, Vector2.left, Vector2.right, Vector2.down};
    bool inSpawn = true;
    public bool ghost1, ghost2, ghost3, ghost4;
    [SerializeField]
    GameObject ghost4nextLocation; //get the next waypoint pos for ghost 4
    public LayerMask ignorePellet, spawnLayer, outerWalls;

    public void scared() {
        currState = (int)CurrState.scared;
        animator.SetTrigger("scared" + getDir());
        if (!GameManager.audioManager.isDeadState())
            GameManager.audioManager.scaredState();
    }

    public void recovery()
    {
        if (currState != (int)CurrState.dead)
        {
            currState = (int)CurrState.recovery;
            animator.SetTrigger("recovery");
        }
    }

    public void dead()
    {
        currState = (int)CurrState.dead;
        animator.SetTrigger("dead");
        GameManager.audioManager.deadState();
    }
    public void normal()
    {

        if (currState != (int)CurrState.dead)
        {
            currState = (int)CurrState.normal;
            animator.SetTrigger("norm" + getDir());
            if (!GameManager.audioManager.isDeadState())
                GameManager.audioManager.normalState();
        }
    }

    string getDir()
    {
        switch (currDir)
        {
            case (int)CurrDir.up: return "Up";
            case (int)CurrDir.down: return "Down";
            case (int)CurrDir.left: return "Left";
            default: return "Right";
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("spawnOpening"))
        {
            inSpawn = false;
            ignorePellet = ignorePellet | (1 << 20);
        }
    }
    public void pause()
    {
        animator.speed = 0;
        tween = null;
    }
    //GhostAI
    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.speed = 0;
    }
    public void initialize()
    {
        spawnPos = transform.position;
        playerPos = GameManager.pacStudentController.gameObject;
        animator.speed = 1;
        ghost4nextLocation = GameManager.levelGenerator.wayPointStart;
        setExitPos();
        setNextPos();
    }
    void setExitPos()
    {
        Vector2 exitPos = exitSpawnPos[Random.Range(0, exitSpawnPos.Length - 1)];
        exitPos.y += 1;
        leftBasePos = exitPos;
    }
    private void Update()
    {
        if (tween != null) //if close enough to the end postion determine the next one else continue to the end position
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), tween.EndPos) < 0.05f)
                setNextPos();
            else
                changePos();
        }
    }

    void setNextPos()
    {
        if (inSpawn == true && Vector2.Distance(new Vector2(transform.position.x, transform.position.y), leftBasePos) < 0.05f)
            inSpawn = false;
        if (tween != null)
            transform.position = tween.EndPos;
        if (currState == (int)CurrState.dead && Vector2.Distance(new Vector2(transform.position.x, transform.position.y), spawnPos) < 0.05f)
        {
            resetGhost();
            inSpawn = true;
        }
        tween = new Tween(transform.position, getNextPos(), Time.time, duration);
    }

    void resetGhost()//if ghost back in spawn after being eaten
    {
        GhostController[] allGhostStates = { GameManager.ghost1, GameManager.ghost2, GameManager.ghost3, GameManager.ghost4};
        int deadGhosts = 0;
        bool reset = false;//once ghost reset once don't do it again
        foreach (GhostController ghostState in allGhostStates) //check the current state of the other ghosts to see if they match and change accordingly
        {
            if (ghostState.gameObject != this.gameObject && ghostState.currState == (int)CurrState.dead) //not current ghost
                deadGhosts++;
            if (!reset && ghostState.currState == (int)CurrState.scared)
            {
                scared();
                reset = true;
            }
            else if (!reset && ghostState.currState == (int)CurrState.recovery)
            {
                currState = (int)CurrState.recovery;
                recovery();
                reset = true;
            }
            else if (!reset && ghostState.currState == (int)CurrState.normal)
            {
                currState = (int)CurrState.normal;
                normal();
                reset = true;
            }
        }
        if(deadGhosts == 0)//if no other ghosts are dead reset the music as well(too the approprate state)
        {
            if (currState == (int)CurrState.normal)
                GameManager.audioManager.normalState();
            else
                GameManager.audioManager.scaredState();
        }
    }

    Vector2 getNextPos()
    {
        if(duration != .7f)
            duration = .7f;

        if (inSpawn) //have trigger determining if left spawn or not
        {
            List<nextPos> validPos = ghost2NextPos(leftBasePos);
            return nextPosInfo(validPos);
        }
        else if(currState == (int)CurrState.dead)
        {
            duration = 5f;
            return spawnPos;
        }
        else if (ghost1 || currState == (int)CurrState.scared || currState == (int)CurrState.recovery)
        {
            List<nextPos> validPos = ghost1NextPos(playerPos.transform.position);
            return nextPosInfo(validPos);
        }
        else if(ghost3)
        {
            List<nextPos> validPos = ghost3NextPos();
            return nextPosInfo(validPos);
        }
        else if (ghost4)
        {
            List<nextPos> validPos = ghost4NextPos();
            return nextPosInfo(validPos);
        }
        else
        {
            List<nextPos> validPos = ghost2NextPos(playerPos.transform.position);
            return nextPosInfo(validPos);
        }
    }
    Vector2 nextPosInfo(List<nextPos> validPos)
    {
        int oldDir = currDir;
        int rand = Random.Range(0, validPos.Count);
        currDir = validPos[rand].direction;
        if (oldDir != currDir)
        {
            if (currState == (int)CurrState.normal)
                animator.SetTrigger("norm" + getDir());
            else if (currState == (int)CurrState.scared)
                animator.SetTrigger("scared" + getDir());
        }
        return validPos[rand].pos;
    }
    List<nextPos> ghost1NextPos(Vector2 target)
    {
        Vector2 currPos = new Vector2((int)transform.position.x, (int)transform.position.y);
        List<nextPos> nextPos = new List<nextPos>();//maybey change this here to a class with the nessesary info for directions as well as animation stuff
        //add wall sensing later
        if (isDirSafe((int)CurrDir.up) && isFurther(currPos, currPos + directions[(int)CurrDir.up], target)) //if not backstepping && actually further to target then next pos Valid
            nextPos.Add(addDir((int)CurrDir.up, currPos));

        if (isDirSafe((int)CurrDir.left) && isFurther(currPos, currPos + directions[(int)CurrDir.left], target))
            nextPos.Add(addDir((int)CurrDir.left, currPos));

        if (isDirSafe((int)CurrDir.right) && isFurther(currPos, currPos + directions[(int)CurrDir.right], target))
            nextPos.Add(addDir((int)CurrDir.right, currPos));

        if (isDirSafe((int)CurrDir.down) && isFurther(currPos, currPos + directions[(int)CurrDir.down], target))
            nextPos.Add(addDir((int)CurrDir.down, currPos));
        if (nextPos.Count == 0) //if no valid direction which is further or == then choose a random valid direction
        {
            nextPos = ghost3NextPos();
        }

        return nextPos;
    }
    bool isFurther(Vector2 currPos, Vector2 nextPos, Vector2 target)
    {
        float currDist = Vector2.Distance(currPos, target);
        return Vector2.Distance(nextPos, target) >= currDist;
    }
    List<nextPos> ghost2NextPos(Vector2 target)
    {
        Vector2 currPos = new Vector2((int)transform.position.x, (int)transform.position.y);
        List<nextPos> nextPos = new List<nextPos>();//maybey change this here to a class with the nessesary info for directions as well as animation stuff
        //add wall sensing later
        if (isDirSafe((int)CurrDir.up) && isCloser(currPos, currPos + directions[(int)CurrDir.up], target)) //if not backstepping && actually closer to target then next pos Valid
            nextPos.Add(addDir((int)CurrDir.up, currPos));
        
        if (isDirSafe((int)CurrDir.left) && isCloser(currPos, currPos + directions[(int)CurrDir.left], target))
            nextPos.Add(addDir((int)CurrDir.left, currPos)); 
        
        if (isDirSafe((int)CurrDir.right) && isCloser(currPos, currPos + directions[(int)CurrDir.right], target))
            nextPos.Add(addDir((int)CurrDir.right, currPos));

        if (isDirSafe((int)CurrDir.down) && isCloser(currPos, currPos + directions[(int)CurrDir.down], target))
            nextPos.Add(addDir((int)CurrDir.down, currPos));
        if (nextPos.Count == 0) //if no valid direction which is closer or == then choose a random valid direction
        {
            nextPos = ghost3NextPos();
        }
        return nextPos;
    }
    bool isCloser(Vector2 currPos, Vector2 nextPos, Vector2 target)
    {
        float currDist = Vector2.Distance(currPos, target);
        return Vector2.Distance(nextPos, target) <= currDist;
    }

    List<nextPos> ghost3NextPos()
    {
        Vector2 currPos = new Vector2((int)transform.position.x, (int)transform.position.y);
        List<nextPos> nextPos = new List<nextPos>();//maybey change this here to a class with the nessesary info for directions as well as animation stuff
        //add wall sensing later
        if (isDirSafe((int)CurrDir.up)) //if not backstepping && actually closer to target then next pos Valid
            nextPos.Add(addDir((int)CurrDir.up, currPos));

        if (isDirSafe((int)CurrDir.left))
            nextPos.Add(addDir((int)CurrDir.left, currPos));

        if (isDirSafe((int)CurrDir.right))
            nextPos.Add(addDir((int)CurrDir.right, currPos));

        if (isDirSafe((int)CurrDir.down))
            nextPos.Add(addDir((int)CurrDir.down, currPos));
        if (nextPos.Count == 0)//only backtrack if no other direction is possible to be valid
        {
            nextPos.Add(new nextPos(getOppDir(), currPos - directions[currDir]));
        }

        return nextPos;
    }

    List<nextPos> ghost4NextPos()
    {
        Vector2 currPos = new Vector2((int)transform.position.x, (int)transform.position.y);
        List<nextPos> nextPos = new List<nextPos>();
        if (ghost4nextLocation != null && Vector2.Distance(currPos, ghost4nextLocation.transform.position) > 2.05f)
        {
            nextPos = ghost2NextPos(ghost4nextLocation.GetComponent<Ghost4Waypoints>().nextObj.transform.position);
        }
        else
        {

            {
                ghost4nextLocation = ghost4nextLocation.GetComponent<Ghost4Waypoints>().nextObj;
                //////////if (Vector2.Distance(currPos, ghost4nextLocation.transform.position) < 2.05f)
                //////////    ghost4nextLocation = ghost4nextLocation.GetComponent<Ghost4Waypoints>().nextObj;
                //////////if (Vector2.Distance(currPos, ghost4nextLocation.transform.position) < 2.05f)
                while (Vector2.Distance(currPos, ghost4nextLocation.transform.position) < 2.05f)
                    ghost4nextLocation = ghost4nextLocation.GetComponent<Ghost4Waypoints>().nextObj;
              //  Debug.Log("made it to location" + ghost4nextLocation.transform.localPosition);
            }
            nextPos = ghost2NextPos(ghost4nextLocation.GetComponent<Ghost4Waypoints>().nextObj.transform.position);

        }
        //check all 4 directions

        return nextPos;
    }
    bool isDirSafe(int dir) //checks if the next direction is possible(not wall and not involve backtracking)
    {
        return -directions[currDir] != directions[dir] && !nextPosWall(directions[dir]);
    }
    nextPos addDir(int dir, Vector2 currPos)//add specified direction as a valid direction to choose from
    {
        return new nextPos(dir, currPos + directions[dir]);
    }
    int getOppDir()//if no direction is avaliable without backtracking then need use this to get other direction
    {
        switch (currDir)
        {
            case (int)CurrDir.down: return (int)CurrDir.up;
            case (int)CurrDir.up: return (int)CurrDir.down;
            case (int)CurrDir.left: return (int)CurrDir.right;
            default: return (int)CurrDir.left; 
        }
    }

    bool nextPosWall(Vector2 nextPos) //checks if the next position the player is moving too is a wall
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, nextPos, 1, layer());
        return hit;
    }

    LayerMask layer()
    {
        if (inSpawn)
            return spawnLayer;
        return ignorePellet;
    }
    void changePos() //move the ghost to the specified location
    {
        if (!GameManager.level1UIManager.statsManager.paused)
        {
            float timeFraction = (Time.time - tween.StartTime) / tween.Duration;
            transform.position = Vector2.Lerp(tween.StartPos, tween.EndPos, timeFraction);
        }
    }
}