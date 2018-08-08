using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hex class makes up each tile within the grid.
/// 
/// 1) Hex is an entity.
/// 2) Hexes are stored using offset coordinates and can convert to and from cube coordinates.
/// 3) Hexes have 3 spriteRenderer's, 1 to display the border, 1 to show permanant highlight and 1 to show mouse hover highlight
/// 4) Neighbours are stored in order of their HexDirection and can be accessed by a HexDirection
/// 
/// </summary>

[SelectionBase]
public class Hex : Entity
{
    [HideInInspector]
    [SerializeField] private Hex[] neighbours;

    [HideInInspector]
    [SerializeField] private List<Hex> allNeighbours;

    [HideInInspector]
    [SerializeField] private bool isTraversable = true;

    [HideInInspector]
    [SerializeField]
    public OffsetCoord coordinates;

    [HideInInspector]
    [SerializeField]
    public float inaccessibleAlpha;

    private GameObject child;

    private SpriteRenderer highlight;

    private SpriteRenderer hover;

    #region API

    public void Init(int x, int y, Color a_colour)
    {
        coordinates = new OffsetCoord(x, y);

        neighbours = new Hex[6];
        allNeighbours = new List<Hex>();

        if (!highlight)
        {
            highlight = GetComponentsInChildren<SpriteRenderer>()[1];
        }

        highlight.color = a_colour;
    }

    public void ShowHighlight(bool a_show, Color a_traversable, Color a_inaccessable)
    {
        if (!highlight)
        {
            highlight = GetComponentsInChildren<SpriteRenderer>()[1];
        }

        highlight.enabled = a_show;

        highlight.color = (IsTraversable) ? a_traversable : a_inaccessable;
    }

    public OffsetCoord Coordinates
    {
        get { return coordinates; }
    }

    #region Unit occupation

    public Unit CurrentUnit { get; private set; }

    public void Enter(Unit unit)
    {
        CurrentUnit = unit;
        team = CurrentUnit.Team;

        isTraversable = false;
    }

    public void Exit()
    {
        CurrentUnit = null;
        isTraversable = true;

        team = TEAM.neutral;
    }

    #endregion

    #region Visual Functions

    // disable or enable all spriteRenders on the Hex
    public void ShowOverlay(bool a_showOverlay)
    {
        if (!child)
        {
            child = transform.GetChild(0).gameObject;
        }

        child.SetActive(a_showOverlay);
    }

    public void Highlight(Color a_colour)
    {
        if (!highlight)
        {
            highlight = GetComponentsInChildren<SpriteRenderer>()[1];
        }

        highlight.enabled = true;
        highlight.color = a_colour;
    }

    public void RemoveHighlight()
    {
        if (!highlight)
        {
            highlight = GetComponentsInChildren<SpriteRenderer>()[1];
        }

        highlight.enabled = false;
    }

    public void MouseEnter(Color a_colour)
    {
        if (!hover)
        {
            hover = GetComponentsInChildren<SpriteRenderer>()[2];
        }

        hover.enabled = true;
        hover.color = a_colour;
    }

    public void MouseExit()
    {
        if (!hover)
        {
            hover = GetComponentsInChildren<SpriteRenderer>()[2];
        }

        hover.enabled = false;
    }

    #endregion

    public void SetNeighbour(HexDirection direction, Hex cell)
    {
        neighbours[(int)direction] = cell;
        cell.neighbours[(int)direction.Opposite()] = this;

        allNeighbours.Add(cell);
        cell.allNeighbours.Add(this);
    }

    public Hex GetNeighbour(HexDirection direction)
    {
        if ((int)direction > HexDirectionUtility.DirectionCount())
        {
            direction = (HexDirection)((int)direction & HexDirectionUtility.DirectionCount());
        }

        if ((int)direction < 0)
        {
            direction = (HexDirection)(HexDirectionUtility.DirectionCount() & (int)direction);
        }

        return neighbours[(int)direction];
    }

    public void SetTraversable(bool a_isTraversable, Color a_colour)
    {
        isTraversable = a_isTraversable;

        if (!highlight)
        {
            highlight = GetComponentsInChildren<SpriteRenderer>()[1];
        }

        highlight.color = a_colour;
    }

    public bool IsTraversable
    {
        get { return isTraversable; }
    }

    public float GScore { get; set; }

    public float HScore { get; set; }

    public Hex Parent { get; set; }

    public float FScore
    {
        get { return HScore + GScore; }
    }

    public List<Hex> Neighbours
    {
        get { return allNeighbours; }
    }

    public void InaccessibleCheck(Color a_inaccessibleColour)
    {
        if (Physics.CheckSphere(transform.position + Vector3.up, 0.3f))
        {
            SetTraversable(false, a_inaccessibleColour);
        }
    }

    public void TraversableCheck(Color a_traversableColour)
    {
        if (!Physics.CheckSphere(transform.position + Vector3.up, 0.3f))
        {
            SetTraversable(true, a_traversableColour);
        }
    }

    public override void TakeDamage(float damageAmount)
    {

    }

    #endregion

    private void Awake()
    {
        if (transform.childCount == 0)
        {
            Destroy(gameObject);
        }
        else
        {
            child = transform.GetChild(0).gameObject;

            if (!isTraversable)
            {
                SpriteRenderer border = GetComponentsInChildren<SpriteRenderer>()[0];
                border.color = new Color(border.color.r, border.color.g, border.color.b, inaccessibleAlpha);
            }

            highlight = GetComponentsInChildren<SpriteRenderer>()[1];
            hover = GetComponentsInChildren<SpriteRenderer>()[2];

            highlight.enabled = false;

            string[] coordText = name.Split(',');

            coordinates = new OffsetCoord(int.Parse(coordText[0]), int.Parse(coordText[1]));

            for (int i = allNeighbours.Count - 1; i > -1; i--)
            {
                if (allNeighbours[i] == null || allNeighbours[i].Equals(null))
                {
                    allNeighbours.RemoveAt(i);
                }
            }
        }
    }
}