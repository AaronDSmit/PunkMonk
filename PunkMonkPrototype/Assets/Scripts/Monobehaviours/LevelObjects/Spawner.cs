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
    public Hex targetHex = null;

    [HideInInspector]
    [SerializeField]
    public Hex doorHex = null;

    private bool doneSpawning = false;

    private TurnManager turnController = null;
    private HexHighlighter hexHighlighter = null;

    private void Awake()
    {
        turnController = Manager.instance.TurnController;
        hexHighlighter = Manager.instance.HexHighlighter;

        turnController.SpawningEvent += StartSpawningEvent;
        turnController.PlayerTurnEvent += HighlightTilesEvent;
    }

    private void OnDestroy()
    {
        turnController.SpawningEvent -= StartSpawningEvent;
        turnController.PlayerTurnEvent -= HighlightTilesEvent;
    }

    private void HighlightTilesEvent(TurnManager.TurnState newState, int turnNumber)
    {
        if (turnNumber == turnToSpawn && index == turnController.BattleID)
        {
            if (targetHex != null)
            {
                if (newState == TurnManager.TurnState.start)
                {
                    hexHighlighter.HighLightArea(new List<Hex> { targetHex }, Color.red, Color.red, this);
                }
                else if (newState == TurnManager.TurnState.end)
                {
                    hexHighlighter.RemoveHighlights(this);
                }
            }
        }
        if (newState == TurnManager.TurnState.end && turnNumber == turnToSpawn && index == turnController.BattleID)
        {
            if (targetHex != null)
            {
                hexHighlighter.HighLightArea(new List<Hex> { targetHex }, Color.red, Color.red, this);
            }
        }
    }

    private void StartSpawningEvent(TurnManager.TurnState newState, int turnNumber)
    {
        if (doneSpawning)
        {
            Destroy(this);
            return;
        }

        if (newState == TurnManager.TurnState.spawning && turnNumber == turnToSpawn && index == turnController.BattleID)
        {
            StartCoroutine(SpawnAndMove());
        }
    }

    private IEnumerator SpawnAndMove()
    {
        turnController.AddSpawner();

        Unit newUnit = SpawnUnit();

        if (doorHex != null && targetHex != null)
        {
            newUnit.WalkDirectlyToTile(doorHex, targetHex);

            yield return new WaitUntil(() => newUnit.IsPerformingAction == false);

            float distanceToEarth = Vector3.Distance(currentHex.transform.position, Manager.instance.PlayerController.EarthUnit.CurrentTile.transform.position);
            float distanceToLightning = Vector3.Distance(currentHex.transform.position, Manager.instance.PlayerController.LightningUnit.CurrentTile.transform.position);
            Hex closestPlayerTile = distanceToEarth < distanceToLightning ? Manager.instance.PlayerController.EarthUnit.CurrentTile : Manager.instance.PlayerController.LightningUnit.CurrentTile;

            newUnit.WalkDirectlyToTile(targetHex, closestPlayerTile);

            yield return new WaitUntil(() => newUnit.IsPerformingAction == false);
        }

        turnController.RemoveSpawner();

        doneSpawning = true;
    }

    public Unit SpawnUnit()
    {
        if (currentHex.CurrentUnit == null)
        {
            Vector3 spawnPos = transform.parent.position;
            spawnPos.y = 0.0f;

            Unit unit = Instantiate(unitToSpawn, spawnPos, Quaternion.identity);
            unit.Spawn(currentHex);

            unit.CurrentVolt = (hasVolt) ? 1 : 0;

            Manager.instance.PlayerController.SubscribeToUnitDeath(unit);

            return unit;
        }
        return null;
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