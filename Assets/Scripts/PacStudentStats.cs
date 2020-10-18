using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PacStudentStats : MonoBehaviour
{
    [SerializeField]
    int score = 0, scaredTime = 10, lives = 3, playMiliSecs = 0, playSecs = 0, playMins = 0;
    public Text scoreTxt, scaredTimeTxt, startTimerTxt, playTimeTxt;
    public GameObject startTimerImg;
    public List<Image> ghostLives = new List<Image>();
    //look into data bindings if thing easily done

    private void Start()
    {
        startTimerTxt = startTimerImg.transform.GetChild(0).GetComponent<Text>();
        Time.timeScale = 0;
        StartCoroutine(startTimer()); 
        //InvokeRepeating("playTime", .01f, .01f);
    }

    private void Update()
    {
        float time = Time.time;
        string mins = ((int)time / 60).ToString("00:");
        string secs = (time % 60).ToString("00.00");
      //  string formatSecs = secs.ToString("00:00.");

        playTimeTxt.text = mins + secs.Replace('.', ':');
    }
    void playTime()
    {
        playMiliSecs++;
        playTimeTxt.text = playMins.ToString("00") + ":" + playSecs.ToString("00") + ":" + playMiliSecs.ToString("00");
    }

    public void addScore(int score, GameObject hitObj)
    {
        this.score += score;
        scoreTxt.text = "" + this.score;
        if(hitObj != null)
            Destroy(hitObj);
    } 
    
    IEnumerator startTimer() //countdown from 3
    {
        startTimerTxt.text = "3";
        yield return new WaitForSecondsRealtime(1);
        startTimerTxt.text = "2";
        yield return new WaitForSecondsRealtime(1);
        startTimerTxt.text = "1";
        yield return new WaitForSecondsRealtime(1);
        startTimerTxt.text = "Go!";
        yield return new WaitForSecondsRealtime(1);
        startTimerImg.SetActive(false);
        Time.timeScale = 1;
        GameManager.setSound.normalState();
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
        Debug.Log("remove Life");
        scaredTime--;
        scaredTimeTxt.text = "" + scaredTime;
        if (scaredTime == 3)
        {
            GameManager.ghost1.recovery();
            GameManager.ghost2.recovery();
            GameManager.ghost3.recovery();
            GameManager.ghost4.recovery();
        }
        if (scaredTime == 0)
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
