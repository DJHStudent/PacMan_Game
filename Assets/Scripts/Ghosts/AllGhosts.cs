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
        currState = (int)CurrState.recovery;
        animator.SetTrigger("recovery");
    }
    public void normal()
    {
        currState = (int)CurrState.normal;
        animator.SetTrigger("norm" + getDir());
        GameManager.setSound.normalState();
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
