using UnityEngine;
using UnityEngine.UI;

public class EnhancePokemonUi : MonoBehaviour
{
    public PlayerControls player;
    public Ally pokemon;
    [Space] public HighlightedButton desc;
    public Image img;

    private void OnEnable() 
    {
        if (pokemon != null && img != null)
        {
            img.sprite = pokemon.currentForm;
        }    
    }

    public void ENHANCE_POKEMON()
    {
        int cost = Mathf.RoundToInt(Mathf.Min( 10000, 100 * Mathf.Pow(3, pokemon.extraLevel) ));
        if (player.currency >= cost && pokemon.extraLevel < 6)
        {
            player.EnhanceAllyPokemonLevel(pokemon, cost);
            desc.RefreshEnhanceMenu();
        }
    }
}
