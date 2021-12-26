using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource backgroundAudio, pacStudentAudio;
    public AudioClip normalClip, scaredClip, deadClip;
    public AudioClip eatPellet, pacWalk, wallCollide;
    public void initialize()
    {
        backgroundAudio = gameObject.GetComponent<AudioSource>();
        pacStudentAudio = GameManager.pacStudentController.gameObject.GetComponent<AudioSource>();
    }
    //pacStudent audio
    public void pause()
    {
        pacStudentAudio.Stop();
    }
    public void hitWall()
    {
        pacStudentAudio.clip = wallCollide;
        pacStudentAudio.volume = 1f;
        pacStudentAudio.loop = false;
        pacStudentAudio.Play();
    }
    public void hitPellet()
    {
        CancelInvoke();
        pacStudentAudio.loop = false;
        pacStudentAudio.clip = eatPellet;
        pacStudentAudio.Play();
        Invoke("resetAudio", .5f); //after audio clip finished play the pacWalk audio again
    }
    public void noWall() //do if after hitting wall their is no wall anymore
    {
        pacStudentAudio.volume = 0.15f;
        //pacStudentAudio.loop = true;
        Invoke("resetAudio", pacStudentAudio.clip.length); //after audio clip finished play the pacWalk audio again

    }
    void resetAudio()//set aduio back to pacWalk
    {
        if (GameManager.pacStudentController.animator.speed != 0)//baciscally only do if paused == false;
        {
            pacStudentAudio.loop = true;
            pacStudentAudio.clip = pacWalk;
            pacStudentAudio.Play();
        }
    }


    //background audio
    public void deadState()
    {
        if (!isDeadState())
        {
            backgroundAudio.clip = deadClip;
            backgroundAudio.Play();
        }
    }
    public void scaredState()
    {
        backgroundAudio.clip = scaredClip;
        backgroundAudio.Play();
    }
    public void normalState()
    {
        backgroundAudio.clip = normalClip;
        backgroundAudio.Play();
    }

    public bool isDeadState()
    {
        return backgroundAudio.clip == deadClip;
    }
}
