using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework.Standard;

[CreateAssetMenu(fileName = "AI_Action", menuName = "AI/new AI action", order = 0)]
public class AI_Action : Action
{
    [SerializeField] private CalculationCanvasType calculationCanvas;

    private AI_Scorer scorer = null;

    private AI_Agent agent = null;

    public bool Init(AI_Agent a_Agent)
    {
        scorer = new AI_Scorer(calculationCanvas, agent, new WorldState());

        if (agent == null)
        {
            agent = a_Agent;

            return true;
        }
        return false;
    }

    public float GetScore(Unit a_player)
    {
        return scorer.GetScore(a_player);
    }
}
