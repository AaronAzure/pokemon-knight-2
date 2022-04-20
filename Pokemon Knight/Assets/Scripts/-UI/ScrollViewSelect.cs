// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollViewSelect : MonoBehaviour, ISelectHandler
{
    [SerializeField] protected RectTransform contentPanel;

    [SerializeField] private Vector2 anchorPos;
    [SerializeField] private GameObject makeAppear;
    [SerializeField] private GameObject makeDisappear;

    public void OnSelect(BaseEventData eventData) 
    { 
        contentPanel.anchoredPosition = anchorPos;
        if (makeAppear != null)
            makeAppear.SetActive(true);
        if (makeDisappear != null)
            makeDisappear.SetActive(false);
    } 

    // // Update is called once per frame
    // void Update()
    // {
    //     if ()
    // }
}
