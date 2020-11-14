using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClick : MonoBehaviour
{
    public void changeScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void leaveScene(int scene)
    {
        Ghost4Waypoints.currDir = Vector2.zero;
        Ghost4Waypoints.straightWall = null;
        SceneManager.LoadScene(scene);
    }
    public void inovvGen(int scene)
    {
        DontDestroyOnLoad(GameObject.Find("InnovSceneManager"));
        GameObject.Find("InnovSceneManager").GetComponent<InnovationSettings>().setValues();
        SceneManager.LoadScene(scene);
    }
}
