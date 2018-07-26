using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.AnimatedValues;
#endif

public class EasyDesignEditor : EditorWindow
{
    // general variables
    [SerializeField] private static EditorWindow window;
    [SerializeField] private static Texture icon;
    [SerializeField] private GUISkin skin;
    [SerializeField] private GUIStyle centeredText;
    [SerializeField] private static int selectedTab = 0;
    [SerializeField] private static bool showInspector;

    // GridManager Generation
    [SerializeField] private static int mapWidth;
    [SerializeField] private static int mapHeight;
    [SerializeField] private static bool confirmMapGeneration = false;
    [SerializeField] private static bool mapSizeSet;

    // Navigation
    [SerializeField] private bool hasTileSelected = false;
    [SerializeField] private static bool mapHasNodes = false;
    [SerializeField] private static bool nodesConnected = false;

    [SerializeField] private AnimBool showNodeVisualSettings;
    [SerializeField] private bool showNodes;
    [SerializeField] private bool showConnections;
    [SerializeField] private Color walkableColour;
    [SerializeField] private Color notWalkableColour;
    [SerializeField] private Color connectionColour;
    [SerializeField] private string buttonText = "Show Visual Settings";

    // Tile settings
    [SerializeField] private Status tileStatus;
    [SerializeField] private float stepSize = 1;
    [SerializeField] private int step;

    // Settings
    [SerializeField] private bool healthyMode = false;

    private void OnEnable()
    {
        skin = (GUISkin)Resources.Load("EditorSkin");

        centeredText = skin.GetStyle("CenteredText");

        if (window != null)
        {
            window.titleContent = new GUIContent("Easy Design", icon);
        }

        showNodeVisualSettings = new AnimBool(true);
        showNodeVisualSettings.valueChanged.AddListener(Repaint);
        showNodeVisualSettings.target = false;
        showNodes = true;
        showConnections = true;

        GridManager grid = GameObject.FindGameObjectWithTag("Manager").GetComponent<GridManager>();

        if (grid && grid.transform.childCount > 0)
        {
            string[] mapSize = grid.transform.GetChild(grid.transform.childCount - 1).name.Split(',');

            mapWidth = int.Parse(mapSize[0]) + 1;
            mapHeight = int.Parse(mapSize[1]) + 1;
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

        /// Only show compiling message while Compiling
        if (EditorApplication.isCompiling)
        {
            EditorGUILayout.HelpBox("Compiling", MessageType.Info);
            showInspector = false;
        }


        /// disable inspector if working for more than 5 hours
        if (healthyMode && EditorApplication.timeSinceStartup > 18000)
        {
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("You have been working for 5 hours, Please close Unity and take a break", MessageType.Error);
            showInspector = false;
        }

        /// Draw Inspector
        if (showInspector)
        {
            GameObject mapGO = GameObject.FindGameObjectWithTag("Manager");
            GridManager grid = null;

            if (mapGO != null)
            {
                grid = GameObject.FindGameObjectWithTag("Manager").GetComponent<GridManager>();
            }

            if (grid == null || mapGO == null)
            {
                EditorGUILayout.HelpBox("No GridManager script found!", MessageType.Error);

                return;
            }

            mapHasNodes = grid.GetComponentsInChildren<Tile>().Length > 0;

            GUI.skin = skin; // use our custom skin

            // Create toolbar using custom tab style
            string[] tabs = { "Grid", "Tiles", "Spawning", "Settings" };
            selectedTab = GUILayout.Toolbar(selectedTab, tabs);

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
                if (EditorApplication.isPlaying)
                {
                    EditorGUILayout.HelpBox("You can't generate a grid during play mode", MessageType.Info);

                    return;
                }

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
                            if (grid.GenerateGrid(mapWidth, mapHeight))
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

                //EditorGUI.BeginDisabledGroup(mapHasNodes);

                if (GUILayout.Button("Generate nodes for grid"))
                {
                    Tile[] tiles = grid.GetComponentsInChildren<Tile>();

                    foreach (Tile tile in tiles)
                    {
                        tile.IsWalkable = true;

                        //if (tile.GetComponentInChildren<Tile>() == null)
                        //{
                        //    GenerateNode(tile);
                        //}
                    }
                }

                //EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!hasTileSelected);

                if (GUILayout.Button("Generate nodes on selected tiles"))
                {
                    //GameObject[] selectedObjects = Selection.gameObjects;

                    //foreach (GameObject obj in selectedObjects)
                    //{
                    //    Tile[] tiles = obj.GetComponentsInChildren<Tile>();

                    //    foreach (Tile tile in tiles)
                    //    {
                    //        if (tile.GetComponentInChildren<Tile>() == null)
                    //        {
                    //            GenerateNode(tile);
                    //        }
                    //    }
                    //}
                }

                EditorGUI.EndDisabledGroup();

                GUI.backgroundColor = oldColor;

                EditorGUILayout.Space();

                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1.0f, 0.19f, 0.19f, 1.0f); // red

                EditorGUI.BeginDisabledGroup(!mapHasNodes);

                if (GUILayout.Button("Remove nodes from grid"))
                {
                    //Tile[] tiles = grid.GetComponentsInChildren<Tile>();

                    //foreach (Tile tile in tiles)
                    //{
                    //    if (tile.transform.Find("NavNode"))
                    //    {
                    //        DestroyImmediate(tile.transform.Find("NavNode").gameObject);
                    //    }
                    //}
                }

                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!hasTileSelected);

                if (GUILayout.Button("Remove nodes on selected tiles"))
                {
                    //GameObject[] selectedObjects = Selection.gameObjects;

                    //foreach (GameObject obj in selectedObjects)
                    //{
                    //    Tile[] tiles = obj.GetComponentsInChildren<Tile>();

                    //    foreach (Tile tile in tiles)
                    //    {
                    //        if (tile.transform.Find("NavNode"))
                    //        {
                    //            DestroyImmediate(tile.transform.Find("NavNode").gameObject);
                    //        }
                    //    }
                    //}
                }

                EditorGUI.EndDisabledGroup();

                GUI.backgroundColor = oldColor;

                EditorGUILayout.Space();

                GUILayout.Label("Connections:", centeredText);

                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(0.39f, 0.78f, 0.19f, 1.0f); // green

                EditorGUI.BeginDisabledGroup(!mapHasNodes || nodesConnected);

                if (GUILayout.Button("Connect nodes on grid"))
                {
                    nodesConnected = true;
                }

                EditorGUI.EndDisabledGroup();

                bool enableConnection = (Selection.gameObjects.Length >= 2);

                if (enableConnection)
                {
                    foreach (GameObject go in Selection.gameObjects)
                    {
                        if (go.GetComponentInChildren<Tile>() == null)
                        {
                            enableConnection = false;
                            break;
                        }
                    }

                    if (!enableConnection)
                    {
                        EditorGUILayout.HelpBox("Not all selected tiles don't have nodes", MessageType.Warning);

                        EditorGUILayout.Space();
                    }
                }

                EditorGUI.BeginDisabledGroup(!enableConnection);

                if (GUILayout.Button("Connect selected nodes"))
                {

                }

                if (GUILayout.Button("Connect selected nodes in sequence"))
                {

                }

                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(Selection.gameObjects.Length != 2);

                if (GUILayout.Button("Connect selected nodes in one direction"))
                {


                }

                EditorGUI.EndDisabledGroup();

                GUI.backgroundColor = oldColor;

                EditorGUILayout.Space();

                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1.0f, 0.19f, 0.19f, 1.0f); // red

