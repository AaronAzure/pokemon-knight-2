using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private static DontDestroy instance;
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
            instance = this;
        else 
            Destroy(this.gameObject);
    }
}
