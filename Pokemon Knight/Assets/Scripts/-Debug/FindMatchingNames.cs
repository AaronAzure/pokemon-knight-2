using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class FindMatchingNames : MonoBehaviour
{
    public void FindMatchingGameObjectNames()
    {
        // Transform[] allObjects = this.GetComponentsInChildren<Transform>();
        List<Transform> allObjects = new List<Transform>();
        foreach (Transform child in this.transform)
            allObjects.Add(child);

        Hashtable matches = new Hashtable();
        // if (matches.)
        foreach (Transform obj in allObjects)
            if (!matches.ContainsKey(obj.name))
                matches.Add(obj.name, 1);
            else
                matches[obj.name] = (int) matches[obj.name] + 1;

        bool found = false;
        foreach (string key in matches.Keys)
            if ((int) matches[key] > 1)
                Debug.Log(key); found = true;

        if (!found)
            Debug.Log("None Found");
    }
}


[CustomEditor(typeof(FindMatchingNames), true)]
public class FindMatchingNamesEditor : Editor {
    public override void OnInspectorGUI() 
    {
        FindMatchingNames myScript = (FindMatchingNames)target;

        if (GUILayout.Button("Find match"))
            myScript.FindMatchingGameObjectNames();
        
        EditorGUILayout.Space();

        base.OnInspectorGUI();
    }
}