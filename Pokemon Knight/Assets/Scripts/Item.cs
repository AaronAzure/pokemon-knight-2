using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Item : MonoBehaviour
{
    private  PlayerControls player;
    public string itemName;

    // [SerializeField] private bool increaseMaxPokemonOut;
    [Space] [SerializeField] private Animator anim;
    
   
    private void Start() 
    {
        if (PlayerPrefsElite.VerifyArray("itemsObtained"))
        {
            string[] itemsObtained = PlayerPrefsElite.GetStringArray("itemsObtained");
            var set = new HashSet<string>(itemsObtained);
            if (set.Contains(itemName))
                Destroy(this.gameObject);
        }

        if (anim != null)
            anim.gameObject.SetActive(false);
    } 

    public void PickupItem()
    {
        anim.gameObject.transform.parent = null;
        anim.gameObject.SetActive(true);
        player.GainItem(itemName);
        Destroy(this.gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))    
        {
            if (player == null)
                player = other.GetComponent<PlayerControls>();

            player.currentItem = this;
            // if (player != null) 
            //     player.IncreaseMaxPokemonOut();
            // PlayerPrefsElite.SetBoolean("item1", true);
        }
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))    
        {
            if (player == null)
                player = other.GetComponent<PlayerControls>();

            player.currentItem = null;
            // PlayerPrefsElite.SetBoolean("item1", true);
            // Destroy(this.gameObject);
        }
    }
}
