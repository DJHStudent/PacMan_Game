using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class PacStudentController : MonoBehaviour
{
    Tween tween;
    const float duration = .9f;
    Animator animator;
    AudioSource pacAudio;
    public AudioClip eatPellet, pacWalk;
    public LayerMask ignorePellet;

    char lastInput = 'D', currentInput = 'D';
    void Start()
    {
        animator = GetComponent<Animator>();
        pacAudio = GetComponent<AudioSource>();
        getNextPos();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pellet") || other.CompareTag("PowerPellet"))//if collide with pellet play the eatPellet audio
        {
            CancelInvoke();
            pacAudio.loop = false;
            pacAudio.clip = eatPellet;
            pacAudio.Play();
            Invoke("resetAudio", .5f); //after audio clip finished play the pacWalk audio again
        }
    }

    void resetAudio()//set aduio back to pacWalk
    {
        pacAudio.loop = true;
        pacAudio.clip = pacWalk;
        pacAudio.Play();
    }
    // Update is called once per frame
    void Update()
    {
        determineKey();
        if (tween != null) //if close enough to the end postion determine the next one else continue to the end position
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), tween.EndPos) < 0.05f)
                getNextPos();
            else
                changePos();
        }
    }

    void determineKey()//get and save the last key that the player has pressed
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
            transform.position = tween.EndPos; //ensures player at final position before moving to a new one
        int startI = Mathf.RoundToInt(transform.position.x), startJ = Mathf.RoundToInt(transform.position.y); //get player current position as an int
        Vector2 nextPos = setNextPos(startI, startJ, lastInput); 
        if (lastInput != currentInput) //only nessesary do if the direction is actually changing
        {
            if (!nextPosWall(nextPos)) //allow chaning direction if the nextPos is not a wall
            {                
                animator.SetTrigger("" + lastInput);
                currentInput = lastInput;
            }
            else //continue to move in same direction otherwise
                nextPos = setNextPos(startI, startJ, currentInput);
        }
        if (!nextPosWall(nextPos)) //if no wall found at nextPos allow movement to that position(done if the direction couldn't change and checks if current direction wall)
        {
            tween = new Tween(transform.position, nextPos, Time.time, duration);
            if (animator.speed == 0) //continue if it was previouly stopped
            {
                animator.speed = 1;
                pacAudio.Play();
            }
        }
        else //if nextPos is a wall then stop the player
        {
            animator.speed = 0;
            pacAudio.Pause();
        }
    }

    bool nextPosWall(Vector2 nextPos) //checks if the next position the player is moving too is a wall or not
    {
        return Physics2D.Raycast(transform.position, nextPos - new Vector2(transform.position.x, transform.position.y), 1, ignorePellet);
    }

    Vector2 setNextPos(int startI, int startJ, char input) //determine the next position the player will be moved too based on the input key given
    {
        switch (input)
        {
            case 'A': return new Vector2(startI - 1, startJ);
            case 'S': return new Vector2(startI, startJ - 1);
            case 'W': return new Vector2(startI, startJ + 1);
            default: return new Vector2(startI + 1, startJ);
        }
    }

    void changePos() //move the player to the specified location
    {
        float timeFraction = (Time.time - tween.StartTime) / tween.Duration;
        transform.position = Vector2.Lerp(tween.StartPos, tween.EndPos, timeFraction);
    }
}
