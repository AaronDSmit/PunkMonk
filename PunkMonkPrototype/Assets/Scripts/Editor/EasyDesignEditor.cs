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
    [SerializeField] private static bool showInspector;
    [SerializeField] private static GridManager grid;

    // GridManager Generation
    [SerializeField] private static int mapWidth;
    [SerializeField] private static int mapHeight;
    [SerializeField] private static bool confirmMapGeneration = false;
    [SerializeField] private static bool mapSizeSet;

    // Navigation
    [SerializeField] private bool hasTileSelected = false;
    [SerializeField] private bool hasTransitionSelected = false;
    [SerializeField] private bool hasStateTransitionSelected = false;
    [SerializeField] private bool hasSpawnerSelected = false;
    [SerializeField] private bool setAllNodesClear = false;
    [SerializeField] private bool setAllNodesBlocked = false;

    [SerializeField] private static bool mapHasNodes = false;

    [SerializeField] private Color walkableColour;
    [SerializeField] private Color notWalkableColour;
    [SerializeField] private Color connectionColour;

    [SerializeField] private Color oldWalkableColour;
    [SerializeField] private Color oldNotWalkableColour;
    [SerializeField] private Color oldConnectionColour;

    // Tile settings
    [SerializeField] private Status tileStatus;
    [SerializeField] private float stepSize = 1;
    [SerializeField] private int step;

    // GameFlow
    [SerializeField] private Unit_type enemyType;
    [SerializeField] private int turnToSpawn;
    [SerializeField] private int everyXTurns;
    [SerializeField] private int loadLevel = 0;
    [SerializeField] private Game_state state = Game_state.battle;

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
    }

    [MenuItem("Window/EasyDesign")]
    public static void ShowWindow()
    {
        window = GetWindow(typeof(EasyDesignEditor), false, "Easy Design");

        icon = (Texture)Resources.Load("save_icon");
        window.titleContent = new GUIContent("Easy Design", icon);
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();

        showInspector = true;

        // Only show compiling message while Compiling
        if (EditorApplication.isCompiling)
        {
            EditorGUILayout.HelpBox("Compiling", MessageType.Info);
            showInspector = false;
        }


        // disable inspector if working for more than 5 hours
        if (healthyMode && EditorApplication.timeSinceStartup > 18000)
        {
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("You have been working for 5 hours, Please close Unity and take a break", MessageType.Error);
            showInspector = false;
        }

        // Draw Inspector
        if (showInspector)
        {
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

            GUI.skin = skin; // use our custom skin

            // Create toolbar using custom tab style
            string[] tabs = { "Grid", "GameFlow", "Settings" };
            selectedTab = GUILayout.Toolbar(selectedTab, tabs);

            mapHasNodes = grid.GetComponentsInChildren<Tile>().Length > 0;

            // Show nodes and connections on first frame of selecting tab
            if (selectedTab == 0 && previousSelectedTab != 0)
            {
                previousSelectedTab = selectedTab;

                Tile[] tiles = grid.GetComponentsInChildren<Tile>();

                foreach (Tile tile in tiles)
                {
                    tile.drawNode = true;
                    tile.drawConnections = true;

                    if (tile.GetComponentInChildren<Spawner>())
                    {
                        tile.GetComponentInChildren<Spawner>().drawText = false;
                    }

                    if (tile.GetComponentInChildren<SceneTransitionPoint>())
                    {
                        tile.GetComponentInChildren<SceneTransitionPoint>().drawText = false;
                    }

                    if (tile.GetComponentInChildren<StateTransitionPoint>())
                    {
                        tile.GetComponentInChildren<StateTransitionPoint>().drawText = false;
                    }
                }
            }
            else if (selectedTab != 0 && previousSelectedTab == 0)
            {
                previousSelectedTab = selectedTab;

                Tile[] tiles = grid.GetComponentsInChildren<Tile>();

                foreach (Tile tile in tiles)
                {
                    tile.drawNode = false;
                    tile.drawConnections = false;

                    if (tile.GetComponentInChildren<Spawner>())
                    {
                        tile.GetComponentInChildren<Spawner>().drawText = true;
                    }

                    if (tile.GetComponentInChildren<SceneTransitionPoint>())
                    {
                        tile.GetComponentInChildren<SceneTransitionPoint>().drawText = true;
                    }

                    if (tile.GetComponentInChildren<StateTransitionPoint>())
                    {
                        tile.GetComponentInChildren<StateTransitionPoint>().drawText = true;
                    }
                }
            }

            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);

            EditorGUILayout.BeginVertical("Box"); // outer box

            GUI.backgroundColor = oldColor;

            if (healthyMode && EditorApplication.timeSinceStartup > 3600)
            {
                EditorGUILayout.Space();

                GUILayout.BeginVertical();

                GUILayout.Space(Mathf.Cos((float)EditorApplication.timeSinceStartup) * 2.0f);

                EditorGUILayout.HelpBox("You have been working for an hour, perhaps close Unity and take a short break.", MessageType.Warning);

                Repaint();

                GUILayout.EndVertical();
            }

            EditorGUILayout.Space();

            #region GridManager Generation

            if (selectedTab == 0)
            {
                GUILayout.Label("Grid Generation:", centeredText);

                mapWidth = EditorGUILayout.IntField("X-Axis Length: ", mapWidth);
                mapHeight = EditorGUILayout.IntField("Z-Axis Length", mapHeight);

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

                if (confirmMapGeneration)
                {
                    if (mapSizeSet)
                    {
                        oldColor = GUI.backgroundColor;
                        GUI.backgroundColor = new Color(0.39f, 0.78f, 0.19f, 1.0f); // green

                        if (GUILayout.Button("Generate GridManager"))
                        {
                            if (grid.GenerateGrid(mapWidth, mapHeight, walkableColour, notWalkableColour, connectionColour))
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

                if (setAllNodesClear)
                {
                    GUILayout.Label("Set ALL nodes clear?", centeredText);

                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button("yes"))
                    {
                        Tile[] tiles = grid.GetComponentsInChildren<Tile>();

                        foreach (Tile tile in tiles)
                        {
                            tile.IsWalkable = true;
                        }

                        setAllNodesClear = false;
                    }

                    oldColor = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(1.0f, 0.19f, 0.19f, 1.0f); // red

                    if (GUILayout.Button("No"))
                    {
                        setAllNodesClear = false;
                    }

                    GUI.backgroundColor = oldColor;

                    EditorGUILayout.EndHorizontal();
                }
                else if (GUILayout.Button("Set all Nodes Clear"))
                {
                    setAllNodesClear = true;
                }

                EditorGUI.BeginDisabledGroup(!hasTileSelected);

                if (GUILayout.Button("Set Selected Tiles Clear"))
                {
                    GameObject[] selectedObjects = Selection.gameObjects;

                    foreach (GameObject obj in selectedObjects)
                    {
                        Tile[] tiles = obj.GetComponentsInChildren<Tile>();

                        foreach (Tile tile in tiles)
                        {
                            tile.IsWalkable = true;
                        }
                    }
                }

                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button("Auto Clear Tiles"))
                {
                    Tile[] tiles = grid.GetComponentsInChildren<Tile>();

                    foreach (Tile tile in tiles)
                    {
                        tile.AutoClear();
                    }
                }

                GUI.backgroundColor = oldColor;

                EditorGUILayout.Space();

                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1.0f, 0.19f, 0.19f, 1.0f); // red

                if (setAllNodesBlocked)
                {
                    GUILayout.Label("Set ALL nodes blocked?", centeredText);

                    EditorGUILayout.BeginHorizontal();

                    oldColor = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(0.39f, 0.78f, 0.19f, 1.0f); // green

                    if (GUILayout.Button("yes"))
                    {
                        Tile[] tiles = grid.GetComponentsInChildren<Tile>();

                        foreach (Tile tile in tiles)
                        {
                            tile.IsWalkable = false;
                        }

                        setAllNodesBlocked = false;
                    }

                    GUI.backgroundColor = oldColor;

                    if (GUILayout.Button("No"))
                    {
                        setAllNodesBlocked = false;
                    }

                    EditorGUILayout.EndHorizontal();
                }
                else if (GUILayout.Button("Set all Nodes Blocked"))
                {
                    setAllNodesBlocked = true;
                }

                EditorGUI.BeginDisabledGroup(!hasTileSelected);

                if (GUILayout.Button("Set Selected Tiles Blocked"))
                {
                    GameObject[] selectedObjects = Selection.gameObjects;

                    foreach (GameObject obj in selectedObjects)
                    {
                        Tile[] tiles = obj.GetComponentsInChildren<Tile>();

                        foreach (Tile tile in tiles)
                        {
                            tile.IsWalkable = false;
                        }
                    }
                }

                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button("Auto Block Tiles"))
                {
                    Tile[] tiles = grid.GetComponentsInChildren<Tile>();

                    foreach (Tile tile in tiles)
                    {
                        tile.AutoBlock();
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
                        Vector3 position;

                        position.x = (transform.position.x + transform.position.z * 0.5f - transform.position.z / 2) * (Tile.innerRadius * 2f);
                        position.y = transform.position.y;
                        position.z = transform.position.z * (Tile.outerRadius * 1.5f);

                        transform.position = position;
                    }
                }

                GUI.backgroundColor = oldColor;

                EditorGUI.EndDisabledGroup();

                GUILayout.Label("Visual Settings:", centeredText);

                walkableColour = EditorGUILayout.ColorField("Can walk", walkableColour);
                notWalkableColour = EditorGUILayout.ColorField("Can't walk", notWalkableColour);
                connectionColour = EditorGUILayout.ColorField("Connection", connectionColour);

                bool isDirty = (oldWalkableColour != walkableColour) || (oldNotWalkableColour != notWalkableColour) || (oldConnectionColour != connectionColour);

                if (isDirty)
                {
                    oldWalkableColour = walkableColour;
                    oldNotWalkableColour = notWalkableColour;
                    oldConnectionColour = connectionColour;

                    ApplyVisualSettingsToTiles();
                }
            }

            #endregion

            #region Tile settings

            //else if (selectedTab == 1)
            //{
            //    if (!hasTileSelected)
            //    {
            //        EditorGUILayout.HelpBox("Select at least one tile to modify.", MessageType.Warning);

            //        EditorGUILayout.Space();
            //    }

            //    tileStatus = (Status)EditorGUILayout.EnumPopup("Tile State:", tileStatus);

            //    EditorGUI.BeginDisabledGroup(!hasTileSelected);

            //    if (GUILayout.Button("Set State"))
            //    {
            //        GameObject[] selectedObjects = Selection.gameObjects;

            //        foreach (GameObject obj in selectedObjects)
            //        {
            //            Tile[] tiles = obj.GetComponentsInChildren<Tile>();

            //            foreach (Tile tile in tiles)
            //            {
            //                //  tile.SetStatus(tileStatus);
            //            }
            //        }
            //    }

            //    EditorGUI.EndDisabledGroup();

            //    EditorGUILayout.Space();

            //    // Tile height settings

            //    GUILayout.Label("Tile Height: ", centeredText);

            //    EditorGUILayout.BeginHorizontal();

            //    EditorGUILayout.BeginVertical();

            //    EditorGUILayout.BeginHorizontal();

            //    stepSize = EditorGUILayout.FloatField(stepSize, GUILayout.Width(50));

            //    if (stepSize < 0.0f)
            //    {
            //        stepSize = 0.0f;
            //    }

            //    if (GUILayout.Button("Set Step Size:", GUILayout.Width(150)))
            //    {

            //    }

            //    EditorGUILayout.EndHorizontal();

            //    EditorGUILayout.BeginHorizontal();

            //    EditorGUI.BeginDisabledGroup(!hasTileSelected);

            //    step = EditorGUILayout.IntField("", step, GUILayout.Width(50));

            //    if (GUILayout.Button("Set Level:", GUILayout.Width(150)))
            //    {
            //        GameObject[] selectedObjects = Selection.gameObjects;

            //        foreach (GameObject obj in selectedObjects)
            //        {
            //            Tile[] tiles = obj.GetComponentsInChildren<Tile>();

            //            foreach (Tile tile in tiles)
            //            {
            //                // tile.SetHeight(step * stepSize);
            //            }
            //        }
            //    }

            //    EditorGUILayout.EndHorizontal();

            //    EditorGUILayout.EndVertical();

            //    EditorGUILayout.BeginVertical();

            //    if (GUILayout.Button("^", GUILayout.Width(50)))
            //    {
            //        GameObject[] selectedObjects = Selection.gameObjects;

            //        foreach (GameObject obj in selectedObjects)
            //        {
            //            Tile[] tiles = obj.GetComponentsInChildren<Tile>();

            //            foreach (Tile tile in tiles)
            //            {
            //                // tile.IncreaseHeight(stepSize);
            //            }
            //        }
            //    }

            //    if (GUILayout.Button("v", GUILayout.Width(50)))
            //    {
            //        GameObject[] selectedObjects = Selection.gameObjects;

            //        foreach (GameObject obj in selectedObjects)
            //        {
            //            Tile[] tiles = obj.GetComponentsInChildren<Tile>();

            //            foreach (Tile tile in tiles)
            //            {
            //                // tile.DecreaseHeight(stepSize);
            //            }
            //        }
            //    }

            //    EditorGUILayout.EndVertical();

            //    EditorGUILayout.EndHorizontal();

            //    EditorGUI.EndDisabledGroup();
            //}

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
                        Tile tile = Selection.gameObjects[0].GetComponent<Tile>();

                        GameObject spawnerGO = new GameObject("Earth Spawner")
                        {
                            tag = "EarthUnitSpawn"
                        };

                        Spawner spawner = spawnerGO.AddComponent<Spawner>();
                        spawner.EntityToSpawn = (Entity)Resources.Load("EarthUnit");
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
                        Tile tile = Selection.gameObjects[0].GetComponent<Tile>();
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
                        Tile tile = Selection.gameObjects[0].GetComponent<Tile>();

                        GameObject spawnerGO = new GameObject("Lightning Spawner")
                        {
                            tag = "LightningUnitSpawn"
                        };

                        Spawner spawner = spawnerGO.AddComponent<Spawner>();
                        spawner.EntityToSpawn = (Entity)Resources.Load("LightningUnit");
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
                        Tile tile = Selection.gameObjects[0].GetComponent<Tile>();
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
                        Tile[] tiles = obj.GetComponentsInChildren<Tile>();

                        foreach (Tile tile in tiles)
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
                        Tile[] tiles = obj.GetComponentsInChildren<Tile>();

                        foreach (Tile tile in tiles)
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
                        Tile tile = Selection.gameObjects[0].GetComponent<Tile>();

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
                    }

                    GUI.backgroundColor = oldColor;
                }
                else
                {
                    oldColor = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(0.39f, 0.78f, 0.19f, 1.0f); // green

                    if (GUILayout.Button("Add Transition"))
                    {
                        Tile tile = Selection.gameObjects[0].GetComponent<Tile>();

                        GameObject transitionGO = new GameObject("stateTransition");

                        StateTransitionPoint sceneTransition = transitionGO.AddComponent<StateTransitionPoint>();
                        sceneTransition.TargetState = state;
                        sceneTransition.drawText = true;

                        SphereCollider trigger = transitionGO.AddComponent<SphereCollider>();
                        trigger.isTrigger = true;

                        sceneTransition.transform.parent = tile.transform;
                        sceneTransition.transform.position = tile.transform.position;
                    }

                    GUI.backgroundColor = oldColor;
                }

                GUILayout.Label("State :");

                state = (Game_state)EditorGUILayout.EnumPopup(state);

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

            else if (selectedTab == 2)
            {
                healthyMode = EditorGUILayout.Toggle("Healthy Mode: ", healthyMode);
            }

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

    private void OnInspectorUpdate()
    {
        if (Selection.gameObjects.Length > 0)
        {
            hasTileSelected = (Selection.gameObjects[0].GetComponentsInChildren<Tile>().Length > 0);

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
        Tile[] tiles = grid.GetComponentsInChildren<Tile>();

        foreach (Tile tile in tiles)
        {
            tile.walkableColour = walkableColour;
            tile.notWalkableColour = notWalkableColour;
            tile.connectionColour = connectionColour;
        }
    }
}