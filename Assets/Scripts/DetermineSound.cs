using UnityEngine;

public class DetermineSound : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip normalClip;
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        PlayAudio(normalClip);
    }
    void PlayAudio(AudioClip clip)
    {
        if(!audioSource.isPlaying)
        {
            audioSource.clip = normalClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
