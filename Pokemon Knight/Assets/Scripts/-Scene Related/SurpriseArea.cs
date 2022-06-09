using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SurpriseArea : MonoBehaviour
{
    public string roomName;
    
	[Space] public Enemy[] enemies;
	public GameObject[] objs;
	[Space] [SerializeField] private bool once;
	private List<string> surprises;

	int gameNumber;


	private void Start() 
	{
		roomName = SceneManager.GetActiveScene().name + " " + this.name;
		gameNumber = PlayerPrefsElite.GetInt("gameNumber");

		if (PlayerPrefsElite.VerifyArray("surprises" + gameNumber))
		{
			surprises = new List<string>(PlayerPrefsElite.GetStringArray("surprises" + gameNumber));

			if (surprises.Contains(roomName))
			{
				once = true;
				this.enabled = false;
			}
			else
			{
				PrepareSurprises();
			}
		}
		else
		{
			PlayerPrefsElite.SetStringArray("surprises" + gameNumber, new string[0]);
			PrepareSurprises();
			surprises = new List<string>();
		}
	}

	void PrepareSurprises()
	{
		foreach (GameObject obj in  objs)
			obj.SetActive(false);
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (!once && other.CompareTag("Player"))	
		{
			once = true;
			foreach (GameObject obj in  objs)
				obj.SetActive(true);
			foreach (Enemy enemy in enemies)
				enemy.CallChilByOther();

			surprises.Add(roomName);
			PlayerPrefsElite.SetStringArray("surprises" + gameNumber, surprises.ToArray());
			this.enabled = false;
		}
	}
}
