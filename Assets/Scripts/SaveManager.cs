using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    const string saveTime = "time";
    const string saveScore = "score";
    void Awake()
    {
        DontDestroyOnLoad(this);
        loadSave();
    }

    public void loadSave()
    {
        if (PlayerPrefs.HasKey(saveTime) && PlayerPrefs.HasKey(saveScore))
        {
            StartUIManager uiManager = GameObject.Find("GameManager").GetComponent<StartUIManager>();
            float time = PlayerPrefs.GetFloat(saveTime);
            string mins = ((int)time / 60).ToString("00:");
            string secs = (time % 60).ToString("00.00");
            uiManager.timeTxt.text = "Time: " + mins + secs.Replace('.', ':');
            uiManager.scoreTxt.text = "Score: " + PlayerPrefs.GetInt(saveScore);
        }
    }

    public void saveStats()
    {
        int score = GameManager.level1UIManager.statsManager.score;
        float currTime = Time.timeSinceLevelLoad - GameManager.level1UIManager.statsManager.levelStartTime;
        if (!PlayerPrefs.HasKey(saveScore) ||score > PlayerPrefs.GetInt(saveScore))
            PlayerPrefs.SetInt(saveScore, score);
        if (!PlayerPrefs.HasKey(saveTime) || currTime < PlayerPrefs.GetFloat(saveTime))
            PlayerPrefs.SetFloat(saveTime, currTime);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
