﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level1UIManager : MonoBehaviour
{
    public Text scoreTxt, scaredTimeTxt, startTimerTxt, playTimeTxt;
    public GameObject startTimerImg;

    public List<Image> ghostLives = new List<Image>();
    public PacStudentStatsManager statsManager;

    void Awake()
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
}
