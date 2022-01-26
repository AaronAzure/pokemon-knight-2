using UnityEngine;
using Cinemachine;

public class TriggerBounds : MonoBehaviour
{
    // [SerializeField] private CinemachineConfiner confiner;
    [SerializeField] private CinemachineVirtualCamera cmOrig;
    [SerializeField] private CinemachineVirtualCamera cmNew;
    
    
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && cmNew != null)    
        {
            // confiner.m_BoundingShape2D = colNew;
            cmOrig.Priority = -10;
            cmNew.Priority  = 10;

        }
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && cmOrig != null)    
        {
            // confiner.m_BoundingShape2D = colOrig;
            cmOrig.Priority = 10;
            cmNew.Priority  = -10;

        }
    }
}
