using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("load", 0.01f);
    }

    // Update is called once per frame
    void load()
    {
        SceneManager.LoadSceneAsync(2);
    }
}
