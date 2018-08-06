using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [HideInInspector]
    [SerializeField]
    private int turnToSpawn;

    [HideInInspector]
    [SerializeField]
    private Unit unitToSpawn;

    [HideInInspector]
    [SerializeField]
    private Color textColour = Color.black;

    [HideInInspector]
    [SerializeField]
    public bool drawText;

    [HideInInspector]
    [SerializeField]
    public int index;

    [HideInInspector]
    [SerializeField]
    public Hex currentHex;

    private void Awake()
    {
        TurnManager.TurnEvent += TurnEvent;
    }

    private void OnDisable()
    {
        TurnManager.TurnEvent -= TurnEvent;
    }

    private void TurnEvent(Turn_state newState, TEAM team, int turnNumber)
    {
        if (newState == Turn_state.start && turnNumber == turnToSpawn)
        {
            SpawnUnit();
        }
    }

    public void SpawnUnit()
    {
        if (currentHex.IsTraversable)
        {
            Vector3 spawnPos = transform.parent.position;
            spawnPos.y = 0.0f;

            Unit unit = Instantiate(unitToSpawn, spawnPos, Quaternion.identity);
            unit.Spawn(currentHex);
        }
    }

    public int TurnToSpawn
    {
        get { return turnToSpawn; }
        set { turnToSpawn = value; }
    }

    public Unit UntiToSpawn
    {
        get { return unitToSpawn; }
        set { unitToSpawn = value; }
    }

    public Color TextColour
    {
        get { return textColour; }
        set { textColour = value; }
    }
}