                if (GUILayout.Button("Remove all Connections"))
                {
                    nodesConnected = false;
                }

                if (GUILayout.Button("Remove Connections on selected nodes"))
                {
                    nodesConnected = false;
                }

                GUI.backgroundColor = oldColor;

                EditorGUILayout.Space();

                GUILayout.Label("Visual Settings:", centeredText);

                if (EditorGUILayout.BeginFadeGroup(showNodeVisualSettings.faded))
                {
                    walkableColour = EditorGUILayout.ColorField("Can walk", walkableColour);
                    notWalkableColour = EditorGUILayout.ColorField("Can't walk", notWalkableColour);
                    connectionColour = EditorGUILayout.ColorField("Connection", connectionColour);

                    showNodes = EditorGUILayout.Toggle("Show Nodes", showNodes);
                    showConnections = EditorGUILayout.Toggle("Show Connections", showConnections);

                    Tile[] tiles = grid.GetComponentsInChildren<Tile>();

                    foreach (Tile tile in tiles)
                    {
                        ApplyVisualSettingsToNodes(tile);
                    }
                }

                EditorGUILayout.EndFadeGroup();

                if (GUILayout.Button(buttonText))
                {
                    showNodeVisualSettings.target = !showNodeVisualSettings.target;

                    if (showNodeVisualSettings.target)
                    {
                        buttonText = "Apply settings";
                    }
                    else
                    {
                        buttonText = "Show Visual Settings";
                    }
                }
            }

            #endregion

            #region Tile settings

            else if (selectedTab == 1)
            {
                if (!hasTileSelected)
                {
                    EditorGUILayout.HelpBox("Select at least one tile to modify.", MessageType.Warning);

                    EditorGUILayout.Space();
                }

                tileStatus = (Status)EditorGUILayout.EnumPopup("Tile State:", tileStatus);

                EditorGUI.BeginDisabledGroup(!hasTileSelected);

                if (GUILayout.Button("Set State"))
                {
                    GameObject[] selectedObjects = Selection.gameObjects;

                    foreach (GameObject obj in selectedObjects)
                    {
                        Tile[] tiles = obj.GetComponentsInChildren<Tile>();

                        foreach (Tile tile in tiles)
                        {
                            //  tile.SetStatus(tileStatus);
                        }
                    }
                }

                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space();

                // Tile height settings

                GUILayout.Label("Tile Height: ", centeredText);

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical();

                EditorGUILayout.BeginHorizontal();

                stepSize = EditorGUILayout.FloatField(stepSize, GUILayout.Width(50));

                if (stepSize < 0.0f)
                {
                    stepSize = 0.0f;
                }

                if (GUILayout.Button("Set Step Size:", GUILayout.Width(150)))
                {

                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginDisabledGroup(!hasTileSelected);

                step = EditorGUILayout.IntField("", step, GUILayout.Width(50));

                if (GUILayout.Button("Set Level:", GUILayout.Width(150)))
                {
                    GameObject[] selectedObjects = Selection.gameObjects;

                    foreach (GameObject obj in selectedObjects)
                    {
                        Tile[] tiles = obj.GetComponentsInChildren<Tile>();

                        foreach (Tile tile in tiles)
                        {
                            // tile.SetHeight(step * stepSize);
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();

                if (GUILayout.Button("^", GUILayout.Width(50)))
                {
                    GameObject[] selectedObjects = Selection.gameObjects;

                    foreach (GameObject obj in selectedObjects)
                    {
                        Tile[] tiles = obj.GetComponentsInChildren<Tile>();

                        foreach (Tile tile in tiles)
                        {
                            // tile.IncreaseHeight(stepSize);
                        }
                    }
                }

                if (GUILayout.Button("v", GUILayout.Width(50)))
                {
                    GameObject[] selectedObjects = Selection.gameObjects;

                    foreach (GameObject obj in selectedObjects)
                    {
                        Tile[] tiles = obj.GetComponentsInChildren<Tile>();

                        foreach (Tile tile in tiles)
                        {
                            // tile.DecreaseHeight(stepSize);
                        }
                    }
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();

                EditorGUI.EndDisabledGroup();
            }

            #endregion

            #region Spawning

            else if (selectedTab == 2)
            {
                EditorGUI.BeginDisabledGroup(!hasTileSelected);

                if (GUILayout.Button("Add Spawner To Selected Tiles"))
                {
                    GameObject[] selectedObjects = Selection.gameObjects;

                    foreach (GameObject obj in selectedObjects)
                    {
                        Tile[] tiles = obj.GetComponentsInChildren<Tile>();

                        foreach (Tile tile in tiles)
                        {
                            GameObject spawnerGO = new GameObject("Enemy Spawner");
                            EnemySpawner spawner = spawnerGO.AddComponent<EnemySpawner>();

                            spawner.transform.parent = tile.transform;
                            spawner.transform.position = tile.transform.position;
                            spawner.transform.localEulerAngles = new Vector3(180, 0.0f, 0.0f);
                            spawner.TurnToSpawn = 2;
                        }
                    }
                }

                if (GUILayout.Button("Remove Spawner From Selected Tiles"))
                {
                    GameObject[] selectedObjects = Selection.gameObjects;

                    foreach (GameObject obj in selectedObjects)
                    {
                        Tile[] tiles = obj.GetComponentsInChildren<Tile>();

                        foreach (Tile tile in tiles)
                        {
                            if (tile.GetComponentInChildren<EnemySpawner>())
                            {
                                DestroyImmediate(tile.GetComponentInChildren<EnemySpawner>().gameObject);
                            }
                        }
                    }
                }

                EditorGUI.EndDisabledGroup();
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

    private void OnInspectorUpdate()
    {
        if (Selection.gameObjects.Length > 0)
        {
            hasTileSelected = (Selection.gameObjects[0].GetComponentsInChildren<Tile>().Length > 0);

            Repaint();
        }
        else
        {
            hasTileSelected = false;
            Repaint();
        }
    }

    private void ApplyVisualSettingsToNodes(Tile tile)
    {
        tile.walkableColour = walkableColour;
        tile.notWalkableColour = notWalkableColour;
        tile.connectionColour = connectionColour;
        tile.drawNode = showNodes;
        tile.drawConnections = showConnections;
    }

    private void GenerateNode(Tile tile)
    {
        GameObject node = new GameObject("NavNode");
        Tile nav = node.AddComponent<Tile>();
        node.AddComponent<SphereCollider>();
        node.layer = LayerMask.NameToLayer("NavNode");

        node.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        node.transform.position += Vector3.up * 0.5f;

        node.transform.SetParent(tile.transform, false);

        ApplyVisualSettingsToNodes(node.GetComponent<Tile>());
    }
}

public enum Status { NONE, OIL, FIRE, WATER, FAULTLINE, ABYSS }
