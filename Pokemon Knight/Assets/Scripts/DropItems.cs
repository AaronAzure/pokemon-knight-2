using UnityEngine;

public class DropItems : MonoBehaviour
{
    [Space] public int rewardSize;
    // public Loot[] loots;
    public Currency expL;
    public Currency expM;
    public Currency expS;

    public void DropLoot(int bonus=0)
    {
        bonus = Mathf.Max(0, bonus);
        int spawnAmount = Mathf.RoundToInt( rewardSize * Mathf.Pow( 1.5f , bonus ) );

        int nCandyL = Mathf.FloorToInt(spawnAmount / 100);
        int nCandyM = Mathf.FloorToInt( (spawnAmount % 100) / 10);
        int nCandyS = spawnAmount % 10;

        for (int i=0; i<nCandyL ; i++)
        {
            var obj = Instantiate(
                expL, 
                transform.position + new Vector3(0,0.2f), 
                Quaternion.Euler(0,0,Random.Range(0,361)),
                transform.parent
            );
            obj.body.AddForce( new Vector2(Random.Range(-3,4), Random.Range(8,14)) , ForceMode2D.Impulse);
        }
        for (int i=0; i<nCandyM ; i++)
        {
            var obj = Instantiate(
                expM, 
                transform.position + new Vector3(0,0.2f), 
                Quaternion.Euler(0,0,Random.Range(0,361)),
                transform.parent.transform
            );
            obj.body.AddForce( new Vector2(Random.Range(-3,4), Random.Range(8,14)) , ForceMode2D.Impulse);
        }
        for (int i=0; i<nCandyS ; i++)
        {
            var obj = Instantiate(
                expS, 
                transform.position + new Vector3(0,0.2f), 
                Quaternion.Euler(0,0,Random.Range(0,361)),
                transform.parent.transform
            );
            obj.body.AddForce( new Vector2(Random.Range(-3,4), Random.Range(8,14)) , ForceMode2D.Impulse);
        }
    }
}


[System.Serializable]
public class Loot
{
    public Currency item;
    // [Range(0f,100f)] public float dropRate=100f;
    public int quantity=1;
    // public int maxQuantity=1;

}