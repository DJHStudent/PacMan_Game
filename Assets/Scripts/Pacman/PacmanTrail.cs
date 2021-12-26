using UnityEngine;

public class PacmanTrail : MonoBehaviour
{
    public GameObject trail;
    public void startTrail()
    {
        Invoke("wait", .19f);
    }
    void wait()
    {
        trail.SetActive(true);
        Destroy(this);
    }
}
