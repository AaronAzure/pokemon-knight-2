using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyTrigger : MonoBehaviour
{
    public Currency currency;

    
    private void OnTriggerEnter2D(Collider2D other) {
        if (currency != null && other.CompareTag("Player"))
        {
            PlayerControls p = other.GetComponent<PlayerControls>();
            currency.CollectedBy(p);
        }
        if (currency != null && other.CompareTag("Mew"))
        {
            Mew mew = other.GetComponent<Mew>();
            currency.CollectedByMew(mew);
        }
    }
}
