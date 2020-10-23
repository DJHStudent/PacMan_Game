using UnityEngine;
using UnityEngine.UI;

public class StartUIManager : MonoBehaviour
{
    public Text timeTxt, scoreTxt;
    void Start()
    {
        GameObject.Find("SaveManager").GetComponent<SaveManager>().loadSave();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
