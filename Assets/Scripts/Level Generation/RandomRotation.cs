﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
