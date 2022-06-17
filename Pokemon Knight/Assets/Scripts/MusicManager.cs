using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource 
        titleMusic, forestMusic, swampMusic, mansionMusic, bossIntroMusic, bossOutroMusic, 
        accoladeIntroMusic, accoladeOutroMusic, 
        bloomIntroMusic, bloomOutroMusic, 
        abyssMusic,  
		hordeMusic;
    [Space][Space] public AudioSource currentMusic;
	public bool playLastMusic;
    private Dictionary<AudioSource, float> origVolumes = new Dictionary<AudioSource, float>();

    void Start() 
    {
        if (titleMusic != null)
        {
            titleMusic.Play();
            currentMusic = titleMusic;
        }
        origVolumes.Add(titleMusic, titleMusic.volume);
        origVolumes.Add(forestMusic, forestMusic.volume);
        origVolumes.Add(swampMusic, swampMusic.volume);
        origVolumes.Add(mansionMusic, mansionMusic.volume);
        origVolumes.Add(bossIntroMusic, bossIntroMusic.volume);
        origVolumes.Add(bossOutroMusic, bossOutroMusic.volume);
        origVolumes.Add(accoladeIntroMusic, accoladeIntroMusic.volume);
        origVolumes.Add(accoladeOutroMusic, accoladeOutroMusic.volume);
        origVolumes.Add(hordeMusic, hordeMusic.volume);
        origVolumes.Add(bloomIntroMusic, bloomIntroMusic.volume);
        origVolumes.Add(bloomOutroMusic, bloomOutroMusic.volume);
        origVolumes.Add(abyssMusic, abyssMusic.volume);
    }

    public void BackToTitle()
    {
        StartCoroutine( TransitionMusic(titleMusic) );
    }
    public IEnumerator TransitionMusic(AudioSource nextMusic)
    {
        int times = 20;
        float fraction = currentMusic.volume / times;
        for (int i=0 ; i<times ; i++)
        {
            yield return new WaitForSecondsRealtime(0.05f);
            currentMusic.volume -= fraction;
        }
        yield return null;
        currentMusic.Stop();

        if (nextMusic == null)
        {
            if (origVolumes.ContainsKey(currentMusic))
                currentMusic.volume = origVolumes[currentMusic];
            else
                currentMusic.volume = 0.25f;
            currentMusic.Play();
            currentMusic.loop = true;
        }
        else
        {
            if (origVolumes.ContainsKey(nextMusic))
                nextMusic.volume = origVolumes[nextMusic];
            else
                nextMusic.volume = 0.25f;
            nextMusic.Play();
            currentMusic = nextMusic;
            nextMusic.loop = true;
        }
    }
    public IEnumerator PlayPrevMusic(AudioSource nextMusic)
    {
        int times = 20;
        float fraction = currentMusic.volume / times;
        for (int i=0 ; i<times ; i++)
        {
            yield return new WaitForSecondsRealtime(0.05f);
            currentMusic.volume -= fraction;
        }
        yield return null;
        currentMusic.Stop();

        if (nextMusic == null)
        {
            if (origVolumes.ContainsKey(currentMusic))
                currentMusic.volume = origVolumes[currentMusic];
            else
                currentMusic.volume = 0.25f;
            currentMusic.Play();
            currentMusic.loop = true;
        }
        else
        {
            if (origVolumes.ContainsKey(nextMusic))
                nextMusic.volume = origVolumes[nextMusic];
            else
                nextMusic.volume = 0.25f;
            nextMusic.Play();
            currentMusic = nextMusic;
            nextMusic.loop = true;
        }
    }
        
    // For dramatic silence
    public IEnumerator StopMusic(AudioSource music)
    {
        int times = 30;
        float fraction = music.volume / times;
        for (int i=0 ; i<times ; i++)
        {
            yield return null;
            music.volume -= fraction;
        }
        yield return null;
        music.Stop();
    }
    public IEnumerator LowerMusic(AudioSource music, float percent)
    {
        int times = 30;
        float fraction = (music.volume  * percent) / times;
        for (int i=0 ; i<times ; i++)
        {
            yield return null;
            music.volume -= fraction;
        }
    }
    public IEnumerator RaiseMusic(AudioSource music, float percent)
    {
        int times = 30;
        float fraction = (music.volume  * times) / percent;
        for (int i=0 ; i<times ; i++)
        {
            yield return null;
            music.volume += fraction;
        }
    }
    public void StartMusic(AudioSource music)
    {
        if (origVolumes.ContainsKey(music))
            music.volume = origVolumes[music];
        else
            music.volume = 0.25f;
        music.Play();
        currentMusic = music;
        music.loop = true;
    }
}
