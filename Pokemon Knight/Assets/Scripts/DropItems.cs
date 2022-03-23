using UnityEngine;

public class DropItems : MonoBehaviour
{
    [Space] public int rewardSize;
    public Loot[] loots;

    public void DropLoot(int bonus=1)
    {
        // Debug.Log("<color=cyan>Dropping Loot</color>");
        foreach (Loot loot in loots)
        {
            // int spawnAmount = (rewardSize * bonus);
            int spawnAmount = (loot.quantity * bonus);
            for (int i=0 ; i<spawnAmount ; i++)
            {
                var obj = Instantiate(loot.item, 
                    transform.position + new Vector3(0,0.2f), 
                    Quaternion.Euler(0,0,Random.Range(0,361))
                );
                obj.body.AddForce( new Vector2(Random.Range(-5,6), Random.Range(5,16)) , ForceMode2D.Impulse);
            }
        }
    }

    public void DropSpecificItems(int amount)
    {
        // Debug.Log("<color=cyan>Dropping Loot</color>");
        int candyL = Mathf.FloorToInt( amount / 125 ); 
        int candyM = Mathf.FloorToInt( (candyL % 125) / 25 ); 
        int candyS = candyL % 25; 

        int maxCandySSpawn = Mathf.Min(candyS, 25);
        for (int i=0 ; i<maxCandySSpawn ; i++)
        {
            var obj = Instantiate(loots[0].item, 
                transform.position, 
                Quaternion.Euler(0,0,Random.Range(0,361))
            );
            obj.body.AddForce( new Vector2(Random.Range(-5,6), Random.Range(10,16)) , ForceMode2D.Impulse);
        }
        int maxCandyMSpawn = Mathf.Min(candyM, 25);
        for (int i=0 ; i<maxCandyMSpawn ; i++)
        {
            var obj = Instantiate(loots[1].item, 
                transform.position, 
                Quaternion.Euler(0,0,Random.Range(0,361))
            );
            obj.body.AddForce( new Vector2(Random.Range(-5,6), Random.Range(10,16)) , ForceMode2D.Impulse);
        }
        int maxCandyLSpawn = Mathf.Min(candyL, 25);
        for (int i=0 ; i<maxCandyLSpawn ; i++)
        {
            var obj = Instantiate(loots[2].item, 
                transform.position, 
                Quaternion.Euler(0,0,Random.Range(0,361))
            );
            obj.body.AddForce( new Vector2(Random.Range(-5,6), Random.Range(10,16)) , ForceMode2D.Impulse);
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