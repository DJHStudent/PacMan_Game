using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class GhostController : MonoBehaviour
{

    public Animator animator;
    Tween tween;
    enum CurrDir { up, left, right, down };
    public enum CurrState { normal, scared, recovery, dead };
    int currDir = (int)CurrDir.up;
    public int currState = (int)CurrState.normal;

    Vector2 spawnPos;
    Vector2[] directions = { Vector2.up, Vector2.left, Vector2.right, Vector2.down};
    bool inSpawn = true;
    public void scared() {
        currState = (int)CurrState.scared;
        animator.SetTrigger("scared" + getDir());
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



    //GhostAI
    private void Start()
    {
        spawnPos = transform.position;
    }

    Vector2 getNextPos(Vector2 target)
    {
        if (inSpawn) //have trigger determining if left spawn or not
        {
            List<Vector2> validPos = ghost2NextPos(target);
            return validPos[Random.Range(0, validPos.Count - 1)];
        }
        else
            return Vector2.zero;
    }

    List<Vector2> ghost2NextPos(Vector2 target)
    {
        Vector2 currPos = new Vector2((int)transform.position.x, (int)transform.position.y);
        float distToTarget = Vector2.Distance(currPos, target);
        List<Vector2> nextPos = new List<Vector2>();//maybey change this here to a class with the nessesary info for directions as well as animation stuff
        //add wall sensing later
        if (-directions[currDir] != directions[(int)CurrDir.up] && Vector2.Distance(currPos + directions[(int)CurrDir.up], target) <= distToTarget) //if not backstepping && actually closer to target then next pos Valid
            nextPos.Add(currPos + directions[(int)CurrDir.up]);
        
        if (-directions[currDir] != directions[(int)CurrDir.left] && Vector2.Distance(currPos - directions[(int)CurrDir.left], target) <= distToTarget)
            nextPos.Add(currPos - directions[(int)CurrDir.left]); 
        
        if (-directions[currDir] != directions[(int)CurrDir.right] && Vector2.Distance(currPos + directions[(int)CurrDir.right], target) <= distToTarget)
            nextPos.Add(currPos + directions[(int)CurrDir.right]);

        if (-directions[currDir] != directions[(int)CurrDir.down] && Vector2.Distance(currPos - directions[(int)CurrDir.down], target) <= distToTarget)
            nextPos.Add(currPos - directions[(int)CurrDir.down]);
        if (nextPos.Count == 0)
            nextPos.Add(currPos - directions[currDir]);

        return nextPos;
    }

    void changePos() //move the player to the specified location
    {
        if (!GameManager.level1UIManager.statsManager.paused)
        {
            float timeFraction = (Time.time - tween.StartTime) / tween.Duration;
            transform.position = Vector2.Lerp(tween.StartPos, tween.EndPos, timeFraction);
        }
    }
}
