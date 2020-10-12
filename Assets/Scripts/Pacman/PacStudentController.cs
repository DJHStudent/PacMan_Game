using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class PacStudentController : MonoBehaviour
{
    Tween tween;
    float duration = .9f;
    Animator animator;

    char lastInput = 'D', currentInput = 'D';
    void Start()
    {
        animator = GetComponent<Animator>();
        getNextPos();
    }

    // Update is called once per frame
    void Update()
    {
        determineKey();
        if (tween != null)
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), tween.EndPos) < 0.05f)
                getNextPos();
            else
                changePos();
        }
    }

    void determineKey()
    {
        if (Input.GetKeyDown(KeyCode.A))
            lastInput = 'A';
        if (Input.GetKeyDown(KeyCode.W))
            lastInput = 'W';
        if (Input.GetKeyDown(KeyCode.S))
            lastInput = 'S';
        if (Input.GetKeyDown(KeyCode.D))
            lastInput = 'D';
    }

    void getNextPos()
    {
        if (tween != null)
            transform.position = tween.EndPos;
        int startI = Mathf.RoundToInt(transform.position.x), startJ = Mathf.RoundToInt(transform.position.y);
        Vector2 nextPos = setNextPos(startI, startJ, lastInput);
        if (!Physics2D.Raycast(transform.position, nextPos - new Vector2(transform.position.x, transform.position.y), 1))
            currentInput = lastInput;
        else
            nextPos = setNextPos(startI, startJ, currentInput);
        if(!Physics2D.Raycast(transform.position, nextPos - new Vector2(transform.position.x, transform.position.y), 1))
            tween = new Tween(transform.position, nextPos, Time.time, duration);
    }

    Vector2 setNextPos(int startI, int startJ, char input)
    {
        switch (input)
        {
            case 'A': return new Vector2(startI - 1, startJ);
            case 'S': return new Vector2(startI, startJ - 1);
            case 'W': return new Vector2(startI, startJ + 1);
            default: return new Vector2(startI + 1, startJ);
        }
    }

    void changePos()
    {
        float timeFraction = (Time.time - tween.StartTime) / tween.Duration;
        transform.position = Vector2.Lerp(tween.StartPos, tween.EndPos, timeFraction);
    }
}
