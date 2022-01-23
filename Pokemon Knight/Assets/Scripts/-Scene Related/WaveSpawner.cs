using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [System.Serializable] public class Wave 
// {
//     public Enemy[] enemies;
// }


[System.Serializable] public class WaveSpawner : MonoBehaviour
{
    public Animator anim;
    public Enemy[] waves;
    public int enemiesLevel=3;
    public int miniBossLevel=5;
    public bool flipEnemies;
    [Space] public float jumpForce=5;
    // public int waveNumber=0;
    [Space] public WaveRoom waveManager;



    public IEnumerator SpawnWaves(int waveNumber, float delay=1)
    {
        // Debug.Log(this.name + " is spawning");
        if (waveNumber < waves.Length)
        {
            if (waves[ waveNumber ] != null)
            {
                anim.SetBool("spawn", true);

                yield return new WaitForSeconds(delay);
                var obj = Instantiate(waves[ waveNumber ], this.transform.position, Quaternion.identity, this.transform);
                obj.lv = enemiesLevel;
                obj.spawner = this;
                if (flipEnemies)
                    obj.model.transform.eulerAngles = new Vector3(0, 180);
                if (obj.isMiniBoss)
                {
                    waveManager.miniBoss = obj;
                    obj.lv = miniBossLevel;
                    obj.StartBossBattle(0);
                }
                obj.body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

                yield return new WaitForSeconds(0.25f);
                anim.SetBool("spawn", false);
            }
            else
                SpawnedDefeated();
        }
        else
            SpawnedDefeated();
    }

    public void SpawnedDefeated()
    {
        // Debug.Log(this.name + " defeated");
        waveManager.ASpawnerLost();
    }
}
