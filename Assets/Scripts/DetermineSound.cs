using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetermineSound : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip normalClip;
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
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
