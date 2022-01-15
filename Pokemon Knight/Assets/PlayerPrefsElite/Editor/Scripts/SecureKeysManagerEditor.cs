using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

[System.Serializable]
[UnityEditor.CustomEditor(typeof(SecureKeysManager))]
[UnityEditor.CanEditMultipleObjects]
public class SecureKeysManagerEditor : Editor
{
    public System.Collections.Generic.List<bool> boolarray;

    public SerializedObject m_Object;

    public SerializedProperty m_Property;

    public Texture2D icologo;

    public Texture2D keyon;

    public Texture2D keyoff;

    public Texture2D pplistofkeys;

    public SerializedProperty myIterator;

    private GUISkin editorSkin;

    public List<int> _toggleArray;

    public bool showAlerts;

    private string path;

    public virtual void OnEnable()
    {
        if (!EditorPrefs.HasKey(PlayerSettings.companyName + PlayerSettings.productName))
        {
            EditorPrefs.SetInt("showAlerts", 0);
            EditorPrefs.SetInt("minVal", 10);
            EditorPrefs.SetInt("maxVal", 16);
            EditorPrefs.SetString(PlayerSettings.companyName + PlayerSettings.productName, "");
        }
        SecureKeysManager s = ((SecureKeysManager)UnityEngine.Object.FindObjectOfType(typeof(SecureKeysManager))) as SecureKeysManager;
        allKeys = s;
        getboolarray();
        showAlerts = EditorPrefs.GetInt("showAlerts") == 1 ? true : false;
        minVal = EditorPrefs.GetInt("minVal");
        maxVal = EditorPrefs.GetInt("maxVal");
        m_Object = new SerializedObject(target);
        SetGFX();
    }

    public virtual void SetGFX()
    {
        string path = "Assets/PlayerPrefsElite/Editor/Gui/Images/";
        if (EditorGUIUtility.isProSkin)
        {
            icologo = ((Texture2D)AssetDatabase.LoadAssetAtPath(path + "secure-manager-logo-on-dark.png", typeof(Texture2D))) as Texture2D;
            keyon = ((Texture2D)AssetDatabase.LoadAssetAtPath(path + "dark-key-set.png", typeof(Texture2D))) as Texture2D;
            keyoff = ((Texture2D)AssetDatabase.LoadAssetAtPath(path + "dark-key-not-set.png", typeof(Texture2D))) as Texture2D;
            pplistofkeys = ((Texture2D)AssetDatabase.LoadAssetAtPath(path + "dark-pp-list-of-keys.png", typeof(Texture2D))) as Texture2D;
        }
        else
        {
            icologo = ((Texture2D)AssetDatabase.LoadAssetAtPath(path + "secure-manager-logo-on-light.png", typeof(Texture2D))) as Texture2D;
            keyon = ((Texture2D)AssetDatabase.LoadAssetAtPath(path + "key-set.png", typeof(Texture2D))) as Texture2D;
            keyoff = ((Texture2D)AssetDatabase.LoadAssetAtPath(path + "key-not-set.png", typeof(Texture2D))) as Texture2D;
            pplistofkeys = ((Texture2D)AssetDatabase.LoadAssetAtPath(path + "pp-list-of-keys.png", typeof(Texture2D))) as Texture2D;
        }
        LoadSkin();
        EditorApplication.RepaintProjectWindow();
    }

    private char[] rndKeys;

    public virtual string genKey()
    {
        string _newkey = "";
        int _count = Random.Range(minVal, maxVal + 1);
        int i = 0;
        while (i < _count)
        {
            _newkey = _newkey + rndKeys[Random.Range(0, rndKeys.Length)];
            i++;
        }
        return _newkey;
    }

    public virtual void LoadSkin()
    {
        if (EditorGUIUtility.isProSkin)
        {
            editorSkin = ((GUISkin)AssetDatabase.LoadAssetAtPath("Assets/PlayerPrefsElite/Editor/Gui/Images/skind.guiskin", typeof(GUISkin))) as GUISkin;
        }
        else
        {
            editorSkin = ((GUISkin)AssetDatabase.LoadAssetAtPath("Assets/PlayerPrefsElite/Editor/Gui/Images/skin.guiskin", typeof(GUISkin))) as GUISkin;
        }
    }

