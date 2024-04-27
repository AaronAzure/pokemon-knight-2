using System.Collections;
using UnityEngine;
// using Rewired;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ButtonLayout : MonoBehaviour
{
    private Rewired.InputManager reInput;

    [Space] private string[] controllers;
    [Space] private bool isSwitchProController;
    public TextMeshProUGUI buttonTxt;
    
    // [Space] public bool hasSpriteAsset=false;
    [Space] public TextMeshPro inGameTxt;


    // Start is called before the first frame update
    void Start()
    {
        // StartCoroutine( FindController() );
        isSwitchProController = PlayerPrefsElite.GetBoolean("nintendoControls");;

        // if (inGameTxt != null)
        //     inGameTxt.text.Replace( "to" , ":)" );
    
        if (!isSwitchProController)
        {
            if (inGameTxt != null)
            {
                Debug.Log("changing text");
                inGameTxt.text = inGameTxt.text.Replace( "<sprite=0>" , "<spprite=3>" );
                inGameTxt.text = inGameTxt.text.Replace( "<sprite=1>" , "<spprite=2>" );
                inGameTxt.text = inGameTxt.text.Replace( "<sprite=2>" , "<spprite=1>" );
                inGameTxt.text = inGameTxt.text.Replace( "<sprite=3>" , "<spprite=0>" );
                inGameTxt.text = inGameTxt.text.Replace( "<sprite=14>", "<spprite=19>");
                inGameTxt.text = inGameTxt.text.Replace( "<sprite=16>", "<spprite=18>");
                inGameTxt.text = inGameTxt.text.Replace( "<sprite=15>", "<spprite=21>");
                inGameTxt.text = inGameTxt.text.Replace( "<sprite=17>", "<spprite=20>");
                inGameTxt.text = inGameTxt.text.Replace( "<spprite", "<sprite");
            }
            else if (buttonTxt != null)
            {
                switch (buttonTxt.text.ToUpper())
                {
                    case "A":
                        buttonTxt.text = "B";
                        break;
                    case "B":
                        buttonTxt.text = "A";
                        break;
                    case "X":
                        buttonTxt.text = "Y";
                        break;
                    case "Y":
                        buttonTxt.text = "X";
                        break;

                    case "L":
                        buttonTxt.text = "LB";
                        break;
                    case "R":
                        buttonTxt.text = "RB";
                        break;
                    case "ZL":
                        buttonTxt.text = "LT";
                        break;
                    case "ZR":
                        buttonTxt.text = "RT";
                        break;
                }
            }
        }
    }

    IEnumerator FindController()
    {
        yield return new WaitForSeconds(0.5f);
        // controllers = ReInput.controllers.GetControllerNames(ControllerType.Joystick);
        // if (controllers.Length > 0 && controllers[0].Contains("Pro Controller"))
        //     isSwitchProController = true;
        // else
        //     Debug.LogError("have not detected controller");
        isSwitchProController = PlayerPrefsElite.GetBoolean("nintendoControls");;
    
        if (!isSwitchProController && buttonTxt != null && controllers.Length > 0)
        {
            switch (buttonTxt.text.ToUpper())
            {
                case "A":
                    buttonTxt.text = "B";
                    break;
                case "B":
                    buttonTxt.text = "A";
                    break;
                case "X":
                    buttonTxt.text = "Y";
                    break;
                case "Y":
                    buttonTxt.text = "X";
                    break;

                case "L":
                    buttonTxt.text = "LB";
                    break;
                case "R":
                    buttonTxt.text = "RB";
                    break;
                case "ZL":
                    buttonTxt.text = "LT";
                    break;
                case "ZR":
                    buttonTxt.text = "RT";
                    break;
            }
        }
    }

    public void SetTextVariables()
    {
        if (this.GetComponent<TextMeshProUGUI>() != null)
            buttonTxt = this.GetComponent<TextMeshProUGUI>();

        if (this.GetComponent<TextMeshPro>() != null)
            inGameTxt = this.GetComponent<TextMeshPro>();
        
    }
}

// [CanEditMultipleObjects] [CustomEditor(typeof(ButtonLayout), true)]
// public class ButtonLayoutEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         DrawDefaultInspector();

//         ButtonLayout myScript = (ButtonLayout)target;
        
//         EditorGUILayout.Space();
//         if (GUILayout.Button("Set TMPRO texts"))
//         {
//             myScript.SetTextVariables();

//             // myScript.ApplyModifiedProperties();
//         }
//         EditorUtility.SetDirty(myScript);

//     }
// }