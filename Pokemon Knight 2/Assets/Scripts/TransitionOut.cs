using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;

public class TransitionOut : MonoBehaviour
{
    [SerializeField] private ProCamera2DTransitionsFX camTransition;
    void Start()
    {
        camTransition.TransitionEnter();
    }
}
