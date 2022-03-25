using UnityEngine;
using Cinemachine;

public class CameraManualFollow : MonoBehaviour
{
    public Transform target;
    public CinemachineVirtualCamera main_camera;
    
    void Update()
    {
        if (target != null)
        {
            float target_x = target.position.x;
            float target_y = target.position.y;

            float rounded_x = RoundToNearestPixel(target_x);
            float rounded_y = RoundToNearestPixel(target_y);

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
