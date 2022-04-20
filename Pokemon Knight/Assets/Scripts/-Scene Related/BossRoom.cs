using System.Collections;
using UnityEngine;
using Cinemachine;

public class BossRoom : MonoBehaviour
{
    [SerializeField] private Enemy boss;
    private bool once;
    [Tooltip("true = roar on enter room\n false = attacked for roar")] [SerializeField] private bool autoStartFight=true;
    [SerializeField] private GameObject[] walls;
    [SerializeField] private CinemachineVirtualCamera cm;

    private void Start() 
    {
        if (boss != null)
        {
            boss.bossRoom = this;
            if (!autoStartFight)
                boss.mustDmgBeforeFight = true;    
            StartCoroutine( DoubleCheck() );
        }
        // BOSS ALREADY DEFEATED
        else if (boss == null)
        {
            Walls(false);
            this.enabled = false;
        }
    }

    IEnumerator DoubleCheck()
    {
        yield return new WaitForSeconds(0.5f);
        if (boss == null)
        {
            Walls(false);
            this.enabled = false;
        }
    }


    public void Walls(bool active)
    {
        // BOSS FIGHT COMMENCES
        if (active)
            once = true;

        foreach (GameObject wall in walls)
            wall.SetActive(active);
        ChangeCameraPriority(active);
    }

    private void ChangeCameraPriority(bool highPriority)
    {
        if (!highPriority)
            cm.Priority = -100;
        else
            cm.Priority = 100;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if ( boss != null )
        {
            if (!once && autoStartFight && other.CompareTag("Player"))    
            {
                once = true;
                if (boss == null)
                    return;
                boss.StartBossBattle();
                Walls(true);
            }
            else if (!once && !autoStartFight && other.CompareTag("Player"))
            {
                ChangeCameraPriority(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (!once && !autoStartFight && boss != null && other.CompareTag("Player"))
        {
            ChangeCameraPriority(false);
        }
    }
}
