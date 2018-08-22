using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAttack { earthBasic, earthSpecial, lightningBasic, lightningSpecial }

/// <summary>
/// 
/// </summary>
public class PlayerController : MonoBehaviour
{
    #region Unity Inspector Fields

    [SerializeField]
    private InteractionRuleset selectionRuleset;

    [Header("Debug Info")]
    [SerializeField]
    private InteractionRuleset currentRuleset;

    #endregion

    #region Reference Fields

    private LineRenderer lineRenderer;

    private Unit selectedUnit;

    private EarthUnit earthUnit;

    private LightningUnit lightningUnit;

    private Hex tileUnderMouse;
    private Hex previousTileUnderMouse;

    private Unit unitUnderMouse;
    private Unit previousUnitUnderMouse;

    private CameraController cam;

    #endregion

    #region Local Fields

    private bool myTurn = true;

    private bool canInteract = false;

    private PlayerAttack currentAttack;

    private List<Hex> tilesWithinRange;

    private List<Hex> tilesAffectByAction;

    private UIManager UI;

    private GridManager grid;

    private Hex earthSnapHex;

    private Hex lightningSnapHex;

    private int encounterKillLimit = 0;

    private int encounterKillCount = 0;

    private bool trackingKills = false;

    private bool LightningDead = false;

    private bool earthDead = false;

    #endregion

    #region Properties

    public int EncounterKillLimit
    {
        get { return encounterKillLimit; }

        set { encounterKillLimit = value; }
    }

    #endregion

    #region Public Methods

    public void Init()
    {
        canInteract = false;

        Manager.instance.TurnController.PlayerTurnEvent += TurnEvent;

        Manager.instance.StateController.OnGameStateChanged += GameStateChanged;

        GameObject earthGO = GameObject.FindGameObjectWithTag("EarthUnit");

        if (earthGO)
        {
            earthUnit = GameObject.FindGameObjectWithTag("EarthUnit").GetComponent<EarthUnit>();
        }
        else
        {
            Debug.LogError("No Earth unit found!");
        }

        GameObject lightningGO = GameObject.FindGameObjectWithTag("LightningUnit");

        if (lightningGO)
        {
            lightningUnit = GameObject.FindGameObjectWithTag("LightningUnit").GetComponent<LightningUnit>();
        }
        else
        {
            Debug.LogError("No lightning unit found!");
        }
    }

    public Unit SpawnEarthUnit(Hex spawnHex = null)
    {
        // use spawn points by default
        if (spawnHex == null)
        {
            GameObject earthGO = GameObject.FindGameObjectWithTag("EarthUnitSpawn");

            if (earthGO)
            {
                Hex spawnHexEarth = earthGO.transform.parent.GetComponent<Hex>();

                Vector3 spawnPosEarth = spawnHexEarth.transform.position;
                spawnPosEarth.y = 0.1f;

                earthUnit = Instantiate(Resources.Load<EarthUnit>("PlayerCharacters/EarthUnit"), spawnPosEarth, Quaternion.identity);
                earthUnit.Spawn(spawnHexEarth);

                earthUnit.OnDeath += PlayerUnitDied;

                return earthUnit;
            }
            else
            {
                Debug.LogError("No Earth  spawn point found!");

                return null;
            }
        }
        else
        {
            // spawn at checkpoint pos

            Vector3 spawnPosEarth = spawnHex.transform.position;
            spawnPosEarth.y = 0.1f;

            earthUnit = Instantiate(Resources.Load<EarthUnit>("PlayerCharacters/EarthUnit"), spawnPosEarth, Quaternion.identity);
            earthUnit.Spawn(spawnHex);

            earthUnit.OnDeath += PlayerUnitDied;

            return earthUnit;
        }
    }

