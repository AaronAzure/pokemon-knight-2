using System.Collections;
using UnityEngine;

public class CandyBagTrigger : MonoBehaviour
{
    public CandyBag candyBag;
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
            candyBag.PlayerEnter();
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
            candyBag.PlayerExit();
    }

}
