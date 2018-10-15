using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public enum PlayerAttack { earthBasic, earthSpecial, lightningBasic, lightningSpecial, lightningSpecial2 }

/// <summary>
/// 
/// </summary>
public class PlayerController : MonoBehaviour
{
    #region Unity Inspector Fields

    [SerializeField]
    private float spawnYLevel = 0.0f;

    [SerializeField]
    private InteractionRuleset selectionRuleset;

    [SerializeField]
    private Color enemyThreatColor = Color.red;

    [Header("Debug Info")]
    [SerializeField]
    private InteractionRuleset currentRuleset;

    [SerializeField]
    private int maxVolt = 3;

    [SerializeField]
    private int currentVolt = 0;

    [SerializeField]
    private AK.Wwise.Event endBattleState;

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

    private CameraController cameraRig;

    private AI_Controller AI_controller;

    #endregion

    #region Local Fields


    private bool myTurn;

    private bool threatHeightlightTiles = false;

    private bool canInteract;

    private PlayerAttack currentAttack;

    private List<Hex> tilesWithinRange;

    private List<Hex> tilesAffectByAction;

    private List<Unit> enemiesAffectByAction;

    private List<AI_Agent> enemiesAlive;

    private GridManager grid;

    private Hex earthStartingHex;
    private HexDirection earthStartingDirection;

    private Hex lightningStartingHex;
    private HexDirection lightningStartingDirection;

    private Hex lightningAttackHex1 = null;

    private int encounterKillLimit;
    private int encounterKillCount;

    private bool encounterHasBoss;
    private int encounterBossDamage;
    private int encounterBossDamageGoal;

    private bool trackingKills;

    private bool lightningDead;

    private bool earthDead;

    GraphicRaycaster UIRaycaster;
    PointerEventData UIPointerEventData;
    EventSystem UIEventSystem;


    #endregion

    #region Properties

    public int EncounterKillLimit
    {
        get { return encounterKillLimit; }

        set { encounterKillLimit = value; }
    }

    public bool EncounterHasBoss
    {
        get { return encounterHasBoss; }

        set { encounterHasBoss = value; }
    }

    public int EncounterBossDamage
    {
        get { return encounterBossDamage; }

        set { encounterBossDamage = value; CheckForEndOfEncounter(); }
    }

    public int EncounterBossDamageGoal
    {
        get { return encounterBossDamageGoal; }

        set { encounterBossDamageGoal = value; }
    }


    public EarthUnit EarthUnit { get { return earthUnit; } }
    public LightningUnit LightningUnit { get { return lightningUnit; } }

    public int CurrentVolt
    {
        get { return currentVolt; }

        set
        {
            currentVolt = Mathf.Clamp(value, 0, maxVolt);

            if (currentVolt > 0)
            {
                earthUnit.HasVolt = true;
                lightningUnit.HasVolt = true;
            }
            else
            {
                earthUnit.HasVolt = false;
                lightningUnit.HasVolt = false;
            }

            Manager.instance.UIController.VoltBar.value = currentVolt;
        }
    }

    #endregion

    #region Public Methods

    public void Init()
    {
        // set default values for local fields
        canInteract = false;
        myTurn = false;

        trackingKills = false;
        encounterKillLimit = 0;
        encounterHasBoss = false;
        encounterBossDamage = 0;
        encounterBossDamageGoal = 0;
        encounterKillCount = 0;
        lightningDead = false;
        earthDead = false;
        AI_controller = GameObject.FindGameObjectWithTag("AI_Controller").GetComponent<AI_Controller>();

        // Subscribe to necessary delegates

        Manager.instance.TurnController.PlayerTurnEvent += TurnEvent; // only care about our turn, not the AI's
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

        // Initialise the ToolTip to find the lightning and earth unit
        ToolTip.instance.Init();
    }

    // Toggles between two player units, requires that both units are alive (used by tab)
    public void SwitchSelection()
    {
        if (!earthDead && !lightningDead)
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
    }

    // Spawn's the earth unit and returns a reference to it, takes in an optional spawnHex
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
                spawnPosEarth.y = spawnYLevel;

