using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTransitionPoint : MonoBehaviour
{
    [SerializeField] private GameState targetState;
    [SerializeField] private GameState fromState;

    [SerializeField] private Conversation conversation;

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
    private HexDirection earthDirection;

    [HideInInspector]
    [SerializeField]
    private Hex lightningHex;

    [HideInInspector]
    [SerializeField]
    private HexDirection lightningDirection;

    [HideInInspector]
    [SerializeField]
    private Hex checkPoint;

    public bool triggered = false;

    public int voltGiven;

    public AK.Wwise.Event MyEvent = null;




    public Hex LightningHex
    {
        get { return lightningHex; }
        set { lightningHex = value; }
    }

    public Hex EarthHex
    {
        get { return earthHex; }
        set { earthHex = value; }
    }

    public HexDirection EarthDirection
    {
        get { return earthDirection; }
        set { earthDirection = value; }
    }

    public HexDirection LightningDirection
    {
        get { return lightningDirection; }
        set { lightningDirection = value; }
    }


    public Hex CheckPoint
    {
        get { return checkPoint; }
    }

    public void SetCheckPoint(Hex a_checkPointhHex)
    {
        checkPoint = a_checkPointhHex;
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

    public Conversation Conversation
    {
        get { return conversation; }

        set { conversation = value; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && (other.CompareTag("EarthUnit") || other.CompareTag("LightningUnit")))
        {
            if (Manager.instance.StateController.CurrentGameState == fromState)
            {
                PlayerController player = Manager.instance.PlayerController;

                player.EncounterKillLimit = numberToKill;
                player.SetUnitStartingHexes(earthHex, earthDirection, lightningHex, lightningDirection);

                player.CurrentVolt += voltGiven;

                if (conversation != null)
                {
                    Manager.instance.DialogueManager.CurrentConversation = conversation;
                }

                if (checkPoint != null)
                {
                    Manager.instance.CheckPointController.SetCheckPoint(this, checkPoint.transform.position);
                }

                Manager.instance.TurnController.BattleID = index;
                Manager.instance.StateController.ChangeGameStateAfterDelay(targetState, StateManager.stateTransitionTime + 0.1f);

                triggered = true;
                MyEvent.Post(gameObject);

            }
        }
    }
}