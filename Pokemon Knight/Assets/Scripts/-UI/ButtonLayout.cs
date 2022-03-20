using System.Collections;
using UnityEngine;
using Rewired;
using TMPro;

public class ButtonLayout : MonoBehaviour
{
    private Rewired.InputManager reInput;

    [Space] private string[] controllers;
    [Space] private bool isSwitchProController;
    public TextMeshProUGUI buttonTxt;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine( FindController() );

    }

    IEnumerator FindController()
    {
        yield return new WaitForSeconds(0.5f);
        controllers = ReInput.controllers.GetControllerNames(ControllerType.Joystick);
        if (controllers.Length > 0 && controllers[0].Contains("Pro Controller"))
            isSwitchProController = true;
        else
            Debug.LogError("have not detected controller");
    
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
}
