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
    [SerializeField] private TextMeshProUGUI atkpower;
    [SerializeField] private TextMeshProUGUI coolDown;
    
    
    [Space] [Header("Pokemon Menu")]
    public bool itemMenu;
    [SerializeField] private ItemUi[] items;


    [Space] [Header("UI (tmpro)")]
    [SerializeField] private TextMeshProUGUI header;
    [SerializeField] private TextMeshProUGUI weight;
    [SerializeField] private TextMeshProUGUI desc;

    private void LateUpdate() 
    {
        newlyHighlighted = EventSystem.current.currentSelectedGameObject;

        //* On highlighted button change
        if (newlyHighlighted != prevHighlighted || prevHighlighted == null)
        {
            prevHighlighted = newlyHighlighted;
            bool found = false;
            if (pokemonMenu)
            {
                if (pokemons != null)
                {
                    foreach (BoxPokemonButton pokemonButton in pokemons)
                    {
                        if (pokemonButton != null && pokemonButton.ally != null 
                            && newlyHighlighted.name == pokemonButton.name)
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
                            coolDown.text = resummonTime.ToString();
                            if (player != null && player.speedScarf)
                            {
                                resummonTime *= 0.7f;
                                coolDown.text = resummonTime.ToString() + "s (" + pokemonButton.ally.resummonTime + "s)";
                            }

                            desc.text = pokemonButton.ally.moveDesc;
                            if (player != null)
                                desc.text += pokemonButton.ally.ExtraDesc(player.lv);

                            break;
                        }
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
            else if (itemMenu)
            {
                if (items != null)
                {
                    foreach (ItemUi itemButton in items)
                    {
                        if (itemButton != null && newlyHighlighted.name == itemButton.name)
                        {
                            found = true;
                            header.text = itemButton.itemName;

                            weight.text = itemButton.weight.ToString();

                            desc.text = itemButton.itemDesc;

                            break;
                        }
                    }

                }
                if (!found)
                {
                    header.text = "";
                    weight.text = "0";
                    desc.text = "";
                }
            }
        }
    }
}
