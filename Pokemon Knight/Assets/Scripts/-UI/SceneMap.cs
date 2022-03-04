// using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SceneMap : MonoBehaviour
{
    public string sceneName;
    public PlayerControls player;
    [Space] public GameObject playerHead;
    public RectTransform parentRect;
    public RectTransform rect;

    // Start is called before the first frame update
    void Start()
    {
        rect = this.GetComponent<RectTransform>();
        parentRect = this.transform.parent.GetComponent<RectTransform>();

        if (player.sceneMaps == null)
            player.sceneMaps = new Dictionary<string, SceneMap>();
            // player.sceneMaps = new HashSet<SceneMap>();
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
        playerHead = this.gameObject.transform.GetChild(0).gameObject;
    }
}


[CanEditMultipleObjects] [CustomEditor(typeof(SceneMap), true)]
public class SceneMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SceneMap myScript = (SceneMap)target;

        if (GUILayout.Button("Set Player Head"))
            myScript.GetPlayerHead();

    }
}