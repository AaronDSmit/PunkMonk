using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : LivingEntity
{
    #region Inspector Variables

    [SerializeField] private int moveRange;

    [SerializeField] private int attackRange;

    [SerializeField] protected float walkSpeed;

    #endregion

    private bool canMove;

    private bool canAttack;

    private System.Action finishedAction;

    // Events
    public delegate void OnVariableChangeDelegate(bool a_newValue);
    public event OnVariableChangeDelegate OnCanAttackChange;
    public event OnVariableChangeDelegate OnCanMoveChange;

    protected override void Awake()
    {
        base.Awake();

        canAttack = true;
        canMove = true;
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