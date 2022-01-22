using System.Collections;
using UnityEngine;

public class ColliderOnAfter : MonoBehaviour
{
    [SerializeField] private float delayOn=0.5f;
    [SerializeField] private float delayOff=0.5f;
    [SerializeField] private Collider2D col;
    // Start is called before the first frame update
    void Start()
    {
        if (col == null)
            col = this.GetComponent<Collider2D>();
        if (col != null && delayOn > 0)
            StartCoroutine( EnableAfter() );
        if (col != null && delayOff > 0)
            StartCoroutine( DisableAfter() );
    }
    IEnumerator EnableAfter()
    {
        yield return new WaitForSeconds(delayOn);
        if (col != null)
            col.enabled = true;
    }
    IEnumerator DisableAfter()
    {
        yield return new WaitForSeconds(delayOff);
        if (col != null)
            col.enabled = false;
    }
}
