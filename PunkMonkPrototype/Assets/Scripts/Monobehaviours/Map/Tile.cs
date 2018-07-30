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
    [SerializeField]
    public bool drawGizmos = true;

    [HideInInspector]
    [SerializeField]
    public bool drawNode = true;

    [HideInInspector]
    [SerializeField]
    public bool drawConnections = true;

    [HideInInspector]
    [SerializeField]
    public Color walkableColour;

    [HideInInspector]
    [SerializeField]
    public Color notWalkableColour;

    [HideInInspector]
    [SerializeField]
    public Color connectionColour;

    private float gScore;

    private SpriteRenderer movementHighlight;

    private SpriteRenderer hoverHighlight;

    private Unit currentUnit;

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

    public void ShowGrid(bool a_show)
    {
        GetComponentInChildren<SpriteRenderer>().enabled = a_show;
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

    public void HighlightMovement(Color colour)
    {
        movementHighlight.enabled = true;
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

        allNeighbours.Add(cell);
        cell.allNeighbours.Add(this);
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
        get { return isWalkable; }
        set { isWalkable = value; }
    }

    public float GScore
    {
        get { return gScore + terrainDifficulty; }
        set { gScore = value; }
    }

    public float HScore { get; set; }

    public Tile Parent { get; set; }

    public float FScore
    {
        get { return HScore + gScore + terrainDifficulty; }
    }

    public List<Tile> Neighbours
    {
        get { return allNeighbours; }
    }

    public void MouseEnter(Color highlightColour)
    {
        hoverHighlight.enabled = true;
        highlightColour.a = 0.4f;
        hoverHighlight.color = highlightColour;
    }

    public void MouseExit()
    {
        hoverHighlight.enabled = false;
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

    private void Awake()
    {
        movementHighlight = GetComponentsInChildren<SpriteRenderer>()[1];
        hoverHighlight = GetComponentsInChildren<SpriteRenderer>()[2];

        string[] coordText = name.Split(',');

        coord = new OffsetCoord(int.Parse(coordText[0]), int.Parse(coordText[1]));
    }

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

    [SerializeField]
    public OffsetCoord coord;

    public void Init(int x, int y)
    {
        coord = new OffsetCoord(x, y);

        neighbours = new Tile[6];
        allNeighbours = new List<Tile>();
    }

    public override string ToString()
    {
        return coord.x + "," + coord.y;
    }

    public static float Distance(Tile a, Tile b)
    {
        // convert offset coordinate to cube coordinate
        Cube ac = OffsetToCube(a.coord);
        Cube bc = OffsetToCube(b.coord);

        return CubeDistance(ac, bc);
    }

    private static float CubeDistance(Cube a, Cube b)
    {
        int dQ = Mathf.Abs(a.q - b.q);
        int dR = Mathf.Abs(a.r - b.r);

        return Mathf.Max(dQ, dR, Mathf.Abs(a.s - b.s));
    }

    public const float outerRadius = 1;

    public const float innerRadius = outerRadius * 0.866025404f;

    #endregion

    public static OffsetCoord CubeToOffset(Cube h)
    {
        int col = h.q + (h.r - (h.r & 1)) / 2;
        int row = h.r;

        return new OffsetCoord(col, row);
    }

    public static Cube OffsetToCube(OffsetCoord h)
    {
        int q = h.x - (h.y - (h.y & 1)) / 2;
        int r = h.y;
        int s = -q - r;

        return new Cube(q, r, s);
    }
}

[System.Serializable]
public struct Cube
{
    // Cube storage, cube constructor
    public readonly int q, r, s;

    public Cube(int _q, int _r, int _s)
    {
        q = _q;
        r = _r;
        s = _s;

        Debug.Assert(q + r + s == 0);
    }
};

[System.Serializable]
public struct OffsetCoord
{
    [SerializeField]
    public readonly int x, y;

    public OffsetCoord(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
};

