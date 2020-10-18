using UnityEngine;

public class DetermineSound : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip normalClip, scaredClip;
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
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
}
