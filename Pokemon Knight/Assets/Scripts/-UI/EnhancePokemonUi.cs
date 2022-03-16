using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhancePokemonUi : MonoBehaviour
{
    public PlayerControls player;
    public Ally pokemon;
    [Space] public HighlightedButton desc;

    public void ENHANCE_POKEMON()
    {
        int cost = Mathf.RoundToInt(Mathf.Min( 10000, 100 * Mathf.Pow(2, pokemon.extraLevel) ));
        if (player.currency >= cost)
        {
            player.EnhanceAllyPokemonLevel(pokemon, cost);
            desc.RefreshEnhanceMenu();
        }
    }
}
