﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>

public class Unit : LivingEntity
{
    #region Unity Inspector Fields

    [Tooltip("A list of actions that this Unit can perform")]
    [SerializeField]
    protected Action[] actions;
    [Tooltip("The distance in tiles that this unit can move in one turn")]
    [SerializeField]
    private int moveRange;
    [Tooltip("The distance in tiles this character can attack")]
    [SerializeField]
    protected int attackRange;
    [Tooltip("The distance in tiles this units special attack")]
    [SerializeField]
    protected int specialAttackRange;
    [Tooltip("The time it takes this unit to rotate in seconds")]
    [SerializeField]
    protected float rotationTime;
    [Tooltip("How the fast this unit walks from tile to tile")]
    [SerializeField]
    protected float walkSpeed;
    [Tooltip("The position that projectiles spawn from")]
    [SerializeField]
    protected Transform projectilePosition;

    [SerializeField]
    private ParticleSystem highlightEffect;

    [SerializeField]
    private GameObject voltEffect = null;


    #endregion

    #region Reference Fields

    protected CameraController cameraController;
    protected Animator animator;

    #endregion

    #region Local Fields

    private bool canMove;

    private bool canAttack;

    private bool canSpecialAttack;

    protected bool hasUsedSpecialAttack;

    private bool isSelected;

    private int actionsPerforming = 0;

    protected System.Action finishedWalking;

    public delegate void VariableChanged(bool a_value);
    public event VariableChanged OnCanMoveChanged;
    public event VariableChanged OnCanAttackChanged;
    public event VariableChanged OnCanSpecialChanged;

    #endregion

    #region Properties

    public Vector3 ShootPos { get { return projectilePosition.position; } }

    public Action GetAction(int a_index)
    {
        return actions[a_index];
    }

    public int MoveRange
    {
        get { return moveRange; }
    }

    public bool IsSelected
    {
        get { return isSelected; }
    }

    public int AttackRange
    {
        get { return attackRange; }
    }

    public int SpecialAttackRange
    {
        get { return specialAttackRange; }
    }

    public bool CanMove
    {
        get { return canMove; }

        set
        {
            canMove = value;

            if (OnCanMoveChanged != null)
            {
                OnCanMoveChanged(canMove);
            }
        }
    }

    public bool CanAttack
    {
        get { return canAttack; }

        set
        {
            canAttack = value;

            if (OnCanAttackChanged != null)
            {
                OnCanAttackChanged(canAttack);
            }
        }
    }

    public bool CanSpecialAttack
    {
        get { return canSpecialAttack; }

        set
        {
            canSpecialAttack = value;

            if (OnCanSpecialChanged != null)
            {
                OnCanSpecialChanged(canSpecialAttack);
            }
        }
    }

    public bool HasActionAvaialible { get { return CanAttack || CanMove || CanSpecialAttack; } }

    public bool IsPerformingAction { get { return actionsPerforming > 0; } }

    #endregion

    #region Public Methods

    public override void TakeDamage(int a_damageAmount, Unit a_damageFrom)
    {
        base.TakeDamage(a_damageAmount, a_damageFrom);

        animator.SetTrigger("Damage");
    }

    public void BasicAttack(Hex[] a_targetTiles, System.Action a_start, System.Action a_finished)
    {
        animator.SetTrigger("BasicAttack");
        StartCoroutine(DelayedBasicAction(a_targetTiles, a_start, a_finished));
    }

    public void SpecialAttack(Hex[] a_targetTiles, System.Action a_start, System.Action a_finished)
    {
        animator.SetTrigger("SpecialAttack");
        StartCoroutine(DelayedSpecialAction(a_targetTiles, a_start, a_finished));
    }

    public bool PreviewDamage(int a_damage)
    {
        return healthBar.PreviewDamage(a_damage);
    }

    public void Highlight(bool a_isHighlited, Color a_outlineColour)
    {
        if (a_isHighlited)
        {
            myRenderer.material.SetFloat("_UseHighlight", 1);
            myRenderer.material.SetFloat("_HighlightAmount", 0.06f);
            myRenderer.material.SetColor("_HighlightColour", a_outlineColour);
        }
        else
        {
            if (isSelected)
            {
                myRenderer.material.SetFloat("_HighlightAmount", 0.03f);
            }
            // TODO: Highlight units that have volt
            //else if (CompareTag("Enemy") && CurrentVolt > 0)
            //{
            //    myRenderer.material.SetFloat("_HighlightAmount", 0.5f);
            //    myRenderer.material.SetColor("_HighlightColour", new Color(1.0f, 0.62f, 0.21f));
            //}
            else
            {
                myRenderer.material.SetInt("_UseHighlight", 0);
            }
        }
    }

    public void WalkDirectlyToTile(Hex a_targetHex, HexDirection a_direction)
    {
        StartCoroutine(WalkDirectlyTo(a_targetHex, a_direction));
    }

