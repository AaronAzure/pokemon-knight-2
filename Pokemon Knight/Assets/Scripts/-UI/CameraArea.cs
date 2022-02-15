using UnityEngine;

public class CameraArea : MonoBehaviour
{
    public Transform camObj;
    public Vector2 camBox;

    [Space] public Collider boundsCol;


    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.magenta;
        if (camObj == null)
            Gizmos.DrawWireCube(this.transform.position, camBox);
        else
            // Gizmos.DrawWireCube(camObj.transform.position, camBox);
            Gizmos.DrawWireCube(camObj.transform.position, camBox);
        
        if (boundsCol != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(boundsCol.bounds.center, boundsCol.bounds.size);

            Gizmos.color = Color.red;
            Vector3 finalBounds = boundsCol.bounds.size + (Vector3) camBox;
            Gizmos.DrawWireCube(boundsCol.bounds.center, finalBounds);
        }
    }
}
