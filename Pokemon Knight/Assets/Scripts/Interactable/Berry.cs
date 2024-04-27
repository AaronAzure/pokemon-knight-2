using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Berry : MonoBehaviour
{
    private PlayerControls player;
    [SerializeField] private string roomName;
    [Space] [SerializeField] private List<string> berries;
    [SerializeField] private HashSet<string> berriesSet;
    private bool once;
   
    private void Start() 
    {
        roomName = SceneManager.GetActiveScene().name + " " + this.name;

        if (PlayerPrefsElite.VerifyArray("berriesCollected" + PlayerPrefsElite.GetInt("gameNumber")))
        {
            berries = new List<string>(
                PlayerPrefsElite.GetStringArray("berriesCollected" + PlayerPrefsElite.GetInt("gameNumber"))
            );
            berriesSet = new HashSet<string>(berries);
            if (berriesSet.Contains(""))
                berriesSet.Remove("");
            if (berriesSet.Contains(roomName))
                Destroy(this.gameObject);
        }
    } 

    public IEnumerator PickupBerry()
    {
        if (player != null && !once)
        {
            once = true;
            yield return new WaitForSeconds(0.33f);

            player.nBerries++;
            player.PickupBerryCo();
            berries.Add(roomName);
            PlayerPrefsElite.SetStringArray("berriesCollected" + PlayerPrefsElite.GetInt("gameNumber"), berries.ToArray());

            Destroy(this.gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))    
        {
            if (player == null)
                player = other.GetComponent<PlayerControls>();

            player.currentBerry = this;
        }
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))    
        {
            if (player == null)
                player = other.GetComponent<PlayerControls>();

            player.currentBerry = null;
        }
    }
}
