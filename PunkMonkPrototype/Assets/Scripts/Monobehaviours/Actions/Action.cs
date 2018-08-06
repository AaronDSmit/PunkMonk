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
    public InteractionRuleset ruleset;

    ActionType actionType;

    public int coolDown;

   // public Spell atttack;

    public string AnimTrigger;
}