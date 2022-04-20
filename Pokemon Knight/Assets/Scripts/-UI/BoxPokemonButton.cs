using UnityEngine;
using UnityEngine.UI;

public class BoxPokemonButton : MonoBehaviour
{

    public PlayerControls playerControls;
    public Ally ally;

    public Image img;
    public string pokemonName = "N/A";
    
    [Space] public Sprite sprite;
    public Sprite evolveSprite1;
    public Sprite evolveSprite2;
    
    [Space] public Button button;
    
    
    [Space] [Header("Canvas - On Acquire")]
    public AcquiredDescUi pokemonAcqDesc;
    public Animator subseqAcqDesc;

    public void ShowDescriptionOfAcquired()
    {
        pokemonAcqDesc.descTxt.text = "<b>" + ally.moveName + "</b>\n" + ally.moveDesc;
        pokemonAcqDesc.headerTxt.text = ally.pokemonName;
        pokemonAcqDesc.acqImg.sprite = this.sprite;
    }

    public void SetNewAlly()
    {
        if (ally == null)
            Debug.LogError(this.gameObject.name +  "  -  ally is null", this.gameObject);
        if (sprite == null)
            Debug.LogError(this.gameObject.name +  "  -  sprite is not serialised", this.gameObject);
        
        if      (ally.extraLevel >= 6 && evolveSprite2 != null)
            playerControls.SetNewAlly(this.ally, this.evolveSprite2, this.button);
        else if (ally.extraLevel >= 3 && evolveSprite1 != null)
            playerControls.SetNewAlly(this.ally, this.evolveSprite1, this.button);
        else
            playerControls.SetNewAlly(this.ally, this.sprite, this.button);
    }
}
