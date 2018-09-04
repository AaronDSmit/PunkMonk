using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    selection,
    movement,
    attack,
    specialAttack
};

[CreateAssetMenu(fileName = "Action", menuName = "Actions/new action", order = 0)]
public class Action : ScriptableObject
{
    public new string name;

    public string flavourText;

    public InteractionRuleset ruleset;

    public InteractionRuleset[] otherRuleset;

    ActionType actionType;

    public int coolDown;

    // public Spell atttack;

    public string AnimTrigger;
}