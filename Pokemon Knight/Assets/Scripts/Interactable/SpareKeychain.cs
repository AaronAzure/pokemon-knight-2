using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpareKeychain : MonoBehaviour
{
    public PlayerControls player;
    [SerializeField] private string roomName;
    [Space] [SerializeField] private List<string> keychain;
    private bool once;
    // [SerializeField] private HashSet<string> kaychainSet;
   
    private void Start() 
    {
        roomName = SceneManager.GetActiveScene().name + " " + this.name;

        if (PlayerPrefsElite.VerifyArray("spareKeychain" + PlayerPrefsElite.GetInt("gameNumber")))
        {
            keychain = new List<string>( PlayerPrefsElite.GetStringArray("spareKeychain" 
                + PlayerPrefsElite.GetInt("gameNumber")) );
            HashSet<string> kaychainSet = new HashSet<string>(keychain);
            if (kaychainSet.Contains(""))
                kaychainSet.Remove("");
            if (kaychainSet.Contains(roomName))
                Destroy(this.gameObject);
        }
    } 

    public IEnumerator PickupSpareKeychain()
    {
        if (player != null && !once)
        {
            once = true;
            yield return new WaitForSeconds(0.33f);
            player.extraWeight++;
            player.PickupKeychainCo();
            
            // keychain.Add(roomName);
            List<string> temp = new List<string>(
                PlayerPrefsElite.GetStringArray("spareKeychain" + PlayerPrefsElite.GetInt("gameNumber"))
            );
            temp.Add(roomName);
            PlayerPrefsElite.SetStringArray("spareKeychain" + PlayerPrefsElite.GetInt("gameNumber"), temp.ToArray());

            Destroy(this.gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))    
        {
            if (player == null)
                player = other.GetComponent<PlayerControls>();

            player.currentKeychain = this;
        }
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))    
        {
            if (player == null)
                player = other.GetComponent<PlayerControls>();

            player.currentKeychain = null;
        }
    }
}
