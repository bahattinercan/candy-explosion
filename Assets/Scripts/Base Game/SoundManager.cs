using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public AudioSource[] destroyNoise;
    public AudioSource backgroundMusic;

    public static SoundManager Instance { get => instance; }

    private void Awake()
    {
        instance = this;        
    }

    private void Start()
    {
        backgroundMusic.clip = Resources.Load<MusicsSO>(typeof(MusicsSO).Name).GetRandomClip();
        if (PlayerPrefs.HasKey("sound"))
        {
            if (PlayerPrefs.GetInt("sound") == 0)
            {
                backgroundMusic.volume = 0;
                backgroundMusic.Play();
            }
            else
            {
                backgroundMusic.Play();
                backgroundMusic.volume = 1;
            }
        }
        else
        {
            backgroundMusic.Play();
            backgroundMusic.volume = 1;
        }
    }

    public void adjustVolume()
    {
        if (PlayerPrefs.HasKey("sound"))
        {
            if (PlayerPrefs.GetInt("sound") == 0)
            {
                backgroundMusic.volume = 0;
            }
            else
            {
                backgroundMusic.volume = 1;
            }
        }
    }

    public void PlayRandomDestroyNoise()
    {
        if (PlayerPrefs.HasKey("sound"))
        {
            if (PlayerPrefs.GetInt("sound") == 1)
            {
                // Choose a random number
                int clipToPlay = Random.Range(0, destroyNoise.Length);
                destroyNoise[clipToPlay].Play();
            }
        }
        else
        {
            // Choose a random number
            int clipToPlay = Random.Range(0, destroyNoise.Length);
            destroyNoise[clipToPlay].Play();
        }
    }
}