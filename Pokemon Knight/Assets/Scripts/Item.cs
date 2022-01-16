using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private bool increaseMaxPokemonOut;
   
    private void Start() 
    {
        if (PlayerPrefsElite.VerifyBoolean("item1") && PlayerPrefsElite.GetBoolean("item1"))
            Destroy(this.gameObject);
    } 
    
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))    
        {
            PlayerControls player = other.GetComponent<PlayerControls>();
            if (player != null) 
                player.IncreaseMaxPokemonOut();
            PlayerPrefsElite.SetBoolean("item1", true);
            Destroy(this.gameObject);
        }
    }
}
