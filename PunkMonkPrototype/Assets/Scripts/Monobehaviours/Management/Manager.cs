﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// This script is used to initialise other Manager classes to ensure they are ready before being accessed. 
/// </summary>
public class Manager : MonoBehaviour
{
    #region Unity Inspector Fields

    [SerializeField]
    private Texture2D cursorTextureHover;

    [SerializeField]
    private Texture2D cursorTextureClick;

    [SerializeField]
    private CursorMode cursorMode = CursorMode.Auto;

    [SerializeField]
    private Vector2 hotSpot = Vector2.zero;

    #endregion

    #region Reference Fields

    private StateManager stateManager;
    private UIManager uiManager;
    private TransitionManager transitionManager;
    private TurnManager turnManager;
    private CheckPointManager checkPointManager;
    private HexHighlighter hexHighlighter;
    private Dialogue dialogueManager;
    private GridManager grid;
    private PlayerController playerController;
    private SFXManager sfxManager;

    #endregion

    #region Local Fields

    public static Manager instance;

    #endregion

    #region Properties

    public GridManager Grid
    {
        get { return grid; }
    }

    public StateManager StateController
    {
        get { return stateManager; }
    }

    public UIManager UIController
    {
        get { return uiManager; }
    }

    public TransitionManager TransitionController
    {
        get { return transitionManager; }
    }

    public TurnManager TurnController
    {
        get { return turnManager; }
    }

    public CheckPointManager CheckPointController
    {
        get { return checkPointManager; }
    }

    public HexHighlighter HexHighlighter
    {
        get { return hexHighlighter; }
    }

    public Dialogue DialogueManager
    {
        get { return dialogueManager; }
    }

    public PlayerController PlayerController
    {
        get { return playerController; }
    }

    public SFXManager SfxManager
    {
        get { return sfxManager; }
    }

    #endregion

    #region Unity Life-cycle Methods

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            sfxManager = GetComponent<SFXManager>();
            stateManager = GetComponent<StateManager>();
            transitionManager = GetComponent<TransitionManager>();
            uiManager = GetComponent<UIManager>();
            turnManager = GetComponent<TurnManager>();
            checkPointManager = GetComponent<CheckPointManager>();
            hexHighlighter = GetComponent<HexHighlighter>();
            dialogueManager = GetComponent<Dialogue>();

            grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridManager>();
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

            StartCoroutine(InitManagers());

            if (cursorTextureHover)
            {
                Cursor.SetCursor(cursorTextureHover, hotSpot, cursorMode);
            }
        }
        else
        {
            //stop the game from having more than one GameStateManager
            Debug.LogError("Multiple Managers exist, deleting duplicates.");
            Destroy(gameObject);
            return;
        }

        //Don't destroy the GameStateManager in scene
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.SetCursor(cursorTextureClick, hotSpot, cursorMode);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Cursor.SetCursor(cursorTextureHover, hotSpot, cursorMode);
        }
    }

    #endregion

    #region Local Methods

    private IEnumerator InitManagers()
    {
        uiManager.Init();

        yield return new WaitUntil(() => uiManager.Ready);

        stateManager.Init();

        yield return new WaitUntil(() => stateManager.Ready);

        turnManager.Init();

        yield return new WaitUntil(() => turnManager.Ready);

        transitionManager.Init();

        yield return new WaitUntil(() => transitionManager.Ready);

        grid.Init();

        yield return new WaitUntil(() => grid.Ready);

        hexHighlighter.Init();

        yield return new WaitUntil(() => hexHighlighter.IsReady);

        sfxManager.Init();

        stateManager.StartGame();
    }

    #endregion
}