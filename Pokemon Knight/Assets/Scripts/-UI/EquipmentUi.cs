using UnityEngine;

public class EquipmentUi : MonoBehaviour
{
    [SerializeField] private PlayerControls player;
    [SerializeField] private Animator anim;
    public bool onPokemonTab=true;

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
            if (toRight && onPokemonTab)
            {
                toRight = false;
                onPokemonTab = false;
                anim.SetTrigger("toItems");
                anim.SetBool("onPokemonTab", false);
                player.canNavigate = false;
            }
            else if (!toRight && !onPokemonTab)
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
