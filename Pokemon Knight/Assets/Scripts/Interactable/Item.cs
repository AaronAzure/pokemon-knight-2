using UnityEngine;
// using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Item : MonoBehaviour
{
    private  PlayerControls player;
    public string itemName;
    private bool once;
    
   
    private void Start() 
    {
        if (PlayerPrefsElite.VerifyArray("itemsObtained" + PlayerPrefsElite.GetInt("gameNumber")))
        {
            string[] itemsObtained = PlayerPrefsElite.GetStringArray("itemsObtained" + PlayerPrefsElite.GetInt("gameNumber"));
            var set = new HashSet<string>(itemsObtained);
            if (set.Contains(itemName))
                Destroy(this.gameObject);
        }
    } 

    public IEnumerator PickupItem()
    {
        if (!once)
        {
            once = true;
            yield return new WaitForSeconds(0.33f);
            
            List<string> temp = new List<string>( 
                PlayerPrefsElite.GetStringArray("itemsObtained" + PlayerPrefsElite.GetInt("gameNumber")) 
            );
            temp.Add(itemName);
            PlayerPrefsElite.SetStringArray("itemsObtained" + PlayerPrefsElite.GetInt("gameNumber"), temp.ToArray());
            player.CheckObtainedItems();
            
            Destroy(this.gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))    
        {
            if (player == null)
                player = other.GetComponent<PlayerControls>();

            player.currentItem = this;
        }
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))    
        {
            if (player == null)
                player = other.GetComponent<PlayerControls>();

            player.currentItem = null;
        }
    }
}
