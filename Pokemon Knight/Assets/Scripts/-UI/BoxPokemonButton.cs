using UnityEngine;

public class BoxPokemonButton : MonoBehaviour
{

    public PlayerControls playerControls;
    public Ally ally;
    public string pokemonName = "N/A";
    public Sprite sprite;


    public void SetNewAlly()
    {
        if (ally == null)
            Debug.LogError(this.gameObject.name +  "  -  ally is null", this.gameObject);
        if (sprite == null)
            Debug.LogError(this.gameObject.name +  "  -  sprite is not serialised", this.gameObject);
        // Debug.Log(this.ally.gameObject.name + "  _  " + this.sprite.name);
        playerControls.SetNewAlly(this.ally, this.sprite);
        // if (playerControls != null && this.ally != null)
        // else
        // {
        //     playerControls.SetNewAlly(this.ally, this.sprite);
        //     Debug.LogError("either (playerControls) or (ally) is not assigned", this.gameObject);
        // }
    }
}
