using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionPoint : MonoBehaviour
{
    [SerializeField] private int nextLevelIndex;

    [HideInInspector]
    [SerializeField] public bool drawText;

    public int NextLevelIndex
    {
        get { return nextLevelIndex; }
        set
        {
            nextLevelIndex = value;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Manager.instance.StateController.CurrentGameState == Game_state.overworld)
        {
            TransitionManager.Transition(nextLevelIndex);
        }
    }
}