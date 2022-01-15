using UnityEngine;
using System.Collections;

public class SecureKeysManager : MonoBehaviour {

	public string[] keys;

	// Use this for initialization
	void Awake () {
		PlayerPrefsElite.setKeys(keys);
		if (!Application.isEditor){Destroy(this);}	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
