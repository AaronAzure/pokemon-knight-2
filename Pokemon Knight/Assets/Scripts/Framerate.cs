using UnityEngine;

public class Framerate : MonoBehaviour
{
    [SerializeField] private int frameRate = 30;
    void Start()
    {
        Application.targetFrameRate = frameRate;   
    }
}
