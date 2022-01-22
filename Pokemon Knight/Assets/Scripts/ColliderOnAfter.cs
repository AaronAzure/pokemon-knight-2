using System.Collections;
using UnityEngine;

public class ColliderOnAfter : MonoBehaviour
{
    [SerializeField] private float delayOn=0.5f;
    [SerializeField] private float delayOff=0.5f;
    [SerializeField] private Collider2D[] cols;
    // Start is called before the first frame update
    void Start()
    {
        // if (cols == null)
        //     cols = this.GetComponent<Collider2D>();
        if (cols != null && cols.Length > 0 && delayOn > 0)
            StartCoroutine( EnableAfter() );
        if (cols != null && cols.Length > 0 && delayOff > 0)
            StartCoroutine( DisableAfter() );
    }
    IEnumerator EnableAfter()
    {
        yield return new WaitForSeconds(delayOn);
        foreach (Collider2D col in cols)
            col.enabled = true;
    }
    IEnumerator DisableAfter()
    {
        yield return new WaitForSeconds(delayOff);
        foreach (Collider2D col in cols)
            col.enabled = false;
    }
}
