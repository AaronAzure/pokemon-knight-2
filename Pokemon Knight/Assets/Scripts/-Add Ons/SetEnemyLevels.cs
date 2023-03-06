using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class SetEnemyLevels : MonoBehaviour
{
    public int levelToSet=1;
    
    public void SetAllChildLevels()
    {
        Enemy[] enemies = GetComponentsInChildren<Enemy>();
        Debug.Log("<color=green>Set " + enemies.Length + " enemies to Lv. " + levelToSet + "</color>");

        foreach (Enemy enemy in enemies)
        {
            var serialisedObj = new SerializedObject(enemy);
            // finding property you want to modify, in this case its vector3, 
            // could be either all custom classes
            serialisedObj.FindProperty("lv").intValue = levelToSet;
            // will apply all changes to your object in scene
            // enemy.lv = levelToSet;

            serialisedObj.ApplyModifiedProperties();
        }
    }
}

[CanEditMultipleObjects] [CustomEditor(typeof(SetEnemyLevels), true)]
public class SetEnemyLevelsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SetEnemyLevels myScript = (SetEnemyLevels)target;
        
        EditorGUILayout.Space();
        if (GUILayout.Button("Set Level To All Children"))
            myScript.SetAllChildLevels();

    }
}
#endif