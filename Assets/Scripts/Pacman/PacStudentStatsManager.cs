using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PacStudentStatsManager : MonoBehaviour
{
    public int score = 0, pelletAmount; 
    int scaredTime = 10, lives = 3;
    public float levelStartTime;

    public bool paused = false;
 
    public void Start()
    {
        GameManager.pacStudentController.pause();
        paused = true;
        pelletAmount = GameManager.levelGenerator.pelletAmount * 4 - 2;
        StartCoroutine(startTimer()); 
        //InvokeRepeating("playTime", .01f, .01f);
    }
    IEnumerator startTimer() //countdown from 3
    {
        GameManager.level1UIManager.setStartTimer("3");
        yield return new WaitForSecondsRealtime(1);
        GameManager.level1UIManager.setStartTimer("2");
        yield return new WaitForSecondsRealtime(1);
        GameManager.level1UIManager.setStartTimer("1");
        yield return new WaitForSecondsRealtime(1);
        GameManager.level1UIManager.setStartTimer("Go!");
        yield return new WaitForSecondsRealtime(1);
        GameManager.level1UIManager.startTimerVisable(false);
        paused = false;
        GameManager.ghost1.initialize();
        GameManager.ghost2.initialize();
        GameManager.ghost3.initialize();
        GameManager.ghost4.initialize();
        levelStartTime = Time.timeSinceLevelLoad;
        GameManager.audioManager.normalState();
    }

    private void Update()
    {
        if(!paused)
            setTime();
    }
    public void addScore(int amount, GameObject hitObj)
    {
        this.score += amount;
        GameManager.level1UIManager.setScore("" + this.score);
        if (hitObj != null)
            Destroy(hitObj);
    }
    void setTime()
    {
        float time = Time.timeSinceLevelLoad - levelStartTime;
        string mins = ((int)time / 60).ToString("00:");
        string secs = (time % 60).ToString("00.00");
        GameManager.level1UIManager.setTime(mins + secs.Replace('.', ':'));
    }
    public void startScared()
    {
        GameManager.level1UIManager.scaredTimerVisability(true);
        scaredTime = 10;
        GameManager.level1UIManager.setScaredTime("" + scaredTime);
        InvokeRepeating("scaredLeft", 1, 1);
    }

    public void scaredLeft()//fixup with ghosts
    {
        scaredTime--;
        GameManager.level1UIManager.setScaredTime("" + scaredTime);
        if (scaredTime == 3)
        {
            GameManager.ghost1.recovery();
            GameManager.ghost2.recovery();
            GameManager.ghost3.recovery();
            GameManager.ghost4.recovery();
        }
        if (scaredTime == 0)
        {
            GameManager.level1UIManager.scaredTimerVisability(false);
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
        GameManager.level1UIManager.setLives(lives);
        gameOver();
    }
    public void removePellet()
    {
        pelletAmount--;
        gameOver();
    }
    public void gameOver()
    {
        if(lives == 0 || pelletAmount == 0)
        {
            //pause everything
            paused = true;
            GameManager.pacStudentController.pause();
            GameManager.level1UIManager.setStartTimer("Game Over");
            GameManager.level1UIManager.startTimerVisable(true);
            Invoke("changeScene", 3);
        }
    }
    void changeScene()
    {
        SceneManager.LoadScene(0);
        GameManager.saveManager.saveStats();
    }
}
