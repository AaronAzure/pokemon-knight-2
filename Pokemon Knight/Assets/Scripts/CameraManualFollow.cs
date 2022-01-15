using UnityEngine;

public class CameraManualFollow : MonoBehaviour
{
    public Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothSpeed=10f;

    private void FixedUpdate() 
    {
        Vector3 desiredPos = target.position + offset;
        Vector3 smoothPos = Vector3.Lerp(this.transform.position, desiredPos, smoothSpeed * Time.deltaTime);
        transform.position = smoothPos;
    }
}
