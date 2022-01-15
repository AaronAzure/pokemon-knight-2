using UnityEngine;
using System.Collections;

public class PlayerPrefsElite : MonoBehaviour {

	public static string prefix = "sum."; 
	public static string _prefix = "ch."; 
	public static string prefix2 = "ar."; 
	public static string _prefix2 = "rd."; 
	public static string[] key;
	private static int id = 0;

	public static void SetInt(string Prefs, int Value){
		PlayerPrefs.SetInt(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}

	public static void SetInt(string Prefs, int Value, int id){
		PlayerPrefs.SetInt(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}

	public static void SetFloat(string Prefs, float Value){
		PlayerPrefs.SetFloat(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}

	public static void SetFloat(string Prefs, float Value, int id){
		PlayerPrefs.SetFloat(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}

	public static void SetString(string Prefs, string Value){
		PlayerPrefs.SetString(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}

	public static void SetString(string Prefs, string Value, int id){
		PlayerPrefs.SetString(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}

	public static int GetInt(string Prefs){
		return PlayerPrefs.GetInt(Prefs);
	}

	public static float GetFloat(string Prefs){
		return PlayerPrefs.GetFloat(Prefs);
	}

	public static string GetString(string Prefs){
		return PlayerPrefs.GetString(Prefs);
	}


	public static bool VerifyString(string Prefs){
		if (sum(PlayerPrefs.GetString(Prefs), key[id]) == PlayerPrefs.GetString(sum(prefix+Prefs, key[id]))){
			return true;
		}else{
			return false;
		}
	}

	public static bool VerifyString(string Prefs, int id){
		if (sum(PlayerPrefs.GetString(Prefs), key[id]) == PlayerPrefs.GetString(sum(prefix+Prefs, key[id]))){
			return true;
		}else{
			return false;
		}
	}

	public static bool VerifyInt(string Prefs){
		if (sum(PlayerPrefs.GetInt(Prefs).ToString(), key[id]) == PlayerPrefs.GetString(sum(prefix+Prefs, key[id]))){
			return true;
		}else{
			return false;
		}
	}

	public static bool VerifyInt(string Prefs, int id){
		if (sum(PlayerPrefs.GetInt(Prefs).ToString(), key[id]) == PlayerPrefs.GetString(sum(prefix+Prefs, key[id]))){
			return true;
		}else{
			return false;
		}
	}

	public static bool VerifyFloat(string Prefs){
		if (sum(PlayerPrefs.GetFloat(Prefs).ToString(), key[id]) == PlayerPrefs.GetString(sum(prefix+Prefs, key[id]))){
			return true;
		}else{
			return false;
		}
	}

	public static bool VerifyFloat(string Prefs, int id){
		if (sum(PlayerPrefs.GetFloat(Prefs).ToString(), key[id]) == PlayerPrefs.GetString(sum(prefix+Prefs, key[id]))){
			return true;
		}else{
			return false;
		}
	}

	public static void Encrypt(string Prefs, string Value){
		PlayerPrefs.SetString(_prefix+sum(_prefix+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}

	public static void Encrypt(string Prefs, string Value, int id){
		PlayerPrefs.SetString(_prefix+sum(_prefix+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}

	public static bool CompareEncrypt(string Prefs, string Value){
		if (PlayerPrefs.GetString(_prefix+sum(_prefix+Prefs, key[id])) == sum(Value, key[id])){
			return true;
		}else{
			return false;
		}
	}

	public static bool CompareEncrypt(string Prefs, string Value, int id){
		if (PlayerPrefs.GetString(_prefix+sum(_prefix+Prefs, key[id])) == sum(Value, key[id])){
			return true;
		}else{
			return false;
		}
	}

	public static void setKeys (string[] keys){
		key = new string[keys.Length];
		key = keys;
	}

	public static string sum(string strToEncrypt, string key){
		#if UNITY_WINRT
		System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
		byte[] bytes = encoding.GetBytes(strToEncrypt+key);
		byte[] hash = UnityEngine.Windows.Crypto.ComputeMD5Hash(bytes);
		
		string t = "";
		for (int i = 0; i < hash.Length; i++){
			t += string.Format("{0:x2}", hash[i]);
		}
		return t;
		#else
		System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
		byte[] bytes = encoding.GetBytes(strToEncrypt+key);
		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash(bytes);
		string hashString = "";
		for (int i = 0; i < hashBytes.Length; i++)
		{
			hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
		}
		return hashString.PadLeft(32, '0');
		#endif	
	}

	public static void SetStringArray(string Prefs, string[] _Value){
		string Value = "";
		for( int y = 0; y < _Value.Length; y++ ){	Value+=_Value[y] + "|";}
		PlayerPrefs.SetString(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix2+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}

	public static void SetStringArray(string Prefs, string[] _Value, int id){
		string Value = "";
		for( int y = 0; y < _Value.Length; y++ ){	Value+=_Value[y] + "|";}
		PlayerPrefs.SetString(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix2+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}

	public static void SetStringArray(string Prefs, ArrayList _Value){
		string Value = "";
		for( int y = 0; y < _Value.Count; y++ ){	Value+=_Value[y] + "|";}
		PlayerPrefs.SetString(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix2+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}

	public static void SetStringArray(string Prefs, ArrayList _Value, int id){
		string Value = "";
		for( int y = 0; y < _Value.Count; y++ ){	Value+=_Value[y] + "|";}
		PlayerPrefs.SetString(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix2+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}
	

	public static void SetIntArray(string Prefs, int[] _Value){
		string Value = "";
		for( int y = 0; y < _Value.Length; y++ ){	Value+=_Value[y].ToString() + "|";}
		PlayerPrefs.SetString(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix2+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}

	public static void SetIntArray(string Prefs, int[] _Value, int id){
		string Value = "";
		for( int y = 0; y < _Value.Length; y++ ){	Value+=_Value[y].ToString() + "|";}
		PlayerPrefs.SetString(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix2+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}

	public static void SetIntArray(string Prefs, ArrayList _Value){
		string Value = "";
		for( int y = 0; y < _Value.Count; y++ ){	Value+=_Value[y].ToString() + "|";}
		PlayerPrefs.SetString(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix2+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}
	
	public static void SetIntArray(string Prefs, ArrayList _Value, int id){
		string Value = "";
		for( int y = 0; y < _Value.Count; y++ ){	Value+=_Value[y].ToString() + "|";}
		PlayerPrefs.SetString(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix2+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}

	public static void SetFloatArray(string Prefs, float[] _Value){
		string Value = "";
		for( int y = 0; y < _Value.Length; y++ ){	Value+=_Value[y].ToString() + "|";}
		PlayerPrefs.SetString(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix2+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}

	public static void SetFloatArray(string Prefs, float[] _Value, int id){
		string Value = "";
		for( int y = 0; y < _Value.Length; y++ ){	Value+=_Value[y].ToString() + "|";}
		PlayerPrefs.SetString(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix2+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}

	public static void SetFloatArray(string Prefs, ArrayList _Value){
		string Value = "";
		for( int y = 0; y < _Value.Count; y++ ){	Value+=_Value[y].ToString() + "|";}
		PlayerPrefs.SetString(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix2+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}
	
	public static void SetFloatArray(string Prefs, ArrayList _Value, int id){
		string Value = "";
		for( int y = 0; y < _Value.Count; y++ ){	Value+=_Value[y].ToString() + "|";}
		PlayerPrefs.SetString(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix2+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}

	public static string[] GetStringArray(string Prefs){
		string[] tmp = PlayerPrefs.GetString(Prefs).Split("|"[0]);
		string[] myString = new string[tmp.Length-1];
		for (int i = 0; i<tmp.Length-1; i++){
			myString[i]=tmp[i];
		}
		return myString;
	}

	public static int[] GetIntArray(string Prefs){
		string[] tmp = PlayerPrefs.GetString(Prefs).Split("|"[0]);
		int[] myInt = new int[tmp.Length-1];
		for (int i = 0; i<tmp.Length-1; i++){
			myInt[i]=int.Parse(tmp[i]);
		}
		return myInt;
	}

	public static float[] GetFloatArray(string Prefs){
		string[] tmp = PlayerPrefs.GetString(Prefs).Split("|"[0]);
		float[] myFloat = new float[tmp.Length-1];
		for (int i = 0; i<tmp.Length-1; i++){
			myFloat[i]=float.Parse(tmp[i]);
		}
		return myFloat;
	}

	public static bool VerifyArray(string Prefs){
		if (sum(PlayerPrefs.GetString(Prefs), key[id]) == PlayerPrefs.GetString(sum(prefix2+Prefs, key[id]))){
			return true;
		}else{
			return false;
		}
	}

	public static bool VerifyArray(string Prefs, int id){
		if (sum(PlayerPrefs.GetString(Prefs), key[id]) == PlayerPrefs.GetString(sum(prefix2+Prefs, key[id]))){
			return true;
		}else{
			return false;
		}
	}

	public static void SetVector2(string Prefs, Vector2 _Value){
		string Value = _Value.x+"|"+_Value.y+"|";
		PlayerPrefs.SetString(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix2+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}

	public static void SetVector2(string Prefs, Vector2 _Value, int id){
		string Value = _Value.x+"|"+_Value.y+"|";
		PlayerPrefs.SetString(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix2+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}

	public static Vector2 GetVector2(string Prefs){
		string[] tmp = PlayerPrefs.GetString(Prefs).Split("|"[0]);
		Vector2 myVector = new Vector2(float.Parse(tmp[0]), float.Parse(tmp[1]));
		return myVector;
	}
	
	public static bool VerifyVector2(string Prefs){
		if (sum(PlayerPrefs.GetString(Prefs), key[id]) == PlayerPrefs.GetString(sum(prefix2+Prefs, key[id]))){
			return true;
		}else{
			return false;
		}
	}

	public static bool VerifyVector2(string Prefs, int id){
		if (sum(PlayerPrefs.GetString(Prefs), key[id]) == PlayerPrefs.GetString(sum(prefix2+Prefs, key[id]))){
			return true;
		}else{
			return false;
		}
	}

	public static void SetVector3(string Prefs, Vector3 _Value){
		string Value = _Value.x+"|"+_Value.y+"|"+_Value.z+"|";
		PlayerPrefs.SetString(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix2+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}
	
	public static void SetVector3(string Prefs, Vector3 _Value, int id){
		string Value = _Value.x+"|"+_Value.y+"|"+_Value.z+"|";
		PlayerPrefs.SetString(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix2+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}

	public static Vector3 GetVector3(string Prefs){
		string[] tmp = PlayerPrefs.GetString(Prefs).Split("|"[0]);
		Vector3 myVector = new Vector3(float.Parse(tmp[0]), float.Parse(tmp[1]), float.Parse(tmp[2]));
		return myVector;
	}
	
	public static bool VerifyVector3(string Prefs){
		if (sum(PlayerPrefs.GetString(Prefs), key[id]) == PlayerPrefs.GetString(sum(prefix2+Prefs, key[id]))){
			return true;
		}else{
			return false;
		}
	}
	
	public static bool VerifyVector3(string Prefs, int id){
		if (sum(PlayerPrefs.GetString(Prefs), key[id]) == PlayerPrefs.GetString(sum(prefix2+Prefs, key[id]))){
			return true;
		}else{
			return false;
		}
	}

	public static void SetVector4(string Prefs, Vector4 _Value){
		string Value = _Value.x+"|"+_Value.y+"|"+_Value.z+"|"+_Value.w+"|";
		PlayerPrefs.SetString(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix2+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}
	
	public static void SetVector4(string Prefs, Vector4 _Value, int id){
		string Value = _Value.x+"|"+_Value.y+"|"+_Value.z+"|"+_Value.w+"|";
		PlayerPrefs.SetString(Prefs, Value);
		PlayerPrefs.SetString(sum(prefix2+Prefs, key[id]), sum(Value.ToString(), key[id]));
	}
	
	public static Vector4 GetVector4(string Prefs){
		string[] tmp = PlayerPrefs.GetString(Prefs).Split("|"[0]);
		Vector4 myVector = new Vector4(float.Parse(tmp[0]), float.Parse(tmp[1]), float.Parse(tmp[2]), float.Parse(tmp[3]));
		return myVector;
	}

	public static bool VerifyVector4(string Prefs){
		if (sum(PlayerPrefs.GetString(Prefs), key[id]) == PlayerPrefs.GetString(sum(prefix2+Prefs, key[id]))){
			return true;
		}else{
			return false;
		}
	}
	
	public static bool VerifyVector4(string Prefs, int id){
		if (sum(PlayerPrefs.GetString(Prefs), key[id]) == PlayerPrefs.GetString(sum(prefix2+Prefs, key[id]))){
			return true;
		}else{
			return false;
		}
	}
	
	public static void SetBoolean(string Prefs, bool _Value){
		PlayerPrefs.SetInt(Prefs, _Value ? 1 : 0);
		PlayerPrefs.SetString(sum(_prefix2+Prefs, key[id]), sum(Random.Range(100000,100000000).ToString(), key[id]));
		PlayerPrefs.SetString(sum(Prefs, key[id]), sum(PlayerPrefs.GetString(sum(_prefix2+Prefs, key[id])) + (_Value ? 1 : 0).ToString(), key[id]));
	}

	public static void SetBoolean(string Prefs, bool _Value, int id){
		PlayerPrefs.SetInt(Prefs, _Value ? 1 : 0);
		PlayerPrefs.SetString(sum(_prefix2+Prefs, key[id]), sum(Random.Range(100000,100000000).ToString(), key[id]));
		PlayerPrefs.SetString(sum(Prefs, key[id]), sum(PlayerPrefs.GetString(sum(_prefix2+Prefs, key[id])) + (_Value ? 1 : 0).ToString(), key[id]));
	}

	public static bool GetBoolean(string Prefs){
		if (PlayerPrefs.GetInt(Prefs)==1){
			return true;
		}else{
			return false;
		}
	}

	public static bool VerifyBoolean(string Prefs){
		if ( PlayerPrefs.GetString(sum(Prefs, key[id])) == sum(PlayerPrefs.GetString(sum(_prefix2+Prefs, key[id])) + PlayerPrefs.GetInt(Prefs).ToString(), key[id])){
			return true;
		}else{
			return false;
		}
	}

	public static bool VerifyBoolean(string Prefs, int id){
		if ( PlayerPrefs.GetString(sum(Prefs, key[id])) == sum(PlayerPrefs.GetString(sum(_prefix2+Prefs, key[id])) + PlayerPrefs.GetInt(Prefs).ToString(), key[id])){
			return true;
		}else{
			return false;
		}
	}

	public static void DeleteKey(string Prefs){
		PlayerPrefs.DeleteKey(Prefs);
	}

	public static void DeleteSecureKey(string Prefs){
		PlayerPrefs.DeleteKey(Prefs);
		PlayerPrefs.DeleteKey(sum(prefix+Prefs, key[id]));
	}

	public static void DeleteSecureKey(string Prefs, int id){
		PlayerPrefs.DeleteKey(Prefs);
		PlayerPrefs.DeleteKey(sum(prefix+Prefs, key[id]));
	}

	public static string EncryptString(string strToEncrypt){
		#if UNITY_WINRT
		int toInt = 0;
		int intStr = 0;
		int toIntStr = 0;
		System.Text.UTF8Encoding bytesToEncodeencoding = new System.Text.UTF8Encoding();
		byte[] bytesToEncode = bytesToEncodeencoding.GetBytes(strToEncrypt);
		string encodedText = System.Convert.ToBase64String (bytesToEncode);
		strToEncrypt = encodedText;
		System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
		byte[] secureKeyBytes = encoding.GetBytes(PlayerPrefsElite.key[id]);
		string secureKeyEncoded = System.Convert.ToBase64String (secureKeyBytes);
		int[] secureKey = new int[secureKeyEncoded.Length];
		for (int x = 0; x < secureKeyEncoded.Length; x++){
			secureKey[x] = System.Convert.ToInt32(secureKeyEncoded[x]);
		}
		string strEncrypt = "";
		string subEncrypt = "";
		for (int y = 0; y <= strToEncrypt.Length - 1; y++){
			subEncrypt = strToEncrypt.Substring(y, 1);
			encoding = new System.Text.UTF8Encoding();
			intStr = (int)encoding.GetBytes(subEncrypt)[0];
			toInt = y % secureKey.Length;
			toIntStr = intStr + System.Convert.ToInt32(secureKey[toInt]); // get char number
			toIntStr = toIntStr % 256;
			strEncrypt = strEncrypt + toIntStr.ToString("X2");
		}
		return strEncrypt;
		#else
		byte[] bytesToEncode = System.Text.Encoding.UTF8.GetBytes (strToEncrypt);
		string encodedText = System.Convert.ToBase64String (bytesToEncode);
		strToEncrypt = encodedText;
		int toInt = 0;
		int intStr = 0;
		int toIntStr = 0;
		byte[] secureKeyBytes = System.Text.Encoding.UTF8.GetBytes (PlayerPrefsElite.key[id]);
		string secureKeyEncoded = System.Convert.ToBase64String (secureKeyBytes);
		int[] secureKey = new int[secureKeyEncoded.Length];
		for (int x = 0; x < secureKeyEncoded.Length; x++){	
			secureKey[x] = System.Convert.ToInt32(secureKeyEncoded[x]);
		}
		string strEncrypt = "";
		for (int y = 0; y <= strToEncrypt.Length - 1; y++){
			intStr = (int)System.Text.Encoding.ASCII.GetBytes(strToEncrypt.Substring(y, 1))[0];
			toInt = y % secureKey.Length;
			toIntStr = intStr + System.Convert.ToInt32(secureKey[toInt]); 
			toIntStr = toIntStr % 256;
			strEncrypt = strEncrypt + toIntStr.ToString("X2");
		}
		return strEncrypt;
		#endif
	}

	public static string EncryptString(string strToEncrypt, int id){
		#if UNITY_WINRT
		int toInt = 0;
		int intStr = 0;
		int toIntStr = 0;
		System.Text.UTF8Encoding bytesToEncodeencoding = new System.Text.UTF8Encoding();
		byte[] bytesToEncode = bytesToEncodeencoding.GetBytes(strToEncrypt);
		string encodedText = System.Convert.ToBase64String (bytesToEncode);
		strToEncrypt = encodedText;
		System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
		byte[] secureKeyBytes = encoding.GetBytes(PlayerPrefsElite.key[id]);
		string secureKeyEncoded = System.Convert.ToBase64String (secureKeyBytes);
		int[] secureKey = new int[secureKeyEncoded.Length];
		for (int x = 0; x < secureKeyEncoded.Length; x++){
			secureKey[x] = System.Convert.ToInt32(secureKeyEncoded[x]);
		}
		string strEncrypt = "";
		string subEncrypt = "";
		for (int y = 0; y <= strToEncrypt.Length - 1; y++){
			subEncrypt = strToEncrypt.Substring(y, 1);
			encoding = new System.Text.UTF8Encoding();
			intStr = (int)encoding.GetBytes(subEncrypt)[0];
			toInt = y % secureKey.Length;
			toIntStr = intStr + System.Convert.ToInt32(secureKey[toInt]); // get char number
			toIntStr = toIntStr % 256;
			strEncrypt = strEncrypt + toIntStr.ToString("X2");
		}
		return strEncrypt;
		#else
		byte[] bytesToEncode = System.Text.Encoding.UTF8.GetBytes (strToEncrypt);
		string encodedText = System.Convert.ToBase64String (bytesToEncode);
		strToEncrypt = encodedText;
		int toInt = 0;
		int intStr = 0;
		int toIntStr = 0;
		byte[] secureKeyBytes = System.Text.Encoding.UTF8.GetBytes (PlayerPrefsElite.key[id]);
		string secureKeyEncoded = System.Convert.ToBase64String (secureKeyBytes);
		int[] secureKey = new int[secureKeyEncoded.Length];
		for (int x = 0; x < secureKeyEncoded.Length; x++){	
			secureKey[x] = System.Convert.ToInt32(secureKeyEncoded[x]);
		}
		string strEncrypt = "";
		for (int y = 0; y <= strToEncrypt.Length - 1; y++){
			intStr = (int)System.Text.Encoding.ASCII.GetBytes(strToEncrypt.Substring(y, 1))[0];
			toInt = y % secureKey.Length;
			toIntStr = intStr + System.Convert.ToInt32(secureKey[toInt]); 
			toIntStr = toIntStr % 256;
			strEncrypt = strEncrypt + toIntStr.ToString("X2");
		}
		return strEncrypt;
		#endif
	}
	
	public static string DecryptString(string strToEncrypt){
		#if UNITY_WINRT
		int toInt = 0;
		int intStr = 0;
		int toIntStr = 0;
		System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
		byte[] secureKeyBytes = encoding.GetBytes(PlayerPrefsElite.key[id]);
		string secureKeyEncoded = System.Convert.ToBase64String (secureKeyBytes);
		int[] secureKey = new int[secureKeyEncoded.Length];
		for (int x = 0; x < secureKeyEncoded.Length; x++){
			secureKey[x] = System.Convert.ToInt32(secureKeyEncoded[x]);
		}
		string toDc = "";
		string strEncrypt = "";
		for (int y = 0; y <= strToEncrypt.Length-1; y+=2){
			intStr = int.Parse(strToEncrypt.Substring(y, 1), System.Globalization.NumberStyles.HexNumber) * 16 + int.Parse(strToEncrypt.Substring((y + 1), 1), System.Globalization.NumberStyles.HexNumber);
			toInt = (y / 2) % secureKey.Length;
			toIntStr = intStr - System.Convert.ToInt32(secureKey[toInt]) + 256;
			toIntStr = toIntStr % 256;
			toDc = ((char)toIntStr).ToString();
			strEncrypt = strEncrypt + toDc;
		}
		byte[] decodedBytes = System.Convert.FromBase64String (strEncrypt);
		System.Text.UTF8Encoding decodedText = new System.Text.UTF8Encoding();
		string decodedTexta = decodedText.GetString(decodedBytes, 0, decodedBytes.Length);
		return decodedTexta;
		#else
		int toInt = 0;
		int intStr = 0;
		int toIntStr = 0;
		byte[] secureKeyBytes = System.Text.Encoding.UTF8.GetBytes (PlayerPrefsElite.key[id]);
		string secureKeyEncoded = System.Convert.ToBase64String (secureKeyBytes);
		int[] secureKey = new int[secureKeyEncoded.Length];
		for (int x = 0; x < secureKeyEncoded.Length; x++){
			secureKey[x] = System.Convert.ToInt32(secureKeyEncoded[x]);
		}
		string toDc = "";
		string strEncrypt = "";
		for (int y = 0; y <= strToEncrypt.Length-1; y+=2){
			intStr = int.Parse(strToEncrypt.Substring(y, 1), System.Globalization.NumberStyles.HexNumber) * 16 + int.Parse(strToEncrypt.Substring((y + 1), 1), System.Globalization.NumberStyles.HexNumber);
			toInt = (y / 2) % secureKey.Length;
			toIntStr = intStr - System.Convert.ToInt32(secureKey[toInt]) + 256;
			toIntStr = toIntStr % 256;
			toDc = ((char)toIntStr).ToString();
			strEncrypt = strEncrypt + toDc;
		}
		byte[] decodedBytes = System.Convert.FromBase64String (strEncrypt);
		string decodedText = System.Text.Encoding.UTF8.GetString (decodedBytes);
		return decodedText;
		#endif
	}

	public static string DecryptString(string strToEncrypt, int id){
		#if UNITY_WINRT
		int toInt = 0;
		int intStr = 0;
		int toIntStr = 0;
		System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
		byte[] secureKeyBytes = encoding.GetBytes(PlayerPrefsElite.key[id]);
		string secureKeyEncoded = System.Convert.ToBase64String (secureKeyBytes);
		int[] secureKey = new int[secureKeyEncoded.Length];
		for (int x = 0; x < secureKeyEncoded.Length; x++){
			secureKey[x] = System.Convert.ToInt32(secureKeyEncoded[x]);
		}
		string toDc = "";
		string strEncrypt = "";
		for (int y = 0; y <= strToEncrypt.Length-1; y+=2){
			intStr = int.Parse(strToEncrypt.Substring(y, 1), System.Globalization.NumberStyles.HexNumber) * 16 + int.Parse(strToEncrypt.Substring((y + 1), 1), System.Globalization.NumberStyles.HexNumber);
			toInt = (y / 2) % secureKey.Length;
			toIntStr = intStr - System.Convert.ToInt32(secureKey[toInt]) + 256;
			toIntStr = toIntStr % 256;
			toDc = ((char)toIntStr).ToString();
			strEncrypt = strEncrypt + toDc;
		}
		byte[] decodedBytes = System.Convert.FromBase64String (strEncrypt);
		System.Text.UTF8Encoding decodedText = new System.Text.UTF8Encoding();
		string decodedTexta = decodedText.GetString(decodedBytes, 0, decodedBytes.Length);
		return decodedTexta;
		#else
		int toInt = 0;
		int intStr = 0;
		int toIntStr = 0;
		byte[] secureKeyBytes = System.Text.Encoding.UTF8.GetBytes (PlayerPrefsElite.key[id]);
		string secureKeyEncoded = System.Convert.ToBase64String (secureKeyBytes);
		int[] secureKey = new int[secureKeyEncoded.Length];
		for (int x = 0; x < secureKeyEncoded.Length; x++){
			secureKey[x] = System.Convert.ToInt32(secureKeyEncoded[x]);
		}
		string toDc = "";
		string strEncrypt = "";
		for (int y = 0; y <= strToEncrypt.Length-1; y+=2){
			intStr = int.Parse(strToEncrypt.Substring(y, 1), System.Globalization.NumberStyles.HexNumber) * 16 + int.Parse(strToEncrypt.Substring((y + 1), 1), System.Globalization.NumberStyles.HexNumber);
			toInt = (y / 2) % secureKey.Length;
			toIntStr = intStr - System.Convert.ToInt32(secureKey[toInt]) + 256;
			toIntStr = toIntStr % 256;
			toDc = ((char)toIntStr).ToString();
			strEncrypt = strEncrypt + toDc;
		}
		byte[] decodedBytes = System.Convert.FromBase64String (strEncrypt);
		string decodedText = System.Text.Encoding.UTF8.GetString (decodedBytes);
		return decodedText;
		#endif
	}
	
}
