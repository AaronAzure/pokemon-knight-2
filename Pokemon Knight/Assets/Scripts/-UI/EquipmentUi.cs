using UnityEngine;
using System.Collections;

public class EquipmentUi : MonoBehaviour
{
	public enum TabType 
	{
		none		= -1,
		enchanceTab	= 0,
		partyTab	= 1,
		itemsTab	= 2,
		mapTab		= 3
	}

    [SerializeField] private PlayerControls player;
    [SerializeField] private Animator anim;
    public bool onPokemonTab=true;
    public bool onEnhanceTab=false;
	public TabUi[] tabs;
	public int currentTab=1;

	private void Start() 
	{
		for (int i=0 ; i<tabs.Length ; i++)
		{
			if (tabs[i].tabUi != null)
			{
				if (currentTab == i)
					tabs[i].tabUi.SetActive(true);
				else
					tabs[i].tabUi.SetActive(false);
			}
		}	
	}

    private void OnEnable() 
    {
        onPokemonTab=true;
        onEnhanceTab=false;
    }

    public void CAN_NAVIGATE_AGAIN()
    {
        if (player != null)
            player.canNavigate = true;
    }
    public void UNPAUSE()
    {
        Time.timeScale = 1;
        onPokemonTab = true;
        this.gameObject.SetActive(false);
        if (player != null)
        {
            player.canNavigate = true;
            player.Resume();
        }
    }
    public int ChangeTabs(bool toRight)
    {
		if (tabs != null)
		{
			if (toRight)
			{
				if ((currentTab + 1) < tabs.Length && 
					tabs[currentTab + 1].tabAnim != null && tabs[currentTab + 1].tabUi != null)
				{
	                player.canNavigate = false;
					tabs[currentTab].tabAnim.SetTrigger("deselect");
					tabs[currentTab].tabUi.SetActive(false);
					currentTab++;
					tabs[currentTab].tabAnim.SetTrigger("select");
					tabs[currentTab].tabUi.SetActive(true);
					StartCoroutine( CanNavigateAgainCo() );
					return currentTab;
				}
			}
			else
			{
				if ((currentTab - 1) >= 0 && 
					tabs[currentTab - 1].tabAnim != null && tabs[currentTab - 1].tabUi != null)
				{
	                player.canNavigate = false;
					tabs[currentTab].tabAnim.SetTrigger("deselect");
					tabs[currentTab].tabUi.SetActive(false);
					currentTab--;
					tabs[currentTab].tabAnim.SetTrigger("select");
					tabs[currentTab].tabUi.SetActive(true);
					StartCoroutine( CanNavigateAgainCo() );
					return currentTab;
				}
			}
		}
		return -1;

        // if (anim != null)
        // {
        //     //* TO ITEM TAB
        //     if (toRight && onPokemonTab)
        //     {
        //         toRight = false;
        //         onPokemonTab = false;
        //         anim.SetTrigger("toItems");
        //         anim.SetBool("onPokemonTab", false);
        //         player.canNavigate = false;
        //     }
        //     //* TO ENHANCE TAB
        //     else if (!toRight && onPokemonTab)
        //     {
        //         toRight = true;
        //         onPokemonTab = false;
        //         onEnhanceTab = true;
        //         anim.SetTrigger("toEnhance");
        //         anim.SetBool("onPokemonTab", false);
        //         player.canNavigate = false;
        //     }
        //     //* TO POKEMON TAB (FROM ENHANCE TAB) 
        //     else if (toRight && !onPokemonTab && onEnhanceTab)
        //     {
        //         toRight = false;
        //         onPokemonTab = true;
        //         onEnhanceTab = false;
        //         anim.SetTrigger("toPokemon");
        //         anim.SetBool("onPokemonTab", true);
        //         player.canNavigate = false;
        //     }
        //     //* TO POKEMON TAB (FROM ITEM TAB)
        //     else if (!toRight && !onPokemonTab && !onEnhanceTab)
        //     {
        //         toRight = true;
        //         onPokemonTab = true;
        //         anim.SetTrigger("toPokemon");
        //         anim.SetBool("onPokemonTab", true);
        //         player.canNavigate = false;
        //     }
        // }
    }

	IEnumerator CanNavigateAgainCo()
	{
		yield return new WaitForSecondsRealtime(0.3f);
		if (player != null)
            player.canNavigate = true;
	}
}


[System.Serializable] public class TabUi
{
	public Animator tabAnim;
	public GameObject tabUi;
}