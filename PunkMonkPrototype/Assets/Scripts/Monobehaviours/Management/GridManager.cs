using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A template to follow for all unity C# classes. Created with help from: https://medium.com/@akb1ggs/structuring-your-unity-monobehaviours-df090b587110
/// </summary>
public class GridManager : MonoBehaviour
{
    #region Unity Inspector Fields

    [SerializeField]
    private Hex tilePrefab;

    [SerializeField]
    private GameObject overlay;

    [SerializeField]
    private bool fadeIn;

    [SerializeField]
    private bool dropIn;

    [SerializeField]
    private float slideSpeed = 1;

    [SerializeField]
    private float starting;

    [SerializeField]
    private float target;

    [SerializeField]
    private float overshoot;

    #endregion

    #region Local Fields

    [HideInInspector]
    [SerializeField]
    private int mapWidth;

    [HideInInspector]
    [SerializeField]
    private int mapHeight;

    [HideInInspector]
    [SerializeField]
    private Hex[] grid;

    [HideInInspector]
    [SerializeField]
    private Color traversableColour;

    private GameObject gridMesh;

    protected Renderer gridRenderer;

    private bool isReady;

    #endregion

    #region Properties

    public int MapWidth
    {
        get { return mapWidth; }
    }

    public int Mapheight
    {
        get { return mapHeight; }
    }

    public bool Ready
    {
        get { return isReady; }
    }

    #endregion

    #region Public Methods

    public Hex[] GetHexLine(Hex a_lhs, Hex a_rhs)
    {
        List<Hex> hexs = new List<Hex>();

        int distance = HexUtility.Distance(a_lhs, a_rhs);

        float scale = HexUtility.outerRadius;

        for (int i = 0; i < distance; i++)
        {
            hexs.Add(GetHexFromPosition(Vector3.Lerp(a_lhs.transform.position, a_rhs.transform.position, scale / distance * i)));
        }

        return hexs.ToArray();
    }


    public void Init()
    {
        StartCoroutine(RemoveExcessTiles());
    }

    public bool GenerateGrid(int width, int height, Color a_traversableColour)
    {
        mapWidth = width;
        mapHeight = height;

        traversableColour = a_traversableColour;

        if (transform.childCount > 0)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        grid = new Hex[mapWidth * mapHeight];

        for (int y = 0, i = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                CreateHex(x, y, i++);
            }
        }

