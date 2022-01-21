using UnityEngine;

public class BoxPokemonButton : MonoBehaviour
{

    public PlayerControls playerControls;
    public Ally ally;
    public Sprite sprite;


    public void SetNewAlly()
    {
        if (playerControls != null && this.ally != null)
            playerControls.SetNewAlly(this.ally, this.sprite);
        else
            Debug.LogError("either (playerControls) or (ally) is not assigned", this.gameObject);
    }
}
