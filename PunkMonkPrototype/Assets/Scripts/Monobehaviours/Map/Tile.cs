using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Status { NONE, OIL, FIRE, WATER, FAULTLINE, ABYSS }

[SelectionBase]
public class Tile : Entity
{
    #region Inspector Variables

    [SerializeField] private Tile[] neighbours;

    [SerializeField] private List<Tile> allNeighbours; // TODO: populate this list

    [SerializeField] private float spreadDelay;

    [SerializeField] private Color oilColour;

    [SerializeField] private Color fireColour;

    [SerializeField] private Color waterColour;

    [SerializeField] private Color faultLineColour;

    [SerializeField] private Color abyssColour;

    [SerializeField] private Status currentStatus;

    // Node values

    [SerializeField] private bool isWalkable = true;

    [SerializeField] private int terrainDifficulty = 0;

    #endregion

    [HideInInspector]
    [SerializeField] public bool drawGizmos = true;

    [HideInInspector]
    [SerializeField] public bool drawNode = true;

    [HideInInspector]
    [SerializeField] public bool drawConnections = true;

    [HideInInspector]
    [SerializeField] public Color walkableColour;

    [HideInInspector]
    [SerializeField] public Color notWalkableColour;

    [HideInInspector]
    [SerializeField] public Color connectionColour;

    private float gScore;

    private SpriteRenderer movementHighlight;

    private Unit currentUnit;

    private void Awake()
    {
        //movementHighlight = GetComponentsInChildren<SpriteRenderer>()[1];

        //renderer = GetComponentInChildren<Renderer>();
    }

    #region API

