using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is used to initialise other Manager classes to ensure they are ready before being accessed. 
/// </summary>
public class Manager : MonoBehaviour
{

    #region Reference Fields

    private StateManager stateManager;
    private UIManager uiManager;
    private TransitionManager transitionManager;
    private TurnManager turnManager;

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

    #endregion

    #region Unity Life-cycle Methods

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            stateManager = GetComponent<StateManager>();
            transitionManager = GetComponent<TransitionManager>();
            uiManager = GetComponent<UIManager>();
            turnManager = GetComponent<TurnManager>();

            StartCoroutine(InitManagers());
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

        stateManager.StartGame();
    }

    #endregion
}