using System.Collections;
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

    #endregion

    #region Local Fields

    public static Manager instance;

    #endregion

    #region Properties

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


    #endregion

    #region Public Methods

    public void SceneLoaded(Scene current, Scene next)
    {
        if (next.buildIndex == 0 || next.buildIndex == 2)
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Unity Life-cycle Methods

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= SceneLoaded;
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += SceneLoaded;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            stateManager = GetComponent<StateManager>();
            transitionManager = GetComponent<TransitionManager>();
            uiManager = GetComponent<UIManager>();
            turnManager = GetComponent<TurnManager>();
            checkPointManager = GetComponent<CheckPointManager>();
            hexHighlighter = GetComponent<HexHighlighter>();

            StartCoroutine(InitManagers());

            if (cursorTextureHover)
            {
                Cursor.SetCursor(cursorTextureHover, hotSpot, cursorMode);
            }
        }
        else
        {
            //stop the game from having more than one GameStateManager
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

        hexHighlighter.Init();

        stateManager.StartGame();
    }

    #endregion
}