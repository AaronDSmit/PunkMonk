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
        Manager.instance.StateController.ChangeGame_state(targetState);
    }
}