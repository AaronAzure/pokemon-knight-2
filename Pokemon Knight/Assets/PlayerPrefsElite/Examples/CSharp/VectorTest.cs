using UnityEngine;
using System.Collections;

public class VectorTest : MonoBehaviour {

	// Declare Vector2 Vector3 Vector4
	Vector2 myVector2;
	public Vector2 myNewVector2;
	Vector3 myVector3;
	public Vector3 myNewVector3;
	Vector4 myVector4;
	public Vector4 myNewVector4;


	// Use this for initialization
	void Start () {

		// Vector2 test
		//set values for myVector2 at startup
		myVector2 = new Vector2(Random.Range(0.0f,100.0f), 1.0f);

		// save it to player preferences
		PlayerPrefsElite.SetVector2("myVector2", myVector2);

		//verify and read from PlayerPrefs
		if (PlayerPrefsElite.VerifyVector2("myVector2")){
			// Assign myVector2 from player preferences to myNewVector2
			myNewVector2 = PlayerPrefsElite.GetVector2("myVector2");
			Debug.Log("myNewVector2 return true");
		}



		// Vector3 test
		//set values for myVector3 at startup
		myVector3 = new Vector3(Random.Range(0.0f,100.0f), 2.0f, 3);
		
		// save it to player preferences
		PlayerPrefsElite.SetVector3("myVector3", myVector3);
		
		//verify and read from PlayerPrefs
		if (PlayerPrefsElite.VerifyVector3("myVector3")){
			// Assign myVector3 from player preferences to myNewVector3
			myNewVector3 = PlayerPrefsElite.GetVector3("myVector3");
			Debug.Log("myNewVector3 return true");
		}


		// Vector4 test
		//set values for myVector4 at startup
		myVector4 = new Vector4(Random.Range(0.0f,100.0f), 2.0f, 3, 100);
		
		// save it to player preferences
		PlayerPrefsElite.SetVector4("myVector4", myVector4);
		
		//verify and read from PlayerPrefs
		if (PlayerPrefsElite.VerifyVector4("myVector4")){
			// Assign myVector4 from player preferences to myNewVector4
			myNewVector4 = PlayerPrefsElite.GetVector4("myVector4");
			Debug.Log("myNewVector4 return true");
		}



	}
	
}
