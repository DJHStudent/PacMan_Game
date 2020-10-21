using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static PacStudentController pacStudentController;
    public static Level1UIManager level1UIManager;
    public static AudioManager audioManager;
    public static LevelGenerator levelGenerator;
    public static SaveManager saveManager;
    public static GhostController ghost1;
    public static GhostController ghost2;
    public static GhostController ghost3;
    public static GhostController ghost4;
    void Awake()
    {
        pacStudentController = GameObject.FindGameObjectWithTag("Player").GetComponent<PacStudentController>();
        Debug.Log(pacStudentController);
        level1UIManager = GetComponent<Level1UIManager>();
        audioManager = GetComponent<AudioManager>();
        levelGenerator = GetComponent<LevelGenerator>();
        saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        ghost1 = GameObject.Find("BlueGhost").GetComponent<GhostController>();
        ghost2 = GameObject.Find("OrangeGhost").GetComponent<GhostController>();
        ghost3 = GameObject.Find("PinkGhost").GetComponent<GhostController>();
        ghost4 = GameObject.Find("RedGhost").GetComponent<GhostController>();
        audioManager.initialize();
    }
}
