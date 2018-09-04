using System.Collections;
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

    [SerializeField]
    private DistanceType distanceType;



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

    public Color InvalidHighlightColour
    {
        get { return invalidHighlightColour; }
    }

    public Color InRangeHighlightColour
    {
        get { return withinRangeHighlightColour; }
    }


    public void CheckValidity(Hex a_hex, Hex a_tileUnderMouse)
    {
        if (useDistanceCheck)
        {
            if (distanceType == DistanceType.absolute)
            {
                WithinRange = WithinDistanceAboslute(a_hex, a_tileUnderMouse);
            }
            else if (distanceType == DistanceType.pathTraveled)
            {
                WithinRange = WithinDistancePath(a_hex, a_tileUnderMouse);
            }
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

    public void CheckValidity(Hex a_hex, Entity a_entityUnderMouse)
    {
        if (useTeamCheck && useDistanceCheck)
        {
            if (distanceType == DistanceType.absolute)
            {
                IsValid = TeamCheck(a_entityUnderMouse) && WithinDistanceAboslute(a_hex, a_entityUnderMouse.CurrentTile);
            }
            else if (distanceType == DistanceType.pathTraveled)
            {
                IsValid = TeamCheck(a_entityUnderMouse) && WithinDistancePath(a_hex, a_entityUnderMouse.CurrentTile);
            }
        }
        else if (useTeamCheck)
        {
            IsValid = TeamCheck(a_entityUnderMouse);
        }
        else if (useDistanceCheck)
        {
            if (distanceType == DistanceType.absolute)
            {
                IsValid = WithinDistanceAboslute(a_hex, a_entityUnderMouse.CurrentTile);
            }
            else if (distanceType == DistanceType.pathTraveled)
            {
                IsValid = WithinDistancePath(a_hex, a_entityUnderMouse.CurrentTile);
            }
        }
        else
        {
            IsValid = true;
        }
    }

    private bool WithinDistanceAboslute(Hex a_hex, Hex a_tileLocation)
    {
        int distance = 0;

        switch (distanceCheckType)
        {
            case DistanceCheck.attackRange:
                distance = a_hex.CurrentUnit.GetComponent<Unit>().AttackRange;
                return HexUtility.Distance(a_hex, a_tileLocation) <= distance;

            case DistanceCheck.specialAttackRange:
                distance = a_hex.CurrentUnit.GetComponent<Unit>().SpecialAttackRange;
                return HexUtility.Distance(a_hex, a_tileLocation) <= distance;

            case DistanceCheck.movementRange:
                distance = a_hex.CurrentUnit.GetComponent<Unit>().MoveRange;
                return HexUtility.Distance(a_hex, a_tileLocation) <= distance;

            case DistanceCheck.custom:
                return HexUtility.Distance(a_hex, a_tileLocation) <= minDistance;
        }

        return false;
    }

    private bool WithinDistancePath(Hex a_hex, Hex a_tileLocation)
    {
        int distance = 0;

        switch (distanceCheckType)
        {
            case DistanceCheck.attackRange:
                distance = a_hex.CurrentUnit.AttackRange;
                return Navigation.PathLength(a_hex, a_tileLocation) <= distance;

            case DistanceCheck.specialAttackRange:
                distance = a_hex.CurrentUnit.SpecialAttackRange;
                return Navigation.PathLength(a_hex, a_tileLocation) <= distance;

            case DistanceCheck.movementRange:
                distance = a_hex.CurrentUnit.MoveRange;
                return Navigation.PathLength(a_hex, a_tileLocation) <= distance;

            case DistanceCheck.custom:
                return Navigation.PathLength(a_hex, a_tileLocation) <= minDistance;
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

    private bool TileOccupationCheck(Hex a_tile)
    {
        if (targetOccupation == TileOccupation.clear)
        {
            return a_tile.IsTraversable;
        }
        else
        {
            return !a_tile.IsTraversable;
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

public enum DistanceType
{
    absolute,
    pathTraveled
}