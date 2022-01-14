using UnityEngine;
using Cinemachine;

public class BossRoom : MonoBehaviour
{
    [SerializeField] private Enemy boss;
    private bool once;
    [SerializeField] private GameObject[] walls;
    [SerializeField] private CinemachineVirtualCamera cm;


    public void Walls(bool active)
    {
        foreach (GameObject wall in walls)
            wall.SetActive(active);
        if (!active)
            cm.Priority = -100;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (!once && other.CompareTag("Player"))    
        {
            cm.Priority = 100;
            once = true;
            boss.StartBossBattle();
            boss.bossRoom = this;
            Walls(true);
        }
    }
}
