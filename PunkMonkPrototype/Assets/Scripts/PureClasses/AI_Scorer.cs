using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Standard;

public class AI_Scorer
{
    private CalculationCanvasType calculationCanvas;
    private AI_Agent agent;
    private WorldState worldState;

    public AI_Scorer(CalculationCanvasType a_calculationCanvas, AI_Agent a_agent, WorldState a_worldState)
    {
        calculationCanvas = a_calculationCanvas;
        agent = a_agent;
        worldState = a_worldState;
    }

    public float GetScore(Unit a_player)
    {
        // Set up all the input nodes to have the right values
        List<InputNode> workList = new List<InputNode>();
        for (int i = 0; i < calculationCanvas.nodes.Count; i++)
        {
            Node node = calculationCanvas.nodes[i];
            if (node.GetID == "inputNode")
                workList.Add((InputNode)node);
        }

        foreach (InputNode node in workList)
        {
            switch (node.inputType)
            {
                case InputType.player:
                    switch (node.playerValue)
                    {
                        case PlayerValue.health:
                            node.Value = a_player.CurrentHealth; // Set to players health
                            break;
                        case PlayerValue.maxHealth:
                            node.Value = a_player.MaxHealth; // Set to players max health
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
                            node.Value = 1;//agent.CanMove; // Set to agents movement
                            break;
                        default:
                            break;
                    }
                    break;
                case InputType.world:
                    switch (node.worldValue)
                    {
                        case WorldValue.distToPlayer:
                            node.Value = worldState.DistanceToPlayer; // Set to distance to the player
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        // Calculate and get the result to return
        calculationCanvas.Calculate();
        return calculationCanvas.Output;
    }

}
