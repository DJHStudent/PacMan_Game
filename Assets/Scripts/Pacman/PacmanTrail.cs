using UnityEngine;

public class PacmanTrail : MonoBehaviour
{
    public GameObject trail;
    void Start()
    {
    }
    private void Update()
    {
        if (!GameManager.levelUIManager.statsManager.paused)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.S))
            {
                Invoke("wait", .19f);
            }
        }
    }
    void wait()
    {
        trail.SetActive(true);
        Destroy(this);
    }
}
