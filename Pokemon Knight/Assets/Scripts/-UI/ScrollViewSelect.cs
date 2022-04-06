// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollViewSelect : MonoBehaviour, ISelectHandler
{
    [SerializeField] protected RectTransform contentPanel;

    [SerializeField] private Vector2 anchorPos;

    public void OnSelect(BaseEventData eventData) 
    { 
        contentPanel.anchoredPosition = anchorPos;
    } 

    // // Update is called once per frame
    // void Update()
    // {
    //     if ()
    // }
}
