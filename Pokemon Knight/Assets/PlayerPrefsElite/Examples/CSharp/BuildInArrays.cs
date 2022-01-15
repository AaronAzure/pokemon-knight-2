using UnityEngine;
using System.Collections;

public class BuildInArrays : MonoBehaviour {

	// Declare built in array
	int[] IntArray;	
	
	// built in string array
	string[] StringArray;

	// built in float array
	float[] FloatArray;

	// declare public arrays for test
	public int[] testIntArray;
	public string[] testStringArray;
	public float[] testFloatArray;
	

	// Use this for initialization
	void Start () {


		//set values for IntArray at startup
		IntArray = new int[] {1, 2, 3, 4, 5};

		//set values for StringArray at startup
		StringArray = new string[] {"abc", "bcd"};

		//set values for FloatArray at startup
		FloatArray = new float[] {0.1f, 100, Random.Range(-10.0f, 10.0f)};

		// save it
		PlayerPrefsElite.SetIntArray("IntArray", IntArray);
		PlayerPrefsElite.SetStringArray("StringArray", StringArray);
		PlayerPrefsElite.SetFloatArray("FloatArray", FloatArray);

		//now verify and read from PlayerPrefs
		if (PlayerPrefsElite.VerifyArray("IntArray")){
			testIntArray = PlayerPrefsElite.GetIntArray("IntArray");
			Debug.Log("IntArray return true");
		}

		if (PlayerPrefsElite.VerifyArray("StringArray")){
			testStringArray = PlayerPrefsElite.GetStringArray("StringArray");
			Debug.Log("StringArray return true");
		}

		if (PlayerPrefsElite.VerifyArray("FloatArray")){
			testFloatArray = PlayerPrefsElite.GetFloatArray("FloatArray");
			Debug.Log("FloatArray return true");
		}
	
	}
	

}
