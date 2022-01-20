using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [System.Serializable] public class Wave 
// {
//     public Enemy[] enemies;
// }


[System.Serializable] public class WaveSpawner : MonoBehaviour
{
    public Enemy[] waves;
    public int enemiesLevel=3;
    public int miniBossLevel=5;
    // public int waveNumber=0;
    [Space] public WaveRoom waveManager;


    public void SpawnWaves(int waveNumber)
    {
        Debug.Log(this.name + " is spawning");
        if (waveNumber < waves.Length)
        {
            if (waves[ waveNumber ] != null)
            {
                var obj = Instantiate(waves[ waveNumber ], this.transform.position, Quaternion.identity, this.transform);
                obj.lv = enemiesLevel;
                obj.spawner = this;
            }
            else
                SpawnedDefeated();
        }
        else
            SpawnedDefeated();
    }

    public void SpawnedDefeated()
    {
        Debug.Log(this.name + " defeated");
        waveManager.ASpawnerLost();
    }
}
