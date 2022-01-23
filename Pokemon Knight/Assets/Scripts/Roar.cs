using UnityEngine;

public class Roar : MonoBehaviour
{
    [SerializeField] private string musicName="rosary";


    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))    
        {
            PlayerControls pc = other.GetComponent<PlayerControls>();
            pc.EngagedBossRoar(musicName);
        }
    }
}
