using UnityEngine;

public class PacmanTrail : MonoBehaviour
{
    public GameObject trail;
    void Start()
    {
        Invoke("wait", .19f);
    }

    void wait()
    {
        trail.SetActive(true);
    }
}
