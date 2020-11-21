using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIManager : MonoBehaviour
{
    public Text scoreTxt, scaredTimeTxt, startTimerTxt, playTimeTxt;
    public GameObject startTimerImg, miniMap;

    public List<Image> ghostLives = new List<Image>();
    public PacStudentStatsManager statsManager;

    public void initilize()
    {
        statsManager = GetComponent<PacStudentStatsManager>();
    }
    void Start()
    {
        startTimerTxt = startTimerImg.transform.GetChild(0).GetComponent<Text>();
    }
    public void setLives(int lives)
    {
        if (lives >= 0)
        {
            Destroy(ghostLives[lives].gameObject);
            ghostLives.RemoveAt(lives);
        }
    }
    public void setTime(string time)
    {
        playTimeTxt.text = time;
    }
    public void setScore(string score)
    {
        scoreTxt.text = score;
    }

    public void setStartTimer(string value)
    {
        startTimerTxt.text = value;
    }
    public void setStartTimerColour(Color colour)
    {
        startTimerTxt.color = colour;
    }

    public void startTimerVisable(bool visability)
    {
        startTimerImg.SetActive(visability);
    }

    public void scaredTimerVisability(bool visability)
    {
        scaredTimeTxt.transform.parent.gameObject.SetActive(visability);
    }
    public void setScaredTime(string time)
    {
        scaredTimeTxt.text = time;
    }

    public void displayMiniMap()
    {
        if (miniMap.activeSelf)
            miniMap.SetActive(false);
        else
            miniMap.SetActive(true);
    }
}