        return (transform.childCount > 0);
    }

    public List<Hex> GetTilesWithinDistance(Hex centerTile, int range, bool CheckIfTraversable = true, bool addOccupiedTiles = false)
    {
        List<Hex> openList = new List<Hex>();
        List<Hex> returnList = new List<Hex>();
        Hex currentTile;

        centerTile.GScore = 0;

        openList.Add(centerTile);

        while (openList.Count != 0)
        {
            currentTile = openList[0];
            openList.Remove(currentTile);

            foreach (Hex neighbour in currentTile.Neighbours)
            {
                if (CheckIfTraversable)
                {
                    if (addOccupiedTiles == true)
                    {
                        if ((neighbour.IsTraversable && !returnList.Contains(neighbour)) || (neighbour.CurrentUnit != null && !returnList.Contains(neighbour)))
                        {
                            neighbour.GScore = HexUtility.Distance(currentTile, neighbour) + currentTile.GScore;
                            if (neighbour.GScore < range + 1)
                            {

                                openList.Add(neighbour);
                                returnList.Add(neighbour);
                            }
                        }
                    }
                    else
                    {
                        if (neighbour.IsTraversable && !returnList.Contains(neighbour))
                        {
                            neighbour.GScore = HexUtility.Distance(currentTile, neighbour) + currentTile.GScore;
                            if (neighbour.GScore < range + 1)
                            {

                                openList.Add(neighbour);
                                returnList.Add(neighbour);
                            }
                        }
                    }



                }
                else if (!returnList.Contains(neighbour))
                {
                    neighbour.GScore = HexUtility.Distance(currentTile, neighbour) + currentTile.GScore;
                    if (neighbour.GScore < range + 1)
                    {
                        openList.Add(neighbour);
                        returnList.Add(neighbour);
                    }
                }
            }

            openList.Sort((x, y) => x.GScore.CompareTo(y.GScore));
        }

        foreach (Hex hex in returnList)
        {
            hex.GScore = 0;
        }

        return returnList;
    }

    public Hex GetHexFromPosition(Vector3 a_position)
    {
        a_position = transform.InverseTransformPoint(a_position);

        OffsetCoord offsetCoord = HexCoordinate.PositionToOffset(a_position);

        int index = offsetCoord.y * MapWidth + offsetCoord.x;

        if (index < MapWidth * mapHeight)
        {
            return grid[index];
        }

        return null;
    }

    [ContextMenu("Bake Mesh")]
    public void BakeMesh()
    {
        if (gridMesh != null)
        {
            Destroy(gridMesh);
        }

        gridMesh = new GameObject();
        gridMesh.AddComponent<MeshFilter>();
        gridMesh.AddComponent<MeshRenderer>();
        gridMesh.AddComponent<GridMesh>();
        gridMesh.GetComponent<GridMesh>().Triangulate(grid);
    }

    public Hex GetHexAt(int x, int z)
    {
        return grid[z * MapWidth + x];
    }

    #endregion

    #region Unity Life-cycle Methods

    private void Awake()
    {
        Manager.instance.StateController.OnGameStateChanged += GameStateChanged;

        if (overlay)
        {
            gridRenderer = overlay.GetComponent<Renderer>();
        }
    }

    #endregion

    #region Local Methods

    private IEnumerator RemoveExcessTiles()
    {
        int traversableCount = 0;
        int perFrame = 5;
        int count = 0;

        foreach (Hex tile in grid)
        {
            traversableCount = 0;

            foreach (Hex neighbour in tile.Neighbours)
            {
                if (neighbour.IsTraversable)
                {
                    traversableCount++;
                    break; // don't look at any more neighbours if one is traversable
                }
            }

            if (traversableCount == 0)
            {

                tile.gameObject.SetActive(false);
                if (tile.transform.childCount > 1)
                {
                    tile.gameObject.SetActive(true);
                }
            }

            if (count >= perFrame)
            {
                count = 0;
                yield return null;
            }
        }

        isReady = true;
    }

    private void CreateHex(int x, int y, int i)
    {
        Vector3 position;
        position.x = (x + y * 0.5f - y / 2) * (HexUtility.innerRadius * 2f);
        position.y = transform.position.y;
        position.z = y * (HexUtility.outerRadius * 1.5f);

        grid[i] = Instantiate(tilePrefab, position, Quaternion.Euler(-90.0f, 0.0f, 0.0f), transform);

        grid[i].Init(x, y, traversableColour);

        grid[i].transform.localScale = new Vector3(HexUtility.outerRadius, HexUtility.outerRadius, HexUtility.outerRadius);

        grid[i].transform.position = position;
        grid[i].name = string.Format("{0}, {1}", x, y);

        if (x > 0)
        {
            grid[i].SetNeighbour(HexDirection.W, grid[i - 1]);
        }
        if (y > 0)
        {
            if ((y & 1) == 0)
            {
                grid[i].SetNeighbour(HexDirection.SE, grid[i - mapWidth]);

                if (x > 0)
                {
                    grid[i].SetNeighbour(HexDirection.SW, grid[i - mapWidth - 1]);
                }
            }
            else
            {
                grid[i].SetNeighbour(HexDirection.SW, grid[i - mapWidth]);

                if (x < mapWidth - 1)
                {
                    grid[i].SetNeighbour(HexDirection.SE, grid[i - mapWidth + 1]);
                }
            }
        }
    }

    private void GameStateChanged(GameState a_oldstate, GameState a_newstate)
    {
        // Show or hide the grid during state transition rather than on state change
        if (a_newstate == GameState.transition)
        {
            // we're not in battle but transitioning to battle
            if (Manager.instance.StateController.StateAfterTransition == GameState.battle)
            {
                Hex[] tiles = GetComponentsInChildren<Hex>();

                foreach (Hex tile in tiles)
                {

                    if (tile.transform.childCount > 1)
                    {
                        bool neighboursAreNotTreversable = false;
                        foreach (Hex neighbour in tile.Neighbours)
                        {
                            if (neighbour.IsTraversable == true)
                            {
                                neighboursAreNotTreversable = true;
                                break;
                            }
                        }

                        if (neighboursAreNotTreversable)
                        {
                            tile.ShowOverlay(true);
                        }
                        else
                        {
                            tile.ShowOverlay(false);
                        }
                    }
                    else
                    {
                        tile.ShowOverlay(true);
                    }
                }

                if (overlay)
                {
                    overlay.SetActive(true);

                    if (fadeIn)
                    {
                        StartCoroutine(FadeGrid(0, 1, StateManager.stateTransitionTime));
                    }

                    if (dropIn)
                    {
                        overlay.transform.position = new Vector3(overlay.transform.position.x, starting, overlay.transform.position.z);
                        StartCoroutine(SlideVertical(target, overshoot));
                    }
                }
            }
            else if (a_oldstate == GameState.battle)
            {
                Hex[] tiles = GetComponentsInChildren<Hex>();

                foreach (Hex tile in tiles)
                {
                    tile.ShowOverlay(false);
                }

                if (overlay)
                {
                    if (fadeIn)
                    {
                        StartCoroutine(FadeGrid(1, 0, StateManager.stateTransitionTime));
                    }

                    if (dropIn)
                    {
                        StartCoroutine(SlideVertical(starting, overshoot));
                    }
                }
            }
        }
    }

    private IEnumerator FadeGrid(float from, float to, float time)
    {
        float currentLerpTime = 0;
        float t = 0;

        while (t < 1)
        {
            currentLerpTime += Time.deltaTime;
            t = currentLerpTime / time;

            gridRenderer.material.SetFloat("_Alpha", Mathf.Lerp(from, to, t));

            yield return null;
        }

        gridRenderer.material.SetFloat("_Alpha", to);
    }

    private IEnumerator SlideVertical(float target, float overshoot)
    {
        while (overlay.transform.position.y < overshoot)
        {
            overlay.transform.Translate(Vector2.up * Time.deltaTime * slideSpeed);
            yield return null;
        }

        while (overlay.transform.position.y > target)
        {
            overlay.transform.Translate(Vector3.down * Time.deltaTime * slideSpeed);

            yield return null;
        }

        overlay.transform.position = new Vector3(overlay.transform.position.x, target, overlay.transform.position.z);
    }

    #endregion
}