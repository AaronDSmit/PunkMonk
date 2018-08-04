using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.AnimatedValues;
#endif


public enum Unit_type { grunt, range }

public class EasyDesignEditor : EditorWindow
{
    #region Variables

    // general variables
    [SerializeField] private static EditorWindow window;
    [SerializeField] private static Texture icon;
    [SerializeField] private GUISkin skin;
    [SerializeField] private GUIStyle centeredText;
    [SerializeField] private static int selectedTab = 0;
    [SerializeField] private static int previousSelectedTab = -1;
    [SerializeField] private static GridManager grid;

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
    [SerializeField] private bool setAllNodesTraversable = false;
    [SerializeField] private bool setAllNodesInaccessible = false;

    [SerializeField] private Color traversableColour;
    [SerializeField] private Color inaccessibleColour;
    [SerializeField] private Color connectionColour;

    [SerializeField] private Color oldWalkableColour;
    [SerializeField] private Color oldNotWalkableColour;
    [SerializeField] private Color oldConnectionColour;

    // GameFlow
    [SerializeField] private Unit_type enemyType;
    [SerializeField] private int turnToSpawn;
    [SerializeField] private int everyXTurns;
    [SerializeField] private int loadLevel = 0;
    [SerializeField] private Game_state state = Game_state.battle;
    [SerializeField] private int stateIndex;

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
    }

    [MenuItem("Window/EasyDesign")]
    public static void ShowWindow()
    {
        window = GetWindow(typeof(EasyDesignEditor), false, "Easy Design");

        icon = (Texture)Resources.Load("save_icon");
        window.titleContent = new GUIContent("Easy Design", icon);

        EditorApplication.playModeStateChanged += PlayModeChanged;
    }

    private static void PlayModeChanged(PlayModeStateChange state)
    {
        Debug.Log(state);
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

        // Show nodes and connections on first frame of selecting tab
        if (selectedTab == 0 && previousSelectedTab != 0)
        {
            previousSelectedTab = selectedTab;


        }
        else if (selectedTab != 0 && previousSelectedTab == 0)
        {
            previousSelectedTab = selectedTab;


        }

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
            GUILayout.Label("Grid Generation:", centeredText);

            mapWidth = EditorGUILayout.IntField("X-Axis: ", mapWidth);
            mapHeight = EditorGUILayout.IntField("Z-Axis", mapHeight);

            mapSizeSet = (mapWidth != 0 && mapHeight != 0);

            // Check if grid has been generated.
            bool mapGenererated = grid.transform.childCount > 0;

            if (!confirmMapGeneration && mapGenererated)
            {
                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1.0f, 0.58f, 0.19f, 1.0f); // orange

                if (GUILayout.Button("Unlock GridManager Generation"))
                {
                    confirmMapGeneration = true;
                }

                GUI.backgroundColor = oldColor;
            }

            if (confirmMapGeneration || !mapGenererated)
            {
                if (mapSizeSet)
                {
                    oldColor = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(0.39f, 0.78f, 0.19f, 1.0f); // green

                    if (GUILayout.Button("Generate GridManager"))
                    {
                        if (grid.GenerateGrid(mapWidth, mapHeight, traversableColour, inaccessibleColour, connectionColour))
                        {
                            confirmMapGeneration = false;
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("There was an error generating the grid.", MessageType.Error);
                        }
                    }

                    GUI.backgroundColor = oldColor;
                }
                else
                {
                    EditorGUILayout.HelpBox("GridManager width has not been set", MessageType.Warning);
                }
            }

            EditorGUILayout.Space();

            GUILayout.Label("Navigation:", centeredText);

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.39f, 0.78f, 0.19f, 1.0f); // green

            if (setAllNodesTraversable)
            {
                GUILayout.Label("Set ALL nodes Traversable?", centeredText);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Yes"))
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

                if (GUILayout.Button("No"))
                {
                    setAllNodesTraversable = false;
                }

                GUI.backgroundColor = oldColor;

                EditorGUILayout.EndHorizontal();
            }
            else if (GUILayout.Button("Set all Nodes Traversable"))
            {
                setAllNodesTraversable = true;
            }

            EditorGUI.BeginDisabledGroup(!hasTileSelected);

            if (GUILayout.Button("Set Selected Tiles Traversable"))
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

            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Auto Traversable Tiles"))
            {
                Hex[] tiles = grid.GetComponentsInChildren<Hex>();

                foreach (Hex tile in tiles)
                {
                    tile.TraversableCheck(traversableColour);
                }
            }

            GUI.backgroundColor = oldColor;

            EditorGUILayout.Space();

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1.0f, 0.19f, 0.19f, 1.0f); // red

            if (setAllNodesInaccessible)
            {
                GUILayout.Label("Set ALL nodes Inaccessible?", centeredText);

                EditorGUILayout.BeginHorizontal();

                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(0.39f, 0.78f, 0.19f, 1.0f); // green

                if (GUILayout.Button("yes"))
                {
                    Hex[] tiles = grid.GetComponentsInChildren<Hex>();

                    foreach (Hex tile in tiles)
                    {
                        tile.SetTraversable(false, inaccessibleColour);
                    }

                    setAllNodesInaccessible = false;
                }

                GUI.backgroundColor = oldColor;

                if (GUILayout.Button("No"))
                {
                    setAllNodesInaccessible = false;
                }

                EditorGUILayout.EndHorizontal();
            }
            else if (GUILayout.Button("Set all Nodes Inaccessible"))
            {
                setAllNodesInaccessible = true;
            }

            EditorGUI.BeginDisabledGroup(!hasTileSelected);

            if (GUILayout.Button("Set Selected Tiles Inaccessible"))
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

            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Auto Inaccessible Tiles"))
            {
                Hex[] tiles = grid.GetComponentsInChildren<Hex>();

                foreach (Hex tile in tiles)
                {
                    tile.InaccessibleCheck(inaccessibleColour);
                }
            }

            GUI.backgroundColor = oldColor;

            EditorGUILayout.Space();

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1.0f, 0.19f, 0.19f, 1.0f); // red

            GUI.backgroundColor = oldColor;

            EditorGUILayout.Space();

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
                    hexSelection[i++] = transform.parent.gameObject;
                }

                Selection.objects = hexSelection;
            }

            GUI.backgroundColor = oldColor;

            EditorGUI.EndDisabledGroup();

            GUILayout.Label("Visual Settings:", centeredText);

            traversableColour = EditorGUILayout.ColorField("Can walk", traversableColour);
            inaccessibleColour = EditorGUILayout.ColorField("Can't walk", inaccessibleColour);
            connectionColour = EditorGUILayout.ColorField("Connection", connectionColour);

            bool isDirty = (oldWalkableColour != traversableColour) || (oldNotWalkableColour != inaccessibleColour) || (oldConnectionColour != connectionColour);

            if (isDirty)
            {
                oldWalkableColour = traversableColour;
                oldNotWalkableColour = inaccessibleColour;
                oldConnectionColour = connectionColour;

                ApplyVisualSettingsToTiles();
            }
        }

        #endregion

        #region GameFlow

        else if (selectedTab == 1)
        {
            #region Player Spawn points 

            GUILayout.Label("Player Spawn Points", centeredText);

            EditorGUI.BeginDisabledGroup(!hasTileSelected || hasSpawnerSelected);

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.64f, 0.26f, 0.26f, 1.0f); // brown

            if (GameObject.FindGameObjectWithTag("EarthUnitSpawn") == null)
            {
                if (GUILayout.Button("Add Earth spawner"))
                {
                    Hex tile = Selection.gameObjects[0].GetComponent<Hex>();

                    GameObject spawnerGO = new GameObject("Earth Spawner")
                    {
                        tag = "EarthUnitSpawn"
                    };

                    Spawner spawner = spawnerGO.AddComponent<Spawner>();
                    spawner.TextColour = new Color(1.0f, 0.2f, 0.2f, 1.0f); // brown
                    spawner.drawText = true;

                    spawner.transform.parent = tile.transform;
                    spawner.transform.position = tile.transform.position;
                    spawner.TurnToSpawn = 0;
                }
            }
            else
            {
                if (GUILayout.Button("Move Earth spawner"))
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
                if (GUILayout.Button("Add Lightning spawner"))
                {
                    Hex tile = Selection.gameObjects[0].GetComponent<Hex>();

                    GameObject spawnerGO = new GameObject("Lightning Spawner")
                    {
                        tag = "LightningUnitSpawn"
                    };

                    Spawner spawner = spawnerGO.AddComponent<Spawner>();
                    spawner.TextColour = new Color(0.0f, 0.0f, 1.0f, 1.0f); // blue
                    spawner.drawText = true;

                    spawner.transform.parent = tile.transform;
                    spawner.transform.position = tile.transform.position;
                    spawner.TurnToSpawn = 0;
                }
            }
            else
            {
                if (GUILayout.Button("Move Lightning spawner"))
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

            #endregion

            #region Enemy Spawn Points

            EditorGUILayout.Space();

            GUILayout.Label("Enemy Spawn Points", centeredText);

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Spawn a ", GUILayout.ExpandWidth(false));

            enemyType = (Unit_type)EditorGUILayout.EnumPopup(enemyType, GUILayout.Width(60));


            GUILayout.Label(" on turn ", GUILayout.ExpandWidth(false));

            turnToSpawn = EditorGUILayout.IntField(turnToSpawn, GUILayout.Width(25));

            GUILayout.Label(" and repeat every ", GUILayout.ExpandWidth(false));

            everyXTurns = EditorGUILayout.IntField(everyXTurns, GUILayout.Width(25));

            EditorGUILayout.EndHorizontal();

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.39f, 0.78f, 0.19f, 1.0f); // green

            if (GUILayout.Button("Add Spawner To Selected Tiles"))
            {
                GameObject[] selectedObjects = Selection.gameObjects;

                foreach (GameObject obj in selectedObjects)
                {
                    Hex[] tiles = obj.GetComponentsInChildren<Hex>();

                    foreach (Hex tile in tiles)
                    {
                        GameObject spawnerGO = new GameObject("Enemy Spawner");
                        Spawner spawner = spawnerGO.AddComponent<Spawner>();
                        spawner.drawText = true;

                        spawner.transform.parent = tile.transform;
                        spawner.transform.position = tile.transform.position;
                        spawner.transform.localEulerAngles = new Vector3(180, 0.0f, 0.0f);
                        spawner.TurnToSpawn = turnToSpawn;
                    }
                }
            }

            GUI.backgroundColor = oldColor;

            EditorGUI.BeginDisabledGroup(!hasSpawnerSelected);

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1.0f, 0.19f, 0.19f, 1.0f); // red

            if (GUILayout.Button("Remove Spawner From Selected Tiles"))
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
            }

            if (GUILayout.Button("Remove All Spawners"))
            {
                Spawner[] spawnPoints = grid.GetComponentsInChildren<Spawner>();

                foreach (Spawner spawner in spawnPoints)
                {
                    DestroyImmediate(spawner.gameObject);
                }
            }

            GUI.backgroundColor = oldColor;

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

                    SphereCollider trigger = transitionGO.AddComponent<SphereCollider>();
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

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1.0f, 0.19f, 0.19f, 1.0f); // red

            EditorGUI.BeginDisabledGroup(!hasTransitionSelected || !hasTileSelected);

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

            EditorGUILayout.Space();

            GUILayout.Label("State Transition:", centeredText);

            EditorGUI.BeginDisabledGroup(Selection.gameObjects.Length != 1 || !hasTileSelected);

            EditorGUILayout.BeginHorizontal();

            // if the selected tile has a scene transition change it rather than add a new one
            if (Selection.gameObjects.Length == 1 && Selection.gameObjects[0].GetComponentInChildren<StateTransitionPoint>())
            {
                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1.0f, 0.58f, 0.19f, 1.0f); // orange

                if (GUILayout.Button("Change Transition"))
                {
                    StateTransitionPoint sceneTransition = Selection.gameObjects[0].GetComponentInChildren<StateTransitionPoint>();
                    sceneTransition.TargetState = state;
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
                    sceneTransition.TargetState = state;
                    sceneTransition.drawText = true;
                    sceneTransition.index = stateIndex;

                    SphereCollider trigger = transitionGO.AddComponent<SphereCollider>();
                    trigger.isTrigger = true;
                    trigger.radius = 0.8f;

                    sceneTransition.transform.parent = tile.transform;
                    sceneTransition.transform.position = tile.transform.position;
                }

                GUI.backgroundColor = oldColor;
            }

            GUILayout.Label("State:");

            state = (Game_state)EditorGUILayout.EnumPopup(state);

            GUILayout.Label("ID:");

            stateIndex = EditorGUILayout.IntField(stateIndex);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.64f, 0.26f, 0.26f, 1.0f); // brown

            if (GUILayout.Button("Earth Hex After Transition"))
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

            stateIndex = EditorGUILayout.IntField(stateIndex);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.39f, 0.39f, 0.78f, 1.0f); // blue

            if (GUILayout.Button("Lightning Hex After Transition"))
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

            stateIndex = EditorGUILayout.IntField(stateIndex);

            EditorGUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();

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

            Repaint();
        }
        else
        {
            hasTileSelected = false;
            hasTransitionSelected = false;
            hasStateTransitionSelected = false;
            hasSpawnerSelected = false;
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

    private void ApplyVisualSettingsToTiles()
    {
        Hex[] tiles = grid.GetComponentsInChildren<Hex>();

        foreach (Hex tile in tiles)
        {
            // tile.walkableColour = traversableColour;
            // tile.notWalkableColour = blockedColour;
            // tile.connectionColour = connectionColour;
        }
    }
}