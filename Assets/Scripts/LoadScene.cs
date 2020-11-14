using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
        GameObject.Find("InnovSceneManager").GetComponent<InnovationSettings>().setValues();
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
