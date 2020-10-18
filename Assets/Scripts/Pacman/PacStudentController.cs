using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class PacStudentController : MonoBehaviour
{

    //issue if colliding with wall and move away in same frame cancels out the audio for wall collide
    Tween tween;
    const float duration = .7f;//.9f;
    Animator animator;
    AudioSource pacAudio;
    public AudioClip eatPellet, pacWalk, wallCollide;
    public LayerMask ignorePellet;
    public ParticleSystem wallPartical, deathPartical;
    [SerializeField]
    Vector2 wallHitPoint, startPos;

    char lastInput = 'D', currentInput = 'D';
    void Start()
    {
        animator = GetComponent<Animator>();
        pacAudio = GetComponent<AudioSource>();
        animator.speed = 0;
        startPos = transform.position;
     //   getNextPos();
    }

    private void OnTriggerEnter2D(Collider2D other) //all collisions pacStudent has with pellets and ghosts
    {
       // if (Time.timeScale == 1) //only check collisions after the game start timer has finished and the game isn't paused anymore
        {
            if (other.CompareTag("Pellet"))
            {
                hitPellet();
                GameManager.pacStudentStats.addScore(10, other.gameObject);
            }
            if (other.CompareTag("Cherry"))
            {
                GameManager.pacStudentStats.addScore(100, other.gameObject);
            }
            else if (other.CompareTag("PowerPellet"))//if collide with pellet play the eatPellet audio and add score etc...
            {
                hitPellet();
                GameManager.ghost1.scared();
                GameManager.ghost2.scared();
                GameManager.ghost3.scared();
                GameManager.ghost4.scared();
                GameManager.pacStudentStats.startScared();
                Destroy(other.gameObject);
            }
            else if (other.CompareTag("Ghost"))
            {
                AllGhosts ghostController = other.gameObject.GetComponent<AllGhosts>();
                Debug.Log(other.gameObject.name);
                if (ghostController.currState == (int)AllGhosts.CurrState.normal) //all code works execpt when pacman dies
                {
                    GameManager.pacStudentStats.removeLife();
                    deathPartical.Play();
                    deathPartical.transform.position = transform.position;
                    transform.position = startPos;
                    animator.speed = 0;
                    pacAudio.Stop();
                    tween = null;
                }
                else if (ghostController.currState == (int)AllGhosts.CurrState.scared || ghostController.currState == (int)AllGhosts.CurrState.recovery)
                {
                    GameManager.pacStudentStats.addScore(300, null);
                    other.GetComponent<AllGhosts>().dead();
                }
            }
        }
    }

    void hitPellet()
    {
        CancelInvoke();
        pacAudio.loop = false;
        pacAudio.clip = eatPellet;
        pacAudio.Play();
        Invoke("resetAudio", .5f); //after audio clip finished play the pacWalk audio again
    }

    void resetAudio()//set aduio back to pacWalk
    {
        if (animator.speed != 0)
        {
            pacAudio.loop = true;
            pacAudio.clip = pacWalk;
            pacAudio.Play();
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
                    pacAudio.volume = 0.15f;
                    pacAudio.loop = true;
                    pacAudio.Play();
                    Invoke("resetAudio", pacAudio.clip.length);
                }
            }
            else if (animator.speed == 1) //only do if in last frame was moving //if nextPos is a wall then stop the player, change audio and play wall particals
            {
                wallPartical.transform.position = wallHitPoint;
                wallPartical.GetComponent<ParticleSystem>().Play();
                animator.speed = 0;
                pacAudio.clip = wallCollide;
                pacAudio.volume = 1f;
                pacAudio.loop = false;
                pacAudio.Play();
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
        float timeFraction = (Time.time - tween.StartTime) / tween.Duration;
        transform.position = Vector2.Lerp(tween.StartPos, tween.EndPos, timeFraction);
    }
}
