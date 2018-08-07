using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Agent : Unit
{
    public float thing = 0;
    public Unit player;

    [SerializeField] private float damage;
    [SerializeField] private float movement;

    private AI_Controller ai_Controller = null;

    public float Damage { get { return damage; } }
    public float Movement { get { return movement; } }

    protected override void Awake()
    {
        base.Awake();

        // Setup the actions
        foreach (AI_Action action in actions)
        {
            action.Init(this);
        }

    }

    private void Update()
    {
        thing = ((AI_Action)actions[0]).GetScore(player);
    }

    public override void Spawn(Hex a_startingTile)
    {
        base.Spawn(a_startingTile);

        // Get the AI Controller and tell it that this agent was spawned
        ai_Controller = GameObject.FindGameObjectWithTag("AI_Controller").GetComponent<AI_Controller>();
        ai_Controller.AddUnit(this);

    }
}
