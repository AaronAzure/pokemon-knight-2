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
    public TextMeshProUGUI evolutionBonusTxt;


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
                            ShowDescription(pokemonButton.ally);

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
                    ShowEnhancementDescription(epu.pokemon);
					break;
                }
            }
        }
    }


	public void ShowDescription(Ally ally)
	{
		header.text = ally.moveName;

		if (ally.cmd.isCustomDmg)
		{
			atkpower.text = ally.cmd.customDmg;
		}
		else
		{
			float atkDmg = ally.atkDmg;
			atkDmg += ( ally.extraDmg * Mathf.CeilToInt(
				((player.lv - 1) + (ally.ExtraEnhancedDmg()) / ally.perLevel)) 
			);
			atkpower.text = atkDmg.ToString();
			if (ally.multiHit > 1)
			{
				float temp = atkDmg;
				atkDmg *= ally.multiHit;
				atkpower.text = atkDmg.ToString() + " (" + temp + "×" + ally.multiHit + ")";
			}
			int bonusDmg = (ally.extraDmg*ally.EnhanceDmgBonus()*ally.multiHit);
			atkpower.text = atkDmg.ToString() + "<color=#8FFF78> (+" + bonusDmg + ")</color>";
		}

		float resummonTime = ally.resummonTime;
		coolDown.text = resummonTime.ToString();
		if (player != null && player.quickCharm)
		{
			resummonTime *= player.coolDownSpeed;
			coolDown.text = resummonTime.ToString() + "s <size=16>";
			coolDown.text += "(" + ally.resummonTime + "s)</size>";
		}

		desc.text = ally.moveDesc;
		if (player != null)
			desc.text += ally.ExtraDesc(player.lv);
	}

	public void ShowEnhancementDescription(Ally ally)
	{
		header.text = ally.pokemonName;
		// if (atkDmg == 0)
		// 	atkpower.text = "???";
		if (ally.cmd.isCustomDmg)
		{
			atkpower.text = ally.cmd.customDmg;
		}
		else
		{
			float atkDmg = ally.atkDmg;
			atkDmg += ( ally.extraDmg * Mathf.CeilToInt(
				((player.lv - 1) + (ally.ExtraEnhancedDmg()) / ally.perLevel)) 
			);
			atkpower.text = atkDmg.ToString();
			if (ally.multiHit > 1)
			{
				float temp = atkDmg;
				atkDmg *= ally.multiHit;
				atkpower.text = atkDmg.ToString() + " (" + temp + "×" + ally.multiHit + ")";
			}
			int bonusDmg = (ally.extraDmg*ally.EnhanceDmgBonus()*ally.multiHit);
			atkpower.text = atkDmg.ToString() + "<color=#8FFF78> (+" + bonusDmg + ")</color>";
		}
		// if (atkDmg == 0 && bonusDmg == 0)
		// 	atkpower.text = "???" + "<color=#8FFF78> (+???)</color>";

		float resummonTime = ally.resummonTime;
		coolDown.text = resummonTime.ToString();

		//* DIAMOND LV VISUAL INDICATOR
		int extraLv = ally.extraLevel;
		for (int i=0 ; i<enhanceImg.Length ; i++)
		{
			//* NEXT LEVEL TO BE ENHANCED
			if (extraLv == i)
				enhanceImg[i].color = new Color(1,1,1,0.3f);
			//* ALREADY ENHANCED
			else if (extraLv > i)
				enhanceImg[i].color = new Color(1,1,1,1f);
			//* NOT YET
			else
				enhanceImg[i].color = new Color(1,1,1,0f);
		}

		if (evolutionBonusTxt != null && ally.cmd.evolutionBonus != "" && extraLv % 3 == 2)
			evolutionBonusTxt.text = "(" + ally.cmd.evolutionBonus + ")";
		else
			evolutionBonusTxt.text = "";

		int enhancementCost = Mathf.RoundToInt(Mathf.Min( 10000, 100 * Mathf.Pow(3, extraLv) ));
		if (player.currency < enhancementCost || extraLv > 5)
			canEnhanceObj.SetActive(false);
		else
			canEnhanceObj.SetActive(true);

		if (extraLv <= 5)
			enhancementCostTxt.text = enhancementCost.ToString();
		else
			enhancementCostTxt.text = "Maxed";
	}
}
