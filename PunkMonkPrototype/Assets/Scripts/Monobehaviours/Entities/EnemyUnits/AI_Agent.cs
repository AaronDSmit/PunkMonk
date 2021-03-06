﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class AI_Agent : Unit
{
    #region Unity Inspector Fields
    [Header("Enemy")]
    [SerializeField]
    protected int damage = 3;

    [SerializeField]
    protected float damageDelayTimer = 0;

    [SerializeField]
    private GameObject bullet;

    [SerializeField]
    private float bulletSpeed = 3;

    [SerializeField]
    private Transform gunTransform;

    #endregion

    #region Reference Fields

    private AI_Controller ai_Controller = null;

    protected Unit[] players = null;

    #endregion

    #region Local Fields

    protected bool turnComplete = false;

    private bool isPerformingAction = true;

    protected Hex[] tilesToAttack = null;

    #endregion

    #region Properties

    public float Damage { get { return damage; } }
    public bool TurnComplete { get { return turnComplete; } }

    #endregion

    #region Public Methods

    public override void Spawn(Hex a_startingTile)
    {
        base.Spawn(a_startingTile);

        healthBar.Show();

        // Get the AI Controller and tell it that this agent was spawned
        ai_Controller = GameObject.FindGameObjectWithTag("AI_Controller").GetComponent<AI_Controller>();
        players = ai_Controller.AddUnit(this);
    }

    public void StartTurn(GridManager a_grid)
    {
        StartCoroutine(DoTurn(a_grid));
    }

    public override void Refresh()
    {
        base.Refresh();

        turnComplete = false;
    }

    #endregion

    #region Unity Life-cycle Methods

    protected override void Awake()
    {
        base.Awake();

        // Setup the actions
        foreach (AI_Action action in actions)
        {
            action.Init(this);
        }
    }

    private void OnEnable()
    {
        Manager.instance.StateController.OnGameStateChanged += GameStateChanged;
    }

    protected void OnDisable()
    {
        Manager.instance.StateController.OnGameStateChanged -= GameStateChanged;
    }

    #endregion

    #region Local Methods

    protected override void DoBasicAttack(Hex[] a_targetTiles, System.Action a_start, System.Action a_finished)
    {
        tilesToAttack = a_targetTiles;


        StartCoroutine(WaitToShoot(a_finished));


    }

    private IEnumerator WaitToShoot(System.Action a_finished)
    {
        if (attackRange > 1 && bullet != null)
        {
            yield return new WaitForSeconds(1.0f);
            GameObject bulletGO = Instantiate(bullet, gunTransform.position + transform.forward, Quaternion.LookRotation(transform.right, Vector3.up));
            bulletGO.GetComponentInChildren<Rigidbody>().AddForce(transform.forward * bulletSpeed, ForceMode.VelocityChange);
            Destroy(bulletGO, 0.5f);
        }
        yield return null;

        StartCoroutine(BasicAttackDamageDelay(damageDelayTimer, a_finished));
    }

    protected virtual IEnumerator DoTurn(GridManager a_grid)
    {
        AI_Action bestAction = null;
        int playerToAttack = 0;
        float bestScore = int.MinValue;
        List<Hex> path = null;

        players[0] = Manager.instance.PlayerController.EarthUnit;
        players[1] = Manager.instance.PlayerController.LightningUnit;

        ScoringInfo[] scoringInfo = new ScoringInfo[2];
        scoringInfo[0] = (players[0] == null || players[0].IsDead) ? null : new ScoringInfo(a_grid, this, players[0]);
        scoringInfo[1] = (players[1] == null || players[1].IsDead) ? null : new ScoringInfo(a_grid, this, players[1]);

        foreach (AI_Action action in actions)
        {
            for (int i = 0; i < 2; i++)
            {
                if (scoringInfo[i] == null)
                    continue;

                float score = action.GetScore(scoringInfo[i]);
                if (scoringInfo[i].Path != null)
                {
                    if (score > bestScore)
                    {
                        bestAction = action;
                        playerToAttack = i;
                        bestScore = score;
                        path = scoringInfo[i].Path;
                    }
                }
            }
        }

        if (bestAction == null)
        {
            Debug.Log("No valid actions found", gameObject);
            turnComplete = true;
            yield break;
        }

        // Remove the path that is out of the movement range
        if (path.Count > MoveRange)
        {
            path.RemoveRange(MoveRange, path.Count - MoveRange);
        }

        // Path find and wait for it to finish
        isPerformingAction = true;
        finishedWalking = FinishedAction;
        StartCoroutine(Walk(path));
        yield return new WaitUntil(() => isPerformingAction == false);

        // Attack the player, checking if it is in range && if we have a clear shot
        if (HexUtility.Distance(currentTile, players[playerToAttack].CurrentTile) <= attackRange && HasClearShot(players[playerToAttack]))
        {
            isPerformingAction = true;
            BasicAttack(new Hex[] { players[playerToAttack].CurrentTile }, null, FinishedAction);
            yield return new WaitUntil(() => isPerformingAction == false);
        }

        turnComplete = true;
        yield break;
    }

    protected virtual IEnumerator BasicAttackDamageDelay(float a_timer, System.Action a_finished)
    {
        //wait for timer before runing code
        yield return new WaitForSeconds(a_timer);

        foreach (Hex tile in tilesToAttack)
        {
            if (tile.CurrentUnit != null)
            {
                if (tile.CurrentUnit.Team != TEAM.ai)
                {
                    tile.CurrentUnit.TakeDamage(damage, this);
                }
            }
        }
        //call the finished call back function
        a_finished();
    }

    protected void FinishedAction()
    {
        isPerformingAction = false;
    }

    private void GameStateChanged(GameState a_oldstate, GameState a_newstate)
    {
        if (a_oldstate == GameState.battle && a_newstate != GameState.battle)
        {
            Die();
        }
    }

    #endregion
}
