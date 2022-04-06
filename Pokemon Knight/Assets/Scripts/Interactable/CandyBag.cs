using UnityEngine;

public class CandyBag : MonoBehaviour
{
    public PlayerControls player;
    public int quantity;
    public Rigidbody2D body;
    public TextHolder holder;
   
    private void Start() 
    {
    } 

    public void Pickup()
    {
        if (player != null)
        {
            player.GainCandy(quantity, true);
            player.hasLostBag = false;
            PlayerPrefsElite.SetBoolean("hasLostBag" + PlayerPrefsElite.GetInt("gameNumber"), false);
            var obj = Instantiate(holder, this.transform.position + new Vector3(0,2), Quaternion.identity);
            obj.text.text = "+" + quantity.ToString();

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
