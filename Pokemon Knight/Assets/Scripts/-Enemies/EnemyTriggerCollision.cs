using UnityEngine;

public class EnemyTriggerCollision : MonoBehaviour
{
    [SerializeField] private Enemy parentScript;
    // Start is called before the first frame update
    void Start()
    {
        if (parentScript == null)
        {
            Debug.LogError("parent Script is NOT SERIALIZED",this.gameObject);
            this.enabled = false;
        }
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))    
        {
            parentScript.PlayerCollision();
        }
    }
}
