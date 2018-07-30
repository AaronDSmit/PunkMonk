using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : LivingEntity
{
    #region Inspector Variables

    [SerializeField] protected Action[] actions;

    [SerializeField] private int moveRange;

    [SerializeField] private int attackRange;

    [SerializeField] private int specialAttackRange;

    [SerializeField] protected float turnTime;

    [SerializeField] protected float walkSpeed;

    [SerializeField] protected Element element;

    #endregion

    private bool canMove;

    private bool canAttack;

    private bool isSelected;

    protected Navigation navigation;

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

        navigation = GameObject.FindGameObjectWithTag("Grid").GetComponent<Navigation>();

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

    public void SnapToGrid(Tile a_targetTile)
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

    public void Attack(Tile[] targetTiles, System.Action start, System.Action finished)
    {
        finishedAction = finished;

        StartCoroutine(TurnToAttack(targetTiles[0]));
    }

    private IEnumerator TurnToAttack(Tile targetTile)
    {
        yield return StartCoroutine(Turn(targetTile.transform.position));

        // m_Animator.SetTrigger(actions[1].AnimTrigger);

        // m_CurrentClipInfo = m_Animator.GetCurrentAnimatorClipInfo(0);

        // yield return new WaitForSeconds(m_CurrentClipInfo[0].clip.length * 0.8f);

        CanAttack = false;

        finishedAction();
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

    public void SpecialAttack(Tile[] targetTiles, System.Action start, System.Action finished)
    {
        finishedAction = finished;

        // StartCoroutine(TurnToAttack(targetUnit));
    }

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

    public void Spawn(Tile a_startingTile)
    {
        currentTile = a_startingTile;

        currentTile.Enter(this);
    }

    public void MoveTo(Tile a_targetTile, System.Action a_finished)
    {
        finishedAction = a_finished;

        List<Tile> path = navigation.FindPath(currentTile, a_targetTile);

        if (path != null)
        {
            StartCoroutine(Walk(path));
        }
        else
        {
            Debug.Log("Couldn't find path");
        }
    }

    private IEnumerator Walk(List<Tile> path)
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
        finishedAction();
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