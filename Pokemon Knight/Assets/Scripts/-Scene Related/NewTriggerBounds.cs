using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class NewTriggerBounds : MonoBehaviour
{
    [SerializeField] private CinemachineConfiner cmBounds;
    public Collider2D oldBounds;
    public Collider2D newBounds;
    [Space] public bool canChangeBack=false;
    // private string sceneName;
    
    // private void Start() 
    // {
    //     if (hasHiddenRoom)
    //     {
    //         sceneName = SceneManager.GetActiveScene().name;
    //         int gameNumber = PlayerPrefsElite.GetInt("gameNumber");
    //         if (PlayerPrefsElite.VerifyArray("hiddenRooms" + gameNumber))
    //         {
    //             List<string> hiddenRooms = new List<string>( PlayerPrefsElite.GetStringArray("hiddenRooms" + gameNumber) );
    //             if (hiddenRooms.Contains(sceneName))
    //             {
    //                 cmNew.Priority = 10;
    //                 cmOrig.Priority = -10;
    //             }

    //         }
    //         else
    //             PlayerPrefsElite.SetStringArray("hiddenRooms" + gameNumber, new string[0]);
    //     }    
    // }
    
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
            cmBounds.m_BoundingShape2D = newBounds;
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (canChangeBack && other.CompareTag("Player"))
            cmBounds.m_BoundingShape2D = oldBounds;

    }
}
