using UnityEngine;

public class DetermineSound : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip normalClip, scaredClip, deadClip;
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void deadState()
    {
        audioSource.clip = deadClip;
        audioSource.Play();
    }
    public void scaredState()
    {
        audioSource.clip = scaredClip;
        audioSource.Play();
    }
    public void normalState()
    {
        audioSource.clip = normalClip;
        audioSource.Play();
    }

    public bool isDeadState()
    {
        return audioSource.clip == deadClip;
    }
}
