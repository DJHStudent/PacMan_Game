using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class PacStudentController : MonoBehaviour
{
    Tween tween;
    const float duration = .7f;//.9f;
    public Animator animator;
    public LayerMask ignorePellet;
    public ParticleSystem wallPartical, deathPartical;
    Vector2 wallHitPoint, startPos;

    char lastInput = 'D', currentInput = 'D';
    void Start()
    {
        animator = GetComponent<Animator>();
        startPos = transform.position;
     //   getNextPos();
    }
    public void pause()
    {
        animator.speed = 0; //basically pause here
        GameManager.audioManager.pause();
    }
    private void OnTriggerEnter2D(Collider2D other) //all collisions pacStudent has with pellets and ghosts
    {
       // if (Time.timeScale == 1) //only check collisions after the game start timer has finished and the game isn't paused anymore
        {
            if (other.CompareTag("Pellet"))
            {
                GameManager.audioManager.hitPellet();
                GameManager.level1UIManager.statsManager.addScore(10, other.gameObject);
                GameManager.level1UIManager.statsManager.removePellet();
            }
            if (other.CompareTag("Cherry"))
            {
                GameManager.level1UIManager.statsManager.addScore(100, other.gameObject);
            }
            else if (other.CompareTag("PowerPellet"))//if collide with pellet play the eatPellet audio and add score etc...
            {
                GameManager.audioManager.hitPellet();
                GameManager.ghost1.scared();
                GameManager.ghost2.scared();
                GameManager.ghost3.scared();
                GameManager.ghost4.scared();
                GameManager.level1UIManager.statsManager.startScared();
                GameManager.level1UIManager.statsManager.removePellet();
                Destroy(other.gameObject);
            }
            else if (other.CompareTag("Ghost"))
            {
                GhostController ghostController = other.gameObject.GetComponent<GhostController>();
                if (ghostController.currState == (int)GhostController.CurrState.normal) //all code works execpt when pacman dies
                {
                    GameManager.level1UIManager.statsManager.removeLife();
                    deathPartical.Play();
                    deathPartical.transform.position = transform.position;
                    transform.position = startPos;
                    pause();
                    tween = null;
                }
                else if (ghostController.currState == (int)GhostController.CurrState.scared || ghostController.currState == (int)GhostController.CurrState.recovery)
                {
                    GameManager.level1UIManager.statsManager.addScore(300, null);
                    other.GetComponent<GhostController>().dead();
                }
            }
        }
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
        if (!GameManager.level1UIManager.statsManager.paused)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                lastInput = 'A';
                if (tween == null)
                    getNextPos();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                lastInput = 'W';
                if (tween == null)
                    getNextPos();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                lastInput = 'S';
                if (tween == null)
                    getNextPos();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                lastInput = 'D';
                if (tween == null)
                    getNextPos();
            }
        }
    }

    void getNextPos()//only do if first a valid key has been pressed
    {
            if (tween != null)
                transform.position = tween.EndPos; //ensures player at final position before moving to a new one
            teleport();
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
                GameManager.audioManager.noWall();
            }
        }
        else if (animator.speed == 1) //only do if in last frame was moving //if nextPos is a wall then stop the player, change audio and play wall particals
        {
            wallPartical.transform.position = wallHitPoint;
            wallPartical.GetComponent<ParticleSystem>().Play();
            pause();
            GameManager.audioManager.hitWall();
        }
    }

    bool nextPosWall(Vector2 nextPos) //checks if the next position the player is moving too is a wall
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, nextPos - new Vector2(transform.position.x, transform.position.y), 1, ignorePellet);
        if (hit)
           wallHitPoint = hit.point; //get the point that the collision occured with the wall
        return hit;
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

    void teleport() //when at portal pos move pacStudent to other side of map
    {
        if (transform.position.x <= -14)
        {
            Vector2 pos = transform.position;
            pos.x = 13;
            transform.position = pos;
        }   
        else if(transform.position.x >= 13)
        {
            Vector2 pos = transform.position;
            pos.x = -14;
            transform.position = pos;
        }
    }

    void changePos() //move the player to the specified location
    {
        if (!GameManager.level1UIManager.statsManager.paused)
        {
            float timeFraction = (Time.time - tween.StartTime) / tween.Duration;
            transform.position = Vector2.Lerp(tween.StartPos, tween.EndPos, timeFraction);
        }
    }
}
