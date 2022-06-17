using System.Collections;
using UnityEngine;

public class ColliderOnAfter : MonoBehaviour
{
	[System.Serializable] public class TurnOn
	{
		public Collider2D col;
		public float time=0.5f;
	}

    public float delayOn=0.5f;
    public float delayOff=0.5f;
    [SerializeField] private Collider2D[] cols;
	[Space] public TurnOn[] turnOns;



    // Start is called before the first frame update
    void Start()
    {
		if (turnOns != null && turnOns.Length > 0)
		{
			for (int i=0 ; i<turnOns.Length ; i++)
				if (turnOns[i] != null)
					StartCoroutine( TurnOnAfter(turnOns[i].col, turnOns[i].time) );
		}
		else
		{
			// if (cols == null)
			//     cols = this.GetComponent<Collider2D>();
			if (cols != null && cols.Length > 0 && delayOn > 0)
				StartCoroutine( EnableAfter() );
			else if (cols != null && cols.Length > 0 && delayOn == 0)
				foreach (Collider2D col in cols)
					col.enabled = true;
			if (cols != null && cols.Length > 0 && delayOff > 0)
				StartCoroutine( DisableAfter() );
		}
    }
    IEnumerator TurnOnAfter(Collider2D col, float time)
	{
        yield return new WaitForSeconds(time);
		col.enabled = true;
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
