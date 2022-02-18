using UnityEngine;
using UnityEngine.UI;

public class BoxPokemonButton : MonoBehaviour
{

    public PlayerControls playerControls;
    public Ally ally;

    public Image img;
    public string pokemonName = "N/A";
    public Sprite sprite;
    [Space] public Button button;






    public void SetNewAlly()
    {
        if (ally == null)
            Debug.LogError(this.gameObject.name +  "  -  ally is null", this.gameObject);
        if (sprite == null)
            Debug.LogError(this.gameObject.name +  "  -  sprite is not serialised", this.gameObject);
        // Debug.Log(this.ally.gameObject.name + "  _  " + this.sprite.name);
        playerControls.SetNewAlly(this.ally, this.sprite, this.button);
    }
}
