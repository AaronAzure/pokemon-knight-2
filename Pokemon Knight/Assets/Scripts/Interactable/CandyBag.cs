using UnityEngine;

public class CandyBag : MonoBehaviour
{
    public PlayerControls player;
    public int quantity;
    public Rigidbody2D body;
   
    private void Start() 
    {
    } 

    public void Pickup()
    {
        if (player != null)
        {
            player.GainCandy(quantity);
            player.hasLostBag = false;
            PlayerPrefsElite.SetBoolean("hasLostBag" + PlayerPrefsElite.GetInt("gameNumber"), false);

            Destroy(this.gameObject);
        }
    }
    
    public void PlayerEnter() 
    {
        if (player != null)
            player.currentBag = this;
    }
    public void PlayerExit() 
    {
        if (player != null)
            player.currentBag = null;
    }
}
