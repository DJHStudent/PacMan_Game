using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PacStudentStatsManager : MonoBehaviour
{
    public int score = 0, pelletAmount; 
    int scaredTime = 10, lives = 3;
    public float levelStartTime;

    public bool paused = false;
 
    public void initilize()
    {
        GameManager.pacStudentController.pause();
        paused = true;
        StartCoroutine(startTimer()); 
        //InvokeRepeating("playTime", .01f, .01f);
    }
    public void determinePellets()
    {
        if (GameManager.activeScene == (int)GameManager.ActiveScene.recreation)
            pelletAmount = GameManager.levelGenerator.pelletAmount * 4 - 2;
        else if (GameManager.activeScene == (int)GameManager.ActiveScene.innovation || GameManager.activeScene == (int)GameManager.ActiveScene.loading)
            pelletAmount = GameManager.randomMap.pelletAmount;
    }
    IEnumerator startTimer() //countdown from 3
    {
        GameManager.levelUIManager.setStartTimer("3");
        yield return new WaitForSecondsRealtime(1);
        GameManager.levelUIManager.setStartTimer("2");
        yield return new WaitForSecondsRealtime(1);
        GameManager.levelUIManager.setStartTimer("1");
        yield return new WaitForSecondsRealtime(1);
        GameManager.levelUIManager.setStartTimer("Go!");
        yield return new WaitForSecondsRealtime(1);
        GameManager.levelUIManager.startTimerVisable(false);
        paused = false;
        foreach (GhostController ghost in GameManager.ghost1)
        {
            ghost.initialize();
        }
        foreach (GhostController ghost in GameManager.ghost2)
        {
            ghost.initialize();
        }
        foreach (GhostController ghost in GameManager.ghost3)
        {
            ghost.initialize();
        }
        foreach (GhostController ghost in GameManager.ghost4)
        {
            ghost.initialize();
        }
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
        GameManager.levelUIManager.setScore("" + this.score);
        if (hitObj != null)
            Destroy(hitObj);
    }
    void setTime()
    {
        float time = Time.timeSinceLevelLoad - levelStartTime;
        string mins = ((int)time / 60).ToString("00:");
        string secs = (time % 60).ToString("00.00");
        GameManager.levelUIManager.setTime(mins + secs.Replace('.', ':'));
    }
    public void startScared()
    {
        GameManager.levelUIManager.scaredTimerVisability(true);
        scaredTime = 10;
        GameManager.levelUIManager.setScaredTime("" + scaredTime);
        InvokeRepeating("scaredLeft", 1, 1);
    }

    public void scaredLeft()//fixup with ghosts
    {
        scaredTime--;
        GameManager.levelUIManager.setScaredTime("" + scaredTime);
        if (scaredTime == 3)
        {
            foreach (GhostController ghost in GameManager.ghost1)
            {
                ghost.recovery();
            }
            foreach (GhostController ghost in GameManager.ghost2)
            {
                ghost.recovery();
            }
            foreach (GhostController ghost in GameManager.ghost3)
            {
                ghost.recovery();
            }
            foreach (GhostController ghost in GameManager.ghost4)
            {
                ghost.recovery();
            }
        }
        if (scaredTime == 0)
        {
            GameManager.levelUIManager.scaredTimerVisability(false);
            foreach (GhostController ghost in GameManager.ghost1)
            {
                ghost.normal();
            }
            foreach (GhostController ghost in GameManager.ghost2)
            {
                ghost.normal();
            }
            foreach (GhostController ghost in GameManager.ghost3)
            {
                ghost.normal();
            }
            foreach (GhostController ghost in GameManager.ghost4)
            {
                ghost.normal();
            }
            CancelInvoke();
        }
    }

    public void removeLife()
    {
        lives--;
        GameManager.levelUIManager.setLives(lives);
        gameOver("Game Over", Color.red);
    }
    public void removePellet()
    {
        pelletAmount--;
        gameOver("You Win", new Color32(0, 224, 102, 255));
    }   
    public void gameOver(string text, Color textColour)
    {
        if(lives == 0 || pelletAmount == 0)
        {
            //pause everything
            paused = true;
            GameManager.pacStudentController.pause();            
            foreach (GhostController ghost in GameManager.ghost1)
            {
                ghost.pause();
            }
            foreach (GhostController ghost in GameManager.ghost2)
            {
                ghost.pause();
            }
            foreach (GhostController ghost in GameManager.ghost3)
            {
                ghost.pause();
            }
            foreach (GhostController ghost in GameManager.ghost4)
            {
                ghost.pause();
            }
            GameManager.levelUIManager.setStartTimer(text);
            GameManager.levelUIManager.setStartTimerColour(textColour);
            GameManager.levelUIManager.startTimerVisable(true);
            Ghost4Waypoints.currDir = Vector2.zero;
            Ghost4Waypoints.straightWall = null;
            Invoke("changeScene", 3);
        }
    }
    void changeScene()
    {
        SceneManager.LoadScene(0);
        GameManager.saveManager.saveStats();
    }
}
