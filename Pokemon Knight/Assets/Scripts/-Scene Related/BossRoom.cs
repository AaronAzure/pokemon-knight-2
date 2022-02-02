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
        if (boss != null && !autoStartFight)
        {
            boss.mustDmgBeforeFight = true;    
            boss.bossRoom = this;
        }
        else if (boss == null)
        {
            Walls(false);
            this.enabled = false;
        }
    }


    public void Walls(bool active)
    {
        foreach (GameObject wall in walls)
            wall.SetActive(active);
        if (!active)
            cm.Priority = -100;
        else
            cm.Priority = 100;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (!once && autoStartFight && other.CompareTag("Player"))    
        {
            once = true;
            if (boss == null)
                return;
            boss.StartBossBattle();
            Walls(true);
        }
    }
}
