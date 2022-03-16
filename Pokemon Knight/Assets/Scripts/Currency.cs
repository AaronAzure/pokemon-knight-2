using UnityEngine;

public class Currency : MonoBehaviour
{
    public int value=1;
    public Rigidbody2D body;
    
    public void CollectedBy(PlayerControls player) 
    {
        player.GainCandy(value);
        // player.currency += value;
        // player.currencyTxt.text = player.currency.ToString();
        Destroy(this.gameObject);
    }
}
