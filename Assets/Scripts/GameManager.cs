using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static LevelGenerator levelGenerator;
    void Awake()
    {
        levelGenerator = GetComponent<LevelGenerator>();
    }
}
