using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;

public class TargetFinder : MonoBehaviour
{
    [SerializeField] private ProCamera2D proCam;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("PLAYER") != null)
        {
            GameObject target = GameObject.Find("PLAYER");
            proCam.AddCameraTarget(target.transform);
        }
    }
}
