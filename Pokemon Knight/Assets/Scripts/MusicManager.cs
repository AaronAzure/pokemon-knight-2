using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource titleMusic, forestMusic, bossIntroMusic, bossOutroMusic;
    public AudioSource currentMusic;
    public AudioSource previousMusic;

    void Start() 
    {
        if (titleMusic != null)
        {
            titleMusic.Play();
            currentMusic = titleMusic;
        }
    }

    public void BackToTitle()
    {
        StartCoroutine( TransitionMusic(titleMusic) );
    }
    public IEnumerator TransitionMusic(AudioSource startMusic, bool rememberLastMusic=false)
    {
        if (rememberLastMusic)
            previousMusic = currentMusic;
        int times = 30;
        float fraction = currentMusic.volume / times;
        for (int i=0 ; i<times ; i++)
        {
            yield return new WaitForEndOfFrame();
            currentMusic.volume -= fraction;
        }
        yield return new WaitForEndOfFrame();
        currentMusic.Stop();

        startMusic.volume = 0.25f;
        startMusic.Play();
        currentMusic = startMusic;
        startMusic.loop = true;
    }
        
    // For dramatic silence
    public IEnumerator LowerMusic(AudioSource music)
    {
        int times = 30;
        float fraction = music.volume / times;
        for (int i=0 ; i<times ; i++)
        {
            yield return new WaitForEndOfFrame();
            music.volume -= fraction;
        }
        yield return new WaitForEndOfFrame();
        music.Stop();
    }
    public void StartMusic(AudioSource music)
    {
        music.volume = 0.25f;
        music.Play();
        currentMusic = music;
        music.loop = true;
    }
}
