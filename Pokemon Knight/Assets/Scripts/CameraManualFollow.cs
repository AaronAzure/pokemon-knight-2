using UnityEngine;
using Cinemachine;

public class CameraManualFollow : MonoBehaviour
{
    public Transform target;
    public Camera main_camera;
    public Vector2 topRightBounds;
    public Vector2 bottomLeftBounds;
    
    [Space] 
    [SerializeField] private float camXSize;
    [SerializeField] private float camYSize;

    void Start()
    {
        if (main_camera != null)
        {
            // camYSize = main_camera.m_Lens.OrthographicSize;
            // camXSize = camYSize * main_camera.m_Lens.Aspect;
            camYSize = main_camera.orthographicSize;
            camXSize = camYSize * main_camera.aspect;
        }
    }
    void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(topRightBounds, new Vector2(bottomLeftBounds.x, topRightBounds.y));
        Gizmos.DrawLine(topRightBounds, new Vector2(topRightBounds.x, bottomLeftBounds.y));
        Gizmos.DrawLine(bottomLeftBounds, new Vector2(topRightBounds.x, bottomLeftBounds.y));
        Gizmos.DrawLine(bottomLeftBounds, new Vector2(bottomLeftBounds.x, topRightBounds.y));
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            float target_x = target.position.x;
            float target_y = target.position.y;

            float rounded_x = RoundToNearestPixel(target_x);
            if (rounded_x - camXSize < bottomLeftBounds.x)
                rounded_x = bottomLeftBounds.x + camXSize;
            if (rounded_x + camXSize > topRightBounds.x)
                rounded_x = topRightBounds.x - camXSize;
            // rounded_x = Mathf.Max( 
            //     Mathf.Min(rounded_x + camXSize, topRightBounds.x), 
            //     rounded_x - camXSize, bottomLeftBounds.x
            // );
            float rounded_y = RoundToNearestPixel(target_y);
            if (rounded_y - camYSize < bottomLeftBounds.y)
                rounded_y = bottomLeftBounds.y + camYSize;
            if (rounded_y + camYSize > topRightBounds.y)
                rounded_y = topRightBounds.y - camYSize;
            // rounded_y = Mathf.Max( 
            //     Mathf.Min(rounded_y + camYSize, topRightBounds.y), 
            //     rounded_y - camYSize, bottomLeftBounds.y
            // );

            Vector3 new_pos = new Vector3(rounded_x, rounded_y, -10.0f); // this is 2d, so my camera is that far from the screen.
            main_camera.transform.position = new_pos;
        }
    }
    public float pixelToUnits = 128f;
    
    public float RoundToNearestPixel(float unityUnits)
    {
        float valueInPixels = unityUnits * pixelToUnits;
        valueInPixels = Mathf.Round(valueInPixels);
        float roundedUnityUnits = valueInPixels * (1 / pixelToUnits);
        return roundedUnityUnits;
    }
}
