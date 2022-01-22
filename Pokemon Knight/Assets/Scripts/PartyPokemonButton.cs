using UnityEngine;
using UnityEngine.UI;

public class PartyPokemonButton : MonoBehaviour
{

    public PlayerControls playerControls;
    public Button button;
    public string buttonSymbol;

    private void Start() 
    {
        if (button == null)
            button = this.GetComponent<Button>();
    }

    public void SendAllyToReplace()
    {
        if (button == null)
            Debug.LogError(this.gameObject.name +  "  -  ally is null", this.gameObject);
        if (buttonSymbol == null)
            Debug.LogError(this.gameObject.name +  "  -  sprite is not serialised", this.gameObject);
        playerControls.SelectAllyToReplace(buttonSymbol, button);
    }
}