    public Unit SpawnLightningUnit(Hex spawnHex = null)
    {
        // use spawn points by default
        if (spawnHex == null)
        {
            GameObject lightningGO = GameObject.FindGameObjectWithTag("LightningUnitSpawn");

            if (lightningGO)
            {
                Hex spawnHexLightning = lightningGO.transform.parent.GetComponent<Hex>();

                Vector3 spawnPosLightning = spawnHexLightning.transform.position;
                spawnPosLightning.y = 0.1f;

                lightningUnit = Instantiate(Resources.Load<LightningUnit>("PlayerCharacters/LightningUnit"), spawnPosLightning, Quaternion.identity);
                lightningUnit.Spawn(spawnHexLightning);

                lightningUnit.GetComponent<OverworldFollower>().Init();

                lightningUnit.OnDeath += PlayerUnitDied;

                return lightningUnit;
            }
            else
            {
                Debug.LogError("No lightning spawn point found!");

                return null;
            }
        }
        else
        {
            // spawn at checkpoint pos

            Vector3 spawnPosLightning = spawnHex.transform.position;
            spawnPosLightning.y = 0.1f;

            lightningUnit = Instantiate(Resources.Load<LightningUnit>("PlayerCharacters/LightningUnit"), spawnPosLightning, Quaternion.identity);
            lightningUnit.Spawn(spawnHex);

            lightningUnit.GetComponent<OverworldFollower>().Init();

            lightningUnit.OnDeath += PlayerUnitDied;

            return lightningUnit;
        }
    }

    // Temp code that would be removed with object pooling
    public void SubscribeToUnitDeath(LivingEntity a_livingEntity)
    {
        a_livingEntity.OnDeath += EnemyDied;
    }

    public void EnemyDied(LivingEntity a_entity)
    {
        if (trackingKills)
        {
            encounterKillCount++;

            if (encounterKillCount >= EncounterKillLimit)
            {
                Manager.instance.StateController.ChangeStateAfterFade(GameState.overworld);
            }
        }
    }

    public void SetUnitSnapHexes(Hex a_earthHex, Hex a_lightningHex)
    {
        earthSnapHex = a_earthHex;
        lightningSnapHex = a_lightningHex;

        trackingKills = (EncounterKillLimit > 0);
        encounterKillCount = 0;
    }

