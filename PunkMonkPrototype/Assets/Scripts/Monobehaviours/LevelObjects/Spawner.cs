using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType { runner, watcher }

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

    [HideInInspector]
    [SerializeField]
    private UnitType spawnType;

    [HideInInspector]
    [SerializeField]
    public bool hasVolt;

    [HideInInspector]
    [SerializeField]
    public bool moveToHex;

    [HideInInspector]
    [SerializeField]
    public Hex targetHex;

    private void Awake()
    {
        Manager.instance.TurnController.EveryTurnEvent += TurnEvent;
    }

    private void TurnEvent(TurnManager.TurnState newState, int turnNumber)
    {
        if (newState == TurnManager.TurnState.start && turnNumber == turnToSpawn && index == Manager.instance.TurnController.BattleID)
        {
            SpawnUnit();
        }
    }

    public void SpawnUnit()
    {
        if (currentHex.CurrentUnit == null)
        {
            Vector3 spawnPos = transform.parent.position;
            spawnPos.y = 0.0f;

            Unit unit = Instantiate(unitToSpawn, spawnPos, Quaternion.identity);
            unit.Spawn(currentHex);

            unit.CurrentVolt = (hasVolt) ? 1 : 0;

            Manager.instance.PlayerController.SubscribeToUnitDeath(unit);
        }
    }

    public int TurnToSpawn
    {
        get { return turnToSpawn; }
        set { turnToSpawn = value; }
    }

    public Unit UnitToSpawn
    {
        get { return unitToSpawn; }
        set { unitToSpawn = value; }
    }

    public Color TextColour
    {
        get { return textColour; }
        set { textColour = value; }
    }

    public UnitType SpawnType
    {
        get { return spawnType; }

        set { spawnType = value; }
    }
}