    private SecureKeysManager allKeys;

    public override void OnInspectorGUI()
    {
        GUILayout.Space(20);
        GUILayout.BeginHorizontal(new GUILayoutOption[] { });
        GUILayout.Label(icologo, new GUILayoutOption[] { GUILayout.MinWidth(72) });
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        if (EditorApplication.isPlaying)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[] { });
            GUILayout.FlexibleSpace();
            EditorGUILayout.HelpBox("\nLocked in Play mode\t\t  \n", MessageType.Info, true);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return;
        }
        if (editorSkin == null)
        {
            LoadSkin();
        }
        if (allKeys == null)
        {
            GUILayout.Space(20);
            GUILayout.BeginHorizontal(new GUILayoutOption[] { });
            GUILayout.FlexibleSpace();
            EditorGUILayout.HelpBox("\nDrag SecureKeysManager prefab into Hierarchy window\t\t  \n", MessageType.Info, true);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return;
        }
        if (!Selection.activeTransform)
        {
            GUILayout.Space(20);
            GUILayout.BeginHorizontal(new GUILayoutOption[] { });
            GUILayout.FlexibleSpace();
            EditorGUILayout.HelpBox("\nSelect SecureKeysManager in Hierarchy window\t\t  \n", MessageType.Info, true);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return;
        }
        GUIStyle myStyle = new GUIStyle(GUI.skin.toggle);
        myStyle.overflow.top = -2;
        //serializedObject.Update();
        if (!settings)
        {

            if (GUILayout.Button("", editorSkin.customStyles[7], new GUILayoutOption[] { }))
            {
                settings = true;
            }
        }
        else
        {
            if (settings)
            {
                if (GUILayout.Button("", editorSkin.customStyles[8], new GUILayoutOption[] { }))
                {
                    settings = false;
                }
                GUILayout.Space(5);
                GUILayout.BeginHorizontal(new GUILayoutOption[] { });
                GUILayout.Label("Show Alerts", new GUILayoutOption[] { GUILayout.Width(84) });
                showAlerts = EditorGUILayout.Toggle(showAlerts, myStyle, new GUILayoutOption[] { });
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(new GUILayoutOption[] { });
                GUILayout.FlexibleSpace();
                GUILayout.Label("Key Length:", new GUILayoutOption[] { });
                GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(0) });
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(new GUILayoutOption[] { });
                GUILayout.Label("Min value:", new GUILayoutOption[] { GUILayout.MinWidth(85) });
                minVal = (int)GUILayout.HorizontalSlider(minVal, 8, 56, new GUILayoutOption[] { GUILayout.MaxWidth(Screen.width) });
                minVal = EditorGUILayout.IntField(minVal, new GUILayoutOption[] { GUILayout.Width(72) });
                if (minVal < 8)
                {
                    minVal = 8;
                }
                if (minVal > 56)
                {
                    minVal = 56;
                }
                if (minVal > maxVal)
                {
                    maxVal = minVal;
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(new GUILayoutOption[] { });
                GUILayout.Label("Max value:", new GUILayoutOption[] { GUILayout.MinWidth(85) });
                maxVal = (int)GUILayout.HorizontalSlider(maxVal, 8, 56, new GUILayoutOption[] { GUILayout.MaxWidth(Screen.width) });
                maxVal = EditorGUILayout.IntField(maxVal, new GUILayoutOption[] { GUILayout.Width(72) });
                if (maxVal < 8)
                {
                    maxVal = 8;
                }
                if (maxVal > 56)
                {
                    maxVal = 56;
                }
                if (maxVal < minVal)
                {
                    minVal = maxVal;
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                if (GUILayout.Button("Reset to Default", new GUILayoutOption[] { GUILayout.Width(105) }))
                {
                    EditorPrefs.SetInt("showAlerts", 1);
                    EditorPrefs.SetInt("minVal", 10);
                    EditorPrefs.SetInt("maxVal", 16);
                    showAlerts = EditorPrefs.GetInt("showAlerts") == 1 ? true : false;
                    minVal = EditorPrefs.GetInt("minVal");
                    maxVal = EditorPrefs.GetInt("maxVal");
                }
                GUILayout.Space(10);
                if (GUI.changed)
                {
                    EditorPrefs.SetInt("showAlerts", showAlerts ? 1 : 0);
                    EditorPrefs.SetInt("minVal", minVal);
                    EditorPrefs.SetInt("maxVal", maxVal);
                }
            }
        }
        GUILayout.Space(15);
        m_Property = m_Object.FindProperty("keys");

        if (m_Property.arraySize < 1)
        {
            EditorGUILayout.HelpBox("At least one key must be generated", MessageType.Warning, true);
            GUILayout.Space(10);
        }

        EditorGUILayout.BeginVertical(new GUILayoutOption[] { });

        int count = 0;

        do
        {
            if ((m_Property.propertyPath != "keys") && !m_Property.propertyPath.StartsWith("keys" + "."))
            {
                break;
            }
            if (m_Property.name == "size")
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[] { });
                GUILayout.Label(pplistofkeys, editorSkin.customStyles[9],
                    new GUILayoutOption[] { GUILayout.Width(86), GUILayout.Height(20) });
                if (allKeys.keys.Length < 1)
                {
                    GUILayout.Label(keyoff, editorSkin.customStyles[9],
                        new GUILayoutOption[] { GUILayout.Width(18), GUILayout.Height(22) });
                }
                else
                {
                    GUILayout.Label(keyon, editorSkin.customStyles[9],
                        new GUILayoutOption[] { GUILayout.Width(18), GUILayout.Height(22) });
                }
                GUILayout.Label(allKeys.keys.Length.ToString(), new GUILayoutOption[] { });
                GUILayout.FlexibleSpace();
                if (allKeys.keys.Length < 1)
                {
                    GUI.enabled = false;
                }
                if (GUILayout.Button("", editorSkin.customStyles[56], new GUILayoutOption[] { }))
                {
                    SaveKeys();
                }
                if (!GUI.enabled)
                {
                    GUI.enabled = true;
                }
                if (!EditorPrefs.HasKey((PlayerSettings.companyName + PlayerSettings.productName) + ".AllKeys"))
                {
                    GUI.enabled = false;
                }
                if (GUILayout.Button("", editorSkin.customStyles[55], new GUILayoutOption[] { }))
                {
                    LoadKeys();
                }
                if (!GUI.enabled)
                {
                    GUI.enabled = true;
                }
                GUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
            else
            {
                if (m_Property.name == "data")
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[] { });
                    GUILayout.Label("key " + count.ToString(), new GUILayoutOption[] { GUILayout.Width(40) });
                    GUILayout.Label(("( " + m_Property.stringValue.Length) + " )",
                        editorSkin.customStyles[10], new GUILayoutOption[] { GUILayout.Width(40) });
                    if (count < boolarray.Count)
                    {
                        boolarray[count] = EditorGUILayout.Toggle(boolarray[count], myStyle,
                            new GUILayoutOption[] { GUILayout.Width(20) });
                    }
                    EditorGUILayout.PropertyField(m_Property, GUIContent.none, true,
                        new GUILayoutOption[] { GUILayout.MinWidth(60) });
                    if (GUILayout.Button("", editorSkin.customStyles[2], new GUILayoutOption[] { }))
                    {
                        GUIUtility.hotControl = 0;
                        GUIUtility.keyboardControl = 0;
                        m_Property.stringValue = genKey();
                    }
                    GUILayout.EndHorizontal();
                    count++;
                }
            }

        } while (m_Property.NextVisible(true));

        EditorGUILayout.EndVertical();
        GUILayout.Space(20);
        GUILayout.BeginHorizontal(new GUILayoutOption[] { });


        if (GUILayout.Button("Delete Selected", new GUILayoutOption[] { GUILayout.Width(105) }))
        {
            if (getToogle())
            {
                if (showAlerts)
                {
                    if (EditorUtility.DisplayDialog("Delete Key", "operation cannot be undone", "Yes", "Cancel"))
                    {
                        var newarray = new List<string>();
                        foreach (var key in allKeys.keys)
                        {
                            newarray.Add(key);
                        }
                        _toggleArray.Sort();

                        for (int i = _toggleArray.Count; i > 0; i--)
                        {
                            newarray.RemoveAt(_toggleArray[i - 1]);
                        }

                        allKeys.keys = newarray.ToArray();
                        getboolarray();
                        m_Object = new SerializedObject(target);
                        EditorUtility.SetDirty(target);
                    }
                }
                else
                {
                    var newarray = new List<string>();
                    foreach (var key in allKeys.keys)
                    {
                        newarray.Add(key);
                    }
                    _toggleArray.Sort();

                    for (int i = _toggleArray.Count; i > 0; i--)
                    {
                        newarray.RemoveAt(_toggleArray[i - 1]);
                    }

                    allKeys.keys = newarray.ToArray();
                    getboolarray();
                    m_Object = new SerializedObject(target);
                    EditorUtility.SetDirty(target);
                }
            }
            GUIUtility.hotControl = 0;
            GUIUtility.keyboardControl = 0;
        }
        if (GUILayout.Button("Generate new key", new GUILayoutOption[] { GUILayout.MinWidth(124) }))
        {
            // newKey = "";
            //newarray = allKeys.keys;
            var newarray = new List<string>();

            foreach (var key in allKeys.keys)
            {
                newarray.Add(key);
            }

            newarray.Add(genKey());
            allKeys.keys = newarray.ToArray();
            getboolarray();
            m_Object = new SerializedObject(target);
            EditorUtility.SetDirty(target);
            GUIUtility.hotControl = 0;
            GUIUtility.keyboardControl = 0;
        }
        GUILayout.EndHorizontal();
        m_Object.ApplyModifiedProperties();
    }

    public bool settings;

    public int minVal;

    public int maxVal;

    public virtual void SaveKeys()
    {
        string allkeys = "";
        int i = 0;
        while (i < allKeys.keys.Length)
        {
            allkeys = allkeys + allKeys.keys[i];
            if (i < (allKeys.keys.Length - 1))
            {
                allkeys = allkeys + ",";
            }
            i++;
        }
        EditorPrefs.SetString((PlayerSettings.companyName + PlayerSettings.productName) + ".AllKeys", allkeys);
        GUIUtility.hotControl = 0;
        GUIUtility.keyboardControl = 0;
        if (showAlerts)
        {
            EditorUtility.DisplayDialog("Keys Saved", "", "Ok");
        }
    }

    public virtual void LoadKeys()
    {
        if ((EditorPrefs.GetString((PlayerSettings.companyName + PlayerSettings.productName) + ".AllKeys") == "") || !EditorPrefs.HasKey((PlayerSettings.companyName + PlayerSettings.productName) + ".AllKeys"))
        {
            if (showAlerts)
            {
                EditorUtility.DisplayDialog("Nothing to load", "", "Ok");
            }
        }
        else
        {
            allKeys.keys = null;
            boolarray = new System.Collections.Generic.List<bool>();
            string[] pt = EditorPrefs.GetString((PlayerSettings.companyName + PlayerSettings.productName) + ".AllKeys").Split(new char[] { ","[0] });
            allKeys.keys = pt;
            getboolarray();
            m_Object = new SerializedObject(target);
            EditorUtility.SetDirty(target);
            m_Object.ApplyModifiedProperties();
            GUIUtility.hotControl = 0;
            GUIUtility.keyboardControl = 0;
            if (showAlerts)
            {
                EditorUtility.DisplayDialog("Loaded Successfully", "", "Ok");
            }
        }
    }

    public virtual void OnInspectorUpdate()
    {
        Repaint();
    }

    public virtual void getboolarray()
    {
        if (allKeys != null)
        {
            boolarray = new System.Collections.Generic.List<bool>();
            int i = 0;
            while (i < allKeys.keys.Length)
            {
                boolarray.Add(false);
                i++;
            }
        }
    }

    public virtual bool getToogle()
    {
        _toggleArray = new List<int>();
        for (int i = 0; i < boolarray.Count; i++)
        {
            if (boolarray[i])
            {
                _toggleArray.Add(i);
            }
        }
        if (_toggleArray.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public SecureKeysManagerEditor()
    {
        boolarray = new System.Collections.Generic.List<bool>();
        showAlerts = true;
        rndKeys = "qwertyuiopasdfghjklzxcvbnm0123456789QWERTYUIOPASDFGHJKLZXCVBNM0123456789".ToCharArray();
        minVal = 10;
        maxVal = 16;
    }

}