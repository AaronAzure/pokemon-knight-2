using UnityEngine;

public class EquipmentUi : MonoBehaviour
{
    [SerializeField] private PlayerControls player;
    [SerializeField] private Animator anim;
    public bool onPokemonTab=true;
    public bool onEnhanceTab=false;

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
    public void ChangeTabs(bool toRight)
    {
        if (anim != null)
        {
            //* TO ITEM TAB
            if (toRight && onPokemonTab)
            {
                toRight = false;
                onPokemonTab = false;
                anim.SetTrigger("toItems");
                anim.SetBool("onPokemonTab", false);
                player.canNavigate = false;
            }
            //* TO ENHANCE TAB
            else if (!toRight && onPokemonTab)
            {
                toRight = true;
                onPokemonTab = false;
                onEnhanceTab = true;
                anim.SetTrigger("toEnhance");
                anim.SetBool("onPokemonTab", false);
                player.canNavigate = false;
            }
            //* TO POKEMON TAB (FROM ENHANCE TAB) 
            else if (toRight && !onPokemonTab && onEnhanceTab)
            {
                toRight = false;
                onPokemonTab = true;
                onEnhanceTab = false;
                anim.SetTrigger("toPokemon");
                anim.SetBool("onPokemonTab", true);
                player.canNavigate = false;
            }
            //* TO POKEMON TAB (FROM ITEM TAB)
            else if (!toRight && !onPokemonTab && !onEnhanceTab)
            {
                toRight = true;
                onPokemonTab = true;
                anim.SetTrigger("toPokemon");
                anim.SetBool("onPokemonTab", true);
                player.canNavigate = false;
            }
            
        }
    }
}
