using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class WaveRoom : MonoBehaviour
{
    // [SerializeField] private Enemy boss;
    [SerializeField] private string roomName;
    [SerializeField] private GameObject[] walls;
    private bool once;
    [SerializeField] private CinemachineVirtualCamera cm;


    [Space] public int waveNumber = 0;
    public int totalWaves = 3;
    public WaveSpawner[] waveSpawners;
    private int spawnersDefeated;
    public Enemy miniBoss;

    void Start() 
    {
        roomName = SceneManager.GetActiveScene().name + " " + this.name;

        if (PlayerPrefsElite.VerifyArray("waveRooms"))
        {
            string[] roomsBeaten = PlayerPrefsElite.GetStringArray("waveRooms");
            foreach (string roomBeaten in roomsBeaten)
                if (roomBeaten == roomName)
                    Destroy(this.gameObject);

        }
        foreach (WaveSpawner ws in waveSpawners)
        {
            ws.waveManager = this;
        }
    }

    public void Walls(bool active)
    {
        foreach (GameObject wall in walls)
            wall.SetActive(active);
        if (!active)
            cm.Priority = -100;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (!once && other.CompareTag("Player"))    
        {
            once = true;
            // if (boss == null)
            //     return;
            cm.Priority = 100;
            once = true;
            
            StartCoroutine( StartWave(2) );
            Walls(true);
        }
    }

    IEnumerator StartWave(float delay=0.5f)
    {
        Debug.Log("Starting wave " + waveNumber);
        float spawnDelay = 1;
        if (waveNumber == totalWaves - 1)
            spawnDelay = 2;

        yield return new WaitForSeconds(delay);
        foreach (WaveSpawner ws in waveSpawners)
        {
            StartCoroutine( ws.SpawnWaves(waveNumber, spawnDelay) );
        }
    }

    public void ASpawnerLost()
    {
        spawnersDefeated++;

        if (spawnersDefeated >= waveSpawners.Length)
        {
            if (waveNumber < totalWaves - 1)
            {
                spawnersDefeated = 0;
                waveNumber++;
                StartCoroutine( StartWave(1) );
            }
            else
            {
                RoomBeaten();
            }
        }
    }

    public void RoomBeaten()
    {
        string[] roomsBeaten = PlayerPrefsElite.GetStringArray("waveRooms");
        for (int i=0 ; i < roomsBeaten.Length ; i++)
        {
            if (roomsBeaten[i] == "")
            {
                roomsBeaten[i] = roomName;
                PlayerPrefsElite.SetStringArray("waveRooms", roomsBeaten);
                break;
            }
        }
        Walls(false);
    }
}