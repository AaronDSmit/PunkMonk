using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : LivingEntity
{
    #region Inspector Variables

    [SerializeField] protected Action[] actions;

    [SerializeField] private int moveRange;

    [SerializeField] private int attackRange;

    [SerializeField] protected float turnTime;

    [SerializeField] protected float walkSpeed;

    #endregion

    private bool canMove;

    private bool canAttack;

    private bool isSelected;

    private System.Action finishedAction;

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

    public void Select(bool a_isSelected, Color a_outlineColour)
    {
        isSelected = a_isSelected;

        if(isSelected)
        {
            myRenderer.material.SetFloat("_UseOutline", 1);
            myRenderer.material.SetFloat("_OutlineWidth", 0.03f);
            myRenderer.material.SetColor("_OutlineColour", a_outlineColour);
        }
        else
        {
            myRenderer.material.SetFloat("_UseOutline", 0);
        }
    }

    public int MoveRange
    {
        get { return moveRange; }
    }

    public int AttackRange
    {
        get { return attackRange; }
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

    public void Spawn(Tile a_startingTile)
    {
        currentTile = a_startingTile;

        currentTile.Enter(this);
    }

    public void MoveTo(Tile a_targetTile, System.Action a_finished)
    {
        finishedAction = a_finished;

        //List<Tile> path = navigation.FindPath(currentTile, targetTile);

        //if (path != null)
        //{
        //    StartCoroutine(Walk(path));
        //}
        //else
        //{
        //    Debug.Log("Couldn't find path");
        //}
    }


    public void BasicAttack(Unit a_targetUnit, System.Action a_finished)
    {
        finishedAction = a_finished;

        // StartCoroutine(TurnToAttack(targetUnit));
    }

    public void SpecialAttack(Unit a_targetUnit, System.Action a_finished)
    {
        finishedAction = a_finished;

        // StartCoroutine(TurnToAttack(targetUnit));
    }

    public void Refresh()
    {
        CanAttack = true;
        CanMove = true;
    }
}