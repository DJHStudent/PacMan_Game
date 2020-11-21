using UnityEngine;

public class CameraMove : MonoBehaviour
{
    Transform pacStudent;
    float minX, maxX, halfwayX;
    float minY, maxY, halfwayY;
    void Start()
    {
        pacStudent = GameManager.pacStudentController.gameObject.transform;
        minX = 20.5f;
        maxX = GameManager.randomMap.width - 21.5f;
        if (minX >= maxX)
            halfwayX = ((float)GameManager.randomMap.width / 2) - .5f;
        minY = 15;
        maxY = GameManager.randomMap.height - 16f;
        if(minY >= maxY)
            halfwayY = ((float)GameManager.randomMap.height / 2) - .5f;
    }

    // Update is called once per frame
    void Update()
    {
        determineNewPos();
    }

    void determineNewPos()
    {
        float xPos;
        float yPos;
        if (minX < maxX)
            xPos = Mathf.Clamp(pacStudent.position.x, minX, maxX);
        else
            xPos = halfwayX;
        if (minY < maxY)
            yPos = Mathf.Clamp(pacStudent.position.y, minY, maxY);
        else
            yPos = halfwayY;
        transform.position = new Vector3(xPos, yPos, -10);
    }
}
