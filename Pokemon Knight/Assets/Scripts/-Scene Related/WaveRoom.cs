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

    private PlayerControls player;


    [Space] public int waveNumber = 0;
    public int totalWaves = 3;
    public WaveSpawner[] waveSpawners;
    public List<WaveSpawner> defeatedSpawners;
    private int spawnersDefeated;
    public Enemy miniBoss;
    public string[] roomsBeaten;

    [Header("Already Beaten")]
    [Space] [SerializeField] private GameObject newRoom;

    void Start() 
    {
        roomName = SceneManager.GetActiveScene().name + " " + this.name;
        roomsBeaten = new string[100];
        defeatedSpawners = new List<WaveSpawner>();

        if (newRoom != null)
            newRoom.SetActive(false);
        
        foreach (GameObject wall in walls)
            wall.SetActive(false);

        if (PlayerPrefsElite.VerifyArray("roomsBeaten" + PlayerPrefsElite.GetInt("gameNumber")))
        {
            roomsBeaten = PlayerPrefsElite.GetStringArray("roomsBeaten" + PlayerPrefsElite.GetInt("gameNumber"));
            var set = new HashSet<string>(roomsBeaten);
            // Debug.Log(roomsBeaten.Length + " " + set.Count);
            if (set.Contains(roomName))
            {
                if (newRoom != null)
                    newRoom.transform.parent = null; newRoom.SetActive(true);
                Destroy(this.gameObject);
            }

        }
        foreach (WaveSpawner ws in waveSpawners)
            ws.waveManager = this;

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

            if (player == null)
                player = other.GetComponent<PlayerControls>(); 
            
            StartCoroutine( StartWave(2) );
            other.GetComponent<PlayerControls>().EnteredWaveRoom();
            Walls(true);
        }
    }

    IEnumerator StartWave(float delay=0.5f)
    {
        // Debug.Log("Starting wave " + waveNumber);
        float spawnDelay = 1;
        if (waveNumber == totalWaves - 1)
            spawnDelay = 2;

        yield return new WaitForSeconds(delay);
        foreach (WaveSpawner ws in waveSpawners)
        {
            StartCoroutine( ws.SpawnWaves(waveNumber, spawnDelay) );
        }
    }

    public void ASpawnerLost(WaveSpawner spawner)
    {
        if (defeatedSpawners.Contains(spawner))
            return;
        defeatedSpawners.Add(spawner);
        spawnersDefeated++;

        if (spawnersDefeated >= waveSpawners.Length)
        {
            if (waveNumber < totalWaves - 1)
            {
                defeatedSpawners.Clear();
                spawnersDefeated = 0;
                waveNumber++;
                StartCoroutine( StartWave(1) );
            }
            else
            {
                defeatedSpawners.Clear();
                RoomBeaten();
            }
        }
    }

    public void RoomBeaten()
    {
        Walls(false);

        List<string> temp = new List<string>(
            PlayerPrefsElite.GetStringArray("roomsBeaten" + PlayerPrefsElite.GetInt("gameNumber")) 
        );
        temp.Add(roomName);
        PlayerPrefsElite.SetStringArray("roomsBeaten" + PlayerPrefsElite.GetInt("gameNumber"), temp.ToArray());
        // player.AddRoomBeaten(roomName);
    }
}
