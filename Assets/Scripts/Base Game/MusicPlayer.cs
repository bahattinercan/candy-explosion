using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = Resources.Load<MusicsSO>(typeof(MusicsSO).Name).GetRandomClip();
        audioSource.Play();
    }
}