                earthUnit = Instantiate(Resources.Load<EarthUnit>("PlayerCharacters/EarthUnit"), spawnPosEarth, Quaternion.identity);
                earthUnit.Spawn(spawnHexEarth);

                cameraRig.Init();

                if (lightningUnit)
                {
                    lightningUnit.GetComponent<OverworldFollower>().Init();
                }

                earthUnit.OnDeath += PlayerUnitDied;

                earthDead = false;

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
            spawnPosEarth.y = spawnYLevel;

            earthUnit = Instantiate(Resources.Load<EarthUnit>("PlayerCharacters/EarthUnit"), spawnPosEarth, Quaternion.identity);
            earthUnit.Spawn(spawnHex);

            earthUnit.OnDeath += PlayerUnitDied;

            earthDead = false;

            return earthUnit;
        }
    }

    // Spawn's the lightning unit and returns a reference to it, takes in an optional spawnHex
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
                spawnPosLightning.y = spawnYLevel;

                lightningUnit = Instantiate(Resources.Load<LightningUnit>("PlayerCharacters/LightningUnit"), spawnPosLightning, Quaternion.identity);
                lightningUnit.Spawn(spawnHexLightning);

                lightningUnit.GetComponent<OverworldFollower>().Init();

                lightningUnit.OnDeath += PlayerUnitDied;

                lightningDead = false;

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
            spawnPosLightning.y = spawnYLevel;

            lightningUnit = Instantiate(Resources.Load<LightningUnit>("PlayerCharacters/LightningUnit"), spawnPosLightning, Quaternion.identity);
            lightningUnit.Spawn(spawnHex);

            lightningUnit.GetComponent<OverworldFollower>().Init();

            lightningUnit.OnDeath += PlayerUnitDied;

            lightningDead = false;

            return lightningUnit;
        }
    }

    public bool CheckActionsAvailable()
    {
        return earthUnit.CanMove || earthUnit.CanAttack || earthUnit.CanSpecialAttack || lightningUnit.CanMove || lightningUnit.CanAttack || lightningUnit.CanSpecialAttack;
    }

    // Temp code that would be removed with object pooling
    public void SubscribeToUnitDeath(LivingEntity a_livingEntity)
    {
        a_livingEntity.OnDeath += EnemyDied;
    }

    public void ToggleHighlightEnemiesThreatTiles()
    {
        if (!threatHeightlightTiles)
        {
            threatHeightlightTiles = true;
            HighlightEnemiesThreatTiles(enemiesAlive, enemyThreatColor);
        }
        else
        {
            threatHeightlightTiles = false;
            RemoveHighlightedTiles();

            if (currentRuleset.actionType == ActionType.movement)
            {
                HighlightTilesInRange(selectedUnit.MoveRange, null, true, true);
            }

            // Highlight area in range to attack
            if (currentRuleset.actionType == ActionType.attack)
            {
                HighlightTilesInRange(selectedUnit.AttackRange, null, false, true);
            }

            // Highlight area in range to special attack
            if (currentRuleset.actionType == ActionType.specialAttack)
            {
                HighlightTilesInRange(selectedUnit.SpecialAttackRange, null, false, true);
            }

        }
    }

    // Track enemy units who have died
    public void EnemyDied(LivingEntity a_entity)
    {
        if (trackingKills)
        {
            encounterKillCount++;

            CheckForEndOfEncounter();
        }
    }

    // set the starting Hex that each unit walks towards at the start of an encounter and the direction to face once they arrive at that hex
    public void SetUnitStartingHexes(Hex a_earthHex, HexDirection a_earthDir, Hex a_lightningHex, HexDirection a_lightningDir)
    {
        earthStartingHex = a_earthHex;
        earthStartingDirection = a_earthDir;

        lightningStartingHex = a_lightningHex;
        lightningStartingDirection = a_lightningDir;

        trackingKills = (EncounterKillLimit > 0);
        encounterKillCount = 0;
    }

    public void ResetUnitDeaths()
    {
        earthDead = false;
        lightningDead = false;
    }

    public void GiveVolt()
    {
        CurrentVolt += 1;
    }

    // Set which action of the selected unit is currently being used
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

        if (threatHeightlightTiles)
        {
            HighlightEnemiesThreatTiles(enemiesAlive, enemyThreatColor);
        }

        // Highlight area in range to walk
        if (currentRuleset.actionType == ActionType.movement)
        {
            HighlightTilesInRange(selectedUnit.MoveRange, null, true, true);
        }

        // Highlight area in range to attack
        if (currentRuleset.actionType == ActionType.attack)
        {
            if (selectedUnit == earthUnit)
            {
                HighlightTilesInRange(selectedUnit.AttackRange, null, false, true, true);
            }
            else if (selectedUnit == lightningUnit)
            {
                HighlightTilesInRange(selectedUnit.AttackRange, null, false, true, false);
            }
        }

        // Highlight area in range to special attack
        if (currentRuleset.actionType == ActionType.specialAttack)
        {
            HighlightTilesInRange(selectedUnit.SpecialAttackRange, null, false, true);
        }

    }
    #endregion

    #region Unity Life-cycle Methods

    private void Awake()
    {
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridManager>();

        tilesWithinRange = new List<Hex>();

        //Fetch the Raycaster from the GameObject (the Canvas)
        UIRaycaster = Manager.instance.transform.GetChild(0).GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        UIEventSystem = Manager.instance.transform.GetChild(0).GetComponent<EventSystem>();

        tilesAffectByAction = new List<Hex>();

        enemiesAffectByAction = new List<Unit>();

        cameraRig = GameObject.FindGameObjectWithTag("CameraRig").GetComponent<CameraController>();

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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Manager.instance.StateController.PauseGame();
        }
    }

    #endregion

    #region Local Methods

    // Process Keyboard Input
    private void ProcessKeyboardInput()
    {
        // Toggles between each unit, only works if both are alive.
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchSelection();
        }

        // Key shortcut to walk
        if (Input.GetKeyDown(KeyCode.Alpha1) && selectedUnit.CanMove)
        {
            SelectAction(0);
        }

        // Key shortcut to select basic attack
        if (Input.GetKeyDown(KeyCode.Alpha2) && selectedUnit.CanAttack)
        {
            SelectAction(1);
        }

        // Key shortcut to select special attack
        if (Input.GetKeyDown(KeyCode.Alpha3) && selectedUnit.CanSpecialAttack)
        {
            SelectAction(2);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleHighlightEnemiesThreatTiles();
        }

    }

    // Process Mouse Input
    private void ProcessMouseInput()
    {
        #region Tile / Unit hover Highlighting

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        // make sure the mouse pointeer isn't above UI and that the raycast is hitting something
        if (!EventSystem.current.IsPointerOverGameObject(-1) && Physics.Raycast(mouseRay, out hitInfo, Mathf.Infinity, currentRuleset.interactableLayers))
        {
            #region Remove Previous highlighting

            // if there is a tile under the cursor and it's a new tile then remove the highlight from the previous tile
            if (tileUnderMouse)
            {
                if (previousTileUnderMouse && previousTileUnderMouse != tileUnderMouse)
                {
                    previousTileUnderMouse.MouseExit();
                }

                previousTileUnderMouse = tileUnderMouse;
            }

            // if there is a unit under the cursor and it's a new unit then remove the highlight from the previous unit
            if (unitUnderMouse)
            {
                if (previousUnitUnderMouse && previousUnitUnderMouse != unitUnderMouse)
                {
                    previousUnitUnderMouse.Highlight(false, Color.green);
                }

                previousUnitUnderMouse = unitUnderMouse;
            }

            #endregion

            // try set values, will be null if there isn't an object under the cursor
            tileUnderMouse = hitInfo.transform.GetComponent<Hex>();
            unitUnderMouse = hitInfo.transform.GetComponent<Unit>();
            Hex currentHex;
            if (selectedUnit == null)
            {
                selectedUnit = earthUnit;

            }
            currentHex = selectedUnit.CurrentTile;

            if (lightningAttackHex1 != null)
            {
                currentHex = lightningAttackHex1;
            }

            if (tileUnderMouse)
            {
                if (currentAttack == PlayerAttack.lightningSpecial)
                {
                    // Check if the tile under the mouse is a valid target
                    currentRuleset.CheckValidity(currentHex, tileUnderMouse, true);
                }
                else
                {
                    // Check if the tile under the mouse is a valid target
                    currentRuleset.CheckValidity(currentHex, tileUnderMouse);
                }

                // show attack shape if the tile is within attack range
                if (currentRuleset.WithinRange && currentRuleset.actionType == ActionType.attack || currentRuleset.actionType == ActionType.specialAttack)
                {
                    ProcessActionHighlighting(tileUnderMouse, hitInfo);
                }
                else if (currentRuleset.IsValid && currentRuleset.actionType == ActionType.movement)
                {
                    // if trying to move then simply highlight the tile under the cursor
                    tileUnderMouse.MouseEnter(currentRuleset.HighlightColour);

                    // get path from Navigation and set the points of the lineRenderer to match it
                    List<Hex> path = Navigation.FindPath(currentHex, tileUnderMouse);
                    if (lineRenderer == null)
                    {
                        lineRenderer = GetComponent<LineRenderer>();
                    }
                    lineRenderer.positionCount = path.Count + 1;

                    lineRenderer.SetPosition(0, currentHex.transform.position + Vector3.up * 0.5f);

                    for (int i = 0; i < path.Count; i++)
                    {
                        lineRenderer.SetPosition(i + 1, path[i].transform.position + Vector3.up * 0.5f);
                    }
                }
                else
                {
                    // no valid tile under the cursor, remove all highlighting and previewing
                    lineRenderer.positionCount = 0;

                    if (tilesAffectByAction.Count > 0)
                    {
                        foreach (Hex tile in tilesAffectByAction)
                        {
                            tile.MouseExit();
                        }

                        tilesAffectByAction.Clear();
                    }

                    if (enemiesAffectByAction.Count > 0)
                    {
                        foreach (Unit unit in enemiesAffectByAction)
                        {
                            unit.PreviewDamage(0);
                        }

                        enemiesAffectByAction.Clear();
                    }
                }
            }
            else
            {
                // no tile is under the mouse, remove highlighting


                // If there was a tile that was highlighted remove the highlight
                if (previousTileUnderMouse)
                {
                    previousTileUnderMouse.MouseExit();
                    previousTileUnderMouse = null;
                }
            }

            if (unitUnderMouse)
            {
                // check if the unit under the cursor is a valid target
                currentRuleset.CheckValidity(currentHex, unitUnderMouse);

                // remove highlight from previous unit if there is one and it isn't this unit
                if (previousUnitUnderMouse != null && previousUnitUnderMouse != unitUnderMouse)
                {
                    previousUnitUnderMouse.Highlight(false, currentRuleset.HighlightColour);

                    previousUnitUnderMouse = unitUnderMouse;
                }

                unitUnderMouse.Highlight(true, currentRuleset.HighlightColour);

                if (currentRuleset.actionType == ActionType.selection && unitUnderMouse.CompareTag("Enemy"))
                {
                    // HighlightTilesInRange(unitUnderMouse.MoveRange);
                }
            }
            else
            {
                // no unit is under the mouse, remove highlighting

                // If there was a unit that was highlighted remove the highlight
                if (previousUnitUnderMouse)
                {
                    previousUnitUnderMouse.Highlight(false, currentRuleset.HighlightColour);
                    previousUnitUnderMouse = null;
                }
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

            if (enemiesAffectByAction.Count > 0)
            {
                foreach (Unit unit in enemiesAffectByAction)
                {
                    unit.PreviewDamage(0);
                }

                enemiesAffectByAction.Clear();
            }
        }

        #endregion

        #region Left Click

        if (Input.GetMouseButtonDown(0) && currentRuleset.IsValid)
        {

            UIPointerEventData = new PointerEventData(UIEventSystem);
            UIPointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            UIRaycaster.Raycast(UIPointerEventData, results);

            bool raycastHitButton = false;


            if (results.Count != 0)
            {
                raycastHitButton = true;
            }


            if (raycastHitButton == false)
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
                        Manager.instance.UIController.LockUI();

                        break;

                    case ActionType.attack:

                        if (tilesAffectByAction.Count > 0)
                        {

                            selectedUnit.BasicAttack(tilesAffectByAction.ToArray(), SpecialAttackStart, SpecialAttackEnd);
                            RemoveHighlightedTiles();
                            currentRuleset = selectionRuleset;

                            canInteract = false;
                        }
                        break;

                    case ActionType.specialAttack:

                        if (tilesAffectByAction.Count > 0)
                        {

                            // simply attack if it's the earth unit
                            if (selectedUnit == earthUnit)
                            {
                                selectedUnit.SpecialAttack(tilesAffectByAction.ToArray(), AttackStart, AttackEnd);
                                RemoveHighlightedTiles();
                                currentRuleset = selectionRuleset;
                                canInteract = false;
                            }
                            else
                            {

                                if (lightningAttackHex1 != null && lightningAttackHex1 != tileUnderMouse)// hex1 has been set, set hex2
                                {
                                    selectedUnit.SpecialAttack(tilesAffectByAction.ToArray(), AttackStart, AttackEnd);
                                    RemoveHighlightedTiles();
                                    currentRuleset = selectionRuleset;
                                    canInteract = false;
                                    lightningAttackHex1 = null;
                                    lineRenderer.positionCount = 0;
                                    break;
                                }


                                // set the hex1 if it's null
                                if (lightningAttackHex1 == null)
                                {
                                    lightningAttackHex1 = tileUnderMouse;
                                    RemoveHighlightedTiles();
                                    currentRuleset = selectedUnit.GetAction(2).otherRuleset[0];
                                    HighlightTilesInRange(selectedUnit.SpecialAttackRange, lightningAttackHex1, false, true);

                                }
                            }
                        }

                        break;
                }
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

    // Called when one of the player units die
    private void PlayerUnitDied(LivingEntity a_unit)
    {
        if (a_unit.CompareTag("LightningUnit"))
        {
            lightningDead = true;
        }

        if (a_unit.CompareTag("EarthUnit"))
        {
            earthDead = true;
        }
    }

    // Set currentRuleset back to selectionRuleset and remove all highlighting
    private void CancelCurrentAction()
    {
        lineRenderer.positionCount = 0;
        currentRuleset = selectionRuleset;

        RemoveHighlightedTiles();

        if (threatHeightlightTiles)
        {
            HighlightEnemiesThreatTiles(enemiesAlive, enemyThreatColor);
        }

        if (lightningAttackHex1 != null)
        {
            lightningAttackHex1 = null;
        }

        if (tileUnderMouse)
        {
            tileUnderMouse.MouseExit();
        }
    }

    #region Callbacks

    // Callback called when a unit starts their attack
    private void AttackStart()
    {
        Manager.instance.UIController.LockUI();
    }

    // Callback called when a unit finishes their attack
    private void AttackEnd()
    {
        canInteract = true;
        Manager.instance.UIController.UnlockUI();
    }

    // Callback called when a unit finishes their special attack
    private void SpecialAttackStart()
    {
        Manager.instance.UIController.LockUI();
    }

    // Callback called when a unit finishes their special attack
    private void SpecialAttackEnd()
    {
        canInteract = true;
        Manager.instance.UIController.UnlockUI();
    }

    // Callback called when a unit finishes walking to a tile
    private void UnitFinishedWalking()
    {
        canInteract = true;
        Manager.instance.UIController.UnlockUI();
    }

    #endregion

    // Set which unit is currently selected
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

            Manager.instance.UIController.UpdateSelectedUnit(selectedUnit);
            selectedUnit.Select(true, currentRuleset.ValidHighlightColour);
        }

        cameraRig.LookAtPosition(selectedUnit.transform.position);
    }

    // Remove current selection, used when it's not the player's turn
    private void DeselectUnit()
    {
        if (selectedUnit)
        {
            selectedUnit.Select(false, currentRuleset.HighlightColour);
            selectedUnit = null;
        }
    }


    private List<Hex> HighlightEnemiesThreatTiles(List<AI_Agent> a_enemies, Color a_color)
    {
        List<Hex> area = new List<Hex>();

        foreach (var enemy in a_enemies)
        {

            List<Hex> aiList = grid.GetTilesWithinDistance(enemy.CurrentTile, enemy.MoveRange + enemy.AttackRange, true, true);

            foreach (var hex in aiList)
            {
                if (!area.Contains(hex))
                {
                    area.Add(hex);
                }

            }
        }



        Manager.instance.HexHighlighter.HighLightArea(area, a_color, a_color, this);

        return area;

    }

    // Apply a permanent highlight to tiles within range of a unit
    private void HighlightTilesInRange(int a_range, Hex a_startingHex = null, bool a_checkIfTreversable = true, bool a_addOccupiedTiles = true, bool a_addOutOfBounds = false)
    {
        if (a_startingHex == null)
        {
            a_startingHex = selectedUnit.CurrentTile;
        }
        List<Hex> area = grid.GetTilesWithinDistance(a_startingHex, a_range, a_checkIfTreversable, a_addOccupiedTiles, a_addOutOfBounds);

        Manager.instance.HexHighlighter.HighLightArea(area, currentRuleset.InRangeHighlightColour, currentRuleset.InRangeHighlightColour, this, new List<Hex> { a_startingHex });
    }

    // remove the highlight from all highlighted tiles
    private void RemoveHighlightedTiles()
    {
        Manager.instance.HexHighlighter.RemoveHighlights(this);

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

        if (enemiesAffectByAction.Count > 0)
        {
            foreach (Unit unit in enemiesAffectByAction)
            {
                unit.PreviewDamage(0);
            }

            enemiesAffectByAction.Clear();
        }
    }

    // Called when the player's turn state changes
    private void TurnEvent(TurnManager.TurnState a_newState, int a_turnNumber)
    {
        if (a_newState == TurnManager.TurnState.start)
        {
            // Re-spawn at checkpoint if both are dead
            enemiesAlive = AI_controller.Agents;

            if (lightningDead && earthDead)
            {
                Manager.instance.CheckPointController.ResetToLastCheckPoint();
                return;
            }

            myTurn = true;
            canInteract = true;

            if (!earthDead && !lightningDead)
            {
                earthUnit.Refresh();
                lightningUnit.Refresh();

                SelectUnit(earthUnit);
            }
            else if (!earthDead)
            {
                earthUnit.Refresh();
                SelectUnit(earthUnit);
            }
            else
            {
                lightningUnit.Refresh();
                SelectUnit(lightningUnit);
            }
        }
        else if (a_newState == TurnManager.TurnState.end)
        {
            myTurn = false;
            canInteract = false;
            RemoveHighlightedTiles();

            DeselectUnit();
        }
    }

    // Called when the game state changes
    private void GameStateChanged(GameState a_oldstate, GameState a_newstate)
    {
        if (a_newstate == GameState.transition && Manager.instance.StateController.StateAfterTransition == GameState.battle || a_newstate == GameState.transition && Manager.instance.StateController.StateAfterTransition == GameState.cinematic)
        {
            earthUnit.WalkDirectlyToTile(earthStartingHex, earthStartingDirection);
            lightningUnit.WalkDirectlyToTile(lightningStartingHex, lightningStartingDirection);

        }
        if (a_oldstate == GameState.battle && a_newstate == GameState.transition)
        {
            endBattleState.Post(gameObject);
        }

        // ensure this script knows it's in over-world state
        if (a_newstate == GameState.battle)
        {
            canInteract = true;
            SelectUnit(earthUnit);
        }
        else if (Manager.instance.StateController.CurrentGameState == GameState.transition && Manager.instance.StateController.StateBeforeTransition == GameState.battle)
        {
            DeselectUnit();
            canInteract = false;

            if (earthDead)
            {
                SpawnEarthUnit(earthStartingHex);
                GetComponent<OverworldController>().Init();
            }

            if (lightningDead)
            {
                SpawnLightningUnit(lightningStartingHex).GetComponent<OverworldFollower>().Init();
                GetComponent<OverworldController>().Init();
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
                GetTilesAffectByLightningSpecialAttack(a_targetTile, a_hitInfo);

                break;
        }
    }

    private void CheckForEndOfEncounter()
    {
        if (encounterKillCount >= EncounterKillLimit && (!encounterHasBoss || encounterBossDamage >= encounterBossDamageGoal))
        {
            Manager.instance.StateController.ChangeGameStateAfterDelay(GameState.overworld, 1.0f);
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

        foreach (Unit unit in enemiesAffectByAction)
        {
            unit.PreviewDamage(0);
        }

        enemiesAffectByAction.Clear();

        angle = Vector3.SignedAngle(Vector3.forward, a_targetTile.transform.position - selectedUnit.CurrentTile.transform.position, Vector3.up);

        snapAngle = ((int)Mathf.Round(angle / 30.0f)) * 30;

        if (snapAngle < 0)
        {
            snapAngle = 360 + snapAngle;
        }

        MakeCone(snapAngle, earthUnit.ConeRange);

        foreach (Hex tile in tilesAffectByAction)
        {
            if (tile.CurrentUnit != null && tile.CurrentUnit.CompareTag("Enemy"))
            {
                enemiesAffectByAction.Add(tile.CurrentUnit);
            }

            tile.MouseEnter(currentRuleset.HighlightColour);
        }






        foreach (Unit unit in enemiesAffectByAction)
        {
            unit.PreviewDamage(earthUnit.BasicAttackDamage);
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

        foreach (Unit unit in enemiesAffectByAction)
        {
            unit.PreviewDamage(0);
        }

        enemiesAffectByAction.Clear();

        tilesAffectByAction.Add(a_targetTile);

        List<Hex> tilesInRange = grid.GetTilesWithinDistance(a_targetTile, earthUnit.SpecialDamageArea, false);

        for (int i = 0; i < tilesInRange.Count; i++)
        {
            tilesAffectByAction.Add(tilesInRange[i]);
        }

        foreach (Hex tile in tilesAffectByAction)
        {
            if (tile.CurrentUnit != null && tile.CurrentUnit.CompareTag("Enemy"))
            {
                enemiesAffectByAction.Add(tile.CurrentUnit);
            }

            tile.MouseEnter(currentRuleset.HighlightColour);
        }

        foreach (Unit unit in enemiesAffectByAction)
        {
            int damageIndex = HexUtility.Distance(a_targetTile, unit.CurrentTile) - 1;
            unit.PreviewDamage(earthUnit.GetSpecialDamage(damageIndex));
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

    private void GetTilesAffectByLightningSpecialAttack(Hex a_targetTile, RaycastHit a_hitInfo)
    {
        // clear highlighting 
        foreach (Hex tile in tilesAffectByAction)
        {
            tile.MouseExit();
        }

        tilesAffectByAction.Clear();

        foreach (Unit unit in enemiesAffectByAction)
        {
            unit.PreviewDamage(0);
        }

        enemiesAffectByAction.Clear();

        // calculate tiles affected



        if (lightningAttackHex1 != null)
        {
            //draw line between lightningAttackHex1 and lightningAttackHex2
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, lightningAttackHex1.transform.position);
            lineRenderer.SetPosition(1, a_hitInfo.transform.position);

            tilesAffectByAction.AddRange(grid.GetHexLine(lightningAttackHex1, a_hitInfo.transform.GetComponent<Hex>()));
        }

        tilesAffectByAction.Add(a_targetTile);




        // highlight new tiles

        foreach (Hex tile in tilesAffectByAction)
        {
            if (tile.CurrentUnit != null && tile.CurrentUnit.CompareTag("Enemy"))
            {
                enemiesAffectByAction.Add(tile.CurrentUnit);
            }

            tile.MouseEnter(currentRuleset.HighlightColour);
        }

        foreach (Unit unit in enemiesAffectByAction)
        {
            unit.PreviewDamage(lightningUnit.SpecialAttackDamage);
        }
    }

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

    #endregion

    #endregion
}