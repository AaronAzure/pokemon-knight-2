using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using System.Net;

[System.Serializable]
public class PlayerPrefsEliteEditor : EditorWindow
{
	public Texture2D icologo;

	public Texture2D aboutlogo;

	private GUISkin editorSkin;

	private bool isLoaded;

	private bool sort;

	private bool sortaz;

	public bool tryedit;

	public int editkey;

	private object olddata;

	private bool _loadFiles;

	private string path;

	public Texture2D icostandart;

	public Texture2D icosecure;

	public Texture2D icolocked;

	public string myString;

	public bool groupEnabled;

	public bool myBool;

	public float myFloat;

	private SecureKeysManager secureKeysManager;

	private Vector2 scrollPosition;

	private Vector2 scrollPositionEdit;

	public bool showAlerts;

	public bool deleteLinked;

	public bool rtUpdate;

	public float updInterval;

	public float updIntervalNP;

	public bool showAbout;

	public bool showSettings;

	public bool isplaying;

	public bool showCode;

	public bool showInfo;

	public bool saveProtected;

	public bool lockProtected;

	public bool[] showdrop;

	public int sortid;

	private int keyfieldsize;

	private int valuefieldsize;

	public object dataold;

	[UnityEditor.MenuItem("Window/PlayerPrefs Elite")]
	public static void Init()
	{
		PlayerPrefsEliteEditor window = ScriptableObject.CreateInstance<PlayerPrefsEliteEditor>();
		window.Show();
	}

	public virtual void setDefault()
	{
		EditorPrefs.SetInt("PPEshowAlerts", 1);
		EditorPrefs.SetInt("PPEdeleteLinked", 1);
		EditorPrefs.SetInt("PPElockProtected", 1);
		EditorPrefs.SetInt("rtUpdate", 1);
		EditorPrefs.SetFloat("updInterval", 0.3f);
		EditorPrefs.SetInt("PPEkeyfieldsize", 80);
		EditorPrefs.SetInt("PPEvaluefieldsize", 227);
	}

	public Texture2D warningicon;

	public virtual void SetGFX()
	{
		string path = "Assets/PlayerPrefsElite/Editor/Gui/Images/";
		if (EditorGUIUtility.isProSkin) {
			icologo = ((Texture2D)AssetDatabase.LoadAssetAtPath(path + "dark-pp-menu-background.png", typeof(Texture2D))) as Texture2D;
			aboutlogo = ((Texture2D)AssetDatabase.LoadAssetAtPath(path + "dark-texture-about.png", typeof(Texture2D))) as Texture2D;
			icostandart = ((Texture2D)AssetDatabase.LoadAssetAtPath(path + "dark-icon-data.png", typeof(Texture2D))) as Texture2D;
			icosecure = ((Texture2D)AssetDatabase.LoadAssetAtPath(path + "dark-icon-data-sheeld.png", typeof(Texture2D))) as Texture2D;
			icolocked = ((Texture2D)AssetDatabase.LoadAssetAtPath(path + "dark-icon-data-sheeld-locked.png", typeof(Texture2D))) as Texture2D;
			warningicon = ((Texture2D)AssetDatabase.LoadAssetAtPath(path + "dark-warning-icon.png", typeof(Texture2D))) as Texture2D;
		} else {
			icologo = ((Texture2D)AssetDatabase.LoadAssetAtPath(path + "pp-menu-background.png", typeof(Texture2D))) as Texture2D;
			aboutlogo = ((Texture2D)AssetDatabase.LoadAssetAtPath(path + "texture-about.png", typeof(Texture2D))) as Texture2D;
			icostandart = ((Texture2D)AssetDatabase.LoadAssetAtPath(path + "icon-data.png", typeof(Texture2D))) as Texture2D;
			icosecure = ((Texture2D)AssetDatabase.LoadAssetAtPath(path + "icon-data-sheeld.png", typeof(Texture2D))) as Texture2D;
			icolocked = ((Texture2D)AssetDatabase.LoadAssetAtPath(path + "icon-data-sheeld-locked.png", typeof(Texture2D))) as Texture2D;
			warningicon = ((Texture2D)AssetDatabase.LoadAssetAtPath(path + "warning-icon.png", typeof(Texture2D))) as Texture2D;
		}
		LoadSkin();
		EditorApplication.RepaintProjectWindow();
	}

	public virtual void LoadSkin()
	{
		if (EditorGUIUtility.isProSkin) {
			editorSkin = ((GUISkin)AssetDatabase.LoadAssetAtPath("Assets/PlayerPrefsElite/Editor/Gui/Images/skind.guiskin", typeof(GUISkin))) as GUISkin;
		} else {
			editorSkin = ((GUISkin)AssetDatabase.LoadAssetAtPath("Assets/PlayerPrefsElite/Editor/Gui/Images/skin.guiskin", typeof(GUISkin))) as GUISkin;
		}
	}

	public virtual void OnEnable()
	{
		if (!EditorPrefs.HasKey("PPEshowAlerts")) {
			setDefault();
		}
		tryedit = false;
		showAlerts = EditorPrefs.GetInt("PPEshowAlerts") == 1 ? true : false;
		deleteLinked = EditorPrefs.GetInt("PPEdeleteLinked") == 1 ? true : false;
		lockProtected = EditorPrefs.GetInt("PPElockProtected") == 1 ? true : false;
		updInterval = EditorPrefs.GetFloat("updInterval");
		isplaying = EditorApplication.isPlaying;
		tmpTime = 0;
		SetGFX();
	}


	private string[] _key;

	private string[] _oldkey;

	private string[] _newkey;

	private object[] _value;

	[System.Serializable]
	public class _PlayerPrefs : object
	{
		public string key;

		public object value;

		public string type;

		public bool secure;

		public bool locked;

		public string securename;

		public int linkid;

		public int keyid;

		public bool array;

		public bool boolean;

		public _PlayerPrefs()
		{
			type = "other";
		}

	}

	public FileInfo _file;

	public PlayerPrefsEliteEditor._PlayerPrefs[] data;

	private System.Collections.Generic.Dictionary<string, object> _plist;

	public virtual string GetStringFromLinuxPrefs(string fromKey, string toKey, string inputlist)
	{
		int from = inputlist.IndexOf(fromKey, StringComparison.Ordinal) + fromKey.Length;
		int to = inputlist.LastIndexOf(toKey, StringComparison.Ordinal);
		return inputlist.Substring(from, to - from);
	}

	public virtual void SetPrefsKeys(SecureKeysManager secureKeysManager, int i)
	{
		int y = 0;
		while (y < secureKeysManager.keys.Length) {
			if (PlayerPrefs.HasKey(PlayerPrefsElite.sum(PlayerPrefsElite.prefix + data[i].key, secureKeysManager.keys[y]))) {
				data[i].secure = true;
				data[i].securename = PlayerPrefsElite.sum(PlayerPrefsElite.prefix + data[i].key, secureKeysManager.keys[y]);
				data[i].keyid = y;
				break;
			} else {
				if (PlayerPrefs.HasKey(PlayerPrefsElite.sum(PlayerPrefsElite.prefix2 + data[i].key, secureKeysManager.keys[y]))) {
					data[i].secure = true;
					data[i].array = true;
					data[i].securename = PlayerPrefsElite.sum(PlayerPrefsElite.prefix2 + data[i].key, secureKeysManager.keys[y]);
					data[i].keyid = y;
					break;
				} else {
					if (PlayerPrefs.HasKey(PlayerPrefsElite.sum(PlayerPrefsElite._prefix2 + data[i].key, secureKeysManager.keys[y]))) {
						data[i].secure = true;
						data[i].boolean = true;
						data[i].securename = PlayerPrefsElite.sum(PlayerPrefsElite._prefix2 + data[i].key, secureKeysManager.keys[y]);
						data[i].keyid = y;
						break;
					} else {
						if (data[i].key.Length > 3) {
							if (data[i].key.Substring(0, 3) == PlayerPrefsElite._prefix) {
								data[i].locked = true;
								data[i].keyid = y;
								break;
							}
						}
					}
				}
			}
			y++;
		}
	}

	public virtual void SortingPrefsKeys(bool sort, int count)
	{
        
		if (sort) {
			var sortArray = new List<string>();
            
			for (int x = 0; x < data.Length; x++) {
				sortArray.Add(data[x].key);
			}
			if (sortaz) {
				sortArray.Sort(SortStringA);
			} else {
				sortArray.Sort(SortStringB);
			}

			_PlayerPrefs[] tempdata = new _PlayerPrefs[count];

			for (int i = 0; i < count; i++) {
				for (int x = 0; x < count; x++) {
					if (data[x].key == sortArray[i].ToString()) {
						tempdata[i] = data[x];
						break;
					}
				}
			}
			data = tempdata;
		}
		for (int i = 0; i < data.Length; i++) {
			if (data[i].secure == true) {
				for (int y = 0; y < data.Length; y++) {
					if (data[i].securename == data[y].key) {
						data[y].secure = true;
						data[y].locked = true;
						data[y].linkid = i;
						data[i].linkid = y;
					}
				}
			}
		}
	}

