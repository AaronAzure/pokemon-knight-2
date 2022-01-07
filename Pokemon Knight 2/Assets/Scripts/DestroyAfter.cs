using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float time = 0.5f;
    void Start()
    {
        Destroy(this.gameObject, time);
    }
}
