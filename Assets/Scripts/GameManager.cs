using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static PacStudentController pacStudentController;
    public static Level1UIManager level1UIManager;
    public static AudioManager audioManager;
    public static LevelGenerator levelGenerator;
    public static RandomSizeGenerator randomMaze;
    public static SaveManager saveManager;
    public static GhostController ghost1;
    public static GhostController ghost2;
    public static GhostController ghost3;
    public static GhostController ghost4;

    public static int activeScene;
    public enum ActiveScene { recreation = 1, innovation};
    void Awake()
    {
        activeScene = SceneManager.GetActiveScene().buildIndex;
        pacStudentController = GameObject.FindGameObjectWithTag("Player").GetComponent<PacStudentController>();
        level1UIManager = GetComponent<Level1UIManager>();
        audioManager = GetComponent<AudioManager>();
        if (activeScene == (int)ActiveScene.recreation)
        {
            levelGenerator = GetComponent<LevelGenerator>();
            pacStudentController.leftTeloPoint = -14;
            pacStudentController.rightTeloPoint = 13;
        }
        else if (activeScene == (int)ActiveScene.innovation)
        {
            randomMaze = GetComponent<RandomSizeGenerator>();
            pacStudentController.setStartPos();

            pacStudentController.leftTeloPoint = 0;
            pacStudentController.rightTeloPoint = randomMaze.width - 1;
        }
        saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        ghost1 = GameObject.Find("BlueGhost").GetComponent<GhostController>();
        ghost2 = GameObject.Find("OrangeGhost").GetComponent<GhostController>();
        ghost3 = GameObject.Find("PinkGhost").GetComponent<GhostController>();
        ghost4 = GameObject.Find("RedGhost").GetComponent<GhostController>();
        audioManager.initialize();
    }
}
