using UnityEngine;

public class Parentless : MonoBehaviour
{
    void Start()
    {
        this.transform.parent = null;
    }
}
