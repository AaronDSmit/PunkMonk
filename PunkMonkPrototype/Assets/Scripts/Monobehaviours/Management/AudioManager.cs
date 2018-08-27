using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] music;

    private AudioSource audioSource;

    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (music.Length > 0)
        {
            audioSource.clip = music[0];
        }
        audioSource.Play();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
