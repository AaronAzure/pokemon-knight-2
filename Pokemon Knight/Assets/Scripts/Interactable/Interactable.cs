using UnityEngine;
using TMPro;

public class Interactable : MonoBehaviour
{
    // [SerializeField] private GameObject textbox;
    public TextMeshPro text;

    private void OnEnable() 
    {
        text.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
            text.gameObject.SetActive(true);
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
            text.gameObject.SetActive(false);
    }
}
