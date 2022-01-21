using UnityEngine;

public class PokemonSets : MonoBehaviour
{
    [SerializeField] private Canvas pokemonSet1;
    [SerializeField] private Canvas pokemonSet2;
    
    public void ChangeToSet1()
    {
        if (pokemonSet1 != null)   pokemonSet1.sortingOrder = 2;
        if (pokemonSet2 != null)   pokemonSet2.sortingOrder = 1;
    }
    public void ChangeToSet2()
    {
        if (pokemonSet1 != null)   pokemonSet1.sortingOrder = 1;
        if (pokemonSet2 != null)   pokemonSet2.sortingOrder = 2;
    }
}
