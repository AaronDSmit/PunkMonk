using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool myTurn = true;

    private bool canInteract = true;

    private Unit selectedUnit;

    private Unit earthUnit;

    private Unit lightningUnit;

    private Tile tileUnderMouse;
    private Tile previousTileUnderMouse;

    private Unit unitUnderMouse;
    private Unit previousUnitUnderMouse;

    private List<Tile> tilesWithinRange;

    [SerializeField]
    private InteractionRuleset selectionRuleset;

    [Header("Debug Info")]
    [SerializeField]
    private InteractionRuleset currentRuleset;

    private void Awake()
    {
        tilesWithinRange = new List<Tile>();

        if (selectionRuleset == null)
        {
            Debug.LogError("Player Controller selection ruleset not set!!!");
        }
        else
        {
            currentRuleset = selectionRuleset;
        }

        StateManager.TurnEvent += TurnEvent;
    }

    public void Init()
    {
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

        SelectUnit(earthUnit);
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

            //RemoveHighlight();
        }

        // Left click
        if (Input.GetMouseButton(0))// && currentRuleset.IsValid)
        {
            if (unitUnderMouse)
            {
                SelectUnit(unitUnderMouse);
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
            selectedUnit.Select(false, currentRuleset.ValidHighlightColour);
        }

        selectedUnit = a_newSelectedUnit;

        selectedUnit.Select(true, currentRuleset.ValidHighlightColour);
    }

    private void DeselectUnit()
    {
        if (selectedUnit)
        {
            selectedUnit.Select(false, currentRuleset.ValidHighlightColour);
        }
    }

    private void UnitFinishedAction()
    {
        canInteract = true;
    }

    private void HighlightTilesInRange(int a_range)
    {

    }

    private void RemoveHighlightedTiles()
    {

    }

    private void TurnEvent(Turn_state a_newState, TEAM a_team, int a_turnNumber)
    {
        if (a_team == TEAM.player)
        {
            if (a_newState == Turn_state.start)
            {
                myTurn = true;

                // SelectUnit(myUnits[0]);

                //foreach (Unit unit in myUnits)
                //{
                //    unit.Refresh();
                //}
            }
            else if (a_newState == Turn_state.end)
            {
                myTurn = false;
                //DeselectUnit();
            }
        }
    }
}