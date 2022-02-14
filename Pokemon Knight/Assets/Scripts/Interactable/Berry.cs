using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Berry : MonoBehaviour
{
    private PlayerControls player;
    [SerializeField] private string roomName;
    [Space] [SerializeField] private string[] berries;
    [SerializeField] private HashSet<string> berriesSet;
    [Space] [SerializeField] private Animator anim;
   
    private void Start() 
    {
        roomName = SceneManager.GetActiveScene().name + " " + this.name;

        if (PlayerPrefsElite.VerifyArray("berries" + PlayerPrefsElite.GetInt("gameNumber")))
        {
            berries = PlayerPrefsElite.GetStringArray("berries" + PlayerPrefsElite.GetInt("gameNumber"));
            berriesSet = new HashSet<string>(berries);
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
            if (berries.Length < berriesSet.Count)
            {
                berries[ berriesSet.Count ] = roomName;
                PlayerPrefsElite.SetStringArray("berries" + PlayerPrefsElite.GetInt("gameNumber"), berries);
            }

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
