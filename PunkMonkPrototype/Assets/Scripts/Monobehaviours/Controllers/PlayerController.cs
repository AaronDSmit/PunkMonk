using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAttack { earthBasic, earthSpecial, lightningBasic, lightningSpecial }

public class PlayerController : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField] private InteractionRuleset selectionRuleset;

    #endregion

    [Header("Debug Info")]
    [SerializeField]
    private InteractionRuleset currentRuleset;

    private bool myTurn = true;

    private bool canInteract = false;

    private PlayerAttack currentAttack;

    private LineRenderer lineRenderer;

    private Unit selectedUnit;

    private Unit earthUnit;

    private Unit lightningUnit;

    private Tile tileUnderMouse;
    private Tile previousTileUnderMouse;

    private Unit unitUnderMouse;
    private Unit previousUnitUnderMouse;

    private List<Tile> tilesWithinRange;

    private List<Tile> tilesAffectByAction;

    private UIManager UI;

    private GridManager grid;


    private void Awake()
    {
        TurnManager.TurnEvent += TurnEvent;

        StateManager.OnGameStateChanged += GameStateChanged;

        UI = GameObject.FindGameObjectWithTag("Manager").GetComponent<UIManager>();

        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridManager>();

        tilesWithinRange = new List<Tile>();

        tilesAffectByAction = new List<Tile>();

        if (selectionRuleset == null)
        {
            Debug.LogError("Player Controller selection ruleset not set!!!");
        }
        else
        {
            currentRuleset = selectionRuleset;
        }

        lineRenderer = GetComponent<LineRenderer>();
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
            if (tileUnderMouse)
            {
                if (previousTileUnderMouse && previousTileUnderMouse != tileUnderMouse)
                {
                    previousTileUnderMouse.MouseExit();
                }

                previousTileUnderMouse = tileUnderMouse;
            }

            if (unitUnderMouse)
            {
                if (previousUnitUnderMouse && previousUnitUnderMouse != unitUnderMouse)
                {
                    previousUnitUnderMouse.Highlight(false, Color.green);
                }

                previousUnitUnderMouse = unitUnderMouse;
            }

            tileUnderMouse = hitInfo.transform.GetComponent<Tile>();
            unitUnderMouse = hitInfo.transform.GetComponent<Unit>();

            if (tileUnderMouse)
            {
                currentRuleset.CheckValidity(selectedUnit, tileUnderMouse);

                if (currentRuleset.WithinRange && currentRuleset.actionType == ActionType.attack || currentRuleset.actionType == ActionType.specialAttack)
                {
                    ProcessActionHighlighting(tileUnderMouse, hitInfo);
                }
                else if (currentRuleset.IsValid && currentRuleset.actionType == ActionType.movement)
                {
                    tileUnderMouse.MouseEnter(currentRuleset.HighlightColour);

                    List<Tile> path = Navigation.FindPath(selectedUnit.CurrentTile, tileUnderMouse);
                    lineRenderer.positionCount = path.Count + 1;

                    lineRenderer.SetPosition(0, selectedUnit.CurrentTile.transform.position + Vector3.up * 0.5f);

                    for (int i = 0; i < path.Count; i++)
                    {
                        lineRenderer.SetPosition(i + 1, path[i].transform.position + Vector3.up * 0.5f);
                    }
                }
                else
                {
                    lineRenderer.positionCount = 0;

                    if (tilesAffectByAction.Count > 0)
                    {
                        foreach (Tile tile in tilesAffectByAction)
                        {
                            tile.MouseExit();
                        }

                        tilesAffectByAction.Clear();
                    }
                }

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

            if (previousTileUnderMouse)
            {
                previousTileUnderMouse.MouseExit();
                previousTileUnderMouse = null;
            }

            // RemoveHighlight();
        }

        // Left click
        if (Input.GetMouseButton(0) && currentRuleset.IsValid)
        {
            switch (currentRuleset.actionType)
            {
                case ActionType.selection:

                    if (unitUnderMouse)
                    {
                        SelectUnit(unitUnderMouse);
                    }

                    break;

                case ActionType.movement:

                    if (tileUnderMouse.IsWalkable) // redundant check, needs to be valid to reach this point anyway
                    {
                        selectedUnit.MoveTo(tileUnderMouse, UnitFinishedAction);
                        RemoveHighlightedTiles();
                        currentRuleset = selectionRuleset;

                        tileUnderMouse.MouseExit();
                        lineRenderer.positionCount = 0;
                        canInteract = false;
                        //UI.ToggleHUDLock();
                    }

                    break;

                case ActionType.attack:

                    selectedUnit.BasicAttack(tilesAffectByAction.ToArray(), AttackStart, AttackEnd);
                    RemoveHighlightedTiles();
                    currentRuleset = selectionRuleset;

                    canInteract = false;

                    break;

                case ActionType.specialAttack:

                    selectedUnit.SpecialAttack(tilesAffectByAction.ToArray(), SpecialAttackStart, SpecialAttackEnd);
                    RemoveHighlightedTiles();
                    currentRuleset = selectionRuleset;

                    canInteract = false;

                    break;
            }
        }

        // Right click cancels current action
        if (Input.GetMouseButton(1))
        {
            lineRenderer.positionCount = 0;
            currentRuleset = selectionRuleset;
            RemoveHighlightedTiles();

            if (tileUnderMouse)
            {
                tileUnderMouse.MouseExit();
            }
        }
    }

    private void AttackStart()
    {

    }

    private void AttackEnd()
    {
        canInteract = true;
    }

    private void SpecialAttackStart()
    {

    }

    private void SpecialAttackEnd()
    {
        canInteract = true;
    }

    private void SelectUnit(Unit a_newSelectedUnit)
    {
        if (selectedUnit)
        {
            if (selectedUnit == a_newSelectedUnit)
            {
                return;
            }

            selectedUnit.Select(false, currentRuleset.HighlightColour);
        }

        selectedUnit = a_newSelectedUnit;

        UI.UpdateSelectedUnit(selectedUnit);

        selectedUnit.Select(true, currentRuleset.HighlightColour);
    }

    private void DeselectUnit()
    {
        if (selectedUnit)
        {
            selectedUnit.Select(false, currentRuleset.HighlightColour);
            selectedUnit = null;
        }
    }

    private void UnitFinishedAction()
    {
        canInteract = true;
    }

    private void HighlightTilesInRange(int a_range)
    {
        if (currentRuleset.actionType == ActionType.movement)
        {
            Tile[] area = grid.GetTilesWithinDistance(selectedUnit.CurrentTile, a_range, false);

            foreach (Tile tile in area)
            {
                tilesWithinRange.Add(tile);

                if (tile.IsWalkable)
                {
                    tile.HighlightMovement(currentRuleset.InRangeHighlightColour);
                }
                else
                {
                    tile.HighlightMovement(currentRuleset.InvalidHighlightColour);
                }
            }
        }
        else
        {
            Tile[] area = grid.GetTilesWithinDistance(selectedUnit.CurrentTile, a_range, true);

            foreach (Tile tile in area)
            {
                tilesWithinRange.Add(tile);
                tile.HighlightMovement(currentRuleset.InRangeHighlightColour);
            }
        }
    }

    private void RemoveHighlightedTiles()
    {
        foreach (Tile tile in tilesWithinRange)
        {
            tile.RemoveHighlight();
        }

        tilesWithinRange.Clear();

        if (tilesAffectByAction.Count > 0)
        {
            foreach (Tile tile in tilesAffectByAction)
            {
                tile.MouseExit();
            }

            tilesAffectByAction.Clear();
        }
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
            Ray earthRay = new Ray(earthUnit.transform.position + Vector3.up * 2, Vector3.down);
            Ray lightningRay = new Ray(lightningUnit.transform.position + Vector3.up * 2, Vector3.down);

            RaycastHit hitinfo;

            int layerMask = 0;
            layerMask |= (1 << LayerMask.NameToLayer("Ground"));

            if (Physics.Raycast(earthRay, out hitinfo, Mathf.Infinity, layerMask))
            {
                earthUnit.SnapToGrid(hitinfo.transform.GetComponent<Tile>());
            }

            if (Physics.Raycast(lightningRay, out hitinfo, Mathf.Infinity, layerMask))
            {
                lightningUnit.SnapToGrid(hitinfo.transform.GetComponent<Tile>());
            }

            canInteract = true;
            SelectUnit(earthUnit);
        }
    }

    public void SelectAction(int actionIndex)
    {
        RemoveHighlightedTiles();

        currentRuleset = selectedUnit.GetAction(actionIndex).ruleset;

        // set the current attack
        if (selectedUnit == earthUnit)
        {
            if (actionIndex == 1)
            {
                currentAttack = PlayerAttack.earthBasic;
            }
            else if (actionIndex == 2)
            {
                currentAttack = PlayerAttack.earthSpecial;
            }
        }
        else if (selectedUnit == lightningUnit)
        {
            if (actionIndex == 1)
            {
                currentAttack = PlayerAttack.lightningBasic;
            }
            else if (actionIndex == 2)
            {
                currentAttack = PlayerAttack.lightningSpecial;
            }
        }

        // Highlight area in range to walk
        if (currentRuleset.actionType == ActionType.movement)
        {
            HighlightTilesInRange(selectedUnit.MoveRange);
        }

        // Highlight area in range to attack
        if (currentRuleset.actionType == ActionType.attack)
        {
            HighlightTilesInRange(selectedUnit.AttackRange);
        }

        // Highlight area in range to special attack
        if (currentRuleset.actionType == ActionType.specialAttack)
        {
            HighlightTilesInRange(selectedUnit.SpecialAttackRange);
        }
    }

    private void ProcessActionHighlighting(Tile a_targetTile, RaycastHit a_hitInfo)
    {
        switch (currentAttack)
        {
            case PlayerAttack.earthBasic:

                GetTilesAffectByEarthAttack(a_targetTile, a_hitInfo);
                break;
            case PlayerAttack.earthSpecial:

                GetTilesAffectByEarthSpecialAttack(a_targetTile);
                break;
            case PlayerAttack.lightningBasic:

                break;
            case PlayerAttack.lightningSpecial:

                break;
        }
    }

    [SerializeField] private float angle;
    [SerializeField] private int snapAngle;

    // returns a tiles within a tilesAffectByAction
    private void GetTilesAffectByEarthAttack(Tile a_targetTile, RaycastHit a_hitInfo)
    {
        foreach (Tile tile in tilesAffectByAction)
        {
            tile.MouseExit();
        }

        tilesAffectByAction.Clear();

        //Debug.DrawLine(selectedUnit.CurrentTile.transform.position + Vector3.up * 0.2f, a_hitInfo.point + Vector3.up * 0.2f, Color.yellow);

        // angle = Vector3.SignedAngle(Vector3.forward, a_hitInfo.point - selectedUnit.CurrentTile.transform.position, Vector3.up);

        angle = Vector3.SignedAngle(Vector3.forward, a_targetTile.transform.position - selectedUnit.CurrentTile.transform.position, Vector3.up);

        snapAngle = ((int)Mathf.Round(angle / 30.0f)) * 30;

        if (snapAngle < 0)
        {
            snapAngle = 360 + snapAngle;
        }

        MakeCone(snapAngle, 6);

        //Debug.DrawLine(selectedUnit.CurrentTile.transform.position + Vector3.up * 0.2f, selectedUnit.CurrentTile.transform.position + new Vector3(Mathf.Sin(Mathf.Deg2Rad * snapAngle), 0.0f, Mathf.Cos(Mathf.Deg2Rad * snapAngle)) * 2 + Vector3.up * 0.2f, Color.magenta);

        foreach (Tile tile in tilesAffectByAction)
        {
            if (!tile.IsWalkable)
            {
                tile.MouseEnter(currentRuleset.HighlightColour);
            }
            else
            {
                tile.MouseEnter(currentRuleset.HighlightColour);
            }
        }
    }

    // returns a tiles within range of landing tiles
    private void GetTilesAffectByEarthSpecialAttack(Tile a_targetTile)
    {
        foreach (Tile tile in tilesAffectByAction)
        {
            tile.MouseExit();
        }

        tilesAffectByAction.Clear();

        tilesAffectByAction.Add(a_targetTile);

        Tile[] tilesInRange = grid.GetTilesWithinDistance(a_targetTile, 1, true);

        for (int i = 0; i < tilesInRange.Length; i++)
        {
            tilesAffectByAction.Add(tilesInRange[i]);
        }

        foreach (Tile tile in tilesAffectByAction)
        {
            if (!tile.IsWalkable)
            {
                tile.MouseEnter(currentRuleset.HighlightColour);
            }
            else
            {
                tile.MouseEnter(currentRuleset.HighlightColour);
            }
        }
    }

    int[] snapAngles = { 30, 90, 150, 210, 270, 330 };
    int[] leftDirIndex = { 4, 5, 0, 1, 2, 3 };
    int[] rightDirIndex = { 2, 3, 4, 5, 0, 1 };

    private void MakeCone(int angle, int range)
    {
        if (range <= 0)
            return;

        HexDirection mainDir;
        HexDirection leftDir;
        HexDirection rightDir;

        for (int a = 0; a < snapAngles.Length; a++)
        {
            if (angle == snapAngles[a])
            {
                mainDir = (HexDirection)a;

                // the tile that is under the cursor
                Tile currentTile = selectedUnit.CurrentTile.GetNeighbour(mainDir);

                if (currentTile)
                {
                    tilesAffectByAction.Add(currentTile);
                }

                int sideRange = 1;

                // main Spine of cone
                for (int i = 0; i < range - 1; i++)
                {
                    // add the left 'border line'
                    if (currentTile)
                    {
                        currentTile = currentTile.GetNeighbour(mainDir);

                        if (currentTile)
                        {
                            tilesAffectByAction.Add(currentTile);
                        }
                    }

                    Tile leftTile = currentTile;
                    Tile rightTile = currentTile;

                    for (int s = 0; s < sideRange; s++)
                    {
                        if (leftTile)
                        {
                            leftDir = (HexDirection)leftDirIndex[a];
                            leftTile = leftTile.GetNeighbour(leftDir);

                            if (leftTile)
                            {
                                tilesAffectByAction.Add(leftTile);
                            }
                        }

                        if (rightTile)
                        {
                            rightDir = (HexDirection)rightDirIndex[a];
                            rightTile = rightTile.GetNeighbour(rightDir);

                            if (rightTile)
                            {
                                tilesAffectByAction.Add(rightTile);
                            }
                        }
                    }

                    if (i % 2 == 1)
                    {
                        sideRange++;
                    }
                }

                break;
            }
        }
    }
}