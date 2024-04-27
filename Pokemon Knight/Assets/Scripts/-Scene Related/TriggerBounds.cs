using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Cinemachine;

public class TriggerBounds : MonoBehaviour
{
    // [SerializeField] private CinemachineConfiner confiner;
    [SerializeField] private CinemachineVirtualCamera cmOrig;
    [SerializeField] private CinemachineVirtualCamera cmNew;
    [Space] public bool canChangeBack=true;
    [Space] public bool hasHiddenRoom=false;
    private string sceneName;
    
    private void Start() 
    {
        if (hasHiddenRoom)
        {
            sceneName = SceneManager.GetActiveScene().name;
            int gameNumber = PlayerPrefsElite.GetInt("gameNumber");
            if (PlayerPrefsElite.VerifyArray("hiddenRooms" + gameNumber))
            {
                List<string> hiddenRooms = new List<string>( PlayerPrefsElite.GetStringArray("hiddenRooms" + gameNumber) );
                if (hiddenRooms.Contains(sceneName))
                {
                    cmNew.Priority = 10;
                    cmOrig.Priority = -10;
                }

            }
            else
                PlayerPrefsElite.SetStringArray("hiddenRooms" + gameNumber, new string[0]);
        }    
    }
    
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && cmNew != null)    
        {
            // confiner.m_BoundingShape2D = colNew;
            cmOrig.Priority = -10;
            cmNew.Priority  = 10;
            // if (hasHiddenRoom)

        }
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (canChangeBack && other.CompareTag("Player") && cmOrig != null)    
        {
            // confiner.m_BoundingShape2D = colOrig;
            cmOrig.Priority = 10;
            cmNew.Priority  = -10;

        }
    }
}
