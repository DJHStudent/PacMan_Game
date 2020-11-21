using System.Collections;
using System.Collections.Generic;
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
{ //issues gjots sometimes not reset, pacman walk audio not instant if die, repawn and start moving again
    public Animator animator;
    SpriteRenderer miniMapRenderer;
    Tween tween;
    float duration = .7f;
    enum CurrDir { up, left, right, down };
    public enum CurrState { normal, scared, recovery, dead };
    public int currDir = (int)CurrDir.down;
    public int currState = (int)CurrState.normal;
    Vector2 spawnPos, leftBasePos;
    public Vector2[] exitSpawnPos;
    public GameObject playerPos;
    Vector2[] directions = { Vector2.up, Vector2.left, Vector2.right, Vector2.down };
    bool inSpawn = true;
    public bool ghost1, ghost2, ghost3, ghost4;
    public static float ghostSpawnDelay = -.5f;

    bool notAtWall = true;
    GameObject ghost4nextLocation; //get the next waypoint pos for ghost 4
    public LayerMask ignorePellet, spawnLayer, outerWalls;
    APathfinding pathFinder;
    public void scared()
    {
        if (currState != (int)CurrState.dead)
        {
            currState = (int)CurrState.scared;
            miniMapRenderer.color = new Color32(0, 64, 255, 255);
            notAtWall = true;
            animator.SetTrigger("scared" + getDir());
            if (!GameManager.audioManager.isDeadState())
                GameManager.audioManager.scaredState();
        }
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
        miniMapRenderer.color = new Color32(0, 0, 0, 0);
        inSpawn = false;
        tween = null;
        setNextPos();
        animator.SetTrigger("dead");
        GameManager.audioManager.deadState();
    }
    public void normal()
    {

        if (currState != (int)CurrState.dead)
        {
            currState = (int)CurrState.normal;
            miniMapRenderer.color = new Color32(255, 0, 0, 255);
            if(pathFinder != null)
                pathFinder.path.Clear();
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
            ignorePellet |= 1 << 20;
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
        miniMapRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }
    public void initialize()
    {
        //GameObject.Find("SceneManager").GetComponent<APathfinding>();
        animator.speed = 1;
        if (GameManager.activeScene == (int)GameManager.ActiveScene.innovation || GameManager.activeScene == (int)GameManager.ActiveScene.loading)
        {
            pathFinder = new APathfinding();
            ghostSpawnDelay += 0.5f;
            StartCoroutine(startWait());
        }
        else if (GameManager.activeScene == (int)GameManager.ActiveScene.recreation)
            initializeWait();
    }
    IEnumerator startWait()//if on innovation scene wait for specified seconds so that all ghosts don't end up clumping tpgether
    {
        yield return new WaitForSeconds(ghostSpawnDelay);
        initializeWait();
    }
    void initializeWait()
    {
        playerPos = GameManager.pacStudentController.gameObject;
        animator.speed = 1;
        if (GameManager.activeScene == (int)GameManager.ActiveScene.recreation)
            ghost4nextLocation = GameManager.levelGenerator.wayPointStart;
        if (GameManager.activeScene == (int)GameManager.ActiveScene.innovation || GameManager.activeScene == (int)GameManager.ActiveScene.loading)
        {
            GameObject[] spawnOpenings = GameObject.FindGameObjectsWithTag("spawnOpening");
            exitSpawnPos = new Vector2[] { spawnOpenings[0].transform.position, spawnOpenings[1].transform.position };
            ghost4nextLocation = GameManager.randomMap.wayPointStart;
        }
        spawnPos = transform.position;
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
        Vector2 pos = transform.position;
        if (inSpawn == true && Vector2.Distance(new Vector2(pos.x, pos.y), leftBasePos) < 0.05f)
            inSpawn = false;
        if (tween != null)
            transform.position = tween.EndPos;
        if (currState == (int)CurrState.dead && Vector2.Distance(new Vector2(pos.x, pos.y), spawnPos) < 0.05f)
        {
            resetGhost();
            inSpawn = true;
        }
        tween = new Tween(pos, getNextPos(), Time.time, duration);
    }

    void resetGhost()//if ghost back in spawn after being eaten
    {
        List<GhostController>[] allGhostStates = { GameManager.ghost1, GameManager.ghost2, GameManager.ghost3, GameManager.ghost4 };
        int deadGhosts = 0;
        bool reset = false;//once ghost reset once don't do it again
        foreach (List<GhostController> currGhosts in allGhostStates) //check the current state of the other ghosts to see if they match and change accordingly
        {
            foreach (GhostController ghostState in currGhosts) //in the array of lists get each element in the list here
            {
                if (ghostState.gameObject != this.gameObject && ghostState.currState == (int)CurrState.dead) //not current ghost but still dead
                    deadGhosts++;
                if (!reset && ghostState.currState == (int)CurrState.scared)
                {
                    currState = (int)CurrState.scared;
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
        }
        if (deadGhosts == 0)//if no other ghosts are dead reset the music as well(too the approprate state)
        {
            if (currState == (int)CurrState.normal)
                GameManager.audioManager.normalState();
            else
                GameManager.audioManager.scaredState();
        }
    }

    Vector2 getNextPos()
    {
        if (duration != .7f)//resets the ghost speed if not at normal speed
            duration = .7f;

        if (inSpawn) //have trigger determining if left spawn or not
        {
            List<nextPos> validPos = ghost2NextPos(leftBasePos);
            return nextPosInfo(validPos);
        }
        else if (currState == (int)CurrState.dead)
        {
            duration = 5f; //change the ghost speed when dead
            return spawnPos;
        }
        else if (ghost1 || currState == (int)CurrState.scared || currState == (int)CurrState.recovery)
        {
            List<nextPos> validPos = ghost1NextPos(playerPos.transform.position);
            return nextPosInfo(validPos);
        }
        else if (ghost3)
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
        if (GameManager.activeScene == (int)GameManager.ActiveScene.recreation || inSpawn)// || !spriteRenderer.isVisible)
        {
            if (isDirSafe((int)CurrDir.up) && isCloser(currPos, currPos + directions[(int)CurrDir.up], target)) //if not backstepping && actually closer to target then next pos Valid
                nextPos.Add(addDir((int)CurrDir.up, currPos));

            if (isDirSafe((int)CurrDir.down) && isCloser(currPos, currPos + directions[(int)CurrDir.down], target))
                nextPos.Add(addDir((int)CurrDir.down, currPos));

            if (isDirSafe((int)CurrDir.left) && isCloser(currPos, currPos + directions[(int)CurrDir.left], target))
                nextPos.Add(addDir((int)CurrDir.left, currPos));

            if (isDirSafe((int)CurrDir.right) && isCloser(currPos, currPos + directions[(int)CurrDir.right], target))
                nextPos.Add(addDir((int)CurrDir.right, currPos));


            if (nextPos.Count == 0) //if no valid direction which is closer or == then choose a random valid direction
            {
                nextPos = ghost3NextPos();
            }
            if (ghost4)
                morePos = true;
            return nextPos;
        }
        else
        {
            Node correctPos = pathFinder.pathFinder((int)transform.position.x, (int)transform.position.y, (int)target.x, (int)target.y, directions[currDir]);
            //get direction from currPos and correctPos
            if (correctPos != null)
            {
                Vector2 dir = (new Vector2(correctPos.currentX, correctPos.currentY) - currPos).normalized;
                int directionValue;
                // switch (dir)    // enum CurrDir { up, left, right, down };
                // {
                //   case Vector2.up: directionValue = 0;
                //   default: directionValue = 3;
                // }
                if (dir == Vector2.up)
                    directionValue = 0;
                else if (dir == Vector2.left)
                    directionValue = 1;
                else if (dir == Vector2.right)
                    directionValue = 2;
                else
                    directionValue = 3;
                //get direction from node and need use it to ensure ony moves to next position//need to convert direction to an int as well
                nextPos.Add(addDir(directionValue, currPos));
            }
            else
            {
                nextPos = ghost3NextPos();
            }
            return nextPos;
        }
    }
    bool morePos = true;
    bool currentPos(Vector2 yPos)
    {
        return yPos.y > 10 || yPos.y < -6;
    }
    bool isCloser(Vector2 currPos, Vector2 nextPos, Vector2 target)
    {
        float currXDist = Mathf.RoundToInt(Mathf.Abs(currPos.x - target.x));
        float currYDist = Mathf.RoundToInt(Mathf.Abs(currPos.y - target.y));
        float nextXDist = Mathf.RoundToInt(Mathf.Abs(nextPos.x - target.x));
        float nextYDist = Mathf.RoundToInt(Mathf.Abs(nextPos.y - target.y));

      //  float manhattanCurrDist = currXDist + currYDist, manhattanNextDist = nextXDist + nextYDist;
        float currDist = Vector2.Distance(currPos, target);
        if(ghost4 && !inSpawn && GameManager.activeScene == (int)GameManager.ActiveScene.recreation)//if on recreation scene and around the wall sticking out of border prefer to actually move up or down
        {
            if (nextXDist == currXDist && nextYDist < currYDist && currentPos(currPos))
            {
                morePos = false;
                return true;
            }
        }
        return Vector2.Distance(nextPos, target) <= currDist && morePos;//manhattanNextDist < manhattanCurrDist;
    }

    List<nextPos> ghost3NextPos()
    {
        Vector2 currPos = new Vector2((int)transform.position.x, (int)transform.position.y);
        List<nextPos> nextPos = new List<nextPos>();
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
        List<nextPos> nextPos;
        Vector2 pathNextPos;
        if (ghost4nextLocation != null && Vector2.Distance(currPos, ghost4nextLocation.transform.position) > 2.05f)
        {
            if (notAtWall) //if away from wall as scared/ dead or just spawned in then continually check if touching a wall
            {
                RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, 1, outerWalls);
                RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, 1, outerWalls);
                RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 1, outerWalls);
                RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, 1, outerWalls);
                if (hitUp && hitUp.collider.gameObject.GetComponent<Ghost4Waypoints>() != null)
                {
                    ghost4nextLocation = hitUp.collider.gameObject.GetComponent<Ghost4Waypoints>().nextObj;
                    if (GameManager.activeScene == (int)GameManager.ActiveScene.innovation || GameManager.activeScene == (int)GameManager.ActiveScene.loading)
                    {
                        pathNextPos = determineNextPos(ghost4nextLocation.transform.position);
                        nextPos = ghost2NextPos(pathNextPos);
                    }
                    else
                        nextPos = ghost2NextPos(ghost4nextLocation.transform.position);
                    notAtWall = false;
                }
                else if (hitDown && hitDown.collider.gameObject.GetComponent<Ghost4Waypoints>() != null)
                {
                    ghost4nextLocation = hitDown.collider.gameObject.GetComponent<Ghost4Waypoints>().nextObj;
                    if (GameManager.activeScene == (int)GameManager.ActiveScene.innovation || GameManager.activeScene == (int)GameManager.ActiveScene.loading)
                    {
                        pathNextPos = determineNextPos(ghost4nextLocation.transform.position);
                        nextPos = ghost2NextPos(pathNextPos);
                    }
                    else
                        nextPos = ghost2NextPos(ghost4nextLocation.transform.position);
                    notAtWall = false;
                }
                else if (hitLeft && hitLeft.collider.gameObject.GetComponent<Ghost4Waypoints>() != null)
                {
                    ghost4nextLocation = hitLeft.collider.gameObject.GetComponent<Ghost4Waypoints>().nextObj;
                    if (GameManager.activeScene == (int)GameManager.ActiveScene.innovation || GameManager.activeScene == (int)GameManager.ActiveScene.loading)
                    {
                        pathNextPos = determineNextPos(ghost4nextLocation.transform.position);
                        nextPos = ghost2NextPos(pathNextPos);
                    }
                    else
                        nextPos = ghost2NextPos(ghost4nextLocation.transform.position);
                    notAtWall = false;
                }
                else if (hitRight && hitRight.collider.gameObject.GetComponent<Ghost4Waypoints>() != null)
                {
                    ghost4nextLocation = hitRight.collider.gameObject.GetComponent<Ghost4Waypoints>().nextObj;
                    if (GameManager.activeScene == (int)GameManager.ActiveScene.innovation || GameManager.activeScene == (int)GameManager.ActiveScene.loading)
                    {
                        pathNextPos = determineNextPos(ghost4nextLocation.transform.position);
                        nextPos = ghost2NextPos(pathNextPos);
                    }
                    else
                        nextPos = ghost2NextPos(ghost4nextLocation.transform.position);
                    notAtWall = false;
                }
                else //not hit anywall
                {
                    if (GameManager.activeScene == (int)GameManager.ActiveScene.innovation || GameManager.activeScene == (int)GameManager.ActiveScene.loading)
                    {
                        pathNextPos = determineNextPos(ghost4nextLocation.transform.position);//need to set ghost waypoint here?
                        nextPos = ghost2NextPos(pathNextPos);
                    }
                    else
                        nextPos = ghost2NextPos(ghost4nextLocation.GetComponent<Ghost4Waypoints>().nextObj.transform.position);
                }
            }
            else //not hit anywall
            {
                if (GameManager.activeScene == (int)GameManager.ActiveScene.innovation || GameManager.activeScene == (int)GameManager.ActiveScene.loading)
                {
                    pathNextPos = determineNextPos(ghost4nextLocation.transform.position);
                    nextPos = ghost2NextPos(pathNextPos);
                }
                else
                    nextPos = ghost2NextPos(ghost4nextLocation.GetComponent<Ghost4Waypoints>().nextObj.transform.position);
            }
        }
        else
        {
                while (Vector2.Distance(currPos, ghost4nextLocation.transform.position) < 2.05f)
                    ghost4nextLocation = ghost4nextLocation.GetComponent<Ghost4Waypoints>().nextObj;
            if (GameManager.activeScene == (int)GameManager.ActiveScene.innovation || GameManager.activeScene == (int)GameManager.ActiveScene.loading)
            {
                pathNextPos = determineNextPos(ghost4nextLocation.transform.position);
                nextPos = ghost2NextPos(pathNextPos);
            }
            else
                nextPos = ghost2NextPos(ghost4nextLocation.GetComponent<Ghost4Waypoints>().nextObj.transform.position);
        }
        return nextPos;
    }
    Vector2 determineNextPos(Vector2 pos)
    {
        Node[,] map = GameManager.randomMap.map;
        //right
        if ((int)pos.x + 1 < GameManager.randomMap.width)
        {
            if (map[(int)pos.x + 1, (int)pos.y].tileType == 1 || map[(int)pos.x + 1, (int)pos.y].tileType == 11 || map[(int)pos.x + 1, (int)pos.y].tileType == 4)
                return new Vector2((int)pos.x + 1, (int)pos.y);
        }
        //left
        if ((int)pos.x - 1 >= 0)
        {
            if (map[(int)pos.x - 1, (int)pos.y].tileType == 1 || map[(int)pos.x - 1, (int)pos.y].tileType == 11 || map[(int)pos.x - 1, (int)pos.y].tileType == 4)
                return new Vector2((int)pos.x - 1, (int)pos.y);
        }
        //up
        if ((int)pos.y + 1 < GameManager.randomMap.height)
        {
            if (map[(int)pos.x, (int)pos.y + 1].tileType == 1 || map[(int)pos.x, (int)pos.y + 1].tileType == 11 || map[(int)pos.x, (int)pos.y + 1].tileType == 4)
                return new Vector2((int)pos.x, (int)pos.y + 1);
        }
        //down
        // (map[(int)pos.x, (int)pos.y + 1].tileType == 1 || map[(int)pos.x, (int)pos.y + 1].tileType == 11 || map[(int)pos.x, (int)pos.y + 1].tileType == 4)
            return new Vector2((int)pos.x, (int)pos.y - 1);
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
        if (!GameManager.levelUIManager.statsManager.paused)
        {
            float timeFraction = (Time.time - tween.StartTime) / tween.Duration;
            transform.position = Vector2.Lerp(tween.StartPos, tween.EndPos, timeFraction);
        }
    }
}