	public virtual void loadFiles()
	{
		int i = 0;
		
		if (Application.platform == RuntimePlatform.LinuxEditor) {
			var filepath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/.config/unity3d/" + PlayerSettings.companyName + "/" + PlayerSettings.productName + "/prefs";
			var linuxdata = new StreamReader(filepath);
			var linuxprefs = linuxdata.ReadToEnd().Split("\n"[0]);
			linuxdata.Close();

			var inputlist = linuxprefs.ToList();

			for (i = 0; i < inputlist.Count; i++) {
				string arraystring = inputlist[i];
				if (!arraystring.Contains("<pref name=")) {
					inputlist.Remove(inputlist[i]);
				}
			}

			data = new _PlayerPrefs[inputlist.Count];

			for (i = 0; i < inputlist.Count; i++) {
				data[i] = new _PlayerPrefs();
				data[i].key = GetStringFromLinuxPrefs("<pref name=\"", "\" type", inputlist[i].ToString());
				;
				data[i].type = GetStringFromLinuxPrefs("type=\"", "\">", inputlist[i].ToString());

				if (data[i].type != "string" && data[i].type != "int" && data[i].type != "float") {
					data[i].type = "other";
				}

				data[i].value = GetStringFromLinuxPrefs("\">", "</pref>", inputlist[i].ToString());

				if (secureKeysManager == null) {
					continue;
				}
				SetPrefsKeys(secureKeysManager, i);
			}
			SortingPrefsKeys(sort, inputlist.Count);
		} else if (Application.platform == RuntimePlatform.OSXEditor) {
			_file = new FileInfo(
				(((((Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Library/Preferences/") +
				"unity.") + PlayerSettings.companyName) + ".") + PlayerSettings.productName) + ".plist");
			if (_file.Exists) {
				_plist =
                    PlistCS.Plist.readPlist(_file.FullName) as System.Collections.Generic.Dictionary<string, object>;
				_key = new string[_plist.Count];
				_value = new object[_plist.Count];
				_plist.Keys.CopyTo(_key, 0);
				_plist.Values.CopyTo(_value, 0);
				data = new PlayerPrefsEliteEditor._PlayerPrefs[_plist.Count];

				for (i = 0; i < _plist.Count; i++) {
					data[i] = new _PlayerPrefs();
					data[i].key = _key[i];
					data[i].value = _value[i];
					if (_value[i].GetType() == typeof(String)) {
						data[i].type = "string";
					} else if (_value[i].GetType() == typeof(int)) {
						data[i].type = "int";
					} else if (_value[i].GetType() == typeof(double)) {
						data[i].type = "float";
					} else {
						data[i].type = "other";
					}
					if (secureKeysManager == null) {
						continue;
					}
					SetPrefsKeys(secureKeysManager, i);
				}

				SortingPrefsKeys(sort, _plist.Count);
				dataold = data;
			}
		} else if (Application.platform == RuntimePlatform.WindowsEditor) {
			RegistryKey unityKey = Registry.CurrentUser.CreateSubKey(
				                       (("Software\\Unity\\UnityEditor\\" + PlayerSettings.companyName) + "\\") +
				                       PlayerSettings.productName);
			_newkey = unityKey.GetValueNames();
			if (_newkey == _oldkey) {
				return;
			}
			_oldkey = _newkey;
			_key = unityKey.GetValueNames();
			_value = new object[_key.Length];
			data = new PlayerPrefsEliteEditor._PlayerPrefs[_key.Length];

			for (i = 0; i < _key.Length; i++) {
				data[i] = new _PlayerPrefs();
				data[i].key = _key[i].Substring(0, _key[i].LastIndexOf("_"));
				if (PlayerPrefs.GetString(_key[i], "String") != "String") {
					data[i].type = "string";
				} else if (PlayerPrefs.GetString(_key[i], "gnirtS") != "gnirtS") {
					data[i].type = "string";
				} else if (PlayerPrefs.GetInt(_key[i], 1) != 1) {
					data[i].type = "int";
				} else if (PlayerPrefs.GetInt(_key[i], 0) != 0) {
					data[i].type = "int";
				} else if (PlayerPrefs.GetFloat(_key[i], 1.0f) != 1.0f) {
					data[i].type = "float";
				} else if (PlayerPrefs.GetFloat(_key[i], 0.0f) != 0.0f) {
					data[i].type = "float";
				} else {
					data[i].type = "other";
				}
				if (secureKeysManager == null) {
					continue;
				}
				SetPrefsKeys(secureKeysManager, i);
			}

			SortingPrefsKeys(sort, _key.Length);

		}

	}

	public string objNames;

	public virtual int SortStringA(string a, string b)
	{
		return string.Compare(a, b);
	}

	public virtual int SortStringB(string a, string b)
	{
		return string.Compare(b, a);
	}

	public virtual Byte[] StringToUTF8ByteArray(string pXmlString)
	{
		UTF8Encoding encoding = new UTF8Encoding();
		Byte[] byteArray = encoding.GetBytes(pXmlString);
		return byteArray;
	}

	public virtual void resetmenu()
	{
		if (showAbout) {
			showAbout = !showAbout;
		}
		if (showSettings) {
			showSettings = !showSettings;
		}
	}

	public virtual void disablemenu(int id)
	{
		int i = 1;
		while (i < 6) {
			if (i != id) {
				showdrop[i] = false;
			}
			i++;
		}
	}

	public virtual void OnGUI()
	{
		if (editorSkin == null) {
			editorSkin = ((GUISkin)AssetDatabase.LoadAssetAtPath("Assets/PlayerPrefsElite/Editor/Gui/Images/skin.guiskin", typeof(GUISkin))) as GUISkin;
		}
		GUIStyle myStyle = new GUIStyle(GUI.skin.toggle);
		myStyle.overflow.top = -2;
		GUILayout.Label(icologo, editorSkin.customStyles[11], new GUILayoutOption[] {
			GUILayout.Width(438),
			GUILayout.Height(126)
		});
		if (GUI.Button(new Rect(4, 63, 92, 62), "", editorSkin.customStyles[12])) {
			if (tryedit) {
				return;
			}
			disablemenu(0);
			loadFiles();
			PlayerPrefs.Save();
		}
		if (!showdrop[0] && !showdrop[1]) {
			if (GUI.Button(new Rect(97, 63, 111, 62), "", editorSkin.customStyles[13])) {
				if (tryedit) {
					return;
				}
				disablemenu(1);
				showdrop[1] = !showdrop[1];
			}
		} else {
			if (showdrop[0] && !showdrop[1]) {
				if (GUI.Button(new Rect(97, 63, 111, 62), "", editorSkin.customStyles[14])) {
					if (tryedit) {
						return;
					}
					disablemenu(1);
					showdrop[1] = !showdrop[1];
				}
			}
		}
		if (!showdrop[0] && showdrop[1]) {
			if (GUI.Button(new Rect(97, 63, 111, 62), "", editorSkin.customStyles[41])) {
				if (tryedit) {
					return;
				}
				disablemenu(1);
				showdrop[1] = !showdrop[1];
			}
		} else {
			if (showdrop[0] && showdrop[1]) {
				if (GUI.Button(new Rect(97, 63, 111, 62), "", editorSkin.customStyles[42])) {
					if (tryedit) {
						return;
					}
					disablemenu(1);
					showdrop[1] = !showdrop[1];
				}
			}
		}
		if ((!showdrop[2] && (sortid == 0)) && !showdrop[3]) {
			if (GUI.Button(new Rect(209, 63, 123, 62), "", editorSkin.customStyles[15])) {
				if (tryedit) {
					return;
				}
				disablemenu(3);
				showdrop[3] = !showdrop[3];
			}
		} else {
			if ((!showdrop[2] && (sortid == 0)) && showdrop[3]) {
				if (GUI.Button(new Rect(209, 63, 123, 62), "", editorSkin.customStyles[43])) {
					if (tryedit) {
						return;
					}
					disablemenu(3);
					showdrop[3] = !showdrop[3];
				}
			} else {
				if ((!showdrop[2] && (sortid == 1)) && !showdrop[3]) {
					if (GUI.Button(new Rect(209, 63, 123, 62), "", editorSkin.customStyles[16])) {
						if (tryedit) {
							return;
						}
						disablemenu(3);
						showdrop[3] = !showdrop[3];
					}
				} else {
					if ((!showdrop[2] && (sortid == 1)) && showdrop[3]) {
						if (GUI.Button(new Rect(209, 63, 123, 62), "", editorSkin.customStyles[44])) {
							if (tryedit) {
								return;
							}
							disablemenu(3);
							showdrop[3] = !showdrop[3];
						}
					} else {
						if ((!showdrop[2] && (sortid == 2)) && !showdrop[3]) {
							if (GUI.Button(new Rect(209, 63, 123, 62), "", editorSkin.customStyles[17])) {
								if (tryedit) {
									return;
								}
								disablemenu(3);
								showdrop[3] = !showdrop[3];
							}
						} else {
							if ((!showdrop[2] && (sortid == 2)) && showdrop[3]) {
								if (GUI.Button(new Rect(209, 63, 123, 62), "", editorSkin.customStyles[45])) {
									if (tryedit) {
										return;
									}
									disablemenu(3);
									showdrop[3] = !showdrop[3];
								}
							}
						}
					}
				}
			}
		}
		if (!showdrop[4]) {
			if (GUI.Button(new Rect(333, 63, 56, 62), "", editorSkin.customStyles[18])) {
				disablemenu(4);
				showdrop[4] = !showdrop[4];
			}
		} else {
			if (GUI.Button(new Rect(333, 63, 56, 62), "", editorSkin.customStyles[39])) {
				disablemenu(4);
				showdrop[4] = !showdrop[4];
			}
		}
		if (!showdrop[5]) {
			if (GUI.Button(new Rect(390, 63, 50, 62), "", editorSkin.customStyles[19])) {
				disablemenu(5);
				showdrop[5] = !showdrop[5];
			}
		} else {
			if (GUI.Button(new Rect(390, 63, 50, 62), "", editorSkin.customStyles[40])) {
				disablemenu(5);
				showdrop[5] = !showdrop[5];
			}
		}
		if (showdrop[5]) {
			GUILayout.BeginHorizontal(new GUILayoutOption[] { });
			GUILayout.Label(aboutlogo, editorSkin.customStyles[11], new GUILayoutOption[] { });
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			if (GUI.Button(new Rect(61, 270, 114, 21), "", editorSkin.customStyles[20])) {
				Application.OpenURL("http://unityplugins.eu");
			}
			GUILayout.Space(60);
		}
		if (showdrop[4]) {
			GUILayout.Space(15);
			GUILayout.BeginHorizontal(new GUILayoutOption[] { });
			GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(10) });
			GUILayout.Label("Show Alerts", new GUILayoutOption[] { GUILayout.Width(90) });
			showAlerts = EditorGUILayout.Toggle(showAlerts, myStyle, new GUILayoutOption[] { });
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[] { });
			GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(10) });
			GUILayout.Label("Delete Linked", new GUILayoutOption[] { GUILayout.Width(90) });
			deleteLinked = EditorGUILayout.Toggle(deleteLinked, myStyle, new GUILayoutOption[] { });
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[] { });
			GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(10) });
			GUILayout.Label("Lock Secure\nData", new GUILayoutOption[] { GUILayout.Width(90) });
			lockProtected = EditorGUILayout.Toggle(lockProtected, myStyle, new GUILayoutOption[] { });
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[] { });
			GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(10) });
			GUILayout.Label("Key field size", new GUILayoutOption[] { GUILayout.Width(90) });
			keyfieldsize = EditorGUILayout.IntField("", keyfieldsize, new GUILayoutOption[] { GUILayout.MaxWidth(30) });
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[] { });
			GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(10) });
			GUILayout.Label("Value field size", new GUILayoutOption[] { GUILayout.Width(90) });
			valuefieldsize = EditorGUILayout.IntField("", valuefieldsize, new GUILayoutOption[] { GUILayout.MaxWidth(30) });
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[] { });
			GUILayout.FlexibleSpace();
			GUILayout.Label("Time, sec:", new GUILayoutOption[] { GUILayout.MinWidth(Screen.width - 332) });
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[] { });
			GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(10) });
			GUILayout.Label("Update interval \nin play mode:", new GUILayoutOption[] { GUILayout.Width(90) });
			updInterval = GUILayout.HorizontalSlider(updInterval, 0.1f, 2f, new GUILayoutOption[] { GUILayout.MaxWidth(201) });
			updInterval = EditorGUILayout.FloatField(updInterval, new GUILayoutOption[] { GUILayout.Width(72) });
			if (updInterval < 0.1f) {
				updInterval = 0.1f;
			}
			if (updInterval > 2) {
				updInterval = 2;
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			GUILayout.BeginHorizontal(new GUILayoutOption[] { });
			GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(10) });
			if (GUILayout.Button("Reset to Default", new GUILayoutOption[] { GUILayout.Width(105) })) {
				setDefault();
				showAlerts = EditorPrefs.GetInt("PPEshowAlerts") == 1 ? true : false;
				deleteLinked = EditorPrefs.GetInt("PPEdeleteLinked") == 1 ? true : false;
				rtUpdate = EditorPrefs.GetInt("rtUpdate") == 1 ? true : false;
				lockProtected = EditorPrefs.GetInt("PPElockProtected") == 1 ? true : false;
				updInterval = EditorPrefs.GetFloat("updInterval");
				keyfieldsize = 80;
				valuefieldsize = 227;
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			GUILayout.BeginHorizontal(new GUILayoutOption[] { });
			GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(10) });
			GUILayout.Label("Removes all keys\nand values from\nthe preferences", new GUILayoutOption[] { GUILayout.Width(120) });
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			GUILayout.BeginHorizontal(new GUILayoutOption[] { });
			GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(10) });
			if (GUILayout.Button("Delete All", new GUILayoutOption[] { GUILayout.Width(105) })) {
				if (showAlerts) {
					if (EditorUtility.DisplayDialog("Delete all keys and values from the preferences?", "operation cannot be undone", "Yes", "Cancel")) {
						PlayerPrefs.DeleteAll();
						PlayerPrefs.Save();
					}
				} else {
					PlayerPrefs.DeleteAll();
					PlayerPrefs.Save();
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			if (GUI.changed) {
				EditorPrefs.SetInt("PPEshowAlerts", showAlerts ? 1 : 0);
				EditorPrefs.SetInt("PPEdeleteLinked", deleteLinked ? 1 : 0);
				EditorPrefs.SetInt("PPElockProtected", lockProtected ? 1 : 0);
				EditorPrefs.SetFloat("updInterval", updInterval);
				EditorPrefs.SetInt("PPEkeyfieldsize", keyfieldsize);
				EditorPrefs.SetInt("PPEvaluefieldsize", valuefieldsize);
			}
		}
		if (secureKeysManager == null) {
			GUILayout.Space(40);
			GUILayout.Box("", editorSkin.customStyles[57], new GUILayoutOption[] { GUILayout.MaxWidth(438) });
			GUILayout.Box(warningicon, editorSkin.customStyles[57], new GUILayoutOption[] { GUILayout.MaxWidth(438) });
			GUILayout.Box("\nSecureKeysManager prefab not found!\n \n Drag \"SecureKeysManager\" prefab into Hierarchy\n(from Project tab - \"PlayerPrefsElite/Prefab/\")\n", editorSkin.customStyles[57], new GUILayoutOption[] { GUILayout.MaxWidth(438) });
			return;
		} else {
			if (secureKeysManager.keys.Length < 1) {
				GUILayout.Space(40);
				GUILayout.Box("", editorSkin.customStyles[57], new GUILayoutOption[] { GUILayout.MaxWidth(438) });
				GUILayout.Box(warningicon, editorSkin.customStyles[57], new GUILayoutOption[] { GUILayout.MaxWidth(438) });
				GUILayout.Box("\n   Generate new key in \"SecureKeysManager\"\t\n", editorSkin.customStyles[57], new GUILayoutOption[] { GUILayout.MaxWidth(438) });
				return;
			}
		}
		if (!(data == null)) {
			GUILayout.Space(20);
			if (tryedit) {
				GUILayout.Space(2);
				scrollPositionEdit = GUILayout.BeginScrollView(scrollPositionEdit, new GUILayoutOption[] { });
				GUILayout.Space(2);
				GUILayout.BeginHorizontal(new GUILayoutOption[] { });
				GUILayout.Box(new GUIContent(getImage(data[editkey].secure, data[editkey].locked), secureString(editkey)), editorSkin.customStyles[36], new GUILayoutOption[] { GUILayout.Width(20) });
				GUILayout.Label(data[editkey].key, editorSkin.customStyles[37], new GUILayoutOption[] {
					GUILayout.Width(100),
					GUILayout.Height(30)
				});
				GUILayout.Label("", editorSkin.customStyles[6], new GUILayoutOption[] { GUILayout.Width(10) });
				if (data[editkey].type == "string") {
					olddata = EditorGUILayout.TextField("", olddata.ToString(), new GUILayoutOption[] {
						GUILayout.MinWidth(30),
						GUILayout.MaxWidth(227)
					});
					if (GUILayout.Button(new GUIContent("", "Restore"), editorSkin.customStyles[48], new GUILayoutOption[] { })) {
						GUIUtility.hotControl = 0;
						GUIUtility.keyboardControl = 0;
						olddata = GetValue(data[editkey].key, data[editkey].type);
					}
				} else {
					if (data[editkey].type == "int") {
						olddata = EditorGUILayout.IntField("", (int)olddata, new GUILayoutOption[] { GUILayout.MaxWidth(227) });
						if (GUILayout.Button(new GUIContent("", "Restore"), editorSkin.customStyles[48], new GUILayoutOption[] { })) {
							GUIUtility.hotControl = 0;
							GUIUtility.keyboardControl = 0;
							olddata = GetValue(data[editkey].key, data[editkey].type);
						}
					} else {
						if (data[editkey].type == "float") {
							olddata = EditorGUILayout.FloatField("", (float)olddata, new GUILayoutOption[] { GUILayout.MaxWidth(227) });
							if (GUILayout.Button(new GUIContent("", "Restore"), editorSkin.customStyles[48], new GUILayoutOption[] { })) {
								GUIUtility.hotControl = 0;
								GUIUtility.keyboardControl = 0;
								olddata = GetValue(data[editkey].key, data[editkey].type);
							}
						}
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(5);
				if (showInfo) {
					GUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Label("Detailed Info", editorSkin.customStyles[46], new GUILayoutOption[] {
						GUILayout.MinWidth(100),
						GUILayout.MaxWidth(343),
						GUILayout.Height(16)
					});
					GUILayout.EndHorizontal();
					GUILayout.Space(5);
					GUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Box("Name: \t\t\t" + data[editkey].key, editorSkin.customStyles[47], new GUILayoutOption[] {
						GUILayout.MinWidth(100),
						GUILayout.MaxWidth(343),
						GUILayout.Height(16)
					});
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Box("Value: \t\t\t" + GetValue(data[editkey].key, data[editkey].type).ToString(), editorSkin.customStyles[47], new GUILayoutOption[] {
						GUILayout.MinWidth(100),
						GUILayout.MaxWidth(343),
						GUILayout.Height(16)
					});
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Box("Type: \t\t\t" + data[editkey].type, editorSkin.customStyles[47], new GUILayoutOption[] {
						GUILayout.MinWidth(100),
						GUILayout.MaxWidth(343),
						GUILayout.Height(16)
					});
					GUILayout.EndHorizontal();
					if (data[editkey].secure) {
						GUILayout.BeginHorizontal(new GUILayoutOption[] { });
						GUILayout.Box("Secured: \t\tYes", editorSkin.customStyles[47], new GUILayoutOption[] {
							GUILayout.MinWidth(100),
							GUILayout.MaxWidth(343),
							GUILayout.Height(16)
						});
						GUILayout.EndHorizontal();
						if (!data[editkey].locked) {
							GUILayout.BeginHorizontal(new GUILayoutOption[] { });
							GUILayout.Box("Secure key id: \t" + data[editkey].keyid, editorSkin.customStyles[47], new GUILayoutOption[] {
								GUILayout.MinWidth(100),
								GUILayout.MaxWidth(343),
								GUILayout.Height(16)
							});
							GUILayout.EndHorizontal();
						}
						GUILayout.BeginHorizontal(new GUILayoutOption[] { });
						GUILayout.Box("Linked to: \t\t" + data[data[editkey].linkid].key, editorSkin.customStyles[47], new GUILayoutOption[] {
							GUILayout.MinWidth(100),
							GUILayout.MaxWidth(343),
							GUILayout.Height(16)
						});
						GUILayout.EndHorizontal();
					}
					if (!data[editkey].secure && data[editkey].locked) {
						GUILayout.BeginHorizontal(new GUILayoutOption[] { });
						GUILayout.Box("Secured: \t\tYes, encrypt", editorSkin.customStyles[47], new GUILayoutOption[] {
							GUILayout.MinWidth(100),
							GUILayout.MaxWidth(343),
							GUILayout.Height(16)
						});
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal(new GUILayoutOption[] { });
						GUILayout.Box("Secure key id: \t" + data[editkey].keyid, editorSkin.customStyles[47], new GUILayoutOption[] {
							GUILayout.MinWidth(100),
							GUILayout.MaxWidth(343),
							GUILayout.Height(16)
						});
						GUILayout.EndHorizontal();
					}
					if (data[editkey].array) {
						GUILayout.BeginHorizontal(new GUILayoutOption[] { });
						GUILayout.Box("Secured: \t\tYes, Array", editorSkin.customStyles[47], new GUILayoutOption[] {
							GUILayout.MinWidth(100),
							GUILayout.MaxWidth(343),
							GUILayout.Height(16)
						});
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal(new GUILayoutOption[] { });
						GUILayout.Box("Secure key id: \t" + data[editkey].keyid, editorSkin.customStyles[47], new GUILayoutOption[] {
							GUILayout.MinWidth(100),
							GUILayout.MaxWidth(343),
							GUILayout.Height(16)
						});
						GUILayout.EndHorizontal();
					}
					if (data[editkey].boolean) {
						GUILayout.BeginHorizontal(new GUILayoutOption[] { });
						GUILayout.Box("Secured: \t\tYes, Boolean", editorSkin.customStyles[47], new GUILayoutOption[] {
							GUILayout.MinWidth(100),
							GUILayout.MaxWidth(343),
							GUILayout.Height(16)
						});
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal(new GUILayoutOption[] { });
						GUILayout.Box("Secure key id: \t" + data[editkey].keyid, editorSkin.customStyles[47], new GUILayoutOption[] {
							GUILayout.MinWidth(100),
							GUILayout.MaxWidth(343),
							GUILayout.Height(16)
						});
						GUILayout.EndHorizontal();
					} else {
						GUILayout.BeginHorizontal(new GUILayoutOption[] { });
						GUILayout.Box("Secured: \t\tNo", editorSkin.customStyles[47], new GUILayoutOption[] {
							GUILayout.MinWidth(100),
							GUILayout.MaxWidth(343),
							GUILayout.Height(16)
						});
						GUILayout.EndHorizontal();
					}
					GUILayout.Space(5);
					GUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Label("", editorSkin.customStyles[46], new GUILayoutOption[] {
						GUILayout.MinWidth(100),
						GUILayout.MaxWidth(343),
						GUILayout.Height(4)
					});
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(262) });
					if (GUILayout.Button(new GUIContent("", "Hide Detailed Info"), editorSkin.customStyles[51], new GUILayoutOption[] {
						GUILayout.Width(110),
						GUILayout.Height(20)
					})) {
						showInfo = false;
					}
					GUILayout.EndHorizontal();
				} else {
					GUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Label("Detailed Info", editorSkin.customStyles[46], new GUILayoutOption[] {
						GUILayout.MinWidth(100),
						GUILayout.MaxWidth(343),
						GUILayout.Height(16)
					});
					GUILayout.EndHorizontal();
					GUILayout.Space(5);
					GUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(262) });
					if (GUILayout.Button(new GUIContent("", "Show Detailed Info"), editorSkin.customStyles[50], new GUILayoutOption[] {
						GUILayout.Width(110),
						GUILayout.Height(20)
					})) {
						showInfo = true;
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.Space(10);
				if (data[editkey].locked) {
					saveProtected = false;
					GUI.enabled = false;
				}
				if (showCode) {
					GUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Label("Code Example", editorSkin.customStyles[46], new GUILayoutOption[] {
						GUILayout.MinWidth(100),
						GUILayout.MaxWidth(343),
						GUILayout.Height(16)
					});
					GUILayout.EndHorizontal();
					GUILayout.Space(5);
					GUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Box("", editorSkin.customStyles[47], new GUILayoutOption[] {
						GUILayout.MinWidth(100),
						GUILayout.MaxWidth(343),
						GUILayout.Height(4)
					});
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Box("Save value:", editorSkin.customStyles[54], new GUILayoutOption[] {
						GUILayout.MinWidth(100),
						GUILayout.MaxWidth(343),
						GUILayout.Height(16)
					});
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Box("", editorSkin.customStyles[47], new GUILayoutOption[] {
						GUILayout.MinWidth(100),
						GUILayout.MaxWidth(343),
						GUILayout.Height(16)
					});
					GUILayout.EndHorizontal();
					if ((data[editkey].type == "string") && !data[editkey].array) {
						GUILayout.BeginHorizontal(new GUILayoutOption[] { });
						GUILayout.Box("static function PlayerPrefsElite.SetString (", editorSkin.customStyles[47], new GUILayoutOption[] {
							GUILayout.MinWidth(100),
							GUILayout.MaxWidth(343),
							GUILayout.Height(16)
						});
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal(new GUILayoutOption[] { });
						GUILayout.Box("\tkey : String, value : String, secureKey : int", editorSkin.customStyles[47], new GUILayoutOption[] {
							GUILayout.MinWidth(100),
							GUILayout.MaxWidth(343),
							GUILayout.Height(16)
						});
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal(new GUILayoutOption[] { });
						GUILayout.Box(") : void", editorSkin.customStyles[47], new GUILayoutOption[] {
							GUILayout.MinWidth(100),
							GUILayout.MaxWidth(343),
							GUILayout.Height(16)
						});
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal(new GUILayoutOption[] { });
						GUILayout.Box("", editorSkin.customStyles[47], new GUILayoutOption[] {
							GUILayout.MinWidth(100),
							GUILayout.MaxWidth(343),
							GUILayout.Height(16)
						});
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal(new GUILayoutOption[] { });
						GUILayout.Box("Example:", editorSkin.customStyles[47], new GUILayoutOption[] {
							GUILayout.MinWidth(100),
							GUILayout.MaxWidth(343),
							GUILayout.Height(16)
						});
						GUILayout.EndHorizontal();
						if (data[editkey].keyid == 0) {
							GUILayout.BeginHorizontal(new GUILayoutOption[] { });
							GUILayout.Box(("PlayerPrefsElite.SetString(\"" + data[editkey].key) + "\", \"Foobar\");", editorSkin.customStyles[54], new GUILayoutOption[] {
								GUILayout.MinWidth(100),
								GUILayout.MaxWidth(343),
								GUILayout.Height(18)
							});
							if (GUILayout.Button(new GUIContent("", "Copy to Clipboard"), editorSkin.customStyles[49], new GUILayoutOption[] { })) {
								EditorGUIUtility.systemCopyBuffer = ("PlayerPrefsElite.SetString(\"" + data[editkey].key) + "\", \"Foobar\");";
							}
							GUILayout.EndHorizontal();
						} else {
							GUILayout.BeginHorizontal(new GUILayoutOption[] { });
							GUILayout.Box(((("PlayerPrefsElite.SetString(\"" + data[editkey].key) + "\", \"Foobar\", ") + data[editkey].keyid) + ");", editorSkin.customStyles[54], new GUILayoutOption[] {
								GUILayout.MinWidth(100),
								GUILayout.MaxWidth(343),
								GUILayout.Height(18)
							});
							if (GUILayout.Button(new GUIContent("", "Copy to Clipboard"), editorSkin.customStyles[49], new GUILayoutOption[] { })) {
								EditorGUIUtility.systemCopyBuffer = ((("PlayerPrefsElite.SetString(\"" + data[editkey].key) + "\", \"Foobar\", ") + data[editkey].keyid) + ");";
							}
							GUILayout.EndHorizontal();
						}
					} else {
						if ((data[editkey].type == "int") && data[editkey].boolean) {
							GUILayout.BeginHorizontal(new GUILayoutOption[] { });
							GUILayout.Box("static function PlayerPrefsElite.SetBoolean (", editorSkin.customStyles[47], new GUILayoutOption[] {
								GUILayout.MinWidth(100),
								GUILayout.MaxWidth(343),
								GUILayout.Height(16)
							});
							GUILayout.EndHorizontal();
							GUILayout.BeginHorizontal(new GUILayoutOption[] { });
							GUILayout.Box("\tkey : String, value : boolean, secureKey : int", editorSkin.customStyles[47], new GUILayoutOption[] {
								GUILayout.MinWidth(100),
								GUILayout.MaxWidth(343),
								GUILayout.Height(16)
							});
							GUILayout.EndHorizontal();
							GUILayout.BeginHorizontal(new GUILayoutOption[] { });
							GUILayout.Box(") : void", editorSkin.customStyles[47], new GUILayoutOption[] {
								GUILayout.MinWidth(100),
								GUILayout.MaxWidth(343),
								GUILayout.Height(16)
							});
							GUILayout.EndHorizontal();
							GUILayout.BeginHorizontal(new GUILayoutOption[] { });
							GUILayout.Box("", editorSkin.customStyles[47], new GUILayoutOption[] {
								GUILayout.MinWidth(100),
								GUILayout.MaxWidth(343),
								GUILayout.Height(16)
							});
							GUILayout.EndHorizontal();
							GUILayout.BeginHorizontal(new GUILayoutOption[] { });
							GUILayout.Box("Example:", editorSkin.customStyles[47], new GUILayoutOption[] {
								GUILayout.MinWidth(100),
								GUILayout.MaxWidth(343),
								GUILayout.Height(16)
							});
							GUILayout.EndHorizontal();
							if (data[editkey].keyid == 0) {
								GUILayout.BeginHorizontal(new GUILayoutOption[] { });
								GUILayout.Box(("PlayerPrefsElite.SetBoolean(\"" + data[editkey].key) + "\", true);", editorSkin.customStyles[54], new GUILayoutOption[] {
									GUILayout.MinWidth(100),
									GUILayout.MaxWidth(343),
									GUILayout.Height(18)
								});
								if (GUILayout.Button(new GUIContent("", "Copy to Clipboard"), editorSkin.customStyles[49], new GUILayoutOption[] { })) {
									EditorGUIUtility.systemCopyBuffer = ("PlayerPrefsElite.SetBoolean(\"" + data[editkey].key) + "\", true);";
								}
								GUILayout.EndHorizontal();
							} else {
								GUILayout.BeginHorizontal(new GUILayoutOption[] { });
								GUILayout.Box(((("PlayerPrefsElite.SetBoolean(\"" + data[editkey].key) + "\", true, ") + data[editkey].keyid) + ");", editorSkin.customStyles[54], new GUILayoutOption[] {
									GUILayout.MinWidth(100),
									GUILayout.MaxWidth(343),
									GUILayout.Height(18)
								});
								if (GUILayout.Button(new GUIContent("", "Copy to Clipboard"), editorSkin.customStyles[49], new GUILayoutOption[] { })) {
									EditorGUIUtility.systemCopyBuffer = ((("PlayerPrefsElite.SetBoolean(\"" + data[editkey].key) + "\", true, ") + data[editkey].keyid) + ");";
								}
								GUILayout.EndHorizontal();
							}
						} else {
							if ((data[editkey].type == "string") && data[editkey].array) {
								GUILayout.BeginHorizontal(new GUILayoutOption[] { });
								GUILayout.Box("static function PlayerPrefsElite.SetStringArray (", editorSkin.customStyles[47], new GUILayoutOption[] {
									GUILayout.MinWidth(100),
									GUILayout.MaxWidth(343),
									GUILayout.Height(16)
								});
								GUILayout.EndHorizontal();
								GUILayout.BeginHorizontal(new GUILayoutOption[] { });
								GUILayout.Box("\tkey : String, value : Array, secureKey : int", editorSkin.customStyles[47], new GUILayoutOption[] {
									GUILayout.MinWidth(100),
									GUILayout.MaxWidth(343),
									GUILayout.Height(16)
								});
								GUILayout.EndHorizontal();
								GUILayout.BeginHorizontal(new GUILayoutOption[] { });
								GUILayout.Box(") : void", editorSkin.customStyles[47], new GUILayoutOption[] {
									GUILayout.MinWidth(100),
									GUILayout.MaxWidth(343),
									GUILayout.Height(16)
								});
								GUILayout.EndHorizontal();
								GUILayout.BeginHorizontal(new GUILayoutOption[] { });
								GUILayout.Box("static function PlayerPrefsElite.SetIntArray (", editorSkin.customStyles[47], new GUILayoutOption[] {
									GUILayout.MinWidth(100),
									GUILayout.MaxWidth(343),
									GUILayout.Height(16)
								});
								GUILayout.EndHorizontal();
								GUILayout.BeginHorizontal(new GUILayoutOption[] { });
								GUILayout.Box("\tkey : String, value : Array, secureKey : int", editorSkin.customStyles[47], new GUILayoutOption[] {
									GUILayout.MinWidth(100),
									GUILayout.MaxWidth(343),
									GUILayout.Height(16)
								});
								GUILayout.EndHorizontal();
								GUILayout.BeginHorizontal(new GUILayoutOption[] { });
								GUILayout.Box(") : void", editorSkin.customStyles[47], new GUILayoutOption[] {
									GUILayout.MinWidth(100),
									GUILayout.MaxWidth(343),
									GUILayout.Height(16)
								});
								GUILayout.EndHorizontal();
								GUILayout.BeginHorizontal(new GUILayoutOption[] { });
								GUILayout.Box("static function PlayerPrefsElite.SetFloatArray (", editorSkin.customStyles[47], new GUILayoutOption[] {
									GUILayout.MinWidth(100),
									GUILayout.MaxWidth(343),
									GUILayout.Height(16)
								});
								GUILayout.EndHorizontal();
								GUILayout.BeginHorizontal(new GUILayoutOption[] { });
								GUILayout.Box("\tkey : String, value : Array, secureKey : int", editorSkin.customStyles[47], new GUILayoutOption[] {
									GUILayout.MinWidth(100),
									GUILayout.MaxWidth(343),
									GUILayout.Height(16)
								});
								GUILayout.EndHorizontal();
								GUILayout.BeginHorizontal(new GUILayoutOption[] { });
								GUILayout.Box(") : void", editorSkin.customStyles[47], new GUILayoutOption[] {
									GUILayout.MinWidth(100),
									GUILayout.MaxWidth(343),
									GUILayout.Height(16)
								});
								GUILayout.EndHorizontal();
								GUILayout.BeginHorizontal(new GUILayoutOption[] { });
								GUILayout.Box("", editorSkin.customStyles[47], new GUILayoutOption[] {
									GUILayout.MinWidth(100),
									GUILayout.MaxWidth(343),
									GUILayout.Height(16)
								});
								GUILayout.EndHorizontal();
								GUILayout.BeginHorizontal(new GUILayoutOption[] { });
								GUILayout.Box("Example:", editorSkin.customStyles[47], new GUILayoutOption[] {
									GUILayout.MinWidth(100),
									GUILayout.MaxWidth(343),
									GUILayout.Height(16)
								});
								GUILayout.EndHorizontal();
								if (data[editkey].keyid == 0) {
									GUILayout.BeginHorizontal(new GUILayoutOption[] { });
									GUILayout.Box("var Foobar: String[];", editorSkin.customStyles[54], new GUILayoutOption[] {
										GUILayout.MinWidth(100),
										GUILayout.MaxWidth(343),
										GUILayout.Height(18)
									});
									GUILayout.EndHorizontal();
									GUILayout.BeginHorizontal(new GUILayoutOption[] { });
									GUILayout.Box(("PlayerPrefsElite.SetStringArray(\"" + data[editkey].key) + "\", Foobar);", editorSkin.customStyles[54], new GUILayoutOption[] {
										GUILayout.MinWidth(100),
										GUILayout.MaxWidth(343),
										GUILayout.Height(18)
									});
									if (GUILayout.Button(new GUIContent("", "Copy to Clipboard"), editorSkin.customStyles[49], new GUILayoutOption[] { })) {
										EditorGUIUtility.systemCopyBuffer = ("PlayerPrefsElite.SetStringArray(\"" + data[editkey].key) + "\", Foobar);";
									}
									GUILayout.EndHorizontal();
								} else {
									GUILayout.BeginHorizontal(new GUILayoutOption[] { });
									GUILayout.Box(((("PlayerPrefsElite.SetStringArray(\"" + data[editkey].key) + "\", Foobar, ") + data[editkey].keyid) + ");", editorSkin.customStyles[54], new GUILayoutOption[] {
										GUILayout.MinWidth(100),
										GUILayout.MaxWidth(343),
										GUILayout.Height(18)
									});
									if (GUILayout.Button(new GUIContent("", "Copy to Clipboard"), editorSkin.customStyles[49], new GUILayoutOption[] { })) {
										EditorGUIUtility.systemCopyBuffer = ((("PlayerPrefsElite.SetStringArray(\"" + data[editkey].key) + "\", Foobar, ") + data[editkey].keyid) + ");";
									}
									GUILayout.EndHorizontal();
								}
							} else {
								if (data[editkey].type == "int") {
									GUILayout.BeginHorizontal(new GUILayoutOption[] { });
									GUILayout.Box("static function PlayerPrefsElite.SetInt (", editorSkin.customStyles[47], new GUILayoutOption[] {
										GUILayout.MinWidth(100),
										GUILayout.MaxWidth(343),
										GUILayout.Height(16)
									});
									GUILayout.EndHorizontal();
									GUILayout.BeginHorizontal(new GUILayoutOption[] { });
									GUILayout.Box("\tkey : String, value : int, secureKey : int", editorSkin.customStyles[47], new GUILayoutOption[] {
										GUILayout.MinWidth(100),
										GUILayout.MaxWidth(343),
										GUILayout.Height(16)
									});
									GUILayout.EndHorizontal();
									GUILayout.BeginHorizontal(new GUILayoutOption[] { });
									GUILayout.Box(") : void", editorSkin.customStyles[47], new GUILayoutOption[] {
										GUILayout.MinWidth(100),
										GUILayout.MaxWidth(343),
										GUILayout.Height(16)
									});
									GUILayout.EndHorizontal();
									GUILayout.BeginHorizontal(new GUILayoutOption[] { });
									GUILayout.Box("", editorSkin.customStyles[47], new GUILayoutOption[] {
										GUILayout.MinWidth(100),
										GUILayout.MaxWidth(343),
										GUILayout.Height(16)
									});
									GUILayout.EndHorizontal();
									GUILayout.BeginHorizontal(new GUILayoutOption[] { });
									GUILayout.Box("Example:", editorSkin.customStyles[47], new GUILayoutOption[] {
										GUILayout.MinWidth(100),
										GUILayout.MaxWidth(343),
										GUILayout.Height(16)
									});
									GUILayout.EndHorizontal();
									if (data[editkey].keyid == 0) {
										GUILayout.BeginHorizontal(new GUILayoutOption[] { });
										GUILayout.Box(("PlayerPrefsElite.SetInt(\"" + data[editkey].key) + "\", 10);", editorSkin.customStyles[54], new GUILayoutOption[] {
											GUILayout.MinWidth(100),
											GUILayout.MaxWidth(343),
											GUILayout.Height(18)
										});
										if (GUILayout.Button(new GUIContent("", "Copy to Clipboard"), editorSkin.customStyles[49], new GUILayoutOption[] { })) {
											EditorGUIUtility.systemCopyBuffer = ("PlayerPrefsElite.SetInt(\"" + data[editkey].key) + "\", 10);";
										}
										GUILayout.EndHorizontal();
									} else {
										GUILayout.BeginHorizontal(new GUILayoutOption[] { });
										GUILayout.Box(((("PlayerPrefsElite.SetInt(\"" + data[editkey].key) + "\",  10, ") + data[editkey].keyid) + ");", editorSkin.customStyles[54], new GUILayoutOption[] {
											GUILayout.MinWidth(100),
											GUILayout.MaxWidth(343),
											GUILayout.Height(18)
										});
										if (GUILayout.Button(new GUIContent("", "Copy to Clipboard"), editorSkin.customStyles[49], new GUILayoutOption[] { })) {
											EditorGUIUtility.systemCopyBuffer = ((("PlayerPrefsElite.SetInt(\"" + data[editkey].key) + "\",  10, ") + data[editkey].keyid) + ");";
										}
										GUILayout.EndHorizontal();
									}
								} else {
									if (data[editkey].type == "float") {
										GUILayout.BeginHorizontal(new GUILayoutOption[] { });
										GUILayout.Box("static function PlayerPrefsElite.SetFloat (", editorSkin.customStyles[47], new GUILayoutOption[] {
											GUILayout.MinWidth(100),
											GUILayout.MaxWidth(343),
											GUILayout.Height(16)
										});
										GUILayout.EndHorizontal();
										GUILayout.BeginHorizontal(new GUILayoutOption[] { });
										GUILayout.Box("\tkey : String, value : float, secureKey : int", editorSkin.customStyles[47], new GUILayoutOption[] {
											GUILayout.MinWidth(100),
											GUILayout.MaxWidth(343),
											GUILayout.Height(16)
										});
										GUILayout.EndHorizontal();
										GUILayout.BeginHorizontal(new GUILayoutOption[] { });
										GUILayout.Box(") : void", editorSkin.customStyles[47], new GUILayoutOption[] {
											GUILayout.MinWidth(100),
											GUILayout.MaxWidth(343),
											GUILayout.Height(16)
										});
										GUILayout.EndHorizontal();
										GUILayout.BeginHorizontal(new GUILayoutOption[] { });
										GUILayout.Box("", editorSkin.customStyles[47], new GUILayoutOption[] {
											GUILayout.MinWidth(100),
											GUILayout.MaxWidth(343),
											GUILayout.Height(16)
										});
										GUILayout.EndHorizontal();
										GUILayout.BeginHorizontal(new GUILayoutOption[] { });
										GUILayout.Box("Example:", editorSkin.customStyles[47], new GUILayoutOption[] {
											GUILayout.MinWidth(100),
											GUILayout.MaxWidth(343),
											GUILayout.Height(16)
										});
										GUILayout.EndHorizontal();
										if (data[editkey].keyid == 0) {
											GUILayout.BeginHorizontal(new GUILayoutOption[] { });
											GUILayout.Box(("PlayerPrefsElite.SetFloat(\"" + data[editkey].key) + "\", 10.0f);", editorSkin.customStyles[54], new GUILayoutOption[] {
												GUILayout.MinWidth(100),
												GUILayout.MaxWidth(343),
												GUILayout.Height(18)
											});
											if (GUILayout.Button(new GUIContent("", "Copy to Clipboard"), editorSkin.customStyles[49], new GUILayoutOption[] { })) {
												EditorGUIUtility.systemCopyBuffer = ("PlayerPrefsElite.SetFloat(\"" + data[editkey].key) + "\", 10.0f);";
											}
											GUILayout.EndHorizontal();
										} else {
											GUILayout.BeginHorizontal(new GUILayoutOption[] { });
											GUILayout.Box(((("PlayerPrefsElite.SetFloat(\"" + data[editkey].key) + "\",  10.0f, ") + data[editkey].keyid) + ");", editorSkin.customStyles[54], new GUILayoutOption[] {
												GUILayout.MinWidth(100),
												GUILayout.MaxWidth(343),
												GUILayout.Height(18)
											});
											if (GUILayout.Button(new GUIContent("", "Copy to Clipboard"), editorSkin.customStyles[49], new GUILayoutOption[] { })) {
												EditorGUIUtility.systemCopyBuffer = ((("PlayerPrefsElite.SetFloat(\"" + data[editkey].key) + "\",  10.0f, ") + data[editkey].keyid) + ");";
											}
											GUILayout.EndHorizontal();
										}
									}
								}
							}
						}
					}
					GUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Box("", editorSkin.customStyles[47], new GUILayoutOption[] {
						GUILayout.MinWidth(100),
						GUILayout.MaxWidth(343),
						GUILayout.Height(4)
					});
					GUILayout.EndHorizontal();
					GUILayout.Space(5);
					GUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Box("", editorSkin.customStyles[47], new GUILayoutOption[] {
						GUILayout.MinWidth(100),
						GUILayout.MaxWidth(343),
						GUILayout.Height(4)
					});
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Box("Verify value:", editorSkin.customStyles[54], new GUILayoutOption[] {
						GUILayout.MinWidth(100),
						GUILayout.MaxWidth(343),
						GUILayout.Height(16)
					});
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Box("", editorSkin.customStyles[47], new GUILayoutOption[] {
						GUILayout.MinWidth(100),
						GUILayout.MaxWidth(343),
						GUILayout.Height(16)
					});
					GUILayout.EndHorizontal();
					string dk = "";
					if ((data[editkey].type == "string") && !data[editkey].array) {
						dk = "String";
					} else {
						if ((data[editkey].type == "int") && data[editkey].boolean) {
							dk = "Boolean";
						} else {
							if (data[editkey].type == "int") {
								dk = "Int";
							} else {
								if (data[editkey].type == "float") {
									dk = "Float";
								} else {
									if (data[editkey].array) {
										dk = "Array";
									}
								}
							}
						}
					}
					GUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Box(("PlayerPrefsElite.Verify" + dk) + "(key:String, secureKey:int) : boolean", editorSkin.customStyles[47], new GUILayoutOption[] {
						GUILayout.MinWidth(100),
						GUILayout.MaxWidth(343),
						GUILayout.Height(16)
					});
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Box("", editorSkin.customStyles[47], new GUILayoutOption[] {
						GUILayout.MinWidth(100),
						GUILayout.MaxWidth(343),
						GUILayout.Height(16)
					});
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Box("Example:", editorSkin.customStyles[47], new GUILayoutOption[] {
						GUILayout.MinWidth(100),
						GUILayout.MaxWidth(343),
						GUILayout.Height(16)
					});
					GUILayout.EndHorizontal();
					if (data[editkey].keyid == 0) {
						GUILayout.BeginHorizontal(new GUILayoutOption[] { });
						GUILayout.Box(((("if (!PlayerPrefsElite.Verify" + dk) + "(\"") + data[editkey].key) + "\")){", editorSkin.customStyles[54], new GUILayoutOption[] {
							GUILayout.MinWidth(100),
							GUILayout.MaxWidth(343),
							GUILayout.Height(18)
						});
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal(new GUILayoutOption[] { });
						GUILayout.Box("\t// return false", editorSkin.customStyles[54], new GUILayoutOption[] {
							GUILayout.MinWidth(100),
							GUILayout.MaxWidth(343),
							GUILayout.Height(18)
						});
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal(new GUILayoutOption[] { });
						GUILayout.Box("\t// modified value detected", editorSkin.customStyles[54], new GUILayoutOption[] {
							GUILayout.MinWidth(100),
							GUILayout.MaxWidth(343),
							GUILayout.Height(18)
						});
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal(new GUILayoutOption[] { });
						GUILayout.Box("\t// do something", editorSkin.customStyles[54], new GUILayoutOption[] {
							GUILayout.MinWidth(100),
							GUILayout.MaxWidth(343),
							GUILayout.Height(18)
						});
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal(new GUILayoutOption[] { });
						GUILayout.Box("}", editorSkin.customStyles[54], new GUILayoutOption[] {
							GUILayout.MinWidth(100),
							GUILayout.MaxWidth(343),
							GUILayout.Height(18)
						});
						if (GUILayout.Button(new GUIContent("", "Copy to Clipboard"), editorSkin.customStyles[49], new GUILayoutOption[] { })) {
							EditorGUIUtility.systemCopyBuffer = ((("if (!PlayerPrefsElite.Verify" + dk) + "(\"") + data[editkey].key) + "\")){\n\t// return false\n\t// modified value detected\n\t// do something\n}";
						}
						GUILayout.EndHorizontal();
					} else {
						GUILayout.BeginHorizontal(new GUILayoutOption[] { });
						GUILayout.Box(((((("if (!PlayerPrefsElite.Verify" + dk) + "(\"") + data[editkey].key) + "\", ") + data[editkey].keyid) + ")){\n\t// return false\n\t// modified value detected\n\t// do something\n}", editorSkin.customStyles[54], new GUILayoutOption[] {
							GUILayout.MinWidth(100),
							GUILayout.MaxWidth(343)
						});
						if (GUILayout.Button(new GUIContent("", "Copy to Clipboard"), editorSkin.customStyles[49], new GUILayoutOption[] { })) {
							EditorGUIUtility.systemCopyBuffer = ((((("if (!PlayerPrefsElite.Verify" + dk) + "(\"") + data[editkey].key) + "\", ") + data[editkey].keyid) + ")){\n\t// return false\n\t// modified value detected\n\t// do something\n}";
						}
						GUILayout.EndHorizontal();
					}
					GUILayout.Space(5);
					GUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Label("", editorSkin.customStyles[46], new GUILayoutOption[] {
						GUILayout.MinWidth(100),
						GUILayout.MaxWidth(343),
						GUILayout.Height(4)
					});
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(262) });
					if (GUILayout.Button(new GUIContent("", "Hide Detailed Info"), editorSkin.customStyles[53], new GUILayoutOption[] {
						GUILayout.Width(110),
						GUILayout.Height(20)
					})) {
						showCode = false;
					}
					GUILayout.EndHorizontal();
				} else {
					GUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Label("Code Example", editorSkin.customStyles[46], new GUILayoutOption[] {
						GUILayout.MinWidth(100),
						GUILayout.MaxWidth(343),
						GUILayout.Height(16)
					});
					GUILayout.EndHorizontal();
					GUILayout.Space(5);
					GUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(262) });
					if (GUILayout.Button(new GUIContent("", "Show Code Example"), editorSkin.customStyles[52], new GUILayoutOption[] {
						GUILayout.Width(110),
						GUILayout.Height(20)
					})) {
						showCode = true;
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.Space(15);
				GUILayout.BeginHorizontal(new GUILayoutOption[] { });
				GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(25) });
				saveProtected = EditorGUILayout.Toggle(saveProtected, myStyle, new GUILayoutOption[] { GUILayout.Width(20) });
				if (data[editkey].secure) {
					GUILayout.Label("Save as protected", new GUILayoutOption[] {
						GUILayout.Width(200),
						GUILayout.Height(18)
					});
				} else {
					GUILayout.Label("Convert to protected", new GUILayoutOption[] {
						GUILayout.Width(200),
						GUILayout.Height(18)
					});
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(20);
				if (!GUI.enabled) {
					GUI.enabled = true;
				}
				GUILayout.BeginHorizontal(new GUILayoutOption[] { });
				GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(25) });
				if (GUILayout.Button("Save", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Width(105) })) {
					if (saveProtected) {
						PlayerPrefsElite.setKeys(secureKeysManager.keys);
						if (data[editkey].secure) {
							if ((data[editkey].type == "string") && !data[editkey].array) {
								PlayerPrefsElite.SetString(data[editkey].key, (string)olddata, data[editkey].keyid);
							} else {
								if ((data[editkey].type == "int") && data[editkey].boolean) {
									PlayerPrefsElite.SetBoolean(data[editkey].key, (bool)olddata, data[editkey].keyid);
								} else {
									if (data[editkey].type == "int") {
										PlayerPrefsElite.SetInt(data[editkey].key, (int)olddata, data[editkey].keyid);
									} else {
										if (data[editkey].type == "float") {
											PlayerPrefsElite.SetFloat(data[editkey].key, (float)olddata, data[editkey].keyid);
										} else {
											if ((data[editkey].type == "string") && data[editkey].array) {
												PlayerPrefs.SetString(data[editkey].key, (string)olddata);
												PlayerPrefs.SetString(PlayerPrefsElite.sum(PlayerPrefsElite.prefix2 + data[editkey].key, PlayerPrefsElite.key[data[editkey].keyid]), PlayerPrefsElite.sum(olddata.ToString(), PlayerPrefsElite.key[data[editkey].keyid]));
											}
										}
									}
								}
							}
						} else {
							if (data[editkey].type == "string") {
								PlayerPrefsElite.SetString(data[editkey].key, (string)olddata);
							} else {
								if (data[editkey].type == "int") {
									PlayerPrefsElite.SetInt(data[editkey].key, (int)olddata);
								} else {
									if (data[editkey].type == "float") {
										PlayerPrefsElite.SetFloat(data[editkey].key, (float)olddata);
									}
								}
							}
						}
						PlayerPrefs.Save();
						GUIUtility.hotControl = 0;
						GUIUtility.keyboardControl = 0;
						tryedit = false;
					} else {
						if (data[editkey].type == "string") {
							PlayerPrefs.SetString(data[editkey].key, (string)olddata);
						} else {
							if (data[editkey].type == "int") {
								PlayerPrefs.SetInt(data[editkey].key, (int)olddata);
							} else {
								if (data[editkey].type == "float") {
									PlayerPrefs.SetFloat(data[editkey].key, (float)olddata);
								}
							}
						}
						GUIUtility.hotControl = 0;
						GUIUtility.keyboardControl = 0;
						tryedit = false;
					}
				}
				GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(2) });
				if (GUILayout.Button("Cancel", new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Width(105) })) {
					GUIUtility.hotControl = 0;
					GUIUtility.keyboardControl = 0;
					tryedit = false;
				}
				GUILayout.EndHorizontal();
				EditorGUILayout.EndScrollView();
				GUILayout.Space(5);
				return;
			}
			GUILayout.Space(5);
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, new GUILayoutOption[] { });
			int i = 0;
			while (i < data.Length) {
				if (showException(i)) {
					EditorGUILayout.BeginHorizontal(new GUILayoutOption[] { });
					GUILayout.Box(new GUIContent(getImage(data[i].secure, data[i].locked), secureString(i)), editorSkin.customStyles[36], new GUILayoutOption[] { GUILayout.Width(20) });
					GUILayout.Label(data[i].key, editorSkin.customStyles[37], new GUILayoutOption[] {
						GUILayout.Width(keyfieldsize),
						GUILayout.MaxHeight(30)
					});
					GUILayout.Label("", editorSkin.customStyles[6], new GUILayoutOption[] { GUILayout.Width(10) });
					GUILayout.Label(GetValue(data[i].key, data[i].type).ToString(), editorSkin.customStyles[38], new GUILayoutOption[] {
						GUILayout.MinWidth(30),
						GUILayout.MaxWidth(valuefieldsize),
						GUILayout.Height(30)
					});
					if (PlayerPrefs.HasKey(data[i].key)) {
						if (lockProtected && data[i].locked) {
							if (GUILayout.Button(new GUIContent("", "Locked"), editorSkin.customStyles[34], new GUILayoutOption[] { })) {
							}
						} else {
							if (GUILayout.Button(new GUIContent("", "Edit"), editorSkin.customStyles[33], new GUILayoutOption[] { })) {
								editkey = i;
								olddata = GetValue(data[i].key, data[i].type);
								showInfo = false;
								showCode = false;
								if (data[i].secure) {
									saveProtected = true;
								} else {
									saveProtected = false;
								}
								disablemenu(0);
								tryedit = true;
							}
						}
					}
					if (PlayerPrefs.HasKey(data[i].key)) {
						if (lockProtected && data[i].locked) {
							GUILayout.Label("", editorSkin.customStyles[6], new GUILayoutOption[] { GUILayout.Width(11) });
							if (GUILayout.Button(new GUIContent("", "Locked"), editorSkin.customStyles[32], new GUILayoutOption[] { })) {
							}
						} else {
							GUILayout.Label("", editorSkin.customStyles[6], new GUILayoutOption[] { GUILayout.Width(11) });
							if (GUILayout.Button(new GUIContent("", "Delete"), editorSkin.customStyles[31], new GUILayoutOption[] { })) {
								if (showAlerts) {
									if (EditorUtility.DisplayDialog(("Delete \"" + data[i].key) + "\" Key", "operation cannot be undone", "Yes", "Cancel")) {
										PlayerPrefs.DeleteKey(data[i].key);
										if (deleteLinked) {
											PlayerPrefs.DeleteKey(data[i].securename);
											if (data[i].locked) {
												PlayerPrefs.DeleteKey(data[data[i].linkid].key);
											}
										}
										PlayerPrefs.Save();
									}
								} else {
									PlayerPrefs.DeleteKey(data[i].key);
									if (deleteLinked) {
										PlayerPrefs.DeleteKey(data[i].securename);
										if (data[i].locked) {
											PlayerPrefs.DeleteKey(data[data[i].linkid].key);
										}
									}
									PlayerPrefs.Save();
								}
							}
						}
					} else {
						if (GUILayout.Button("", editorSkin.customStyles[35], new GUILayoutOption[] { })) {
						}
						GUILayout.Label("", editorSkin.customStyles[6], new GUILayoutOption[] { GUILayout.Width(11) });
						if (GUILayout.Button("", editorSkin.customStyles[32], new GUILayoutOption[] { })) {
						}
					}
					EditorGUILayout.EndHorizontal();
				}
				i++;
			}
			EditorGUILayout.EndScrollView();
			GUILayout.Space(5);
		}
		if (showdrop[1] && !showdrop[0]) {
			if (GUI.Button(new Rect(97, 125, 112, 25), "", editorSkin.customStyles[23])) {
				showdrop[1] = !showdrop[1];
			}
			if (GUI.Button(new Rect(97, 150, 112, 25), "", editorSkin.customStyles[22])) {
				showdrop[1] = !showdrop[1];
				showdrop[0] = !showdrop[0];
				showall = true;
			}
		}
		if (showdrop[1] && showdrop[0]) {
			if (GUI.Button(new Rect(97, 125, 112, 25), "", editorSkin.customStyles[24])) {
				showdrop[1] = !showdrop[1];
				showdrop[0] = !showdrop[0];
				showall = false;
			}
			if (GUI.Button(new Rect(97, 150, 112, 25), "", editorSkin.customStyles[21])) {
				showdrop[1] = !showdrop[1];
			}
		}
		if (showdrop[3]) {
			if (sortid == 0) {
				if (GUI.Button(new Rect(209, 125, 126, 25), "", editorSkin.customStyles[27])) {
					showdrop[3] = !showdrop[3];
					sortid = 0;
					sort = false;
				}
				if (GUI.Button(new Rect(209, 150, 126, 21), "", editorSkin.customStyles[26])) {
					showdrop[3] = !showdrop[3];
					sortid = 1;
					sort = true;
					sortaz = true;
				}
				if (GUI.Button(new Rect(209, 171, 126, 25), "", editorSkin.customStyles[30])) {
					showdrop[3] = !showdrop[3];
					sortid = 2;
					sort = true;
					sortaz = false;
				}
			} else {
				if (sortid == 1) {
					if (GUI.Button(new Rect(209, 125, 126, 25), "", editorSkin.customStyles[28])) {
						showdrop[3] = !showdrop[3];
						sortid = 0;
						sort = false;
					}
					if (GUI.Button(new Rect(209, 150, 126, 21), "", editorSkin.customStyles[25])) {
						showdrop[3] = !showdrop[3];
						sortid = 1;
						sort = true;
						sortaz = true;
					}
					if (GUI.Button(new Rect(209, 171, 126, 25), "", editorSkin.customStyles[30])) {
						showdrop[3] = !showdrop[3];
						sortid = 2;
						sort = true;
						sortaz = false;
					}
				} else {
					if (sortid == 2) {
						if (GUI.Button(new Rect(209, 125, 126, 25), "", editorSkin.customStyles[28])) {
							showdrop[3] = !showdrop[3];
							sortid = 0;
							sort = false;
							sort = false;
						}
						if (GUI.Button(new Rect(209, 150, 126, 21), "", editorSkin.customStyles[26])) {
							showdrop[3] = !showdrop[3];
							sortid = 1;
							sort = true;
							sortaz = true;
						}
						if (GUI.Button(new Rect(209, 171, 126, 25), "", editorSkin.customStyles[29])) {
							showdrop[3] = !showdrop[3];
							sortid = 2;
							sort = true;
							sortaz = false;
						}
					}
				}
			}
		}
	}

	public virtual Texture2D getImage(bool secure, bool locked)
	{
		if (!secure && !locked) {
			return icostandart;
		} else {
			if (secure && !locked) {
				return icosecure;
			} else {
				if (!secure && locked) {
					return icolocked;
				} else {
					if (secure && locked) {
						return icolocked;
					} else {
						return icostandart;
					}
				}
			}
		}
	}

	public virtual string secureString(int i)
	{
		string returnString = "";
		if (data[i].secure && data[i].locked) {
			returnString = ("data is protected, identified by key \"" + data[data[i].linkid].key) + "\"";
		} else {
			if (data[i].secure && !data[i].locked) {
				returnString = "data is protected";
			} else {
				if (!data[i].secure && data[i].locked) {
					returnString = "data is protected, encrypt";
				} else {
					returnString = "data is not protected";
				}
			}
		}
		if (!data[i].locked) {
			returnString = returnString + (", " + data[i].type);
		}
		return returnString;
	}

	public virtual void OnInspectorUpdate()
	{
		if (EditorApplication.isPlaying) {
			if (secureKeysManager == null) {
				secureKeysManager = ((SecureKeysManager)UnityEngine.Object.FindObjectOfType(typeof(SecureKeysManager))) as SecureKeysManager;
			}
			if (EditorApplication.timeSinceStartup > tmpTime) {
				if ((secureKeysManager != null) && (secureKeysManager.keys.Length > 0)) {
					loadFiles();
				}
				Repaint();
				tmpTime = (float)(EditorApplication.timeSinceStartup + updInterval);
			}
			if ((isplaying != EditorApplication.isPlayingOrWillChangePlaymode) && !isplaying) {
				isplaying = EditorApplication.isPlaying;
				PlayerPrefs.Save();
				tryedit = false;
			}
		} else {
			if (EditorApplication.timeSinceStartup > tmpTime) {
				if (secureKeysManager == null) {
					secureKeysManager = ((SecureKeysManager)UnityEngine.Object.FindObjectOfType(typeof(SecureKeysManager))) as SecureKeysManager;
				}
				if ((secureKeysManager != null) && (secureKeysManager.keys.Length > 0)) {
					loadFiles();
				}
				Repaint();
				tmpTime = (float)(EditorApplication.timeSinceStartup + updIntervalNP);
			}
			if (isplaying) {
				isplaying = false;
				if (secureKeysManager == null) {
					secureKeysManager = ((SecureKeysManager)UnityEngine.Object.FindObjectOfType(typeof(SecureKeysManager))) as SecureKeysManager;
				}
			}
		}
	}

	public float tmpTime;

	private bool showall;
	// private string result;

	public virtual bool showException(int i)
	{
		if (showall) {
			return true;
		} else {
			if (data[i].key == "UnityGraphicsQuality") {
				return false;
			} else {
				if (data[i].locked) {
					return false;
				} else {
					return true;
				}
			}
		}
	}

	public virtual object GetValue(string name, string _str)
	{
		if (PlayerPrefs.HasKey(name)) {
			if (_str == "string") {
				return PlayerPrefs.GetString(name);
			} else {
				if (_str == "int") {
					return PlayerPrefs.GetInt(name);
				} else {
					if (_str == "float") {
						return PlayerPrefs.GetFloat(name);
					} else {
						if (_str == "other") {
							return PlayerPrefs.GetString(name);
						} else {
							return PlayerPrefs.GetString(name);
						}
					}
				}
			}
		} else {
			return "" as string;
		}
	}

	public PlayerPrefsEliteEditor()
	{
		myString = "Hello World";
		myBool = true;
		myFloat = 1.23f;
		updInterval = 0.2f;
		updIntervalNP = 1f;
		showdrop = new bool[6];
		keyfieldsize = 80;
		valuefieldsize = 227;
		objNames = "";
	}

}