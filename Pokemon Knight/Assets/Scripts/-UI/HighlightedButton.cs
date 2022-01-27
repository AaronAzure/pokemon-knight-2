using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class HighlightedButton : MonoBehaviour
{
    [SerializeField] private GameObject prevHighlighted;
    [SerializeField] private GameObject newlyHighlighted;
    [SerializeField] private PlayerControls player;

    
    [Space] [Header("Pokemon Menu")]
    public bool pokemonMenu;
    [SerializeField] private BoxPokemonButton[] pokemons;
    [SerializeField] private TextMeshProUGUI header;
    [SerializeField] private TextMeshProUGUI atkpower;
    [SerializeField] private TextMeshProUGUI coolDown;
    [SerializeField] private TextMeshProUGUI desc;
    
    
    [Space] [Header("Pokemon Menu")]
    public bool itemMenu;

    private void LateUpdate() 
    {
        newlyHighlighted = EventSystem.current.currentSelectedGameObject;

        //* On highlighted button change
        if (newlyHighlighted != prevHighlighted || prevHighlighted == null)
        {
            prevHighlighted = newlyHighlighted;
            bool found = false;
            foreach (BoxPokemonButton pokemonButton in pokemons)
            {
                if (newlyHighlighted.name == pokemonButton.name)
                {
                    found = true;
                    header.text = pokemonButton.ally.moveName;

                    float atkDmg = pokemonButton.ally.atkDmg;
                    atkDmg += ( pokemonButton.ally.extraDmg * Mathf.CeilToInt(((player.lv - 1) / pokemonButton.ally.perLevel)) );
                    atkpower.text = atkDmg.ToString();
                    if (pokemonButton.ally.multiHit > 1)
                    {
                        float temp = atkDmg;
                        atkDmg *= pokemonButton.ally.multiHit;
                        atkpower.text = atkDmg.ToString() + " (" + temp + "×" + pokemonButton.ally.multiHit + ")";
                    }

                    float resummonTime = pokemonButton.ally.resummonTime;
                    if (player != null && player.speedScarf)
                        resummonTime *= 0.7f;
                    coolDown.text = resummonTime.ToString();

                    desc.text = pokemonButton.ally.moveDesc;

                    break;
                }
            }
            if (!found)
            {
                header.text = "";
                atkpower.text = "0";
                coolDown.text = "0";
                desc.text = "";
            }
        }
    }
}
