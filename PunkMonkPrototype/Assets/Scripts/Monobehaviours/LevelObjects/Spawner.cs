using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [HideInInspector]
    [SerializeField] private int turnToSpawn;

    [HideInInspector]
    [SerializeField] private Entity entityToSpawn = null;

    [HideInInspector]
    [SerializeField] private Color textColour = Color.white;

    [HideInInspector]
    [SerializeField] public bool drawText;

    private void Awake()
    {
        StateManager.TurnEvent += TurnEvent;
    }

    private void TurnEvent(Turn_state newState, Team team, int turnNumber)
    {
        if (newState == Turn_state.start && turnNumber == turnToSpawn)
        {
            Entity entity = Instantiate(entityToSpawn, transform.parent.position, Quaternion.identity);
        }
    }

    public int TurnToSpawn
    {
        get { return turnToSpawn; }

        set
        {
            turnToSpawn = value;
        }
    }

    public Entity EntityToSpawn
    {
        get { return entityToSpawn; }
        set { entityToSpawn = value; }
    }

    public Color TextColour
    {
        get { return textColour; }
        set { textColour = value; }
    }

}