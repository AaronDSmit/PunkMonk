using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    SELECTION,
    MOVEMENT,
    ATTACK,
    SPELL
};

[CreateAssetMenu(fileName = "Action", menuName = "Actions/new action", order = 0)]
public class Action : ScriptableObject
{
    // public InteractionRuleset ruleset;

    ActionType actionType;

    public int coolDown;

    // public Spell prefab;

    public string AnimTrigger;
}