    public void IncreaseHeight(float stepSize)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + stepSize, transform.position.z);
    }

    public void DecreaseHeight(float stepSize)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - stepSize, transform.position.z);
    }

    public void SetHeight(float height)
    {
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
    }

    public void SetStatus(Status status)
    {
        currentStatus = status;

        ChangeColour();
    }

    public Unit CurrentUnit
    {
        get { return currentUnit; }
    }

    public void Enter(Unit unit)
    {
        currentUnit = unit;

        if (currentStatus == Status.FIRE)
        {
            currentUnit.TakeDamage(Element.FIRE, 25);
        }

        isWalkable = false;
    }

    public void Exit()
    {
        currentUnit = null;
        isWalkable = true;
    }

    public void HighlightMovement(bool highlight, Color colour)
    {
        movementHighlight.enabled = highlight;
        colour.a = 0.4f;
        movementHighlight.color = colour;
    }

    public void RemoveHighlight()
    {
        movementHighlight.enabled = false;
    }

    public void SetNeighbour(HexDirection direction, Tile cell)
    {
        neighbours[(int)direction] = cell;
        cell.neighbours[(int)direction.Opposite()] = this;
    }

    public Tile GetNeighbour(HexDirection direction)
    {
        if ((int)direction > HexDirectionExtensions.DirectionCount())
        {
            direction = (HexDirection)((int)direction & HexDirectionExtensions.DirectionCount());
        }

        if ((int)direction < 0)
        {
            direction = (HexDirection)(HexDirectionExtensions.DirectionCount() & (int)direction);
        }

        return neighbours[(int)direction];
    }

    public bool IsWalkable
    {
        set { isWalkable = value; }
    }

    public List<Tile> Neighbours
    {
        get { return allNeighbours; }
    }

    public override void TakeDamage(Element damageType, float damageAmount)
    {
        if (currentStatus == Status.OIL && damageType == Element.FIRE)
        {
            currentStatus = Status.FIRE;

            if (currentUnit)
            {
                currentUnit.TakeDamage(damageType, damageAmount);
            }

            ChangeColour();

            StartCoroutine(SpreadStatus(Element.FIRE));
        }

        if (currentStatus == Status.FAULTLINE && damageType == Element.EARTH)
        {
            currentStatus = Status.ABYSS;
            isWalkable = false;

            if (currentUnit)
            {
                // well fuck
            }

            ChangeColour();

            StartCoroutine(SpreadStatus(Element.EARTH));
        }
    }

    private IEnumerator SpreadStatus(Element damageType)
    {
        yield return new WaitForSeconds(spreadDelay);

        foreach (Tile tile in neighbours)
        {
            tile.TakeDamage(damageType, 25);
        }
    }

    #endregion

    // https://answers.unity.com/questions/283271/material-leak-in-editor.html?childToView=322397#answer-322397

    private void ChangeColour()
    {
        if (Application.isEditor)
        {
            Renderer myRenderer = GetComponentInChildren<Renderer>();
            Material tempMaterial;

            switch (currentStatus)
            {
                case Status.NONE:

                    tempMaterial = new Material(myRenderer.sharedMaterial)
                    {
                        color = Color.white
                    };

                    myRenderer.sharedMaterial = tempMaterial;
                    break;
                case Status.OIL:

                    tempMaterial = new Material(myRenderer.sharedMaterial)
                    {
                        color = oilColour
                    };

                    myRenderer.sharedMaterial = tempMaterial;

                    break;
                case Status.FIRE:

                    tempMaterial = new Material(myRenderer.sharedMaterial)
                    {
                        color = fireColour
                    };

                    myRenderer.sharedMaterial = tempMaterial;

                    break;
                case Status.WATER:

                    tempMaterial = new Material(myRenderer.sharedMaterial)
                    {
                        color = waterColour
                    };

                    myRenderer.sharedMaterial = tempMaterial;

                    break;
                case Status.FAULTLINE:

                    tempMaterial = new Material(myRenderer.sharedMaterial)
                    {
                        color = faultLineColour
                    };

                    myRenderer.sharedMaterial = tempMaterial;

                    break;
                case Status.ABYSS:

                    tempMaterial = new Material(myRenderer.sharedMaterial)
                    {
                        color = abyssColour
                    };

                    myRenderer.sharedMaterial = tempMaterial;

                    break;
            }
        }
        else
        {
            //switch (currentStatus)
            //{
            //    case Status.NONE:
            //        renderer.material.color = Color.white;
            //        break;
            //    case Status.OIL:
            //        renderer.material.color = oilColour;
            //        break;
            //    case Status.FIRE:
            //        renderer.material.color = fireColour;
            //        break;
            //    case Status.WATER:
            //        renderer.material.color = waterColour;
            //        break;
            //    case Status.FAULTLINE:
            //        renderer.material.color = faultLineColour;
            //        break;
            //    case Status.ABYSS:
            //        renderer.material.color = abyssColour;
            //        break;
            //}
        }
    }

    private void OnDrawGizmos()
    {
        if (drawNode)
        {
            Gizmos.color = (isWalkable) ? walkableColour : notWalkableColour;
            Gizmos.DrawSphere(transform.position, 0.3f);
        }

        if (drawConnections)
        {
            if (neighbours.Length > 0)
            {
                Gizmos.color = connectionColour;

                foreach (Tile tile in neighbours)
                {
                    if (tile != null)
                    {
                        DrawArrow.ForGizmo(transform.position + (tile.transform.position - transform.position) * 0.2f, (tile.transform.position - transform.position) * 0.6f);
                    }
                }
            }
        }
    }

    #region Hex Data

    public void Init(int q, int r)
    {
        this.q = q;
        this.r = r;
        s = -(q + r);

        neighbours = new Tile[6];
    }

    // Q + R + S = 0
    // S = -(Q + R)

    [SerializeField, HideInInspector]
    private int q;  // Column
    [SerializeField, HideInInspector]
    private int r;  // Row
    [SerializeField, HideInInspector]
    private int s;

    static readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2;

    private float radius = 1f;

    public int Q
    {
        get { return q; }
    }

    public int R
    {
        get { return r; }
    }

    public int S
    {
        get { return s; }
    }

    public Vector3 WorldPosition()
    {
        return new Vector3(
            HexHorizontalSpacing() * (Q + R / 2f),
            0,
            HexVerticalSpacing() * R
        );
    }

    public override string ToString()
    {
        return q + "," + r;
    }

    private float HexHeight()
    {
        return radius * 2;
    }

    private float HexWidth()
    {
        return WIDTH_MULTIPLIER * HexHeight();
    }

    private float HexVerticalSpacing()
    {
        return HexHeight() * 0.75f;
    }

    private float HexHorizontalSpacing()
    {
        return HexWidth();
    }

    public static float Distance(Tile a, Tile b)
    {
        int dQ = Mathf.Abs(a.Q - b.Q);
        int dR = Mathf.Abs(a.R - b.R);

        return Mathf.Max(dQ, dR, Mathf.Abs(a.S - b.S));
    }

    public const float outerRadius = 1;

    public const float innerRadius = outerRadius * 0.866025404f;

    #endregion
}