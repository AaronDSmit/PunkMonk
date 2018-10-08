using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class EasyDesignEditor : EditorWindow
{
    #region Variables

    // general variables
    [SerializeField]
    private static EditorWindow window;
    [SerializeField]
    private static Texture icon;
    [SerializeField]
    private GUISkin skin;
    [SerializeField]
    private GUIStyle centeredText;
    [SerializeField]
    private static int selectedTab = 0;

    [SerializeField]
    private static int previousTab = 0;

    [SerializeField]
    private static GridManager grid;
    [SerializeField]
    private static float hexScale = 1;
    [SerializeField]
    private static float hexAlpha = 0.35f;

    // GridManager Generation
    [SerializeField]
    private int mapWidth;
    [SerializeField]
    private int mapHeight;
    [SerializeField]
    private static bool confirmMapGeneration = false;
    [SerializeField]
    private static bool mapSizeSet;

    // Navigation
    [SerializeField]
    private bool hasTileSelected = false;
    [SerializeField]
    private bool hasTransitionSelected = false;
    [SerializeField]
    private bool hasStateTransitionSelected = false;
    [SerializeField]
    private bool hasSpawnerSelected = false;
    [SerializeField]
    private bool hasTriggerSelected = false;

    [SerializeField]
    private HexState tileState = HexState.Traversable;

    [SerializeField]
    private bool setAllNodesTraversable = false;
    [SerializeField]
    private bool setAllNodesInaccessible = false;

    [SerializeField]
    private bool autoAllNodesTraversable = false;
    [SerializeField]
    private bool autoAllNodesInaccessible = false;

    [SerializeField]
    private bool fillNodes = false;

    [SerializeField]
    private Color traversableColour;
    [SerializeField]
    private Color inaccessibleColour;
    [SerializeField]
    private float inaccessibleAlpha = 0;
    [SerializeField]
    private Color outOfBoundsColour;

    // GameFlow
    [SerializeField]
    private UnitType enemyType;
    [SerializeField]
    private bool hasVolt = false;
    [SerializeField]
    private int turnToSpawn;
    [SerializeField]
    private int everyXTurns;
    [SerializeField]
    private int loadLevel = 0;
    [SerializeField]
    private GameState targetState = GameState.overworld;
    [SerializeField]
    private GameState currentState = GameState.battle;
    [SerializeField]
    private int numberToKill = 0;
    [SerializeField]
    private bool hasBoss = false;
    [SerializeField]
    private int bossDamageGoal = 0;

    [SerializeField]
    private Conversation convo;

    [SerializeField]
    private int currentID = 0;
    [SerializeField]
    private string spawnerButtonName;
    [SerializeField]
    private Spawner[] selectedSpawners;
    [SerializeField]
    private StateTransitionPoint[] selectedTransitionPoints;

    [SerializeField]
    private HexDirection earthDirection;

    [SerializeField]
    private HexDirection lightningDirection;

    [SerializeField]
    private int voltGiven;

    // Dialogue
    [SerializeField]
    private Conversation conversation;

    [SerializeField]
    private int selectedSpeachIndex;

    [SerializeField]
    private Conversation loadedConversation;

    private Vector2 scroll;

    [SerializeField]
    private bool confirmSave;

    [SerializeField]
    private Color greenColour = new Color(0.54f, 1.0f, 0.24f);
    [SerializeField]
    private Color redColour = new Color(0.83f, 0.18f, 0.18f);
    [SerializeField]
    private Color orangeColour = new Color(1.0f, 0.62f, 0.21f);
    [SerializeField]
    private Color blueColour = new Color(0.55f, 0.9f, 0.89f);
    [SerializeField]
    private Color brownColour = new Color(0.84f, 0.64f, 0.49f);
    [SerializeField]
    private Color yellowColour = new Color(0.93f, 0.58f, 0.04f);

    // Settings
    [SerializeField]
    private bool healthyMode = true;

    [SerializeField]
    private bool toggleSnap = false;

    private Vector3 prevPosition;


    #endregion

    private void OnEnable()
    {

        EditorApplication.update += Update;

        // load custom skin and window icon
        skin = (GUISkin)Resources.Load("EditorSkin");

        if (skin != null)
        {
            centeredText = skin.GetStyle("CenteredText");

            if (window != null)
            {
                window.titleContent = new GUIContent("Easy Design", icon);
            }
        }
        GameObject gridGo = GameObject.FindGameObjectWithTag("Manager");

        if (gridGo && grid == null)
        {
            grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridManager>();

            if (grid && grid.transform.childCount > 0)
            {
                string[] mapSize = grid.transform.GetChild(grid.transform.childCount - 1).name.Split(',');

                mapWidth = int.Parse(mapSize[0]) + 1;
                mapHeight = int.Parse(mapSize[1]) + 1;
            }
        }

        traversableColour = new Color(0.0f, 1.0f, 0.0f, hexAlpha);
        inaccessibleColour = new Color(1.0f, 1.0f, 0.0f, hexAlpha);
        outOfBoundsColour = new Color(1.0f, 0.0f, 0.0f, hexAlpha);
        //inaccessibleAlpha = 0.4f;

        conversation = CreateInstance<Conversation>();

        selectedSpeachIndex = 0;

        EditorApplication.playModeStateChanged += PlayModeChanged;

    }

    [MenuItem("Window/EasyDesign")]
    public static void ShowWindow()
    {
        window = GetWindow(typeof(EasyDesignEditor), false, "Easy Design");

        icon = (Texture)Resources.Load("icon");
        window.titleContent = new GUIContent("Easy Design", icon);
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= PlayModeChanged;
    }

    private void PlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            GameObject gridGo = GameObject.FindGameObjectWithTag("Manager");

            if (gridGo && grid == null)
            {
                grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridManager>();

                Hex[] tiles = grid.GetComponentsInChildren<Hex>();

                foreach (Hex tile in tiles)
                {
                    tile.inaccessibleAlpha = inaccessibleAlpha;
                }
            }

            HexUtility.UpdateScale(hexScale);
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {

        }
    }

    private void OnGUI()
    {

        if (skin == null)
        {
            skin = GUI.skin;
            centeredText = GUIStyle.none;
            //centeredText = skin.GetStyle("CenteredText");

        }

        EditorGUI.BeginChangeCheck();

        // Only show compiling message while Compiling
        if (EditorApplication.isCompiling)
        {
            EditorGUILayout.HelpBox("Compiling", MessageType.Info);

            return;
        }

        // disable inspector if working for more than 5 hours
        if (healthyMode && EditorApplication.timeSinceStartup > 18000)
        {
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("You have been working for 5 hours, Please close Unity and take a break", MessageType.Error);

            return;
        }

        GUI.skin = skin; // use a custom skin

        // If in play mode then don't draw any of the inspector
        if (EditorApplication.isPlaying)
        {
            BeginColouredVerticalBox(new Color(0.5f, 0.5f, 0.5f, 1.0f));

            GUILayout.FlexibleSpace(); // outer box

            EditorGUILayout.EndVertical(); // outer box
        }
        else
        {
            // If there's no grid manager then don't draw any of the inspector
            if (grid == null)
            {
                EditorGUILayout.HelpBox("No GridManager script found!", MessageType.Error);

                return;
            }

            // Create toolbar using custom tab style
            string[] tabs = { "Grid", "GameFlow", "Dialogue", "Settings" };
            selectedTab = GUILayout.Toolbar(selectedTab, tabs);

            if (previousTab != selectedTab)
            {
                previousTab = selectedTab;
                GUI.FocusControl("");
            }

            BeginColouredVerticalBox(new Color(0.5f, 0.5f, 0.5f, 1.0f)); // outer box

            if (healthyMode && EditorApplication.timeSinceStartup > 3600)
            {
                EditorGUILayout.Space();

                EditorGUILayout.HelpBox("You have been working for an hour, perhaps close Unity and take a short break.", MessageType.Warning);
            }

            EditorGUILayout.Space();

            #region GridManager Generation

            if (selectedTab == 0)
            {
                #region Grid Generation

                GUILayout.Label("Grid Generation:", centeredText);

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("X-Axis:", GUILayout.ExpandWidth(false));
                mapWidth = EditorGUILayout.IntField(mapWidth, GUILayout.ExpandWidth(false));

                GUILayout.Label("Z-Axis:", GUILayout.ExpandWidth(false));
                mapHeight = EditorGUILayout.IntField(mapHeight, GUILayout.ExpandWidth(false));

                EditorGUILayout.EndHorizontal();

                mapSizeSet = (mapWidth != 0 && mapHeight != 0);

                if (!mapSizeSet)
                {
                    EditorGUILayout.HelpBox("Axis's can't be 0", MessageType.Warning);
                }
                else if (confirmMapGeneration)
                {
                    BeginColouredVerticalBox(orangeColour);

                    GUILayout.Label("Create New Map?", centeredText);

                    EditorGUILayout.BeginHorizontal();

                    if (ColouredButton("Yes!", greenColour))
                    {
                        HexUtility.UpdateScale(hexScale);

                        if (grid.GenerateGrid(mapWidth, mapHeight, traversableColour))
                        {
                            confirmMapGeneration = false;
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("There was an error generating the grid.", MessageType.Error);
                        }

                        confirmMapGeneration = false;
                    }

                    if (ColouredButton("No...", redColour))
                    {
                        confirmMapGeneration = false;
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();
                }
                else if (ColouredButton("Generate Grid", greenColour))
                {
                    confirmMapGeneration = true;
                }

                #endregion

                EditorGUILayout.Space();

                #region Set Selected Tiles To Traversable/Inaccessible

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("State To Set Tiles", centeredText);
                tileState = (HexState)EditorGUILayout.EnumPopup(tileState);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginDisabledGroup(!hasTileSelected);

                EditorGUILayout.BeginHorizontal();

                if (ColouredButton("Set Selected", greenColour))
                {
                    GameObject[] selectedObjects = Selection.gameObjects;

                    foreach (GameObject obj in selectedObjects)
                    {
                        Hex[] tiles = obj.GetComponentsInChildren<Hex>();

                        foreach (Hex tile in tiles)
                        {
                            tile.SetHexState(tileState, GetHexStateColour(tileState));
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();

                EditorGUI.EndDisabledGroup();

                #endregion

                #region Set All Tiles To Traversable/Inaccessible

                EditorGUILayout.BeginHorizontal();

                if (!setAllNodesInaccessible && setAllNodesTraversable)
                {
                    BeginColouredVerticalBox(orangeColour);

                    GUILayout.Label("Set All Tiles?", centeredText);

                    EditorGUILayout.BeginHorizontal();

                    if (ColouredButton("Yes!", greenColour))
                    {
                        Hex[] tiles = grid.GetComponentsInChildren<Hex>();

                        foreach (Hex tile in tiles)
                        {
                            tile.SetHexState(tileState, GetHexStateColour(tileState));
                        }

                        setAllNodesTraversable = false;
                    }

                    if (ColouredButton("No...", redColour))
                    {
                        setAllNodesTraversable = false;
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();
                }
                else if (!setAllNodesInaccessible)
                {
                    if (ColouredButton("Set All Tiles", greenColour))
                    {
                        setAllNodesTraversable = true;
                    }
                }

                EditorGUILayout.EndHorizontal();

                #endregion

                #region Auto Traversable/Inaccessible

                EditorGUILayout.BeginHorizontal();

                if (!autoAllNodesInaccessible && autoAllNodesTraversable)
                {
                    BeginColouredVerticalBox(orangeColour);

                    GUILayout.Label("Set Automatically?", centeredText);

                    EditorGUILayout.BeginHorizontal();

                    if (ColouredButton("Yes!", greenColour))
                    {
                        Hex[] tiles = grid.GetComponentsInChildren<Hex>();

                        foreach (Hex tile in tiles)
                        {
                            tile.TraversableCheck(traversableColour);
                        }

                        autoAllNodesTraversable = false;
                    }

                    if (ColouredButton("No...", redColour))
                    {
                        autoAllNodesTraversable = false;
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();
                }
                else if (!autoAllNodesInaccessible)
                {
                    if (ColouredButton("Set Automaticlly", greenColour))
                    {
                        autoAllNodesTraversable = true;
                    }
                }

                EditorGUILayout.EndHorizontal();

                #endregion

                #region Fill Tiles Traversable/Inaccessible

                EditorGUI.BeginDisabledGroup(!hasTileSelected || Selection.gameObjects.Length != 1);

                EditorGUILayout.BeginHorizontal();

                if (fillNodes)
                {
                    BeginColouredVerticalBox(orangeColour);

                    GUILayout.Label("Fill Nodes?", centeredText);

                    EditorGUILayout.BeginHorizontal();

                    if (ColouredButton("Yes!", greenColour))
                    {
                        Hex hex = Selection.gameObjects[0].GetComponent<Hex>();
                        Hex currentHex;

                        HexState originalHexState = hex.HexState;

                        List<Hex> openList = new List<Hex>();
                        List<Hex> closeList = new List<Hex>();

                        openList.Add(hex);

                        while (openList.Count > 0)
                        {
                            currentHex = openList[0];
                            openList.RemoveAt(0);
                            closeList.Add(currentHex);

                            currentHex.SetHexState(tileState, GetHexStateColour(tileState));

                            foreach (Hex neighbour in currentHex.Neighbours)
                            {
                                if (neighbour.HexState == originalHexState && !openList.Contains(neighbour) && !closeList.Contains(neighbour))
                                {
                                    openList.Add(neighbour);
                                }
                            }
                        }

                        fillNodes = false;
                    }

                    if (ColouredButton("No...", redColour))
                    {
                        fillNodes = false;
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();
                }
                else
                {
                    if (ColouredButton("Fill Area", greenColour))
                    {
                        fillNodes = true;
                    }
                }

                EditorGUILayout.EndHorizontal();

                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();

                GUILayout.Label("Use this button if your grid is out dated but still has the correct colours", centeredText);

                if (ColouredButton("Update Tiles", orangeColour))
                {
                    Hex[] hexs = grid.GetComponentsInChildren<Hex>();

                    foreach (Hex hex in hexs)
                    {
                        SpriteRenderer highlight = hex.GetComponentsInChildren<SpriteRenderer>()[1];

                        if (highlight.color == new Color(0.0f, 1.0f, 0.0f, 0.35f))
                        {
                            hex.SetHexState(HexState.Traversable, traversableColour);
                        }
                        else if (highlight.color == new Color(1.0f, 0.0f, 0.0f, 0.35f))
                        {
                            hex.SetHexState(HexState.OutOfBounds, outOfBoundsColour);
                        }
                    }
                }

                #endregion

                EditorGUILayout.Space();

                #region Asset Assist

                GUILayout.Label("Greybox Assist:", centeredText);

                EditorGUI.BeginDisabledGroup(Selection.gameObjects.Length == 0);

                if (ColouredButton("Snap Objects", greenColour))
                {
                    foreach (Transform transform in Selection.GetTransforms(SelectionMode.TopLevel))
                    {
                        Vector3 position = grid.GetHexFromPosition(transform.position).transform.position;

                        position.y = transform.position.y;
                        transform.position = position;
                    }
                }

                if (ColouredButton("Select Hexes", greenColour))
                {
                    GameObject[] hexSelection = new GameObject[Selection.GetTransforms(SelectionMode.TopLevel).Length];
                    int i = 0;

                    foreach (Transform transform in Selection.GetTransforms(SelectionMode.TopLevel))
                    {
                        if (transform.parent != null && transform.parent.GetComponent<Hex>())
                        {
                            hexSelection[i++] = transform.parent.gameObject;
                        }
                    }

                    Selection.objects = hexSelection;
                }

                EditorGUI.EndDisabledGroup();

                #endregion

                EditorGUILayout.Space();

                GUILayout.Label("Visual Settings:", centeredText);

                //traversableColour = EditorGUILayout.ColorField("Traversable", traversableColour);
                //inaccessibleColour = EditorGUILayout.ColorField("Inaccessible", inaccessibleColour);
                //connectionColour = EditorGUILayout.ColorField("Connection", connectionColour);

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Inaccessible Alpha");
                inaccessibleAlpha = EditorGUILayout.Slider(inaccessibleAlpha, 0.0f, 1.0f);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Hex Scale:");
                    hexScale = EditorGUILayout.FloatField(hexScale);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Global Hex Alpha:");
                    float prevHexAlpha = hexAlpha;
                    hexAlpha = EditorGUILayout.Slider(hexAlpha, 0.0f, 1.0f);
                    if (prevHexAlpha != hexAlpha)
                    {
                        traversableColour = new Color(traversableColour.r, traversableColour.g, traversableColour.b, hexAlpha);
                        inaccessibleColour = new Color(inaccessibleColour.r, inaccessibleColour.g, inaccessibleColour.b, hexAlpha);
                        outOfBoundsColour = new Color(outOfBoundsColour.r, outOfBoundsColour.g, outOfBoundsColour.b, hexAlpha);

                        Hex[] tiles = grid.GetComponentsInChildren<Hex>();

                        foreach (Hex tile in tiles)
                        {
                            tile.SetHexState(tile.HexState, GetHexStateColour(tile.HexState));
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            #endregion

            #region GameFlow

            else if (selectedTab == 1)
            {
                #region Player Spawn points 

                GUILayout.Label("Player Spawn Points", centeredText);

                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginDisabledGroup(!hasTileSelected || hasSpawnerSelected);

                if (GameObject.FindGameObjectWithTag("EarthUnitSpawn") == null)
                {
                    if (ColouredButton("Add Earth Spawn", brownColour))
                    {
                        Hex tile = Selection.gameObjects[0].GetComponent<Hex>();

                        GameObject spawnerGO = new GameObject("Earth Spawner")
                        {
                            tag = "EarthUnitSpawn"
                        };

                        Spawner spawner = spawnerGO.AddComponent<Spawner>();
                        spawner.TextColour = brownColour;
                        spawner.drawText = true;
                        spawner.index = -1;

                        spawner.transform.parent = tile.transform;
                        spawner.transform.position = tile.transform.position;
                        spawner.TurnToSpawn = 0;
                    }
                }
                else
                {
                    if (ColouredButton("Move Earth Spawn", brownColour))
                    {
                        Hex tile = Selection.gameObjects[0].GetComponent<Hex>();
                        Spawner spawner = GameObject.FindGameObjectWithTag("EarthUnitSpawn").GetComponent<Spawner>();

                        spawner.transform.parent = tile.transform;
                        spawner.transform.position = tile.transform.position;
                        spawner.TurnToSpawn = 0;
                    }
                }

                if (GameObject.FindGameObjectWithTag("LightningUnitSpawn") == null)
                {
                    if (ColouredButton("Add Lightning spawn", blueColour))
                    {
                        Hex tile = Selection.gameObjects[0].GetComponent<Hex>();

                        GameObject spawnerGO = new GameObject("Lightning Spawner")
                        {
                            tag = "LightningUnitSpawn"
                        };

                        Spawner spawner = spawnerGO.AddComponent<Spawner>();
                        spawner.TextColour = blueColour;
                        spawner.drawText = true;
                        spawner.index = -1;

                        spawner.transform.parent = tile.transform;
                        spawner.transform.position = tile.transform.position;
                        spawner.TurnToSpawn = 0;
                    }
                }
                else
                {
                    if (ColouredButton("Move Lightning Spawn", blueColour))
                    {
                        Hex tile = Selection.gameObjects[0].GetComponent<Hex>();
                        Spawner spawner = GameObject.FindGameObjectWithTag("LightningUnitSpawn").GetComponent<Spawner>();

                        spawner.transform.parent = tile.transform;
                        spawner.transform.position = tile.transform.position;
                        spawner.TurnToSpawn = 0;
                    }
                }

                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();

                #endregion

                EditorGUILayout.Space();

                #region Enemy Spawn Points

                GUILayout.Label("Enemy Spawn Points", centeredText);

                EditorGUI.BeginDisabledGroup(!hasTileSelected);

                EditorGUILayout.BeginHorizontal();

                // if the selected tile has a Spawner change it rather than add a new one
                if (hasSpawnerSelected)
                {
                    if (ColouredButton("Change " + spawnerButtonName, orangeColour))
                    {
                        foreach (Spawner spawner in selectedSpawners)
                        {
                            spawner.index = currentID;
                            spawner.TurnToSpawn = turnToSpawn;
                            spawner.hasVolt = hasVolt;

                            if (enemyType == UnitType.watcher)
                            {
                                spawner.UnitToSpawn = Resources.Load<AI_Agent>("EnemyCharacters/WatcherUnit");
                                spawner.TextColour = Color.magenta;
                            }
                            else if (enemyType == UnitType.runner)
                            {
                                spawner.UnitToSpawn = Resources.Load<AI_Agent>("EnemyCharacters/RunnerUnit");
                                spawner.TextColour = Color.red;
                            }
                            else if (enemyType == UnitType.missile)
                            {
                                spawner.UnitToSpawn = Resources.Load<AI_Agent>("EnemyCharacters/MissileUnit");
                                spawner.TextColour = Color.grey;
                            }
                        }

                        UpdateTriggerConnections();
                    }
                }
                else
                {
                    Hex[] selectedTiles = Selection.GetFiltered<Hex>(SelectionMode.Deep);

                    string buttonName = (selectedTiles.Length == 1) ? "Add Spawner" : "Add Spawners";

                    if (ColouredButton(buttonName, greenColour))
                    {
                        foreach (Hex tile in selectedTiles)
                        {
                            GameObject spawnerGO = new GameObject("EnemySpawner");
                            Spawner spawner = spawnerGO.AddComponent<Spawner>();
                            spawner.drawText = true;
                            spawner.currentHex = tile;
                            spawner.index = currentID;
                            spawner.hasVolt = hasVolt;

                            if (enemyType == UnitType.watcher)
                            {
                                spawner.UnitToSpawn = Resources.Load<Unit>("EnemyCharacters/WatcherUnit");
                                spawner.TextColour = Color.red;
                            }
                            else if (enemyType == UnitType.runner)
                            {
                                spawner.UnitToSpawn = Resources.Load<Unit>("EnemyCharacters/RunnerUnit");
                                spawner.TextColour = Color.red;
                            }
                            else if (enemyType == UnitType.missile)
                            {
                                spawner.UnitToSpawn = Resources.Load<AI_Agent>("EnemyCharacters/MissileUnit");
                                spawner.TextColour = Color.grey;
                            }

                            spawner.transform.parent = tile.transform;
                            spawner.transform.position = tile.transform.position;
                            spawner.transform.localEulerAngles = new Vector3(180, 0.0f, 0.0f);
                            spawner.TurnToSpawn = turnToSpawn;
                        }

                        UpdateSpawnerSelection();


                    }
                }

                EditorGUI.EndDisabledGroup();

                GUILayout.Label("Turn:");

                turnToSpawn = EditorGUILayout.IntField(turnToSpawn);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Enemy: ");

                enemyType = (UnitType)EditorGUILayout.EnumPopup(enemyType);

                GUILayout.Label("Has volt: ");

                hasVolt = EditorGUILayout.Toggle(hasVolt, skin.GetStyle("toggle"));

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUI.BeginDisabledGroup(!hasSpawnerSelected);

                    if (ColouredButton("Set Door Hex", yellowColour))
                    {
                        // Find a hex that isn't a spawner
                        Hex hex = null;
                        foreach (GameObject go in Selection.gameObjects)
                        {
                            if (go.GetComponentInChildren<Spawner>() == false)
                            {
                                hex = go.GetComponent<Hex>();
                                break;
                            }
                        }
                        // If a hex was found set all selected spawners to use it
                        if (hex)
                        {
                            foreach (Spawner spawner in selectedSpawners)
                            {
                                spawner.doorHex = hex;
                            }
                        }
                    }

                    if (ColouredButton("Set Target Hex", orangeColour))
                    {
                        // Find a hex that isn't a spawner
                        Hex hex = null;
                        foreach (GameObject go in Selection.gameObjects)
                        {
                            if (go.GetComponentInChildren<Spawner>() == false)
                            {
                                hex = go.GetComponent<Hex>();
                                break;
                            }
                        }
                        // If a hex was found set all selected spawners to use it
                        if (hex)
                        {
                            foreach (Spawner spawner in selectedSpawners)
                            {
                                spawner.targetHex = hex;
                            }
                        }
                    }

                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginDisabledGroup(!hasSpawnerSelected);

                if (ColouredButton("Remove " + spawnerButtonName, redColour))
                {
                    GameObject[] selectedObjects = Selection.gameObjects;

                    foreach (GameObject obj in selectedObjects)
                    {
                        Hex[] tiles = obj.GetComponentsInChildren<Hex>();

                        foreach (Hex tile in tiles)
                        {
                            if (tile.GetComponentInChildren<Spawner>())
                            {
                                DestroyImmediate(tile.GetComponentInChildren<Spawner>().gameObject);
                            }
                        }
                    }

                    UpdateTriggerConnections();
                    UpdateSpawnerSelection();
                    currentID = 0;
                }

                EditorGUI.EndDisabledGroup();

                if (ColouredButton("Remove All Spawners", redColour))
                {
                    Spawner[] spawnPoints = grid.GetComponentsInChildren<Spawner>();

                    foreach (Spawner spawner in spawnPoints)
                    {
                        if (spawner.name == "EnemySpawner")
                        {
                            DestroyImmediate(spawner.gameObject);
                        }
                    }

                    UpdateTriggerConnections();
                    UpdateSpawnerSelection();
                    currentID = 0;
                }

                EditorGUILayout.EndHorizontal();

                #endregion

                EditorGUILayout.Space();

                #region Spawn Triggers

                GUILayout.Label("Spawn Triggers:", centeredText);

                EditorGUI.BeginDisabledGroup(Selection.gameObjects.Length != 1);

                EditorGUILayout.BeginHorizontal();

                // if the selected tile has a Spawner change it rather than add a new one
                if (Selection.gameObjects.Length == 1 && Selection.gameObjects[0].GetComponentInChildren<SpawnTrigger>())
                {

                    if (ColouredButton("Change Trigger", orangeColour))
                    {
                        SpawnTrigger spawnTrigger = Selection.gameObjects[0].GetComponentInChildren<SpawnTrigger>();
                        spawnTrigger.index = currentID;
                        spawnTrigger.UpdateSpawnerList();
                    }
                }
                else
                {
                    if (ColouredButton("Add Trigger", greenColour))
                    {
                        Hex tile = Selection.gameObjects[0].GetComponent<Hex>();

                        GameObject go = new GameObject("SpawnTrigger");
                        go.transform.parent = tile.transform;
                        go.transform.localPosition = Vector3.zero;

                        BoxCollider collider = go.AddComponent<BoxCollider>();
                        collider.isTrigger = true;

                        SpawnTrigger spawnTrigger = go.AddComponent<SpawnTrigger>();
                        spawnTrigger.UpdateSpawnerList();
                    }
                }

                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(Selection.gameObjects.Length != 1 || !hasTriggerSelected);

                if (ColouredButton("Remove Trigger", redColour))
                {
                    Hex tile = Selection.gameObjects[0].GetComponent<Hex>();
                    SpawnTrigger destroyedTrigger = tile.GetComponentInChildren<SpawnTrigger>();

                    DestroyImmediate(destroyedTrigger);

                    UpdateTriggerConnections();
                    UpdateSpawnerSelection();
                }

                EditorGUILayout.EndHorizontal();

                EditorGUI.EndDisabledGroup();

                #endregion

                EditorGUILayout.Space();

                #region State Transition

                GUILayout.Label("State Transition:", centeredText);

                EditorGUI.BeginDisabledGroup(Selection.gameObjects.Length != 1 || !hasTileSelected);

                EditorGUILayout.BeginHorizontal();

                // if the selected tile has a scene transition change it rather than add a new one
                if (Selection.gameObjects.Length == 1 && Selection.gameObjects[0].GetComponentInChildren<StateTransitionPoint>())
                {
                    if (ColouredButton("Change Transition", orangeColour))
                    {
                        StateTransitionPoint sceneTransition = Selection.gameObjects[0].GetComponentInChildren<StateTransitionPoint>();
                        sceneTransition.TargetState = targetState;
                        sceneTransition.CurrentState = currentState;
                        sceneTransition.numberToKill = numberToKill;
                        sceneTransition.hasBoss = hasBoss;
                        sceneTransition.bossDamage = bossDamageGoal;
                        sceneTransition.voltGiven = voltGiven;
                        sceneTransition.EarthDirection = earthDirection;
                        sceneTransition.LightningDirection = lightningDirection;
                        sceneTransition.Conversation = convo;

                        sceneTransition.index = currentID;
                    }
                }
                else
                {
                    if (ColouredButton("Add Transition", greenColour))
                    {
                        Hex tile = Selection.gameObjects[0].GetComponent<Hex>();

                        GameObject transitionGO = new GameObject("stateTransition");

                        StateTransitionPoint sceneTransition = transitionGO.AddComponent<StateTransitionPoint>();
                        sceneTransition.TargetState = targetState;
                        sceneTransition.CurrentState = currentState;
                        sceneTransition.numberToKill = numberToKill;
                        sceneTransition.voltGiven = voltGiven;

                        sceneTransition.Conversation = convo;

                        sceneTransition.drawText = true;
                        sceneTransition.index = currentID;

                        BoxCollider trigger = transitionGO.AddComponent<BoxCollider>();
                        trigger.isTrigger = true;

                        sceneTransition.transform.parent = tile.transform;
                        sceneTransition.transform.position = tile.transform.position;
                    }
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("From ");

                currentState = (GameState)EditorGUILayout.EnumPopup(currentState);

                GUILayout.Label(" To ");

                targetState = (GameState)EditorGUILayout.EnumPopup(targetState);

                EditorGUILayout.EndHorizontal();


                if (targetState == GameState.battle)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Enemies to kill ");
                        numberToKill = EditorGUILayout.IntField(numberToKill);
                        GUILayout.Label("Has Boss");
                        hasBoss = EditorGUILayout.Toggle(hasBoss);
                    }
                    EditorGUILayout.EndHorizontal();

                    if (hasBoss)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("Boss Damage ");
                            bossDamageGoal = EditorGUILayout.IntField(bossDamageGoal);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                else if (targetState == GameState.cinematic)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Conversation");
                        convo = EditorGUILayout.ObjectField("", convo, typeof(Conversation), true) as Conversation;
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Volt Given to the player");

                voltGiven = EditorGUILayout.IntField(voltGiven);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                if (ColouredButton("Earth Hex", brownColour))
                {
                    Hex tile = Selection.gameObjects[0].GetComponent<Hex>();

                    StateTransitionPoint[] transitions = FindObjectsOfType<StateTransitionPoint>();

                    foreach (StateTransitionPoint point in transitions)
                    {
                        if (point.index == currentID)
                        {
                            point.EarthHex = tile;
                            point.EarthDirection = earthDirection;
                        }
                    }
                }

                earthDirection = (HexDirection)EditorGUILayout.EnumPopup(earthDirection);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                if (ColouredButton("Lightning Hex", blueColour))
                {
                    Hex tile = Selection.gameObjects[0].GetComponent<Hex>();

                    StateTransitionPoint[] transitions = FindObjectsOfType<StateTransitionPoint>();

                    foreach (StateTransitionPoint point in transitions)
                    {
                        if (point.index == currentID)
                        {
                            point.LightningHex = tile;
                            point.LightningDirection = lightningDirection;
                        }
                    }
                }

                lightningDirection = (HexDirection)EditorGUILayout.EnumPopup(lightningDirection);

                EditorGUILayout.EndHorizontal();

                // Check point
                if (ColouredButton("Check Point Hex", yellowColour))
                {
                    Hex tile = Selection.gameObjects[0].GetComponent<Hex>();

                    StateTransitionPoint[] transitions = FindObjectsOfType<StateTransitionPoint>();

                    foreach (StateTransitionPoint point in transitions)
                    {
                        if (point.index == currentID)
                        {
                            point.SetCheckPoint(tile);
                        }
                    }
                }

                EditorGUILayout.Space();

                EditorGUI.EndDisabledGroup();

                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginDisabledGroup(!hasStateTransitionSelected || !hasTileSelected);

                if (ColouredButton("Remove State Transition", redColour))
                {
                    GameObject[] selectedObjects = Selection.gameObjects;

                    StateTransitionPoint transition = selectedObjects[0].GetComponentInChildren<StateTransitionPoint>();

                    DestroyImmediate(transition.gameObject);
                }

                EditorGUI.EndDisabledGroup();

                if (ColouredButton("Remove All State Transitions", redColour))
                {
                    StateTransitionPoint[] transitions = grid.GetComponentsInChildren<StateTransitionPoint>();

                    foreach (StateTransitionPoint transition in transitions)
                    {
                        DestroyImmediate(transition.gameObject);
                    }
                }

                EditorGUILayout.EndHorizontal();

                #endregion

                EditorGUILayout.Space();

                #region Scene Transition

                GUILayout.Label("Scene Transition:", centeredText);

                EditorGUI.BeginDisabledGroup(Selection.gameObjects.Length != 1 || !hasTileSelected);

                EditorGUILayout.BeginHorizontal();

                // if the selected tile has a scene transition change it rather than add a new one
                if (Selection.gameObjects.Length == 1 && Selection.gameObjects[0].GetComponentInChildren<SceneTransitionPoint>())
                {
                    if (ColouredButton("Change Transition", orangeColour))
                    {
                        SceneTransitionPoint transition = Selection.gameObjects[0].GetComponentInChildren<SceneTransitionPoint>();
                        transition.NextLevelIndex = loadLevel;
                    }
                }
                else
                {
                    if (ColouredButton("Add Transition", greenColour))
                    {
                        Hex tile = Selection.gameObjects[0].GetComponent<Hex>();

                        GameObject transitionGO = new GameObject("SceneTransition");

                        SceneTransitionPoint transition = transitionGO.AddComponent<SceneTransitionPoint>();
                        transition.NextLevelIndex = loadLevel;
                        transition.drawText = true;

                        BoxCollider trigger = transitionGO.AddComponent<BoxCollider>();
                        trigger.isTrigger = true;

                        transition.transform.parent = tile.transform;
                        transition.transform.position = tile.transform.position;
                    }
                }

                GUILayout.Label("Load Level :");

                loadLevel = EditorGUILayout.IntField(loadLevel);

                EditorGUILayout.EndHorizontal();

                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginDisabledGroup(!hasTransitionSelected || !hasTileSelected);

                if (ColouredButton("Remove Scene Transition", redColour))
                {
                    GameObject[] selectedObjects = Selection.gameObjects;

                    SceneTransitionPoint transition = selectedObjects[0].GetComponentInChildren<SceneTransitionPoint>();

                    DestroyImmediate(transition.gameObject);
                }

                EditorGUI.EndDisabledGroup();

                if (ColouredButton("Remove All Scene Transitions", redColour))
                {
                    SceneTransitionPoint[] transitions = grid.GetComponentsInChildren<SceneTransitionPoint>();

                    foreach (SceneTransitionPoint transition in transitions)
                    {
                        DestroyImmediate(transition.gameObject);
                    }
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("ID:");

                currentID = EditorGUILayout.IntField(currentID);

                EditorGUILayout.EndHorizontal();

                #endregion
            }

            #endregion

            #region Dialogue

            else if (selectedTab == 2)
            {
                if (ColouredButton("New", greenColour))
                {
                    conversation = CreateInstance<Conversation>();
                    selectedSpeachIndex = 0;

                    loadedConversation = null;
                }

                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginDisabledGroup(loadedConversation == null);

                if (ColouredButton("Load", greenColour))
                {
                    conversation = loadedConversation;

                    selectedSpeachIndex = conversation.dialogue.Count - 1;
                    loadedConversation = null;
                }

                EditorGUI.EndDisabledGroup();

                loadedConversation = EditorGUILayout.ObjectField("", loadedConversation, typeof(Conversation), true) as Conversation;

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                BeginColouredVerticalBox(new Color(0.3f, 0.3f, 0.3f));

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Name");

                conversation.name = EditorGUILayout.TextField(conversation.name, skin.GetStyle("textfield"));

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                if (ColouredButton("Insert", greenColour))
                {
                    if (conversation.dialogue.Count == 0)
                    {
                        conversation.dialogue.Insert(selectedSpeachIndex, new Speach());
                        selectedSpeachIndex++;
                    }
                    else
                    {
                        if (selectedSpeachIndex >= conversation.dialogue.Count)
                        {
                            selectedSpeachIndex = conversation.dialogue.Count - 1;
                        }

                        conversation.dialogue.Insert(selectedSpeachIndex, new Speach());
                        selectedSpeachIndex++;
                    }

                    GUI.FocusControl("MyTextField");
                }

                selectedSpeachIndex = EditorGUILayout.IntField(selectedSpeachIndex, skin.GetStyle("textfield"), GUILayout.ExpandWidth(false));

                EditorGUI.BeginDisabledGroup(conversation.dialogue.Count == 0);

                if (ColouredButton("Remove", redColour))
                {
                    if (selectedSpeachIndex >= conversation.dialogue.Count)
                    {
                        selectedSpeachIndex = conversation.dialogue.Count - 1;
                    }

                    conversation.dialogue.RemoveAt(selectedSpeachIndex);
                }

                EditorGUI.EndDisabledGroup();

                if (selectedSpeachIndex < 0)
                {
                    selectedSpeachIndex = 0;
                }

                EditorGUILayout.EndHorizontal();

                scroll = EditorGUILayout.BeginScrollView(scroll);

                for (int i = 0; i < conversation.dialogue.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Label("" + i, centeredText);

                    EditorGUILayout.BeginVertical();

                    Color oldColor = GUI.backgroundColor;
                    GUI.backgroundColor = (conversation.dialogue[i].speaker == Speach.Speaker.Clade) ? brownColour : blueColour;

                    conversation.dialogue[i].speaker = (Speach.Speaker)EditorGUILayout.EnumPopup(conversation.dialogue[i].speaker);

                    GUI.backgroundColor = oldColor;

                    GUI.SetNextControlName("MyTextField");
                    conversation.dialogue[i].text = EditorGUILayout.TextArea(conversation.dialogue[i].text, GUILayout.Height(50));

                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();


                EditorGUILayout.EndVertical();

                if (confirmSave)
                {
                    BeginColouredVerticalBox(orangeColour);

                    GUILayout.Label("Save over file?", centeredText);

                    EditorGUILayout.BeginHorizontal();

                    if (ColouredButton("Yes!", greenColour))
                    {
                        // A Conversation with the same name doesn't exit
                        if (AssetDatabase.LoadAssetAtPath<Conversation>("Assets/Scripts/ScriptableObjects/Dialogue/" + conversation.name + ".asset") == null)
                        {
                            AssetDatabase.CreateAsset(conversation, "Assets/Scripts/ScriptableObjects/Dialogue/" + conversation.name + ".asset");
                        }

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        conversation = CreateInstance<Conversation>();
                        selectedSpeachIndex = 0;

                        confirmSave = false;
                    }

                    if (ColouredButton("No...", redColour))
                    {
                        confirmSave = false;
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();
                }
                else if (ColouredButton("Save Conversation", greenColour))
                {
                    if (AssetDatabase.LoadAssetAtPath<Conversation>("Assets/Scripts/ScriptableObjects/Dialogue/" + conversation.name + ".asset") != null)
                    {
                        confirmSave = true;
                    }
                    else
                    {
                        AssetDatabase.CreateAsset(conversation, "Assets/Scripts/ScriptableObjects/Dialogue/" + conversation.name + ".asset");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        conversation = CreateInstance<Conversation>();
                        selectedSpeachIndex = 0;
                    }
                }
            }

            #endregion

            #region Settings

            else if (selectedTab == 3)
            {
                healthyMode = EditorGUILayout.Toggle("Healthy Mode: ", healthyMode, skin.GetStyle("toggle"));
                toggleSnap = EditorGUILayout.Toggle("Toggle Grid Snap: ", toggleSnap, skin.GetStyle("toggle"));
            }

            #endregion

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical(); // outer box
        }

        /// End of OnGUI repaint scene and mark it as dirty.
        if (EditorGUI.EndChangeCheck())
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            SceneView.RepaintAll();
        }
    }

    public bool ColouredButton(string a_name, Color a_colour)
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = a_colour;

        bool output = GUILayout.Button(a_name);

        GUI.backgroundColor = oldColor;

        return output;
    }

    public void BeginColouredVerticalBox(Color a_colour)
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = a_colour;

        EditorGUILayout.BeginVertical("Box");

        GUI.backgroundColor = oldColor;
    }

    private void UpdateTriggerConnections()
    {
        SpawnTrigger[] triggers = FindObjectsOfType<SpawnTrigger>();

        foreach (SpawnTrigger trigger in triggers)
        {
            trigger.UpdateSpawnerList();
        }
    }

    private void UpdateSpawnerSelection()
    {
        selectedSpawners = Selection.GetFiltered<Spawner>(SelectionMode.Deep);

        hasSpawnerSelected = (selectedSpawners.Length > 0);

        spawnerButtonName = (selectedSpawners.Length > 1) ? "Spawners" : "Spawner";

        if (hasSpawnerSelected)
        {
            enemyType = selectedSpawners[0].SpawnType;
            turnToSpawn = selectedSpawners[0].TurnToSpawn;
            hasVolt = selectedSpawners[0].hasVolt;

            currentID = selectedSpawners[0].index;
        }
    }

    private void UpdateStateTransitionSelection()
    {
        selectedTransitionPoints = Selection.GetFiltered<StateTransitionPoint>(SelectionMode.Deep);

        hasStateTransitionSelected = (selectedTransitionPoints.Length > 0);

        if (hasStateTransitionSelected)
        {
            numberToKill = selectedTransitionPoints[0].numberToKill;
            currentState = selectedTransitionPoints[0].CurrentState;
            targetState = selectedTransitionPoints[0].TargetState;

            currentID = selectedTransitionPoints[0].index;

            voltGiven = selectedTransitionPoints[0].voltGiven;
        }
    }

    private void OnSelectionChange()
    {
        if (Selection.gameObjects.Length > 0)
        {
            hasTileSelected = (Selection.gameObjects[0].GetComponentsInChildren<Hex>().Length > 0);

            hasTransitionSelected = (Selection.gameObjects[0].GetComponentsInChildren<SceneTransitionPoint>().Length > 0);

            UpdateStateTransitionSelection();

            UpdateSpawnerSelection();

            hasTriggerSelected = (Selection.gameObjects[0].GetComponentsInChildren<SpawnTrigger>().Length > 0);

            Repaint();
        }
        else
        {
            hasTileSelected = false;
            hasTransitionSelected = false;
            hasStateTransitionSelected = false;
            hasSpawnerSelected = false;
            hasTriggerSelected = false;

            Repaint();
        }
    }

    private void Update()
    {
        if (toggleSnap
     && !EditorApplication.isPlaying
     && Selection.transforms.Length > 0
     && Selection.transforms[0].position != prevPosition)
        {
            Snap();
            prevPosition = Selection.transforms[0].position;
        }
    }

    private void OnInspectorUpdate()
    {
        if (grid == null)
        {
            GameObject gridGo = GameObject.FindGameObjectWithTag("Manager");

            if (gridGo)
            {
                grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridManager>();

                if (grid && grid.transform.childCount > 0)
                {
                    string[] mapSize = grid.transform.GetChild(grid.transform.childCount - 1).name.Split(',');

                    mapWidth = int.Parse(mapSize[0]) + 1;
                    mapHeight = int.Parse(mapSize[1]) + 1;
                }

                Repaint();
            }
        }
    }

    private void Snap()
    {
        foreach (var transform in Selection.transforms)
        {
            var t = transform.transform.position;
            t.x = Round(t.x);
            //t.y = Round(t.y);
            t.z = Round(t.z);
            transform.transform.position = t;
        }
    }

    private float Round(float input)
    {
        return 1 * Mathf.Round((input / 1));
    }

    private Color GetHexStateColour(HexState a_hexState)
    {
        switch (a_hexState)
        {
            case HexState.Untraversable:
                return inaccessibleColour;
            case HexState.Traversable:
                return traversableColour;
            case HexState.OutOfBounds:
                return outOfBoundsColour;
            default:
                return Color.black;
        }
    }

}