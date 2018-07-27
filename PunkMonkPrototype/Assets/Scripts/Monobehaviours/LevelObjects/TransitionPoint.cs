using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
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
        TransitionManager.Transition(nextLevelIndex);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TransitionManager.Transition(nextLevelIndex);
        }
    }
}