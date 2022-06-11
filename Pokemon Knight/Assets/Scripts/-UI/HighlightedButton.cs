using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    
    
    [Space] [Header("Item Menu")]
    public bool itemMenu;
    [SerializeField] private ItemUi[] items;
    
    
    [Space] [Header("Enhance Menu")]
    public bool enhanceMenu;
    [SerializeField] private EnhancePokemonUi[] enhancePokemonUis;
    public Image[] enhanceImg;
    public GameObject canEnhanceObj;
    public TextMeshProUGUI enhancementCostTxt;


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
                            atkDmg += ( pokemonButton.ally.extraDmg * Mathf.CeilToInt(
                                ((player.lv - 1) + (pokemonButton.ally.ExtraEnhancedDmg()) / pokemonButton.ally.perLevel)) 
                            );
                            atkpower.text = atkDmg.ToString();
                            if (pokemonButton.ally.multiHit > 1)
                            {
                                float temp = atkDmg;
                                atkDmg *= pokemonButton.ally.multiHit;
                                atkpower.text = atkDmg.ToString() + " (" + temp + "×" + pokemonButton.ally.multiHit + ")";
                            }
							if (pokemonButton.ally.cmd.isCustomDmg)
							{
								
							}
							if (atkDmg == 0)
                            	atkpower.text = "???";

                            float resummonTime = pokemonButton.ally.resummonTime;
                            coolDown.text = resummonTime.ToString();
                            if (player != null && player.quickCharm)
                            {
                                resummonTime *= player.coolDownSpeed;
                                coolDown.text = resummonTime.ToString() + "s <size=16>";
                                coolDown.text += "(" + pokemonButton.ally.resummonTime + "s)</size>";
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
                            header.text = itemButton.camelisedItemName;

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
        
            else if (enhanceMenu)
            {
                RefreshEnhanceMenu();
            }
        
        }
    }

    public void RefreshEnhanceMenu()
    {
        if (enhancePokemonUis != null)
        {
            foreach (EnhancePokemonUi epu in enhancePokemonUis)
            {
                if (epu != null && epu.pokemon != null && newlyHighlighted.name == epu.name)
                {
                    header.text = epu.pokemon.pokemonName;
                    float atkDmg = epu.pokemon.atkDmg;
                    atkDmg += ( epu.pokemon.extraDmg * Mathf.CeilToInt(
                        ((player.lv - 1) + (epu.pokemon.ExtraEnhancedDmg()) / epu.pokemon.perLevel)) 
                    );
                    atkpower.text = atkDmg.ToString();
                    if (epu.pokemon.multiHit > 1)
                    {
                        float temp = atkDmg;
                        atkDmg *= epu.pokemon.multiHit;
                        atkpower.text = atkDmg.ToString() + " (" + temp + "×" + epu.pokemon.multiHit + ")";
                    }
					if (atkDmg == 0)
                    	atkpower.text = "???";
					int bonusDmg = (epu.pokemon.extraDmg*epu.pokemon.EnhanceDmgBonus()*epu.pokemon.multiHit);
                    atkpower.text = atkDmg.ToString() + "<color=#8FFF78> (+" + bonusDmg + ")</color>";
					if (atkDmg == 0 && bonusDmg == 0)
                    	atkpower.text = "???" + "<color=#8FFF78> (+???)</color>";

                    float resummonTime = epu.pokemon.resummonTime;
                    coolDown.text = resummonTime.ToString();

                    int extraLv = epu.pokemon.extraLevel;
                    for (int i=0 ; i<enhanceImg.Length ; i++)
                    {
                        if (extraLv == i)
                        {
                            enhanceImg[i].color = new Color(1,1,1,0.3f);
                        }
                        else if (extraLv > i)
                        {
                            enhanceImg[i].color = new Color(1,1,1,1f);
                        }
                        else
                        {
                            enhanceImg[i].color = new Color(1,1,1,0f);
                        }
                    }

                    int enhancementCost = Mathf.RoundToInt(Mathf.Min( 10000, 100 * Mathf.Pow(3, extraLv) ));
                    if (player.currency < enhancementCost || extraLv > 5)
                        canEnhanceObj.SetActive(false);
                    else
                        canEnhanceObj.SetActive(true);

                    if (extraLv <= 5)
                        enhancementCostTxt.text = enhancementCost.ToString();
                    else
                        enhancementCostTxt.text = "Maxed";

                    break;
                }
            }
        }
    }

}
