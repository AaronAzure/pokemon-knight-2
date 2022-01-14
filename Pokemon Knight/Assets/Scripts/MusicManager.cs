using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource titleMusic, forestMusic, bossIntroMusic, bossOutroMusic;

    public IEnumerator TransitionMusic(AudioSource stopMusic, AudioSource startMusic)
    {
        int times = 30;
        float fraction = stopMusic.volume / times;
        for (int i=0 ; i<times ; i++)
        {
            yield return new WaitForEndOfFrame();
            stopMusic.volume -= fraction;
        }
        yield return new WaitForEndOfFrame();
        stopMusic.Stop();
        startMusic.volume = 0.25f;
        startMusic.Play();
        startMusic.loop = true;
    }
        
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
        music.loop = true;
    }
}
