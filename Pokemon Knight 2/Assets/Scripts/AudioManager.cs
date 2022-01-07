using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    [SerializeField] private AudioSource rosaryIntro, rosaryOutro;
    [SerializeField] private Animator anim;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else 
            Destroy(this.gameObject);
    }

    public IEnumerator TransitionBossTrack()
    {
        anim.SetTrigger("changeSong");
        yield return new WaitForSeconds(0.5f);
        rosaryIntro.Stop();
        rosaryOutro.Play();
    }
}
