using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/MusicsSO")]
public class MusicsSO : ScriptableObject
{
    public List<AudioClip> clips;

    public AudioClip GetRandomClip()
    {
        return clips[Random.Range(0,clips.Count - 1)];
    }
}
