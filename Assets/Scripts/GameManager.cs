using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static PacStudentController pacStudentController;
    public static PacStudentStats pacStudentStats;
    public static DetermineSound setSound;
    public static Ghost1 ghost1;
    public static Ghost2 ghost2;
    public static Ghost3 ghost3;
    public static Ghost4 ghost4;
    void Awake()
    {
        pacStudentController = GameObject.FindGameObjectWithTag("Player").GetComponent<PacStudentController>();
        pacStudentStats = GetComponent<PacStudentStats>();
        setSound = GetComponent<DetermineSound>();
        ghost1 = GameObject.Find("BlueGhost").GetComponent<Ghost1>();
        ghost2 = GameObject.Find("OrangeGhost").GetComponent<Ghost2>();
        ghost3 = GameObject.Find("PinkGhost").GetComponent<Ghost3>();
        ghost4 = GameObject.Find("RedGhost").GetComponent<Ghost4>();
    }
}
