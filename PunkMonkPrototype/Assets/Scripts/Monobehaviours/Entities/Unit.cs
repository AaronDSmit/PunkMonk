using System.Collections;
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
    [Tooltip("Toggles on and off the glamCam")]
    [SerializeField]
    protected bool glamCam;

    #endregion

    #region Reference Fields

    protected CameraController cameraController;

    #endregion

    #region Local Fields

    private bool canMove;

    private bool canAttack;

    private bool canSpecialAttack;

    private bool isSelected;

    protected System.Action finishedWalking;

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

        set { canMove = value; }
    }

    public bool CanAttack
    {
        get { return canAttack; }

        set { canAttack = value; }
    }

    public bool CanSpecialAttack
    {
        get { return canSpecialAttack; }

        set { canSpecialAttack = value; }
    }

    #endregion

    #region Public Methods

    public void BasicAttack(Hex[] a_targetTiles, System.Action a_start, System.Action a_finished)
    {
        StartCoroutine(DelayedBasicAction(a_targetTiles, a_start, a_finished));
    }

    public void SpecialAttack(Hex[] a_targetTiles, System.Action a_start, System.Action a_finished)
    {
        StartCoroutine(DelayedSpecialAction(a_targetTiles, a_start, a_finished));
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
            else if (CompareTag("Enemy") && CurrentVolt > 0)
            {
                myRenderer.material.SetFloat("_HighlightAmount", 0.5f);
                myRenderer.material.SetColor("_HighlightColour", new Color(1.0f, 0.62f, 0.21f));
            }
            else
            {
                myRenderer.material.SetInt("_UseHighlight", 0);
            }
        }
    }

    public void WalkDirectlyToTile(Hex a_targetHex)
    {
        StartCoroutine(WalkDirectlyTo(a_targetHex));
    }

    public void TeleportToHex(Hex a_targetHex)
    {
        currentTile.Exit();
        currentTile = a_targetHex;
        currentTile.Enter(this);

        transform.position = new Vector3(currentTile.transform.position.x, transform.position.y, currentTile.transform.position.z);
    }

    public void Select(bool a_isSelected, Color a_outlineColour)
    {
        isSelected = a_isSelected;

        if (isSelected)
        {
            myRenderer.material.SetFloat("_UseHighlight", 1);
            myRenderer.material.SetFloat("_HighlightAmount", 0.04f);
            myRenderer.material.SetColor("_HighlightColour", a_outlineColour);
        }
        else
        {
            myRenderer.material.SetFloat("_UseHighlight", 0);
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

        CurrentVolt = maxVolt;
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
        CanSpecialAttack = CurrentVolt > 0;
    }

    #endregion

    #region Unity Life-cycle Methods

    protected override void Awake()
    {
        base.Awake();

        cameraController = GameObject.FindGameObjectWithTag("CameraRig").GetComponent<CameraController>();

        canAttack = true;
        canMove = true;
        canSpecialAttack = true;
    }

    #endregion

    #region Local Methods

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

    private IEnumerator WalkDirectlyTo(Hex a_targetHex)
    {
        Vector3 targetPos = a_targetHex.transform.position;
        targetPos.y = transform.position.y;
        Vector3 vecBetween = targetPos - transform.position;
        vecBetween.y = 0.0f;

        while (vecBetween.magnitude > 0.1f)
        {
            transform.position += vecBetween.normalized * (walkSpeed / 2) * Time.deltaTime;

            vecBetween = targetPos - transform.position;
            vecBetween.y = 0.0f;

            yield return null;
        }

        currentTile.Exit();
        currentTile = a_targetHex;
        currentTile.Enter(this);
        transform.position = targetPos;
    }

    protected IEnumerator Rotate(Vector3 a_targetPos)
    {
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
    }

    protected IEnumerator Walk(List<Hex> a_path)
    {
        int index = 0;

        while (index < a_path.Count)
        {
            Vector3 targetPos = a_path[index].transform.position;
            targetPos.y = transform.position.y;

            // yield return StartCoroutine(Turn(targetPos));

            float distance = Vector3.Distance(transform.position, targetPos);

            while (distance > 0.1f)
            {
                distance = Vector3.Distance(transform.position, targetPos);

                Vector3 vecBetween = targetPos - transform.position;

                transform.position += vecBetween.normalized * Time.deltaTime * walkSpeed;

                yield return null;
            }

            transform.position = targetPos;

            currentTile.Exit();
            currentTile = a_path[index];
            currentTile.Enter(this);

            index++;
        }

        CanMove = false;
        finishedWalking();
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

    #endregion
}