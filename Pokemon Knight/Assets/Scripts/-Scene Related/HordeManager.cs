using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HordeManager : MonoBehaviour
{
    private PlayerControls playerControls;
    [SerializeField] private string roomName;
    [Space] [SerializeField] private GameObject subwayDoorHolder;
    private List<string> subwaysCleared;
    [Space] [SerializeField] private Collider2D benchCol;
    [SerializeField] private List<Enemy> enemies;
    private bool beaten;
    private bool started;


    // Start is called before the first frame update
    void Start()
    {

        roomName = SceneManager.GetActiveScene().name + " " + this.name;
        subwayDoorHolder.gameObject.SetActive(false);

        if (benchCol != null)
            benchCol.enabled = false;

        if (PlayerPrefsElite.VerifyArray("subwaysCleared" + PlayerPrefsElite.GetInt("gameNumber")))
        {
            subwaysCleared = new List<string>( 
                PlayerPrefsElite.GetStringArray("subwaysCleared" + PlayerPrefsElite.GetInt("gameNumber"))
            );
            // var set = new HashSet<string>(subwaysCleared);
            if (subwaysCleared.Contains(roomName))
            {
                subwayDoorHolder.gameObject.SetActive(true);
                beaten = true;
                if (benchCol != null)
                    benchCol.enabled = true;
                foreach (Enemy enemy in enemies)
                    Destroy(enemy.gameObject);
                this.enabled = false;
            }
        }
        else
        {
            subwaysCleared = new List<string>();
            PlayerPrefsElite.SetStringArray("subwaysCleared" + PlayerPrefsElite.GetInt("gameNumber"), 
                subwaysCleared.ToArray()
            ); 
        }

        foreach (Enemy enemy in enemies)
            enemy.horde = this;
        playerControls = GameObject.Find("PLAYER").GetComponent<PlayerControls>();
        started = true;
    }

    
    public void RemoveFromEnemies(Enemy enemy=null)
    {
        if (enemies.Contains(enemy))
            enemies.Remove(enemy);
        else
            Debug.Log("<color=red>Not in Enemy list</color>", this.gameObject);
        
        if (enemies.Count <= 0 || enemies == null)
        {
            subwaysCleared.Add(roomName);
            subwayDoorHolder.gameObject.SetActive(true);

            PlayerPrefsElite.SetStringArray("subwaysCleared" + PlayerPrefsElite.GetInt("gameNumber"), 
                subwaysCleared.ToArray()
            ); 

            playerControls.CheckSubwaysCleared();
            beaten = true;

            if (benchCol != null)
                benchCol.enabled = true;
            this.enabled = false;

            if (playerControls != null)
                playerControls.BossBattleOver();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (!beaten && started && other.CompareTag("Player"))    
        {
            if (playerControls == null)
                playerControls = other.GetComponent<PlayerControls>();
            playerControls.StartHordeBattleMusic();
        }
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (!beaten && started && other.CompareTag("Player"))    
        {
            if (playerControls == null)
                playerControls = other.GetComponent<PlayerControls>();
            playerControls.BossBattleOver();
        }
    }
}


// [System.Serializable]
// public class Bench
// {
//     public Interactable interactableScript;
//     public RestBench benchScript;
// }