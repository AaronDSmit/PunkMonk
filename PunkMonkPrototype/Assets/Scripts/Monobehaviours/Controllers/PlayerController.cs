using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField] private InteractionRuleset selectionRuleset;

    #endregion

    private bool myTurn = true;

    private bool canInteract = false;

    private Unit selectedUnit;

    private Unit earthUnit;

    private Unit lightningUnit;

    private Tile tileUnderMouse;
    private Tile previousTileUnderMouse;

    private Unit unitUnderMouse;
    private Unit previousUnitUnderMouse;

    private List<Tile> tilesWithinRange;

    private UIManager UI;

    private GridManager grid;

    [Header("Debug Info")]
    [SerializeField] private InteractionRuleset currentRuleset;

    private void Awake()
    {
        TurnManager.TurnEvent += TurnEvent;

        StateManager.OnGameStateChanged += GameStateChanged;

        UI = GameObject.FindGameObjectWithTag("Manager").GetComponent<UIManager>();

        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridManager>();

        tilesWithinRange = new List<Tile>();

        if (selectionRuleset == null)
        {
            Debug.LogError("Player Controller selection ruleset not set!!!");
        }
        else
        {
            currentRuleset = selectionRuleset;
        }
    }

    public void Init()
    {
        canInteract = false;

        GameObject earthGO = GameObject.FindGameObjectWithTag("EarthUnit");

        if (earthGO)
        {
            earthUnit = GameObject.FindGameObjectWithTag("EarthUnit").GetComponent<Unit>();
        }
        else
        {
            Debug.LogError("No Earth unit found!");
        }

        GameObject lightningGO = GameObject.FindGameObjectWithTag("LightningUnit");

        if (lightningGO)
        {
            lightningUnit = GameObject.FindGameObjectWithTag("LightningUnit").GetComponent<Unit>();
        }
        else
        {
            Debug.LogError("No lightning unit found!");
        }
    }

    private void Update()
    {
        if (myTurn && canInteract)
        {
            ProcessKeyboardInput();
            ProcessMouseInput();
        }
    }

    // Process Keyboard Input
    private void ProcessKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (selectedUnit == earthUnit)
            {
                SelectUnit(lightningUnit);
            }
            else
            {
                SelectUnit(earthUnit);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectUnit(earthUnit);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectUnit(lightningUnit);
        }
    }

    // Process Mouse Input
    private void ProcessMouseInput()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(mouseRay, out hitInfo, Mathf.Infinity, currentRuleset.interactableLayers))
        {
            tileUnderMouse = hitInfo.transform.GetComponent<Tile>();
            unitUnderMouse = hitInfo.transform.GetComponent<Unit>();

            if (tileUnderMouse)
            {
                currentRuleset.CheckValidity(selectedUnit, tileUnderMouse);

                //HighlightObject(objectUnderMouse);
            }
            else if (unitUnderMouse)
            {
                currentRuleset.CheckValidity(selectedUnit, unitUnderMouse);

                if (previousUnitUnderMouse != null && previousTileUnderMouse != unitUnderMouse)
                {
                    previousUnitUnderMouse.Highlight(false, currentRuleset.HighlightColour);

                    previousUnitUnderMouse = unitUnderMouse;
                }

                unitUnderMouse.Highlight(true, currentRuleset.HighlightColour);
            }
        }
        else
        {
            if (unitUnderMouse)
            {
                unitUnderMouse.Highlight(false, currentRuleset.HighlightColour);
                unitUnderMouse = null;
            }

            // RemoveHighlight();
        }

        // Left click
        if (Input.GetMouseButton(0) && currentRuleset.IsValid)
        {
            switch (currentRuleset.actionType)
            {
                case ActionType.SELECTION:

                    if (unitUnderMouse)
                    {
                        SelectUnit(unitUnderMouse);
                    }

                    break;
                case ActionType.MOVEMENT:

                    if (tileUnderMouse.IsWalkable)
                    {
                        selectedUnit.MoveTo(tileUnderMouse, UnitFinishedAction);
                        RemoveHighlightedTiles();
                        currentRuleset = selectionRuleset;

                        canInteract = false;
                        //UI.ToggleHUDLock();
                    }

                    break;
                case ActionType.ATTACK:
                    break;
                case ActionType.SPELL:
                    break;
                default:
                    break;
            }
        }

        // Right click cancels current action
        if (Input.GetMouseButton(1))
        {
            currentRuleset = selectionRuleset;
            RemoveHighlightedTiles();
        }
    }

    private void SelectUnit(Unit a_newSelectedUnit)
    {
        if (selectedUnit)
        {
            if (selectedUnit == a_newSelectedUnit)
            {
                return;
            }

            selectedUnit.Select(false, currentRuleset.ValidHighlightColour);
        }

        selectedUnit = a_newSelectedUnit;

        UI.UpdateSelectedUnit(selectedUnit);

        selectedUnit.Select(true, currentRuleset.ValidHighlightColour);
    }

    private void DeselectUnit()
    {
        if (selectedUnit)
        {
            selectedUnit.Select(false, currentRuleset.ValidHighlightColour);
            selectedUnit = null;
        }
    }

    private void UnitFinishedAction()
    {
        canInteract = true;
    }

    private void HighlightTilesInRange(int a_range)
    {
        Tile[] area = grid.GetTilesWithinDistance(selectedUnit.CurrentTile, a_range);

        foreach (Tile tile in area)
        {
            tilesWithinRange.Add(tile);
            tile.HighlightMovement(true, currentRuleset.ValidHighlightColour);
        }
    }

    private void RemoveHighlightedTiles()
    {
        foreach (Tile tile in tilesWithinRange)
        {
            tile.RemoveHighlight();
        }

        tilesWithinRange.Clear();
    }

    private void TurnEvent(Turn_state a_newState, TEAM a_team, int a_turnNumber)
    {
        if (a_team == TEAM.player)
        {
            if (a_newState == Turn_state.start)
            {
                myTurn = true;

                SelectUnit(earthUnit);

                earthUnit.Refresh();
                lightningUnit.Refresh();
            }
            else if (a_newState == Turn_state.end)
            {
                myTurn = false;
                DeselectUnit();
            }
        }
    }

    private void GameStateChanged(Game_state _oldstate, Game_state _newstate)
    {
        // ensure this script knows it's in over-world state
        if (_newstate == Game_state.battle)
        {
            canInteract = true;
            SelectUnit(earthUnit);
        }
    }

    public void SelectAction(int actionIndex)
    {
        RemoveHighlightedTiles();

        currentRuleset = selectedUnit.GetAction(actionIndex).ruleset;

        // Highlight area in range to walk
        if (currentRuleset.actionType == ActionType.MOVEMENT)
        {
            HighlightTilesInRange(selectedUnit.GetComponent<Unit>().MoveRange);
        }

        // Highlight area in range to attack
        if (currentRuleset.actionType == ActionType.ATTACK || currentRuleset.actionType == ActionType.SPELL)
        {
            HighlightTilesInRange(selectedUnit.GetComponent<Unit>().AttackRange);
        }
    }
}