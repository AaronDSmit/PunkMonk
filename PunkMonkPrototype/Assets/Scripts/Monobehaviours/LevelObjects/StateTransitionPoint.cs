using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTransitionPoint : MonoBehaviour
{
    [SerializeField] private Game_state targetState;

    [HideInInspector]
    [SerializeField] public bool drawText;

    public Game_state TargetState
    {
        get { return targetState; }

        set { targetState = value; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("EarthUnit"))
        {
            if (Manager.instance.StateController.CurrentGameState == Game_state.overworld)
            {
                Manager.instance.StateController.ChangeStateAfterFade(targetState);
            }
        }
    }
}