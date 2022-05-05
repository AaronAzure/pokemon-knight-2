using UnityEngine;
using UnityEngine.UI;

public class BoxPokemonButton : MonoBehaviour
{

    public PlayerControls playerControls;
    public Ally ally;

    public Image img;
    public string pokemonName = "N/A";
    
    // [Space] public Sprite sprite;
    // public Sprite evolveSprite1;
    // public Sprite evolveSprite2;
    
    [Space] public Button button;
    
    
    [Space] [Header("Canvas - On Acquire")]
    public AcquiredDescUi pokemonAcqDesc;
    public Animator subseqAcqDesc;

    private void OnEnable() 
    {
        if (img != null && ally != null)
            img.sprite = ally.currentForm;
    }

    public void ShowDescriptionOfAcquired()
    {
        pokemonAcqDesc.descTxt.text = "<b>" + ally.moveName + "</b>\n" + ally.moveDesc;
        pokemonAcqDesc.headerTxt.text = ally.pokemonName;
        pokemonAcqDesc.acqImg.sprite = this.ally.currentForm;
    }

    public void SetNewAlly()
    {
        if (ally == null)
            Debug.LogError(this.gameObject.name +  "  -  ally is null", this.gameObject);
        // if (sprite == null)
        //     Debug.LogError(this.gameObject.name +  "  -  sprite is not serialised", this.gameObject);
        
        // if      (ally.IsAtThirdEvolution() && evolveSprite2 != null)
        //     playerControls.SetNewAlly(this.ally, this.ally.currentForm, this.button);
        // else if (ally.IsAtSecondEvolution() && evolveSprite1 != null)
        //     playerControls.SetNewAlly(this.ally, this.ally.currentForm, this.button);
        // else
        playerControls.SetNewAlly(this.ally, this.ally.currentForm, this.button);
    }
}
