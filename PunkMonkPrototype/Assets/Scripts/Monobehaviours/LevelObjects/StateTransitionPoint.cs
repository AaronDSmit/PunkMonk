using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTransitionPoint : MonoBehaviour
{
    [SerializeField] private GameState targetState;
    [SerializeField] private GameState fromState;

    [HideInInspector]
    [SerializeField]
    public bool drawText;

    [HideInInspector]
    [SerializeField]
    public int index;

    [HideInInspector]
    [SerializeField]
    public int numberToKill;

    [HideInInspector]
    [SerializeField]
    private Hex earthHex;

    [HideInInspector]
    [SerializeField]
    private Hex lightningHex;

    [HideInInspector]
    [SerializeField]
    private Hex checkPoint;

    public bool triggered = false;

    public int voltGivenToEarth;
    public int voltGivenToLightning;

    public Hex LightningHex
    {
        get { return lightningHex; }
    }

    public Hex EarthHex
    {
        get { return earthHex; }
    }

    public Hex CheckPoint
    {
        get { return checkPoint; }
    }

    public void SetCheckPoint(Hex a_checkPointhHex)
    {
        checkPoint = a_checkPointhHex;
    }

    public void SetEarthHex(Hex a_earthHex)
    {
        earthHex = a_earthHex;
    }

    public void SetLightninghHex(Hex a_lightningHex)
    {
        lightningHex = a_lightningHex;
    }

    public GameState TargetState
    {
        get { return targetState; }

        set { targetState = value; }
    }

    public GameState CurrentState
    {
        get { return fromState; }

        set { fromState = value; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && (other.CompareTag("EarthUnit") || other.CompareTag("LightningUnit")))
        {
            if (Manager.instance.StateController.CurrentGameState == fromState)
            {
                PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

                player.EncounterKillLimit = numberToKill;
                player.SetUnitSnapHexes(earthHex, lightningHex);

                player.GiveEarthVolt(voltGivenToEarth);
                player.GiveLightningVolt(voltGivenToLightning);


                if (checkPoint != null)
                {
                    Manager.instance.CheckPointController.SetCheckPoint(this, checkPoint.transform.position);
                }

                Manager.instance.TurnController.BattleID = index;
                Manager.instance.StateController.ChangeGameStateAfterDelay(targetState, StateManager.stateTransitionTime + 0.1f);

                triggered = true;
            }
        }
    }
}