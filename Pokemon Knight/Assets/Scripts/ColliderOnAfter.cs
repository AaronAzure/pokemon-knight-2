using System.Collections;
using UnityEngine;

public class ColliderOnAfter : MonoBehaviour
{
    [SerializeField] private float delay=0.5f;
    [SerializeField] private Collider2D col;
    // Start is called before the first frame update
    void Start()
    {
        if (col == null)
            col = this.GetComponent<Collider2D>();
        if (col != null)
            StartCoroutine( EnableAfter() );
    }
    IEnumerator EnableAfter()
    {
        yield return new WaitForSeconds(delay);
        if (col != null)
            col.enabled = true;
    }
}