    public void ResetUnitDeaths()
    {
        earthDead = false;
        LightningDead = false;
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

    #endregion

    #region Unity Life-cycle Methods

    private void Awake()
    {
        UI = GameObject.FindGameObjectWithTag("Manager").GetComponent<UIManager>();

        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridManager>();

        tilesWithinRange = new List<Hex>();

        tilesAffectByAction = new List<Hex>();

        cam = GameObject.FindGameObjectWithTag("CameraRig").GetComponent<CameraController>();

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


    private void Update()
    {
        if (myTurn && canInteract)
        {
            ProcessKeyboardInput();
            ProcessMouseInput();
        }
    }

    #endregion

    #region Local Methods

    // Process Keyboard Input
    private void ProcessKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CancelCurrentAction();

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
            SelectAction(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectAction(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectAction(2);
        }
    }

    // Process Mouse Input
    private void ProcessMouseInput()
    {

        #region Tile / Unit hover Highlighting

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(mouseRay, out hitInfo, Mathf.Infinity, currentRuleset.interactableLayers))
        {
            #region Remove Previous highlighting

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

            #endregion

            tileUnderMouse = hitInfo.transform.GetComponent<Hex>();
            unitUnderMouse = hitInfo.transform.GetComponent<Unit>();

            if (tileUnderMouse)
            {
                // Check if the tile under the mouse is a valid target
                currentRuleset.CheckValidity(selectedUnit, tileUnderMouse);

                if (currentRuleset.WithinRange && currentRuleset.actionType == ActionType.attack || currentRuleset.actionType == ActionType.specialAttack)
                {
                    ProcessActionHighlighting(tileUnderMouse, hitInfo);
                }
                else if (currentRuleset.IsValid && currentRuleset.actionType == ActionType.movement)
                {
                    tileUnderMouse.MouseEnter(currentRuleset.HighlightColour);

                    List<Hex> path = Navigation.FindPath(selectedUnit.CurrentTile, tileUnderMouse);
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
                        foreach (Hex tile in tilesAffectByAction)
                        {
                            tile.MouseExit();
                        }

                        tilesAffectByAction.Clear();
                    }
                }
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
            // If there was a unit that was highlighted remove the highlight
            if (unitUnderMouse)
            {
                unitUnderMouse.Highlight(false, currentRuleset.HighlightColour);
                unitUnderMouse = null;
            }

            // If there was a tile that was highlighted remove the highlight
            if (previousTileUnderMouse)
            {
                previousTileUnderMouse.MouseExit();
                previousTileUnderMouse = null;
            }
        }

        #endregion

        #region Left Click

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

                    selectedUnit.MoveTo(tileUnderMouse, UnitFinishedWalking);
                    RemoveHighlightedTiles();
                    currentRuleset = selectionRuleset;

                    tileUnderMouse.MouseExit();
                    lineRenderer.positionCount = 0;
                    canInteract = false;
                    UI.LockUI();

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

        #endregion

        #region Right Click

        if (Input.GetMouseButton(1))
        {
            // Right click cancels current action
            CancelCurrentAction();
        }

        #endregion
    }

    private void PlayerUnitDied(LivingEntity a_unit)
    {
        if (a_unit.CompareTag("LightningUnit"))
        {
            LightningDead = true;
        }

        if (a_unit.CompareTag("EarthUnit"))
        {
            earthDead = true;
        }

        if (LightningDead && earthDead)
        {
            // Respawn at checkpoint
            Manager.instance.CheckPointController.ResetToLastCheckPoint();
        }
    }

    private void CancelCurrentAction()
    {
        lineRenderer.positionCount = 0;
        currentRuleset = selectionRuleset;
        RemoveHighlightedTiles();

        if (tileUnderMouse)
        {
            tileUnderMouse.MouseExit();
        }
    }

    private void AttackStart()
    {
        UI.LockUI();
    }

    private void AttackEnd()
    {
        canInteract = true;
        UI.UnlockUI();
    }

    private void SpecialAttackStart()
    {
        UI.LockUI();
    }

    private void SpecialAttackEnd()
    {
        canInteract = true;
        UI.UnlockUI();
    }

    private void SelectUnit(Unit a_newSelectedUnit)
    {
        if (a_newSelectedUnit != null)
        {
            if (selectedUnit)
            {
                if (selectedUnit == a_newSelectedUnit)
                {
                    return;
                }

                DeselectUnit();
            }

            selectedUnit = a_newSelectedUnit;

            UI.UpdateSelectedUnit(selectedUnit);
            selectedUnit.Select(true, currentRuleset.ValidHighlightColour);
        }

        cam.LookAtPosition(selectedUnit.transform.position);
    }

    private void DeselectUnit()
    {
        if (selectedUnit)
        {
            selectedUnit.Select(false, currentRuleset.HighlightColour);
            selectedUnit = null;
        }
    }

    private void UnitFinishedWalking()
    {
        canInteract = true;
        UI.UnlockUI();
    }

    private void HighlightTilesInRange(int a_range)
    {
        if (currentRuleset.actionType == ActionType.movement)
        {
            List<Hex> area = grid.GetTilesWithinDistance(selectedUnit.CurrentTile, a_range);

            foreach (Hex tile in area)
            {
                tilesWithinRange.Add(tile);

                if (tile.IsTraversable)
                {
                    tile.Highlight(currentRuleset.InRangeHighlightColour);
                }
                else
                {
                    tile.Highlight(currentRuleset.InvalidHighlightColour);
                }
            }
        }
        else
        {
            List<Hex> area = grid.GetTilesWithinDistance(selectedUnit.CurrentTile, a_range, false);

            foreach (Hex tile in area)
            {
                tilesWithinRange.Add(tile);
                tile.Highlight(currentRuleset.InRangeHighlightColour);
            }
        }
    }

    private void RemoveHighlightedTiles()
    {
        foreach (Hex tile in tilesWithinRange)
        {
            tile.RemoveHighlight();
        }

        tilesWithinRange.Clear();

        if (tilesAffectByAction.Count > 0)
        {
            foreach (Hex tile in tilesAffectByAction)
            {
                tile.MouseExit();
            }

            tilesAffectByAction.Clear();
        }
    }

    private void TurnEvent(TurnManager.TurnState a_newState, int a_turnNumber)
    {
        if (a_newState == TurnManager.TurnState.start)
        {
            myTurn = true;

            earthUnit.Refresh();
            lightningUnit.Refresh();

            if (earthUnit)
            {
                SelectUnit(earthUnit);
            }
            else
            {
                SelectUnit(lightningUnit);
            }
        }
        else if (a_newState == TurnManager.TurnState.end)
        {
            myTurn = false;

            DeselectUnit();
        }
    }

    private void GameStateChanged(GameState a_oldstate, GameState a_newstate)
    {
        // ensure this script knows it's in over-world state
        if (a_newstate == GameState.battle)
        {
            earthUnit.WalkDirectlyToTile(earthSnapHex);
            lightningUnit.WalkDirectlyToTile(lightningSnapHex);

            canInteract = true;
            SelectUnit(earthUnit);
        }
        else if (a_oldstate == GameState.battle)
        {
            DeselectUnit();

            if (earthDead)
            {
                SpawnEarthUnit();
            }

            if (LightningDead)
            {
                SpawnLightningUnit();
            }
        }
    }

    private void ProcessActionHighlighting(Hex a_targetTile, RaycastHit a_hitInfo)
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
                GetTilesAffectByLightningAttack(a_targetTile);
                break;
            case PlayerAttack.lightningSpecial:
                GetTilesAffectByLightningSpecialAttack(a_targetTile);
                break;
        }
    }

    #region Earth Attack highlighting

    private float angle;
    private int snapAngle;

    // returns a tiles within a tilesAffectByAction
    private void GetTilesAffectByEarthAttack(Hex a_targetTile, RaycastHit a_hitInfo)
    {
        foreach (Hex tile in tilesAffectByAction)
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

        MakeCone(snapAngle, earthUnit.ConeRange);

        //Debug.DrawLine(selectedUnit.CurrentTile.transform.position + Vector3.up * 0.2f, selectedUnit.CurrentTile.transform.position + new Vector3(Mathf.Sin(Mathf.Deg2Rad * snapAngle), 0.0f, Mathf.Cos(Mathf.Deg2Rad * snapAngle)) * 2 + Vector3.up * 0.2f, Color.magenta);

        foreach (Hex tile in tilesAffectByAction)
        {
            if (!tile.IsTraversable)
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
    private void GetTilesAffectByEarthSpecialAttack(Hex a_targetTile)
    {
        foreach (Hex tile in tilesAffectByAction)
        {
            tile.MouseExit();
        }

        tilesAffectByAction.Clear();

        tilesAffectByAction.Add(a_targetTile);

        List<Hex> tilesInRange = grid.GetTilesWithinDistance(a_targetTile, 1, false);

        for (int i = 0; i < tilesInRange.Count; i++)
        {
            tilesAffectByAction.Add(tilesInRange[i]);
        }

        foreach (Hex tile in tilesAffectByAction)
        {
            if (!tile.IsTraversable)
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
                Hex currentTile = selectedUnit.CurrentTile.GetNeighbour(mainDir);

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

                    Hex leftTile = currentTile;
                    Hex rightTile = currentTile;

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

    #endregion


    #region Lighting Attack Highlighting

    private void GetTilesAffectByLightningAttack(Hex a_targetTile)
    {
        foreach (Hex tile in tilesAffectByAction)
        {
            tile.MouseExit();
        }

        tilesAffectByAction.Clear();

        tilesAffectByAction.Add(a_targetTile);

        foreach (Hex tile in tilesAffectByAction)
        {
            tile.MouseEnter(currentRuleset.HighlightColour);
        }
    }

    private void GetTilesAffectByLightningSpecialAttack(Hex a_targetTile)
    {
        foreach (Hex tile in tilesAffectByAction)
        {
            tile.MouseExit();
        }

        tilesAffectByAction.Clear();

        tilesAffectByAction.Add(a_targetTile);

        foreach (Hex tile in tilesAffectByAction)
        {
            tile.MouseEnter(currentRuleset.HighlightColour);
        }
    }

    #endregion

    #endregion
}