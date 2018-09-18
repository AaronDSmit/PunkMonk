using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework.Standard;

[CreateAssetMenu(fileName = "AI_Action", menuName = "AI/new AI action", order = 0)]
public class AI_Action : Action
{
    [SerializeField] private CalculationCanvasType calculationCanvas = null;

    private AI_Scorer scorer = null;

    private AI_Agent agent = null;

    public void Init(AI_Agent a_Agent)
    {
        agent = a_Agent;

        scorer = new AI_Scorer(calculationCanvas, agent);

    }

    public float GetScore(ScoringInfo a_scoringInfo)
    {
        return scorer.GetScore(a_scoringInfo);
    }
}
