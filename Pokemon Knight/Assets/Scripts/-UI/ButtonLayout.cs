using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class ButtonLayout : MonoBehaviour
{
    private Rewired.Player player;
    private Rewired.InputManager reInput;
    private Rewired.Joystick controls;


    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(0);
        // player.controllers.GetController(0);
        // reInput.inp
    }
}
