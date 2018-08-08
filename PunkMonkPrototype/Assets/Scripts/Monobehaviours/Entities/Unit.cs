using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for any game object that can take damage AND die of a result of reaching zero life.
/// 
/// </summary>

public class Unit : LivingEntity
{
    #region Inspector Variables

    [SerializeField] protected Action[] actions;

    [SerializeField] private int moveRange;

    [SerializeField] protected int attackRange;

    [SerializeField] protected int specialAttackRange;

    [SerializeField] protected float rotationTIme;

    [SerializeField] protected float walkSpeed;

    #endregion

    protected bool canMove;

    protected bool canAttack;

    protected bool isSelected;

    protected System.Action finishedWalking;

    // Used to outline the character
    private Renderer myRenderer;

    // Events
    public delegate void OnVariableChangeDelegate(bool a_newValue);
    public event OnVariableChangeDelegate OnCanAttackChange;
    public event OnVariableChangeDelegate OnCanMoveChange;

    protected override void Awake()
    {
        base.Awake();

        myRenderer = GetComponentInChildren<Renderer>();

        canAttack = true;
        canMove = true;
    }

    public void Highlight(bool a_isHighlited, Color a_outlineColour)
    {
        if (a_isHighlited)
        {
            myRenderer.material.SetFloat("_UseOutline", 1);
            myRenderer.material.SetFloat("_OutlineWidth", 0.06f);
            myRenderer.material.SetColor("_OutlineColour", a_outlineColour);
        }
        else
        {
            if (isSelected)
            {
                myRenderer.material.SetFloat("_OutlineWidth", 0.03f);
            }
            else
            {
                myRenderer.material.SetInt("_UseOutline", 0);
            }
        }
    }

    protected bool HasClearShot(Unit a_currentUnit, Unit a_targetUnit)
    {
        // Unit height should be replaced with 
        float unitHeight = 1.8f;
        // StartPos should be the transform from where the Unit shoots from
        Vector3 startPos = a_currentUnit.CurrentTile.transform.position + Vector3.up * (unitHeight / 2.0f);
        // TargetPos is the centre of the target unit
        Vector3 targetPos = a_targetUnit.CurrentTile.transform.position + Vector3.up * (unitHeight / 2.0f);
        Ray ray = new Ray(startPos, targetPos - startPos);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            // Check if we hit the target unit
            if (hitInfo.collider.gameObject == a_targetUnit.gameObject)
            {
                // We hit the target unit, we have a clear shit
                return true;
            }
            // We hit something, no clear shot
            return false;
        }

        // The raycast hit nothing, we have a clear shot
        return true;
    }

    protected bool HasClearShot(Hex a_currentTile, Unit a_targetUnit)
    {
        // Unit height should be replaced with 
        float unitHeight = 1.8f;
        // StartPos should be the transform from where the Unit shoots from
        Vector3 startPos = a_currentTile.transform.position + Vector3.up * (unitHeight / 2.0f);
        // TargetPos is the centre of the target unit
        Vector3 targetPos = a_targetUnit.CurrentTile.transform.position + Vector3.up * (unitHeight / 2.0f);
        Ray ray = new Ray(startPos, targetPos - startPos);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            // Check if we hit the target unit
            if (hitInfo.collider.gameObject == a_targetUnit.gameObject)
            {
                // We hit the target unit, we have a clear shit
                return true;
            }
            // We hit something, no clear shot
            return false;
        }

        // The raycast hit nothing, we have a clear shot
        return true;
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
            myRenderer.material.SetFloat("_UseOutline", 1);
            myRenderer.material.SetFloat("_OutlineWidth", 0.04f);
            myRenderer.material.SetColor("_OutlineColour", a_outlineColour);
        }
        else
        {
            myRenderer.material.SetFloat("_UseOutline", 0);
        }
    }
    public virtual void Spawn(Hex a_startingTile)
    {
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
    }

    protected IEnumerator Rotate(Vector3 targetPos)
    {
        Vector3 targetDirection = targetPos - transform.position;

        //create the rotation we need to be in to look at the target
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        float currentLerpTime = 0.0f;
        float t = 0.0f;

        while (t < 1)
        {
            currentLerpTime += Time.deltaTime;
            t = currentLerpTime / rotationTIme;

            //rotate us over time according to speed until we are in the required rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);

            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            yield return null;
        }
    }

    protected IEnumerator Walk(List<Hex> path)
    {
        int index = 0;

        while (index < path.Count)
        {
            Vector3 targetPos = path[index].transform.position;
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
            currentTile = path[index];
            currentTile.Enter(this);

            index++;
        }

        CanMove = false;
        finishedWalking();
    }

    #region Basic Attack
    public void BasicAttack(Hex[] targetTiles, System.Action start, System.Action finished)
    {
        StartCoroutine(DelayedBasicAction(targetTiles, start, finished));
    }

    private IEnumerator DelayedBasicAction(Hex[] targetTiles, System.Action start, System.Action finished)
    {
        yield return StartCoroutine(Rotate(targetTiles[0].transform.position));

        DoBasicAttack(targetTiles, start, finished);
    }

    protected virtual void DoBasicAttack(Hex[] targetTiles, System.Action start, System.Action finished)
    {
        // keep empty
    }

    #endregion

    #region Special Attack
    public void SpecialAttack(Hex[] targetTiles, System.Action start, System.Action finished)
    {
        StartCoroutine(DelayedSpecialAction(targetTiles, start, finished));
    }

    private IEnumerator DelayedSpecialAction(Hex[] targetTiles, System.Action start, System.Action finished)
    {
        yield return StartCoroutine(Rotate(targetTiles[0].transform.position));

        DoSpecialAttack(targetTiles, start, finished);
    }

    protected virtual void DoSpecialAttack(Hex[] targetTiles, System.Action start, System.Action finished)
    {
        // Keep empty
    }

    #endregion

    #region Getters / Setters

    public Action GetAction(int index)
    {
        return actions[index];
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

        set
        {
            canMove = value;

            if (OnCanMoveChange != null)
            {
                OnCanMoveChange(canMove);
            }
        }
    }

    public bool CanAttack
    {
        get { return canAttack; }

        set
        {
            canAttack = value;

            if (OnCanAttackChange != null)
            {
                OnCanAttackChange(canAttack);
            }
        }
    }

    #endregion
}