    public void WalkDirectlyToTile(Hex a_targetHex, Hex a_lookAtHex)
    {
        StartCoroutine(WalkDirectlyTo(a_targetHex, a_lookAtHex));
    }

    public void TeleportToHex(Hex a_targetHex)
    {
        currentTile.Exit();
        currentTile = a_targetHex;
        currentTile.Enter(this);

        transform.position = new Vector3(currentTile.transform.position.x, transform.position.y, currentTile.transform.position.z);
    }

    public void RotateTo(Vector3 a_position)
    {
        StartCoroutine(Rotate(a_position));
    }

    public void Select(bool a_isSelected, Color a_outlineColour)
    {
        isSelected = a_isSelected;

        if (isSelected)
        {
            myRenderer.material.SetFloat("_UseHighlight", 1);
            myRenderer.material.SetFloat("_HighlightAmount", 0.04f);
            myRenderer.material.SetColor("_HighlightColour", a_outlineColour);
            if (highlightEffect)
                highlightEffect.Play();
        }
        else
        {
            myRenderer.material.SetFloat("_UseHighlight", 0);
            if (highlightEffect)
                highlightEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    public virtual void Spawn(Hex a_startingTile)
    {
        if (healthBar)
        {
            healthBar.MaxHealth = maxHealth;
            healthBar.CurrentHealth = CurrentHealth;
        }

        currentTile = a_startingTile;

        currentTile.Enter(this);
    }

    public void MoveTo(Hex a_targetTile, System.Action a_finished)
    {
        finishedWalking = a_finished;

        List<Hex> path = Navigation.FindPath(currentTile, a_targetTile);

        if (path != null)
        {
            StartCoroutine(Walk(path));
        }
        else
        {
            Debug.Log("Couldn't find path");
        }
    }

    public virtual void Refresh()
    {
        CanAttack = true;
        CanMove = true;
        hasUsedSpecialAttack = false;
        CanSpecialAttack = HasVolt;
    }

    public void HasKilled()
    {
        //if (CurrentVolt > 0)
        {
            // only set it to true if you haven't already used special attack
            canSpecialAttack = !hasUsedSpecialAttack;

            if (OnCanSpecialChanged != null)
            {
                OnCanSpecialChanged(canSpecialAttack);
            }
        }
    }

    #endregion

    #region Unity Life-cycle Methods

    protected override void Awake()
    {
        base.Awake();

        cameraController = GameObject.FindGameObjectWithTag("CameraRig").GetComponent<CameraController>();
        animator = GetComponentInChildren<Animator>();

        Manager.instance.StateController.OnGameStateChanged += GameStateChanged;

        canAttack = true;
        canMove = true;
        canSpecialAttack = false;

        cameraController.onGlamCamStart += healthBar.Hide;
        cameraController.onGlamCamEnd += healthBar.Show;
    }

    private void OnDestroy()
    {
        cameraController.onGlamCamStart -= healthBar.Hide;
        cameraController.onGlamCamEnd -= healthBar.Show;
    }

    #endregion

    #region Local Methods

    private void GameStateChanged(GameState a_oldstate, GameState a_newstate)
    {
        if (IsDead)
        {
            return;
        }

        if (a_newstate == GameState.battle)
        {
            healthBar.Show();
        }
        else if (a_oldstate == GameState.battle)
        {
            healthBar.Hide();
        }
    }

    protected bool HasClearShot(Unit a_targetUnit)
    {
        // StartPos should be the transform from where the Unit shoots from
        Vector3 startPos = ShootPos;
        // TargetPos is the centre of the target unit
        Vector3 targetPos = a_targetUnit.CurrentTile.transform.position + Vector3.up * 0.8f;
        Vector3 vecBetween = targetPos - startPos;
        Ray ray = new Ray(startPos, vecBetween);
        RaycastHit[] hits = Physics.RaycastAll(ray, vecBetween.magnitude, LayerMask.GetMask(new string[] { "Unit", "Level", "Ground" }));

        foreach (RaycastHit hitInfo in hits)
        {
            // Check if we hit the target unit
            if (hitInfo.collider.gameObject == a_targetUnit.gameObject)
            {
                // We hit the target unit, we have a clear shot
                return true;
            }
            else if (hitInfo.collider.gameObject != gameObject)
            {
                // If we hit something other than this gameobject
                return false;
            }
        }

        return false;
    }

    private IEnumerator WalkDirectlyTo(Hex a_targetHex, HexDirection a_direction)
    {
        actionsPerforming++;
        animator.SetBool("Running", true);

        Vector3 targetPos = a_targetHex.transform.position;
        targetPos.y = transform.position.y;

        yield return StartCoroutine(Rotate(targetPos));

        Vector3 vecBetween = targetPos - transform.position;
        vecBetween.y = 0.0f;

        float speed = vecBetween.magnitude / StateManager.stateTransitionTime;

        while (vecBetween.magnitude > 0.1f)
        {
            transform.position += vecBetween.normalized * speed * Time.deltaTime;

            vecBetween = targetPos - transform.position;
            vecBetween.y = 0.0f;

            yield return null;
        }

        // lastly turn to face the starting direction

        Vector3 targetDir = a_targetHex.GetNeighbour(a_direction).transform.position;
        targetDir.y = transform.position.y;

        yield return StartCoroutine(Rotate(targetDir));

        currentTile.Exit();
        currentTile = a_targetHex;
        currentTile.Enter(this);
        transform.position = targetPos;

        animator.SetBool("Running", false);
        actionsPerforming--;
    }

    private IEnumerator WalkDirectlyTo(Hex a_targetHex, Hex a_lookAtHex = null)
    {
        actionsPerforming++;

        yield return StartCoroutine(WalkToHex(a_targetHex));

        // lastly turn to face the starting direction

        if (a_lookAtHex)
        {
            Vector3 targetDir = a_lookAtHex.transform.position;
            targetDir.y = transform.position.y;
            yield return StartCoroutine(Rotate(targetDir));
        }

        actionsPerforming--;
    }

    protected IEnumerator Rotate(Vector3 a_targetPos)
    {
        actionsPerforming++;

        Vector3 targetDirection = a_targetPos - transform.position;

        //create the rotation we need to be in to look at the target
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        float currentLerpTime = 0.0f;
        float t = 0.0f;

        while (t < 1)
        {
            currentLerpTime += Time.deltaTime;
            t = currentLerpTime / rotationTime;

            //rotate us over time according to speed until we are in the required rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);

            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            yield return null;
        }

        actionsPerforming--;
    }

    protected IEnumerator Walk(List<Hex> a_path)
    {
        actionsPerforming++;
        animator.SetBool("Running", true);

        float totalDistance = Vector3.Distance(transform.position, a_path[0].transform.position);
        for (int i = 0; i < a_path.Count - 1; i++)
        {
            totalDistance += Vector3.Distance(a_path[i].transform.position, a_path[i + 1].transform.position);
        }

        float distanceTraveled = 0;

        currentTile.Exit();

        int targetHexIndex = 0;

        while (distanceTraveled < totalDistance)
        {
            targetHexIndex = (int)Mathf.Floor((distanceTraveled / totalDistance) * a_path.Count);

            Vector3 targetPos = a_path[targetHexIndex].transform.position;

            StartCoroutine(Rotate(targetPos));

            Vector3 vecBetween = targetPos - transform.position;

            transform.position += vecBetween.normalized * Time.deltaTime * walkSpeed;

            cameraController.LookAtPosition(transform.position);

            distanceTraveled += (vecBetween.normalized * Time.deltaTime * walkSpeed).magnitude;

            yield return null;
        }

        transform.position = a_path[a_path.Count - 1].transform.position;

        a_path[a_path.Count - 1].Enter(this);
        currentTile = a_path[a_path.Count - 1];

        CanMove = false;
        finishedWalking();

        animator.SetBool("Running", false);
        actionsPerforming--;
    }

    protected IEnumerator WalkToHex(Hex a_hex, System.Action a_finishedAction = null)
    {
        actionsPerforming++;
        animator.SetBool("Running", true);

        Vector3 targetPos = a_hex.transform.position;
        targetPos.y = transform.position.y;

        yield return StartCoroutine(Rotate(targetPos));

        currentTile.Exit();

        float distance = Vector3.Distance(transform.position, targetPos);

        while (distance > Time.deltaTime * walkSpeed)
        {
            distance = Vector3.Distance(transform.position, targetPos);

            Vector3 vecBetween = targetPos - transform.position;

            transform.position += vecBetween.normalized * Time.deltaTime * walkSpeed;

            yield return null;
        }

        transform.position = targetPos;

        currentTile = a_hex;
        currentTile.Enter(this);

        animator.SetBool("Running", false);

        if (a_finishedAction != null)
            a_finishedAction();

        actionsPerforming--;
    }

    private IEnumerator DelayedBasicAction(Hex[] a_targetTiles, System.Action a_start, System.Action a_finished)
    {
        yield return StartCoroutine(Rotate(a_targetTiles[0].transform.position));

        DoBasicAttack(a_targetTiles, a_start, a_finished);
    }

    private IEnumerator DelayedSpecialAction(Hex[] a_targetTiles, System.Action a_start, System.Action a_finished)
    {
        yield return StartCoroutine(Rotate(a_targetTiles[0].transform.position));

        DoSpecialAttack(a_targetTiles, a_start, a_finished);
    }

    protected virtual void DoBasicAttack(Hex[] a_targetTiles, System.Action a_start, System.Action a_finished)
    {
        // keep empty
    }

    protected virtual void DoSpecialAttack(Hex[] a_targetTiles, System.Action a_start, System.Action a_finished)
    {
        // Keep empty
    }

    protected override void VoltChanged()
    {
        base.VoltChanged();

        CanSpecialAttack = HasVolt;

        if (voltEffect)
        {
            voltEffect.SetActive(HasVolt);
        }
    }

    #endregion
}