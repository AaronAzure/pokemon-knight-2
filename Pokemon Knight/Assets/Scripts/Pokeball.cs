using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class Pokeball : MonoBehaviour
{
    [Space] [Tooltip("PokeballTrail prefab - return back to player")] public FollowTowards trailObj;
    [Header("Flash")]
    [SerializeField] protected GameObject model;
    [SerializeField] protected SpriteRenderer[] renderers;
    [SerializeField] protected Material flashMat;
    public string powerup;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine( BackToBall() );
    }

    protected IEnumerator BackToBall()
    {
        yield return new WaitForSeconds(4f);
        int times = 40;
        float x = model.transform.localScale.x / times;
        foreach (SpriteRenderer renderer in renderers)
        {
            if (flashMat != null)
                renderer.material = flashMat;
        }
        for (int i=0 ; i<times ; i++)
        {
            model.transform.localScale -= new Vector3(x,x);
            // yield return new WaitForSeconds(0.01f);
            yield return new WaitForEndOfFrame();
        }
        if (trailObj != null)
        {
            var returnObj = Instantiate(trailObj, this.transform.position, Quaternion.identity, null);
            // returnObj.button = this.button;
            // returnObj.cooldownTime = this.resummonTime;
            var trainer = GameObject.Find("PLAYER").GetComponent<PlayerControls>();
            if (trainer != null)
            {
                returnObj.player = trainer;
                returnObj.target = trainer.transform;
                returnObj.powerupName = this.powerup;
                returnObj.justForShow = true;
            }
            else
            {
                Debug.LogError(" PlayerControls not assigned to Ally.trainer");
            }
        }

        // yield return new WaitForSeconds(0.01f);
        yield return new WaitForEndOfFrame();
        Destroy(this.gameObject);
    }
}
