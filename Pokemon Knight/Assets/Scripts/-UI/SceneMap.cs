// using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SceneMap : MonoBehaviour
{
    public string sceneName;
    public PlayerControls player;
    [Space] public GameObject playerHead;
    [Space] public GameObject lostBag;
    [Space] public RectTransform parentRect;
    public RectTransform rect;


    private void Awake() 
    {
        if (sceneName == "")
            sceneName = this.gameObject.name;
    }
    // Start is called before the first frame update
    void Start()
    {
        rect = this.GetComponent<RectTransform>();
        parentRect = this.transform.parent.GetComponent<RectTransform>();

        if (player == null)
            Debug.LogError("PlayerControls not registered!! FOR " + this.gameObject.name, this.gameObject);

        if (player.sceneMaps == null)
            player.sceneMaps = new Dictionary<string, SceneMap>();

        player.sceneMaps.Add(sceneName, this);

        int gameNumber = PlayerPrefsElite.GetInt("gameNumber");
        if (PlayerPrefsElite.VerifyArray("visitedScenes" + gameNumber))
        {
            HashSet<string> sceneSet = new HashSet<string>( PlayerPrefsElite.GetStringArray("visitedScenes" + gameNumber) );
            if (sceneSet.Contains(sceneName))
                Visited();
            else
                this.gameObject.SetActive(false);
        }
        else
            this.gameObject.SetActive(false);
    }

    public void Visited()
    {
        this.gameObject.SetActive(true);
    }
    public void EnterScene()
    {
        // if (sceneName == currentScene)
        playerHead.SetActive(true);
    }
    public void LeftScene()
    {
        playerHead.SetActive(false);
    }
    
    public void GetPlayerHead()
    {
        var serialisedObj = new SerializedObject(this);
        // playerHead = this.gameObject.transform.GetChild(0).gameObject;
        serialisedObj.FindProperty("playerHead").objectReferenceValue = this.gameObject.transform.GetChild(0).gameObject;
        
        serialisedObj.FindProperty("sceneName").stringValue = this.gameObject.name;
        
        serialisedObj.ApplyModifiedProperties();
    }
    
    public void GetBagChild()
    {
        var serialisedObj = new SerializedObject(this);
        // playerHead = this.gameObject.transform.GetChild(0).gameObject;
        RectTransform[] childs = this.gameObject.GetComponentsInChildren<RectTransform>();
        if (lostBag == null)
            serialisedObj.FindProperty("lostBag").objectReferenceValue = 
                this.gameObject.transform.GetChild(childs.Length - 1).gameObject;
        // childs[ childs.Length - 1 ].anchoredPosition = Vector2.zero;
        childs[ childs.Length - 1 ].anchorMax = new Vector2(0.5f , 0.9f);
        childs[ childs.Length - 1 ].anchorMin = new Vector2(0.5f , 0.9f);
        // serialisedObj.FindProperty("sceneName").stringValue = this.gameObject.name;
        
        serialisedObj.ApplyModifiedProperties();
    }
}


[CanEditMultipleObjects] [CustomEditor(typeof(SceneMap), true)]
public class SceneMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SceneMap myScript = (SceneMap)target;

        if (GUILayout.Button("Set Player Head"))
            myScript.GetPlayerHead();
        
        EditorGUILayout.Space();
        if (GUILayout.Button("Set Bag"))
            myScript.GetBagChild();

        EditorGUILayout.Space();
        DrawDefaultInspector();
    }
}