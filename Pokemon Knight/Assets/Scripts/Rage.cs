using UnityEngine;

public class Rage : MonoBehaviour
{
    [SerializeField] private string musicName="rosary";


    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))    
        {
            PlayerControls pc = other.GetComponent<PlayerControls>();
            pc.BossRage(musicName);
        }
    }
}
