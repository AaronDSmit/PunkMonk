using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    // Global Manager ref
    public static Manager instance;

    private StateManager stateManager;
    private UIManager uiManager;
    private TransitionManager transitionManager;
    private TurnManager turnManager;

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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            stateManager = GetComponent<StateManager>();
            transitionManager = GetComponent<TransitionManager>();
            uiManager = GetComponent<UIManager>();
            turnManager = GetComponent<TurnManager>();
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

    private void Start()
    {
        StartCoroutine(InitManagers());
    }

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

        stateManager.StartGame();
    }
}