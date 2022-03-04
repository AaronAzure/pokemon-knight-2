using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;
using Cinemachine;

public class TargetFinder : MonoBehaviour
{
    [SerializeField] private ProCamera2D proCam;
    [SerializeField] private CinemachineVirtualCamera[] cm;
    [SerializeField] private CameraManualFollow manualFollow;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("PLAYER") != null)
        {
            GameObject target = GameObject.Find("PLAYER/MODEL");
            // GameObject target = GameObject.Find("PLAYER/CAM TARGET");
            if (proCam != null) proCam.AddCameraTarget(target.transform);
            if (cm != null) {
                foreach (CinemachineVirtualCamera c in cm)
                    if (c != null)
                        c.Follow = target.transform;
            }
            if (manualFollow != null) manualFollow.target = target.transform;
        }
    }
}
