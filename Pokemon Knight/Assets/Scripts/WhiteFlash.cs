using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WhiteFlash : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer[] renderers;
    [SerializeField] protected Material flashMat;
    [SerializeField] protected Material origMat;

    IEnumerator Flash()
    {
        foreach (SpriteRenderer renderer in renderers)
        {
            if (flashMat != null && origMat != null)
                renderer.material = flashMat;
        }
        
        yield return new WaitForSeconds(0.1f);
        foreach (SpriteRenderer renderer in renderers)
        {
            if (flashMat != null && origMat != null)
                renderer.material = origMat;
        }
    }
}
