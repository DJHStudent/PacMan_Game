using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PacStudentStats : MonoBehaviour
{
    [SerializeField]
    int score = 0, scaredTime = 10, lives = 3;
    public Text scoreTxt, scaredTimeTxt;
    public List<Image> ghostLives = new List<Image>();
    //look into data bindings if thing easily done

    public void addScore(int score, GameObject hitObj)
    {
        this.score += score;
        scoreTxt.text = "" + this.score;
        Destroy(hitObj);
    } 
    
    public void startScared()
    {
        scaredTimeTxt.transform.parent.gameObject.SetActive(true);
        scaredTime = 10;
        scaredTimeTxt.text = "" + scaredTime;
        InvokeRepeating("scaredLeft", 1, 1);
    }

    public void scaredLeft()
    {
        scaredTime--;
        scaredTimeTxt.text = "" + scaredTime; 
        if(scaredTime == 3)
        {
            GameManager.ghost1.recovery();
            GameManager.ghost2.recovery();
            GameManager.ghost3.recovery();
            GameManager.ghost4.recovery();
        }
        if(scaredTime == 0)
        {
            scaredTimeTxt.transform.parent.gameObject.SetActive(false);
            GameManager.ghost1.normal();
            GameManager.ghost2.normal();
            GameManager.ghost3.normal();
            GameManager.ghost4.normal();
            CancelInvoke();
        }
    }

    public void removeLife()
    {
        lives--;
        Destroy(ghostLives[lives].gameObject);
        ghostLives.RemoveAt(lives);
    }
}
