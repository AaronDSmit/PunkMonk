using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.AnimatedValues;
#endif

public enum Unit_type { runner, watcher }

public class EasyDesignEditor : EditorWindow
{
    #region Variables

    // general variables
    [SerializeField] private static EditorWindow window;
    [SerializeField] private static Texture icon;
    [SerializeField] private GUISkin skin;
    [SerializeField] private GUIStyle centeredText;
    [SerializeField] private static int selectedTab = 0;
    [SerializeField] private static GridManager grid;
    [SerializeField] private static float HexScale = 1;

    // GridManager Generation
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    [SerializeField] private static bool confirmMapGeneration = false;
    [SerializeField] private static bool mapSizeSet;

    // Navigation
    [SerializeField] private bool hasTileSelected = false;
    [SerializeField] private bool hasTransitionSelected = false;
    [SerializeField] private bool hasStateTransitionSelected = false;
    [SerializeField] private bool hasSpawnerSelected = false;
    [SerializeField] private bool hasTriggerSelected = false;

    [SerializeField] private bool setAllNodesTraversable = false;
    [SerializeField] private bool setAllNodesInaccessible = false;

    [SerializeField] private bool autoAllNodesTraversable = false;
    [SerializeField] private bool autoAllNodesInaccessible = false;

    [SerializeField] private bool fillNodesTraversable = false;
    [SerializeField] private bool fillNodesInaccessible = false;

    [SerializeField] private Color traversableColour;
    [SerializeField] private Color inaccessibleColour;
    [SerializeField] private Color connectionColour;
    [SerializeField] private float inaccessibleAlpha;

    // GameFlow
    [SerializeField] private Unit_type enemyType;
    [SerializeField] private bool hasVolt = false;
    [SerializeField] private int turnToSpawn;
    [SerializeField] private int everyXTurns;
    [SerializeField] private int loadLevel = 0;
    [SerializeField] private GameState targetState = GameState.overworld;
    [SerializeField] private GameState currentState = GameState.battle;
    [SerializeField] private int numberToKill;
    [SerializeField] private int stateIndex;
    [SerializeField] private int spawnIndex;

    // Settings
    [SerializeField] private bool healthyMode = true;

    #endregion

    private void OnEnable()
    {
        // load custom skin and window icon
        skin = (GUISkin)Resources.Load("EditorSkin");

        centeredText = skin.GetStyle("CenteredText");

        if (window != null)
        {
            window.titleContent = new GUIContent("Easy Design", icon);
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

        traversableColour = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        inaccessibleColour = new Color(1.0f, 0.0f, 0.0f, 0.35f);
        connectionColour = new Color(1.0f, 1.0f, 1.0f, 0.35f);

        inaccessibleAlpha = 0.4f;

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

            HexUtility.UpdateScale(HexScale);
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {

        }
    }

    private void OnGUI()
    {
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

        // If there's no grid manager then don't draw any of the inspector
        if (grid == null)
        {
            EditorGUILayout.HelpBox("No GridManager script found!", MessageType.Error);

            return;
        }

        // If in play mode then don't draw any of the inspector
        if (EditorApplication.isPlaying)
        {
            EditorGUILayout.HelpBox("You can't use this program during play mode", MessageType.Info);

            return;
        }

        GUI.skin = skin; // use a custom skin

        // Create toolbar using custom tab style
        string[] tabs = { "Grid", "GameFlow", "Settings" };
        selectedTab = GUILayout.Toolbar(selectedTab, tabs);

        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 1.0f); // grey

        EditorGUILayout.BeginVertical("Box"); // outer box

        GUI.backgroundColor = oldColor;

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

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.39f, 0.78f, 0.19f, 1.0f); // green

            if (!mapSizeSet)
            {
                EditorGUILayout.HelpBox("Axis's can't be 0", MessageType.Warning);
            }
            else if (confirmMapGeneration)
            {
                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1.0f, 0.58f, 0.19f, 1.0f); // orange

