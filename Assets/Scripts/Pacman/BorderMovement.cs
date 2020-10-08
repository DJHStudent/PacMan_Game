using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderMovement : MonoBehaviour
{
    private Tween tween;
    float duration = 20;
    public RectTransform[] positions;
    int currPos = 0;
    void Start()
    {
        tween = new Tween(transform.position, new Vector2(positions[currPos].position.x, positions[currPos].position.y), Time.time, duration);
    }

    // Update is called once per frame
    void Update()
    {
        move();
        if(Vector2.Distance(new Vector2(transform.position.x, transform.position.y), tween.EndPos) < .1f)
        {
            currPos++;
            tween = new Tween(transform.position, new Vector2(positions[currPos].position.x, positions[currPos].position.y), Time.time, duration);
            if (currPos >= 3)
                currPos = -1;

        }
    }

    private void move()
    {
        float timeFraction = (Time.time - tween.StartTime) / tween.Duration;
        transform.position = Vector2.Lerp(tween.StartPos, tween.EndPos, timeFraction);
    }
}
