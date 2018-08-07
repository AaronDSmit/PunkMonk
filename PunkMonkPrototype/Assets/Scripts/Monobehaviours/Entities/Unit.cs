using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : LivingEntity
{
    #region Inspector Variables

    [SerializeField] protected Action[] actions;

    [SerializeField] private int moveRange;

    [SerializeField] protected int attackRange;

    [SerializeField] protected int specialAttackRange;

    [SerializeField] protected float turnTime;

    [SerializeField] protected float walkSpeed;

    #endregion

    protected Element element;

    protected bool canMove;

    protected bool canAttack;

    protected bool isSelected;

    protected System.Action finishedWalking;

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

    public void SnapToGrid(Hex a_targetTile)
    {
        currentTile.Exit();
        currentTile = a_targetTile;
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

    protected IEnumerator Turn(Vector3 targetPos)
    {
        Vector3 targetDirection = targetPos - transform.position;

        //create the rotation we need to be in to look at the target
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        float currentLerpTime = 0.0f;
        float t = 0.0f;

        while (t < 1)
        {
            currentLerpTime += Time.deltaTime;
            t = currentLerpTime / turnTime;

            //rotate us over time according to speed until we are in the required rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);

            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            yield return null;
        }
    }

    #region Basic Attack
    public void BasicAttack(Hex[] targetTiles, System.Action start, System.Action finished)
    {
        StartCoroutine(DelayedBasicAction(targetTiles, start, finished));
    }

    private IEnumerator DelayedBasicAction(Hex[] targetTiles, System.Action start, System.Action finished)
    {
        yield return StartCoroutine(Turn(targetTiles[0].transform.position));

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
        yield return StartCoroutine(Turn(targetTiles[0].transform.position));

        DoSpecialAttack(targetTiles, start, finished);
    }

    protected virtual void DoSpecialAttack(Hex[] targetTiles, System.Action start, System.Action finished)
    {
        // Keep empty
    }

    #endregion

    public Action GetAction(int index)
    {
        return actions[index];
    }

    public int MoveRange
    {
        get { return moveRange; }
    }

    public Element Element
    {
        get { return element; }
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

    public void Refresh()
    {
        CanAttack = true;
        CanMove = true;
    }
}