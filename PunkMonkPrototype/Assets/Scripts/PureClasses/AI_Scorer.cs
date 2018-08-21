using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Standard;

public class AI_Scorer
{
    private CalculationCanvasType calculationCanvas;
    private AI_Agent agent;

    public AI_Scorer(CalculationCanvasType a_calculationCanvas, AI_Agent a_agent)
    {
        calculationCanvas = a_calculationCanvas;
        agent = a_agent;
    }

    public float GetScore(ScoringInfo a_scoringInfo)
    {

        // Set up all the input nodes to have the right values
        List<InputNode> workList = new List<InputNode>();
        for (int x = 0; x < calculationCanvas.nodes.Count; x++)
        {
            Node node = calculationCanvas.nodes[x];
            if (node.GetID == "inputNode")
                workList.Add((InputNode)node);
        }

        foreach (InputNode node in workList)
        {
            Unit player = a_scoringInfo.Player;
            switch (node.inputType)
            {
                case InputType.player:
                    switch (node.playerValue)
                    {
                        case PlayerValue.health:
                            node.Value = player.CurrentHealth; // Set to players health
                            break;
                        case PlayerValue.maxHealth:
                            node.Value = player.MaxHealth; // Set to players max health
                            break;
                        default:
                            break;
                    }
                    break;
                case InputType.agent:
                    switch (node.agentValue)
                    {
                        case AgentValue.damage:
                            node.Value = agent.Damage; // Set to agents damage
                            break;
                        case AgentValue.movement:
                            node.Value = agent.CanMove ? agent.MoveRange : 0; // Set to agents movement
                            break;
                        default:
                            break;
                    }
                    break;
                case InputType.world:
                    switch (node.worldValue)
                    {
                        case WorldValue.movesToAttackRange:
                            node.Value = a_scoringInfo.MovesToAttackRange; // Set to moves to attack range
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        calculationCanvas.Calculate();
        return calculationCanvas.Output;
    }

}
