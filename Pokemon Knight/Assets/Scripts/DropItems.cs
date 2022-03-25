using UnityEngine;

public class DropItems : MonoBehaviour
{
    [Space] public int rewardSize;
    // public Loot[] loots;
    public Currency expL;
    public Currency expM;
    public Currency expS;

    public void DropLoot(int bonus=1)
    {
        bonus = Mathf.Max(1, bonus);
        int spawnAmount = (rewardSize * bonus);

        // Debug.Log("amount = " + spawnAmount + ", bonus = "+ bonus);
        int nCandyL = Mathf.FloorToInt(spawnAmount / 100);
        int nCandyM = Mathf.FloorToInt( (spawnAmount % 100) / 10);
        int nCandyS = spawnAmount % 10;
        // Debug.Log("sml = " + nCandyS + ", med = " + nCandyM + ", lrg = " + nCandyL);

        for (int i=0; i<nCandyL ; i++)
        {
            var obj = Instantiate(
                expL, 
                transform.position + new Vector3(0,0.2f), 
                Quaternion.Euler(0,0,Random.Range(0,361))
            );
            obj.body.AddForce( new Vector2(Random.Range(-5,6), Random.Range(5,16)) , ForceMode2D.Impulse);
        }
        for (int i=0; i<nCandyM ; i++)
        {
            var obj = Instantiate(
                expM, 
                transform.position + new Vector3(0,0.2f), 
                Quaternion.Euler(0,0,Random.Range(0,361))
            );
            obj.body.AddForce( new Vector2(Random.Range(-5,6), Random.Range(5,16)) , ForceMode2D.Impulse);
        }
        for (int i=0; i<nCandyS ; i++)
        {
            var obj = Instantiate(
                expS, 
                transform.position + new Vector3(0,0.2f), 
                Quaternion.Euler(0,0,Random.Range(0,361))
            );
            obj.body.AddForce( new Vector2(Random.Range(-5,6), Random.Range(5,16)) , ForceMode2D.Impulse);
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