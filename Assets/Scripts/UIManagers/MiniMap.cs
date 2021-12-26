using UnityEngine;

public class MiniMap : MonoBehaviour
{
    void Start()
    {
        int mapWidth = GameManager.randomMap.width, mapHeight = GameManager.randomMap.height;
        float halfwayX = ((float)mapWidth / 2) - .5f;
        float halfwayY = ((float)mapHeight / 2) - .5f;
        transform.position = new Vector3(halfwayX, halfwayY, -10);
        if (mapWidth > mapHeight)
            GetComponent<Camera>().orthographicSize = (float)mapWidth / 2;
        else
            GetComponent<Camera>().orthographicSize = (float)mapHeight / 2;
    }

    void displayMinimap()
    {
        if (Input.GetKeyDown(KeyCode.M))
            GameManager.levelUIManager.displayMiniMap();
    }

    void Update()
    {
        displayMinimap();
    }
}
