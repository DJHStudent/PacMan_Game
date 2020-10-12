using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryController : MonoBehaviour
{
    Tween tween;
    const float duration = 25f;
    bool moving = false;
    Vector2 startPos, endPos;
    void Start()
    {
        InvokeRepeating("startMovement", 1, 30); //do method every 30 seconds
        Camera camera = Camera.main;

        //float height = 2 * camera.orthographicSize;
        //float width = height * camera.aspect;
    }

    void Update()
    {
        if (moving == true) //if currently moving across the screen continue that movement and stop it when it reaches endPos
        {
            changePos();
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), tween.EndPos) < 0.05f)
            {
                moving = false;
                gameObject.SetActive(false);
            }
        }
    }

    void startMovement() //enable the cherry to move across the screen
    {
        gameObject.SetActive(true);
        startPos = Camera.main.ViewportToWorldPoint(new Vector2(1.1f, .5f)); //set the start and end points to be just outside the camera
        endPos = Camera.main.ViewportToWorldPoint(new Vector2(-0.1f, .5f));
        tween = new Tween(startPos, endPos, Time.time, duration);
        moving = true; //tell update to check if finsihed movemenet or not
    }
    void changePos() //the actual movement of the cherry across the screen
    {
        float timeFraction = (Time.time - tween.StartTime) / tween.Duration;
        transform.position = Vector2.Lerp(tween.StartPos, tween.EndPos, timeFraction);
    }
}
