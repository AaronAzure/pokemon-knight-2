using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class Wave 
{
    public Enemy enemies;
    public bool canUseBuffs;
    [Space] public bool jumpOut=true;
}


[System.Serializable] public class WaveSpawner : MonoBehaviour
{


    public Animator anim;

    public Wave[] waves;
    public int enemiesLevel=3;
    public int miniBossLevel=5;
    public bool flipEnemies;
    public bool facePlayer;
    public bool alwaysAttackPlayer;
    [Space] public float jumpForce=5;
    // public int waveNumber=0;
    [Space] public WaveRoom waveManager;
    [Space] public SpriteRenderer mirrorImg;



    public IEnumerator SpawnWaves(int waveNumber, float delay=1)
    {
        // Debug.Log(this.name + " is spawning");
        if (waveNumber < waves.Length)
        {
            if (waves[ waveNumber ].enemies != null)
            {
                anim.SetBool("spawn", true);

				bool isMirror = false;
				if (mirrorImg != null && waves[ waveNumber ].enemies.pokemonSpr != null)
				{
					mirrorImg.gameObject.SetActive(false);
					mirrorImg.gameObject.SetActive(true);
					mirrorImg.sprite = waves[ waveNumber ].enemies.pokemonSpr;
					isMirror = true;
				}

                yield return new WaitForSeconds(delay);
                var obj = Instantiate(waves[ waveNumber ].enemies, this.transform.position, 
                    Quaternion.identity, this.transform);
                obj.lv = enemiesLevel;
                obj.isInRoom = true;
                obj.spawner = this;
                obj.spawnedByWave = true;
                obj.canUseBuffs = waves[ waveNumber ].canUseBuffs;
                if (flipEnemies)
                    obj.model.transform.eulerAngles = new Vector3(0, 180);
                if (facePlayer)
                    obj.GetComponent<Enemy>().LookAtTarget();
                if (alwaysAttackPlayer)
                    obj.alwaysAttackPlayer = true;
                if (obj.isMiniBoss)
                {
                    waveManager.miniBoss = obj;
                    obj.lv = miniBossLevel;
                    obj.StartBossBattle(0);
                    waveManager.hasABoss = true;
                }

				if (waves[ waveNumber ].jumpOut)
                	obj.body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

                yield return new WaitForSeconds(0.25f);
                anim.SetBool("spawn", false);
				if (isMirror)
				{
					mirrorImg.gameObject.SetActive(false);
					mirrorImg.sprite = null;
				}
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
        waveManager.ASpawnerLost(this);
    }
}
