using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubwayStopButton : MonoBehaviour
{
    public PlayerControls playerControls;
    [Tooltip("PlayerPref string name")] public string areaName = "N/A";
    public string destination = "";
    public Button button;
    // [Space] public string camelisedStopName;
    // [Space] public Button button;

    // void Awake() 
    // {
    //     button = GetComponent<Button>();
    // }

    public void GoToStop()
    {
        playerControls.newSceneName = this.destination;
        playerControls.newScenePos = playerControls.transform.position + new Vector3(0,0.2f);
        playerControls.TakeTheTrain();
    }
    public void UnlockStop()
    {
        Debug.Log("STATION UNLOCKED");
        this.gameObject.SetActive(true);
    }
    public void LockStop()
    {
        Debug.Log("STATION LOCKED");
        this.gameObject.SetActive(false);
    }
}
