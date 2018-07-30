﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ruleset", menuName = "Interaction Ruleset/Ruleset", order = 0)]
public class InteractionRuleset : ScriptableObject
{
    public LayerMask interactableLayers;

    [SerializeField]
    private Color withinRangeHighlightColour;

    [SerializeField]
    private Color validHighlightColour;

    [SerializeField]
    private Color invalidHighlightColour;

    [SerializeField]
    public ActionType actionType;

    [SerializeField]
    private bool useDistanceCheck;

    [SerializeField]
    private bool useTeamCheck;

    [SerializeField] private bool useTileOccupationCheck;

    [SerializeField]
    private TileOccupation targetOccupation;

    [SerializeField]
    private DistanceCheck distanceCheckType;

    [SerializeField]
    private int minDistance;

    [SerializeField]
    private TargetTeam targetTeam;

    public bool IsValid { get; private set; }

    public bool WithinRange { get; private set; }

    public bool CorrectTileOccupation { get; private set; }

    public bool CorrectTeam { get; private set; }


    public Color HighlightColour
    {
        get { return (IsValid == true) ? validHighlightColour : invalidHighlightColour; }
    }

    public Color ValidHighlightColour
    {
        get { return validHighlightColour; }
    }

    public Color InRangeHighlightColour
    {
        get { return withinRangeHighlightColour; }
    }

    public void CheckValidity(Unit a_selectedObject, Tile a_tileUnderMouse)
    {
        if (useDistanceCheck)
        {
            WithinRange = WithinDistance(a_selectedObject, a_tileUnderMouse);
        }
        else
        {
            WithinRange = true;
        }

        if (useTileOccupationCheck)
        {
            CorrectTileOccupation = TileOccupationCheck(a_tileUnderMouse);
        }
        else
        {
            CorrectTileOccupation = true;
        }

        if (useTeamCheck)
        {
            CorrectTeam = TeamCheck(a_tileUnderMouse);
        }
        else
        {
            CorrectTeam = true;
        }

        IsValid = WithinRange && CorrectTileOccupation && CorrectTeam;
    }

    public void CheckValidity(Unit a_selectedObject, Entity a_entityUnderMouse)
    {
        if (useTeamCheck && useDistanceCheck)
        {
            IsValid = TeamCheck(a_entityUnderMouse) && WithinDistance(a_selectedObject, a_entityUnderMouse.CurrentTile);
        }
        else if (useTeamCheck)
        {
            IsValid = TeamCheck(a_entityUnderMouse);
        }
        else if (useDistanceCheck)
        {
            IsValid = WithinDistance(a_selectedObject, a_entityUnderMouse.CurrentTile);
        }
        else
        {
            IsValid = true;
        }
    }

    private bool WithinDistance(Entity a_selectedObject, Tile a_tileLocation)
    {
        int distance = 0;

        switch (distanceCheckType)
        {
            case DistanceCheck.attackRange:
                distance = a_selectedObject.GetComponent<Unit>().AttackRange;
                return Tile.Distance(a_selectedObject.CurrentTile, a_tileLocation) <= distance;

            case DistanceCheck.specialAttackRange:
                distance = a_selectedObject.GetComponent<Unit>().SpecialAttackRange;
                return Tile.Distance(a_selectedObject.CurrentTile, a_tileLocation) <= distance;

            case DistanceCheck.movementRange:
                distance = a_selectedObject.GetComponent<Unit>().MoveRange;
                return Tile.Distance(a_selectedObject.CurrentTile, a_tileLocation) <= distance;

            case DistanceCheck.custom:
                return Tile.Distance(a_selectedObject.CurrentTile, a_tileLocation) <= minDistance;
        }

        return false;
    }

    private bool TeamCheck(Entity a_GO)
    {
        if (targetTeam == TargetTeam.friendly)
        {
            return (a_GO.Team == TEAM.player || a_GO.Team == TEAM.neutral);
        }
        else
        {
            return (a_GO.Team == TEAM.ai || a_GO.Team == TEAM.neutral);
        }
    }

    private bool TileOccupationCheck(Tile a_tile)
    {
        if(targetOccupation == TileOccupation.clear)
        {
            return a_tile.IsWalkable;
        }
        else
        {
            return !a_tile.IsWalkable;
        }
    }
}

public enum DistanceCheck
{
    attackRange,
    specialAttackRange,
    movementRange,
    custom
};

public enum TargetTeam
{
    friendly,
    enemy
};

public enum TileOccupation
{
    clear,
    blocked
}