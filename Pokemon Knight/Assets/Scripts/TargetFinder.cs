using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;
using Cinemachine;

public class TargetFinder : MonoBehaviour
{
    [SerializeField] private ProCamera2D proCam;
    [SerializeField] private CinemachineVirtualCamera cm;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("PLAYER") != null)
        {
            GameObject target = GameObject.Find("PLAYER");
            if (proCam != null) proCam.AddCameraTarget(target.transform);
            if (cm != null) cm.Follow = target.transform;
        }
    }
}
