using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Berry : MonoBehaviour
{
    private PlayerControls player;
    [SerializeField] private string roomName;
    [Space] [SerializeField] private List<string> berries;
    [SerializeField] private HashSet<string> berriesSet;
    [Space] [SerializeField] private Animator anim;
   
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

        if (anim != null)
            anim.gameObject.SetActive(false);
    } 

    public void PickupBerry()
    {
        if (anim != null)
        {
            anim.gameObject.transform.parent = null;
            anim.gameObject.SetActive(true);
        }
        if (player != null)
        {
            player.nBerries++;
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
