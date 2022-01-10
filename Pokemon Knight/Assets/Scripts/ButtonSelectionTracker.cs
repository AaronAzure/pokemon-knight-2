// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSelectionTracker : MonoBehaviour
{
    [SerializeField] private Image selection;
    [SerializeField] private GameObject currentHighlighted;

    void OnEnable() 
    {
        EventSystem.current.SetSelectedGameObject(currentHighlighted);
        // currentHighlighted =     
    }
    void Start()
    {
        selection.transform.position = EventSystem.current.currentSelectedGameObject.gameObject.transform.position;
    }

    void LateUpdate()
    {
        selection.transform.position = EventSystem.current.currentSelectedGameObject.gameObject.transform.position;
        Debug.Log(EventSystem.current.currentSelectedGameObject.name);
    }
}
