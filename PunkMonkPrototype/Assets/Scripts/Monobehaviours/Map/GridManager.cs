
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Hex tilePrefab;

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
    private Color clearHexColour;

    [HideInInspector]
    [SerializeField]
    private Color blockedHexColour;

    [HideInInspector]
    [SerializeField]
    private Color connectionColour;

    private void Awake()
    {
        StateManager.OnGameStateChanged += GameStateChanged;
    }

    public bool GenerateGrid(int width, int height, Color a_hexClearColour, Color a_hexBlockedColour, Color a_hexConnectColour)
    {
        mapWidth = width;
        mapHeight = height;

        clearHexColour = a_hexClearColour;
        blockedHexColour = a_hexBlockedColour;
        connectionColour = a_hexConnectColour;

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

    private void CreateHex(int x, int y, int i)
    {
        Vector3 position;
        position.x = (x + y * 0.5f - y / 2) * (HexUtility.innerRadius * 2f);
        position.y = transform.position.y;
        position.z = y * (HexUtility.outerRadius * 1.5f);

        grid[i] = Instantiate(tilePrefab, position, Quaternion.Euler(-90.0f, 0.0f, 0.0f), transform);

        grid[i].Init(x, y);

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

    private void GameStateChanged(Game_state _oldstate, Game_state _newstate)
    {
        // ensure this script knows it's in over-world state
        if (_newstate == Game_state.battle)
        {
            Hex[] tiles = GetComponentsInChildren<Hex>();

            foreach (Hex tile in tiles)
            {
                tile.ShowOverlay(true);
            }
        }
        else
        {
            Hex[] tiles = GetComponentsInChildren<Hex>();

            foreach (Hex tile in tiles)
            {
                tile.ShowOverlay(false);
            }
        }
    }

    public Hex[] GetTilesWithinDistance(Hex centerTile, int range, bool ignoreBlockedTiles = false)
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
                if (ignoreBlockedTiles)
                {
                    if (!returnList.Contains(neighbour))
                    {
                        neighbour.GScore = HexUtility.Distance(currentTile, neighbour) + currentTile.GScore;
                        if (neighbour.GScore < range + 1)
                        {
                            openList.Add(neighbour);
                            returnList.Add(neighbour);
                        }
                    }
                }
                else if (neighbour.IsTraversable && !returnList.Contains(neighbour))
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

        return returnList.ToArray();
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

    public Hex GetHexAt(int x, int z)
    {
        return grid[z * MapWidth + x];
    }

    public int MapWidth
    {
        get { return mapWidth; }
    }

    public int Mapheight
    {
        get { return mapHeight; }
    }
}