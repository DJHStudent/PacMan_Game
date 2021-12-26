using System.Collections.Generic;
using UnityEngine;

public class SeedInfo
{
    public int seed, width, height;
    public SeedInfo(int seed, int width, int height)
    {
        this.width = width;
        this.height = height;
        this.seed = seed;
    }
}
public class SaveManager : MonoBehaviour
{
    const string saveTime = "time";
    const string saveScore = "score";
    private static SaveManager saveInstance;

    public List<SeedInfo> seeds = new List<SeedInfo>();
    void Awake()
    {
        DontDestroyOnLoad(this);
        handleDuplicates();
        loadSave();
    }
    void handleDuplicates()
    {
        if (saveInstance == null)
        {
            saveInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void loadSave()
    {
        if (PlayerPrefs.HasKey(saveTime) && PlayerPrefs.HasKey(saveScore))
        {
            StartUIManager uiManager = GameObject.Find("GameManager").GetComponent<StartUIManager>();
            float time = PlayerPrefs.GetFloat(saveTime);
            string mins = ((int)time / 60).ToString("00:");
            string secs = (time % 60).ToString("00.00");
            uiManager.timeTxt.text = "Time:\n" + mins + secs.Replace('.', ':');
            uiManager.scoreTxt.text = "Score:\n" + PlayerPrefs.GetInt(saveScore);
        }
    }

    public void saveStats()
    {
        int score = GameManager.levelUIManager.statsManager.score;
        float currTime = Time.timeSinceLevelLoad - GameManager.levelUIManager.statsManager.levelStartTime;
        if (!PlayerPrefs.HasKey(saveScore) || score > PlayerPrefs.GetInt(saveScore))
        {
            PlayerPrefs.SetInt(saveScore, score);
            PlayerPrefs.SetFloat(saveTime, currTime);
        }
        if (!PlayerPrefs.HasKey(saveTime) || score >= PlayerPrefs.GetInt(saveScore) && currTime < PlayerPrefs.GetFloat(saveTime))
            PlayerPrefs.SetFloat(saveTime, currTime);
    }
}
