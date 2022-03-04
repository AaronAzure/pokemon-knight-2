using UnityEngine;
using System.Collections;

public class FlashUltimate : MonoBehaviour
{
    private float lastTime;
    private ParticleSystem ps;
    public bool slowTime;

    private void Awake ()
    {
        ps = GetComponent<ParticleSystem> ();
    }

    void Start ()
    {
        lastTime = Time.realtimeSinceStartup;
        if (slowTime)
            StartCoroutine( SlowTime() );
    }

    void Update()
    {
        float deltaTime = Time.realtimeSinceStartup - lastTime;
        ps.Simulate (deltaTime, true, false);
        lastTime = Time.realtimeSinceStartup;
    }

    IEnumerator SlowTime()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.3f);
        // while (Time.timeScale < 1f)
        // {
        //     Time.timeScale += 0.01f;
        //     yield return null;
        // }
        Time.timeScale = 1f;
    }
}