                EditorGUILayout.BeginVertical("Box");

                GUI.backgroundColor = oldColor;

                GUILayout.Label("Create New Map?", centeredText);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Yes!"))
                {
                    HexUtility.UpdateScale(HexScale);

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

                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1.0f, 0.19f, 0.19f, 1.0f); // red

                if (GUILayout.Button("No..."))
                {
                    confirmMapGeneration = false;
                }

                GUI.backgroundColor = oldColor;

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
            else if (GUILayout.Button("Generate Grid"))
            {
                confirmMapGeneration = true;
            }

            GUI.backgroundColor = oldColor;

            #endregion

            EditorGUILayout.Space();

            #region Set Selected Tiles To Traversable/Inaccessible

            GUILayout.Label("Set Selected Tiles To", centeredText);

            EditorGUI.BeginDisabledGroup(!hasTileSelected);

            EditorGUILayout.BeginHorizontal();

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.39f, 0.78f, 0.19f, 1.0f); // green

            if (GUILayout.Button("Traversable"))
            {
                GameObject[] selectedObjects = Selection.gameObjects;

                foreach (GameObject obj in selectedObjects)
                {
                    Hex[] tiles = obj.GetComponentsInChildren<Hex>();

                    foreach (Hex tile in tiles)
                    {
                        tile.SetTraversable(true, traversableColour);
                    }
                }
            }

            GUI.backgroundColor = oldColor;

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1.0f, 0.19f, 0.19f, 1.0f); // red

            if (GUILayout.Button("Inaccessible"))
            {
                GameObject[] selectedObjects = Selection.gameObjects;

                foreach (GameObject obj in selectedObjects)
                {
                    Hex[] tiles = obj.GetComponentsInChildren<Hex>();

                    foreach (Hex tile in tiles)
                    {
                        tile.SetTraversable(false, inaccessibleColour);
                    }
                }
            }

            GUI.backgroundColor = oldColor;

            EditorGUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();

            #endregion

            EditorGUILayout.Space();

            #region Set All Tiles To Traversable/Inaccessible

            GUILayout.Label("Set All Tiles To", centeredText);

            EditorGUILayout.BeginHorizontal();

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.39f, 0.78f, 0.19f, 1.0f); // green

            if (!setAllNodesInaccessible && setAllNodesTraversable)
            {
                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1.0f, 0.58f, 0.19f, 1.0f); // orange

                EditorGUILayout.BeginVertical("Box");

                GUI.backgroundColor = oldColor;

                GUILayout.Label("All Traversable?", centeredText);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Yes!"))
                {
                    Hex[] tiles = grid.GetComponentsInChildren<Hex>();

                    foreach (Hex tile in tiles)
                    {
                        tile.SetTraversable(true, traversableColour);
                    }

                    setAllNodesTraversable = false;
                }

                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1.0f, 0.19f, 0.19f, 1.0f); // red

                if (GUILayout.Button("No..."))
                {
                    setAllNodesTraversable = false;
                }

                GUI.backgroundColor = oldColor;

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
            else if (!setAllNodesInaccessible)
            {
                if (GUILayout.Button("Traversable"))
                {
                    setAllNodesTraversable = true;
                }
            }

            GUI.backgroundColor = oldColor;

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1.0f, 0.19f, 0.19f, 1.0f); // red

            if (!setAllNodesTraversable && setAllNodesInaccessible)
            {
                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1.0f, 0.58f, 0.19f, 1.0f); // orange

                EditorGUILayout.BeginVertical("Box");

                GUI.backgroundColor = oldColor;

                GUILayout.Label("All Inaccessible?", centeredText);

                EditorGUILayout.BeginHorizontal();

                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(0.39f, 0.78f, 0.19f, 1.0f); // green

                if (GUILayout.Button("yes!"))
                {
                    Hex[] tiles = grid.GetComponentsInChildren<Hex>();

                    foreach (Hex tile in tiles)
                    {
                        tile.SetTraversable(false, inaccessibleColour);
                    }

                    setAllNodesInaccessible = false;
                }

                GUI.backgroundColor = oldColor;

                if (GUILayout.Button("No..."))
                {
                    setAllNodesInaccessible = false;
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
            else if (!setAllNodesTraversable)
            {
                if (GUILayout.Button("Inaccessible"))
                {
                    setAllNodesInaccessible = true;
                }
            }

            GUI.backgroundColor = oldColor;

            EditorGUILayout.EndHorizontal();

            #endregion

            EditorGUILayout.Space();

            #region Auto Traversable/Inaccessible

            GUILayout.Label("Automatically Set Tiles To", centeredText);

            EditorGUILayout.BeginHorizontal();

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.39f, 0.78f, 0.19f, 1.0f); // green

            if (!autoAllNodesInaccessible && autoAllNodesTraversable)
            {
                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1.0f, 0.58f, 0.19f, 1.0f); // orange

                EditorGUILayout.BeginVertical("Box");

                GUI.backgroundColor = oldColor;

                GUILayout.Label("Automatically Traversable?", centeredText);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Yes!"))
                {
                    Hex[] tiles = grid.GetComponentsInChildren<Hex>();

                    foreach (Hex tile in tiles)
                    {
                        tile.TraversableCheck(traversableColour);
                    }

                    autoAllNodesTraversable = false;
                }

                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1.0f, 0.19f, 0.19f, 1.0f); // red

                if (GUILayout.Button("No..."))
                {
                    autoAllNodesTraversable = false;
                }

                GUI.backgroundColor = oldColor;

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
            else if (!autoAllNodesInaccessible)
            {
                if (GUILayout.Button("Traversable"))
                {
                    autoAllNodesTraversable = true;
                }
            }

            GUI.backgroundColor = oldColor;

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1.0f, 0.19f, 0.19f, 1.0f); // red

            if (!autoAllNodesTraversable && autoAllNodesInaccessible)
            {
                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1.0f, 0.58f, 0.19f, 1.0f); // orange

                EditorGUILayout.BeginVertical("Box");

                GUI.backgroundColor = oldColor;

                GUILayout.Label("Automatically Inaccessible?", centeredText);

                EditorGUILayout.BeginHorizontal();

                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(0.39f, 0.78f, 0.19f, 1.0f); // green

                if (GUILayout.Button("yes!"))
                {
                    Hex[] tiles = grid.GetComponentsInChildren<Hex>();

                    foreach (Hex tile in tiles)
                    {
                        tile.InaccessibleCheck(inaccessibleColour);
                    }

                    autoAllNodesInaccessible = false;
                }

                GUI.backgroundColor = oldColor;

                if (GUILayout.Button("No..."))
                {
                    autoAllNodesInaccessible = false;
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
            else if (!autoAllNodesTraversable)
            {
                if (GUILayout.Button("Inaccessible"))
                {
                    autoAllNodesInaccessible = true;
                }
            }

            GUI.backgroundColor = oldColor;

            EditorGUILayout.EndHorizontal();

            #endregion

            EditorGUILayout.Space();

            #region Fill Tiles Traversable/Inaccessible

            GUILayout.Label("Fill Area", centeredText);

            EditorGUI.BeginDisabledGroup(!hasTileSelected || Selection.gameObjects.Length != 1);

            EditorGUILayout.BeginHorizontal();

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.39f, 0.78f, 0.19f, 1.0f); // green

            if (!fillNodesInaccessible && fillNodesTraversable)
            {
                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1.0f, 0.58f, 0.19f, 1.0f); // orange

                EditorGUILayout.BeginVertical("Box");

                GUI.backgroundColor = oldColor;

                GUILayout.Label("Fil Traversable?", centeredText);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Yes!"))
                {
                    Hex hex = Selection.gameObjects[0].GetComponent<Hex>();
                    Hex currentHex;

                    List<Hex> openList = new List<Hex>();
                    List<Hex> closeList = new List<Hex>();

                    openList.Add(hex);

                    while (openList.Count > 0)
                    {
                        currentHex = openList[0];
                        openList.RemoveAt(0);
                        closeList.Add(currentHex);

                        currentHex.SetTraversable(true, traversableColour);

                        foreach (Hex neighbour in currentHex.Neighbours)
                        {
                            if (!neighbour.IsTraversable && !openList.Contains(neighbour) && !closeList.Contains(neighbour))
                            {
                                openList.Add(neighbour);
                            }
                        }
                    }

                    fillNodesTraversable = false;
                }

                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1.0f, 0.19f, 0.19f, 1.0f); // red

                if (GUILayout.Button("No..."))
                {
                    fillNodesTraversable = false;
                }

                GUI.backgroundColor = oldColor;

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
            else if (!fillNodesInaccessible)
            {
                if (GUILayout.Button("Traversable"))
                {
                    fillNodesTraversable = true;
                }
            }

            GUI.backgroundColor = oldColor;

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1.0f, 0.19f, 0.19f, 1.0f); // red

            if (!fillNodesTraversable && fillNodesInaccessible)
            {
                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1.0f, 0.58f, 0.19f, 1.0f); // orange

                EditorGUILayout.BeginVertical("Box");

                GUI.backgroundColor = oldColor;

                GUILayout.Label("Fill Inaccessible?", centeredText);

                EditorGUILayout.BeginHorizontal();

                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(0.39f, 0.78f, 0.19f, 1.0f); // green

                if (GUILayout.Button("yes!"))
                {
                    Hex hex = Selection.gameObjects[0].GetComponent<Hex>();
                    Hex currentHex;

                    List<Hex> openList = new List<Hex>();
                    List<Hex> closeList = new List<Hex>();

                    openList.Add(hex);

                    while (openList.Count > 0)
                    {
                        currentHex = openList[0];
                        openList.RemoveAt(0);
                        closeList.Add(currentHex);

                        currentHex.SetTraversable(false, inaccessibleColour);

                        foreach (Hex neighbour in currentHex.Neighbours)
                        {
                            if (neighbour.IsTraversable && !openList.Contains(neighbour) && !closeList.Contains(neighbour))
                            {
                                openList.Add(neighbour);
                            }
                        }
                    }

                    fillNodesInaccessible = false;
                }

                GUI.backgroundColor = oldColor;

                if (GUILayout.Button("No..."))
                {
                    fillNodesInaccessible = false;
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
            else if (!fillNodesTraversable)
            {
                if (GUILayout.Button("Inaccessible"))
                {
                    fillNodesInaccessible = true;
                }
            }

            GUI.backgroundColor = oldColor;

            EditorGUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();

            #endregion

            EditorGUILayout.Space();

            #region Assest Assist

            GUILayout.Label("Greybox Assist:", centeredText);

            EditorGUI.BeginDisabledGroup(Selection.gameObjects.Length == 0);

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.39f, 0.78f, 0.19f, 1.0f); // green

            if (GUILayout.Button("Snap Objects"))
            {
                foreach (Transform transform in Selection.GetTransforms(SelectionMode.TopLevel))
                {
                    Vector3 position = grid.GetHexFromPosition(transform.position).transform.position;

                    position.y = transform.position.y;
                    transform.position = position;
                }
            }

            if (GUILayout.Button("Select Hexes"))
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

            GUI.backgroundColor = oldColor;

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

            GUILayout.Label("Hex Scale:");
            HexScale = EditorGUILayout.FloatField(HexScale);

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

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.64f, 0.26f, 0.26f, 1.0f); // brown

            if (GameObject.FindGameObjectWithTag("EarthUnitSpawn") == null)
            {
                if (GUILayout.Button("Add Earth Spawn"))
                {
                    Hex tile = Selection.gameObjects[0].GetComponent<Hex>();

                    GameObject spawnerGO = new GameObject("Earth Spawner")
                    {
                        tag = "EarthUnitSpawn"
                    };

                    Spawner spawner = spawnerGO.AddComponent<Spawner>();
                    spawner.TextColour = new Color(1.0f, 0.2f, 0.2f, 1.0f); // brown
                    spawner.drawText = true;
                    spawner.index = -1;

                    spawner.transform.parent = tile.transform;
                    spawner.transform.position = tile.transform.position;
                    spawner.TurnToSpawn = 0;
                }
            }
            else
            {
                if (GUILayout.Button("Move Earth Spawn"))
                {
                    Hex tile = Selection.gameObjects[0].GetComponent<Hex>();
                    Spawner spawner = GameObject.FindGameObjectWithTag("EarthUnitSpawn").GetComponent<Spawner>();

                    spawner.transform.parent = tile.transform;
                    spawner.transform.position = tile.transform.position;
                    spawner.TurnToSpawn = 0;
                }
            }

            GUI.backgroundColor = oldColor;

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.39f, 0.39f, 0.78f, 1.0f); // blue

            if (GameObject.FindGameObjectWithTag("LightningUnitSpawn") == null)
            {
                if (GUILayout.Button("Add Lightning spawn"))
                {
                    Hex tile = Selection.gameObjects[0].GetComponent<Hex>();

                    GameObject spawnerGO = new GameObject("Lightning Spawner")
                    {
                        tag = "LightningUnitSpawn"
                    };

                    Spawner spawner = spawnerGO.AddComponent<Spawner>();
                    spawner.TextColour = new Color(0.0f, 0.0f, 1.0f, 1.0f); // blue
                    spawner.drawText = true;
                    spawner.index = -1;

                    spawner.transform.parent = tile.transform;
                    spawner.transform.position = tile.transform.position;
                    spawner.TurnToSpawn = 0;
                }
            }
            else
            {
                if (GUILayout.Button("Move Lightning Spawn"))
                {
                    Hex tile = Selection.gameObjects[0].GetComponent<Hex>();
                    Spawner spawner = GameObject.FindGameObjectWithTag("LightningUnitSpawn").GetComponent<Spawner>();

                    spawner.transform.parent = tile.transform;
                    spawner.transform.position = tile.transform.position;
                    spawner.TurnToSpawn = 0;
                }
            }

            GUI.backgroundColor = oldColor;

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();

            #endregion

            EditorGUILayout.Space();

            #region Enemy Spawn Points

            GUILayout.Label("Enemy Spawn Points", centeredText);

            EditorGUI.BeginDisabledGroup(Selection.gameObjects.Length != 1 || !hasTileSelected);

            EditorGUILayout.BeginHorizontal();

            // if the selected tile has a Spawner change it rather than add a new one
            if (Selection.gameObjects.Length == 1 && Selection.gameObjects[0].GetComponentInChildren<Spawner>())
            {
                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1.0f, 0.58f, 0.19f, 1.0f); // orange

                if (GUILayout.Button("Change Spawner"))
                {
                    Spawner spawner = Selection.gameObjects[0].GetComponentInChildren<Spawner>();
                    spawner.index = spawnIndex;

                    if (enemyType == Unit_type.watcher)
                    {
                        spawner.UntiToSpawn = Resources.Load<AI_Agent>("EnemyCharacters/WatcherUnit");
                        spawner.TextColour = Color.red;
                    }
                    else if (enemyType == Unit_type.runner)
                    {
                        spawner.UntiToSpawn = Resources.Load<AI_Agent>("EnemyCharacters/RunnerUnit");
                        spawner.TextColour = Color.red;
                    }

                    spawner.TurnToSpawn = turnToSpawn;

                    SpawnTrigger[] triggers = FindObjectsOfType<SpawnTrigger>();

                    foreach (SpawnTrigger trigger in triggers)
                    {
                        trigger.UpdateSpawnerList();
                    }
                }

                GUI.backgroundColor = oldColor;
            }
            else
            {
                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(0.39f, 0.78f, 0.19f, 1.0f); // green

                if (GUILayout.Button("Add Spawner"))
                {
                    Hex tile = Selection.gameObjects[0].GetComponent<Hex>();

                    GameObject spawnerGO = new GameObject("EnemySpawner");
                    Spawner spawner = spawnerGO.AddComponent<Spawner>();
                    spawner.drawText = true;
                    spawner.currentHex = tile;
                    spawner.index = spawnIndex;

                    if (enemyType == Unit_type.watcher)
                    {
                        spawner.UntiToSpawn = Resources.Load<Unit>("EnemyCharacters/WatcherUnit");
                        spawner.TextColour = Color.red;
                    }
                    else if (enemyType == Unit_type.runner)
                    {
                        spawner.UntiToSpawn = Resources.Load<Unit>("EnemyCharacters/RunnerUnit");
                        spawner.TextColour = Color.red;
                    }

                    spawner.transform.parent = tile.transform;
                    spawner.transform.position = tile.transform.position;
                    spawner.transform.localEulerAngles = new Vector3(180, 0.0f, 0.0f);
                    spawner.TurnToSpawn = turnToSpawn;

                    SpawnTrigger[] triggers = FindObjectsOfType<SpawnTrigger>();

                    foreach (SpawnTrigger trigger in triggers)
                    {
                        trigger.UpdateSpawnerList();
                    }
                }

                GUI.backgroundColor = oldColor;
            }

            EditorGUI.EndDisabledGroup();

            GUILayout.Label("Turn:");

            turnToSpawn = EditorGUILayout.IntField(turnToSpawn);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Enemy: ");

            enemyType = (Unit_type)EditorGUILayout.EnumPopup(enemyType);

            GUILayout.Label("Has volt: ");

            hasVolt = EditorGUILayout.Toggle(hasVolt);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(!hasSpawnerSelected);

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1.0f, 0.19f, 0.19f, 1.0f); // red

            if (GUILayout.Button("Remove Spawner"))
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

                SpawnTrigger[] triggers = FindObjectsOfType<SpawnTrigger>();

                foreach (SpawnTrigger trigger in triggers)
                {
                    trigger.UpdateSpawnerList();
                }
            }

            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Remove All Spawner’s"))
            {
                Spawner[] spawnPoints = grid.GetComponentsInChildren<Spawner>();

                foreach (Spawner spawner in spawnPoints)
                {
                    if (spawner.name == "EnemySpawner")
                    {
                        DestroyImmediate(spawner.gameObject);
                    }
                }
            }

            GUI.backgroundColor = oldColor;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.Label("Spawn Triggers:", centeredText);

            EditorGUI.BeginDisabledGroup(Selection.gameObjects.Length != 1);

            EditorGUILayout.BeginHorizontal();

            // if the selected tile has a Spawner change it rather than add a new one
            if (Selection.gameObjects.Length == 1 && Selection.gameObjects[0].GetComponentInChildren<SpawnTrigger>())
            {
                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1.0f, 0.58f, 0.19f, 1.0f); // orange

                if (GUILayout.Button("Change Trigger"))
                {
                    SpawnTrigger spawnTrigger = Selection.gameObjects[0].GetComponentInChildren<SpawnTrigger>();
                    spawnTrigger.index = spawnIndex;
                    spawnTrigger.UpdateSpawnerList();
                }

                GUI.backgroundColor = oldColor;
            }
            else
            {
                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(0.39f, 0.78f, 0.19f, 1.0f); // green

                if (GUILayout.Button("Add Trigger"))
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

                GUI.backgroundColor = oldColor;
            }

            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(Selection.gameObjects.Length != 1 || !hasTriggerSelected);
             
            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1.0f, 0.19f, 0.19f, 1.0f); // red

            if (GUILayout.Button("Remove Trigger"))
            {
                Hex tile = Selection.gameObjects[0].GetComponent<Hex>();
                SpawnTrigger destroyedTrigger = tile.GetComponentInChildren<SpawnTrigger>();

                DestroyImmediate(destroyedTrigger);

                SpawnTrigger[] triggers = FindObjectsOfType<SpawnTrigger>();

                foreach (SpawnTrigger trigger in triggers)
                {
                    trigger.UpdateSpawnerList();
                }
            }

            GUI.backgroundColor = oldColor;

            EditorGUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();

            #endregion

            EditorGUILayout.Space();

            GUILayout.Label("Scene Transition:", centeredText);

            EditorGUI.BeginDisabledGroup(Selection.gameObjects.Length != 1 || !hasTileSelected);

            EditorGUILayout.BeginHorizontal();

            // if the selected tile has a scene transition change it rather than add a new one
            if (Selection.gameObjects.Length == 1 && Selection.gameObjects[0].GetComponentInChildren<SceneTransitionPoint>())
            {
                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1.0f, 0.58f, 0.19f, 1.0f); // orange

                if (GUILayout.Button("Change Transition"))
                {
                    SceneTransitionPoint transition = Selection.gameObjects[0].GetComponentInChildren<SceneTransitionPoint>();
                    transition.NextLevelIndex = loadLevel;
                }

                GUI.backgroundColor = oldColor;
            }
            else
            {
                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(0.39f, 0.78f, 0.19f, 1.0f); // green

                if (GUILayout.Button("Add Transition"))
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

                GUI.backgroundColor = oldColor;
            }

            GUILayout.Label("Load Level :");

            loadLevel = EditorGUILayout.IntField(loadLevel);

            EditorGUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(!hasTransitionSelected || !hasTileSelected);

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1.0f, 0.19f, 0.19f, 1.0f); // red

            if (GUILayout.Button("Remove Scene Transition"))
            {
                GameObject[] selectedObjects = Selection.gameObjects;

                SceneTransitionPoint transition = selectedObjects[0].GetComponentInChildren<SceneTransitionPoint>();

                DestroyImmediate(transition.gameObject);
            }

            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Remove All Scene Transitions"))
            {
                SceneTransitionPoint[] transitions = grid.GetComponentsInChildren<SceneTransitionPoint>();

                foreach (SceneTransitionPoint transition in transitions)
                {
                    DestroyImmediate(transition.gameObject);
                }
            }

            GUI.backgroundColor = oldColor;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.Label("State Transition:", centeredText);

            EditorGUI.BeginDisabledGroup(Selection.gameObjects.Length != 1 || !hasTileSelected);

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("From ");

            currentState = (GameState)EditorGUILayout.EnumPopup(currentState);

            GUILayout.Label(" To ");

            targetState = (GameState)EditorGUILayout.EnumPopup(targetState);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            if (targetState == GameState.battle)
            {
                GUILayout.Label("Enemies to kill ");
                numberToKill = EditorGUILayout.IntField(numberToKill);
            }

            // if the selected tile has a scene transition change it rather than add a new one
            if (Selection.gameObjects.Length == 1 && Selection.gameObjects[0].GetComponentInChildren<StateTransitionPoint>())
            {
                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1.0f, 0.58f, 0.19f, 1.0f); // orange

                if (GUILayout.Button("Change Transition"))
                {
                    StateTransitionPoint sceneTransition = Selection.gameObjects[0].GetComponentInChildren<StateTransitionPoint>();
                    sceneTransition.TargetState = targetState;
                    sceneTransition.CurrentState = currentState;
                    sceneTransition.numberToKill = numberToKill;

                    sceneTransition.index = stateIndex;
                }

                GUI.backgroundColor = oldColor;
            }
            else
            {
                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(0.39f, 0.78f, 0.19f, 1.0f); // green

                if (GUILayout.Button("Add Transition"))
                {
                    Hex tile = Selection.gameObjects[0].GetComponent<Hex>();

                    GameObject transitionGO = new GameObject("stateTransition");

                    StateTransitionPoint sceneTransition = transitionGO.AddComponent<StateTransitionPoint>();
                    sceneTransition.TargetState = targetState;
                    sceneTransition.CurrentState = currentState;
                    sceneTransition.numberToKill = numberToKill;

                    sceneTransition.drawText = true;
                    sceneTransition.index = stateIndex;

                    BoxCollider trigger = transitionGO.AddComponent<BoxCollider>();
                    trigger.isTrigger = true;

                    sceneTransition.transform.parent = tile.transform;
                    sceneTransition.transform.position = tile.transform.position;
                }

                GUI.backgroundColor = oldColor;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.64f, 0.26f, 0.26f, 1.0f); // brown

            if (GUILayout.Button("Earth Hex"))
            {
                Hex tile = Selection.gameObjects[0].GetComponent<Hex>();

                StateTransitionPoint[] transitions = FindObjectsOfType<StateTransitionPoint>();

                foreach (StateTransitionPoint point in transitions)
                {
                    if (point.index == stateIndex)
                    {
                        point.SetEarthHex(tile);
                    }
                }
            }

            GUI.backgroundColor = oldColor;

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.39f, 0.39f, 0.78f, 1.0f); // blue

            if (GUILayout.Button("Lightning Hex"))
            {
                Hex tile = Selection.gameObjects[0].GetComponent<Hex>();

                StateTransitionPoint[] transitions = FindObjectsOfType<StateTransitionPoint>();

                foreach (StateTransitionPoint point in transitions)
                {
                    if (point.index == stateIndex)
                    {
                        point.SetLightninghHex(tile);
                    }
                }
            }

            GUI.backgroundColor = oldColor;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.BeginHorizontal();

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1.0f, 0.19f, 0.19f, 1.0f); // red

            EditorGUI.BeginDisabledGroup(!hasStateTransitionSelected || !hasTileSelected);

            if (GUILayout.Button("Remove State Transition"))
            {
                GameObject[] selectedObjects = Selection.gameObjects;

                StateTransitionPoint transition = selectedObjects[0].GetComponentInChildren<StateTransitionPoint>();

                DestroyImmediate(transition.gameObject);
            }

            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Remove All State Transitions"))
            {
                StateTransitionPoint[] transitions = grid.GetComponentsInChildren<StateTransitionPoint>();

                foreach (StateTransitionPoint transition in transitions)
                {
                    DestroyImmediate(transition.gameObject);
                }
            }

            GUI.backgroundColor = oldColor;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("ID:");

            stateIndex = spawnIndex = EditorGUILayout.IntField(stateIndex);

            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region Settings

        else if (selectedTab == 2)
        {
            healthyMode = EditorGUILayout.Toggle("Healthy Mode: ", healthyMode);
        }

        #endregion

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical(); // outer box

        /// End of OnGUI repaint scene and mark it as dirty.
        if (EditorGUI.EndChangeCheck())
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            SceneView.RepaintAll();
        }
    }

    private void OnInspectorUpdate()
    {
        if (Selection.gameObjects.Length > 0)
        {
            hasTileSelected = (Selection.gameObjects[0].GetComponentsInChildren<Hex>().Length > 0);

            hasTransitionSelected = (Selection.gameObjects[0].GetComponentsInChildren<SceneTransitionPoint>().Length > 0);

            hasStateTransitionSelected = (Selection.gameObjects[0].GetComponentsInChildren<StateTransitionPoint>().Length > 0);

            hasSpawnerSelected = (Selection.gameObjects[0].GetComponentsInChildren<Spawner>().Length > 0);

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
    }
}