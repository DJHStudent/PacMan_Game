using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllGhosts : MonoBehaviour
{

    public Animator animator;
    enum CurrDir { up, left, right, down };
    public enum CurrState { normal, scared, recovery, dead };
    int currDir = (int)CurrDir.up;
    public int currState = (int)CurrState.normal;


    public void scared() {
        currState = (int)CurrState.scared;
        animator.SetTrigger("scared" + getDir());
        GameManager.setSound.scaredState();
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
        GameManager.setSound.deadState();
    }
    public void normal()
    {

        if (currState != (int)CurrState.dead)
        {
            currState = (int)CurrState.normal;
            animator.SetTrigger("norm" + getDir());
            if (!GameManager.setSound.isDeadState())
                GameManager.setSound.normalState();
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
}
