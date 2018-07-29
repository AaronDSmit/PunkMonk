
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;

    [HideInInspector]
    [SerializeField] private int mapWidth;

    [HideInInspector]
    [SerializeField] private int mapHeight;

    [HideInInspector]
    [SerializeField] private List<Tile> map = new List<Tile>();

    [HideInInspector]
    [SerializeField] private Dictionary<string, Tile> grid = new Dictionary<string, Tile>();

    private bool generateWithColour = false;

    private Color traversableTileColour;

    private Color blockedTileColour;

    private Color connectionColour;

    private void Start()
    {
        grid.Clear();

        Tile[] tiles = GetComponentsInChildren<Tile>();

        foreach (Tile tile in tiles)
        {
            grid.Add(tile.ToString(), tile);
        }

        StateManager.OnGameStateChanged += GameStateChanged;
    }

    public bool GenerateGrid(int a_width, int a_height, Color a_walkable, Color a_notWalkable, Color a_connection)
    {
        traversableTileColour = a_walkable;
        blockedTileColour = a_notWalkable;
        connectionColour = a_connection;

        generateWithColour = true;

        return GenerateGrid(a_width, a_height);
    }

    public bool GenerateGrid(int width, int height)
    {
        mapWidth = width;
        mapHeight = height;

        if (transform.childCount > 0)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        map.Clear();
        grid.Clear();

        for (int y = 0, i = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                CreateTile(x, y, i++);
            }
        }

        generateWithColour = false;

        return (transform.childCount > 0);
    }

    private void CreateTile(int x, int y, int i)
    {
        Tile newHex = Instantiate(tilePrefab, new Vector3(), Quaternion.Euler(-90.0f, 0.0f, 0.0f), transform);

        newHex.Init(x, y);

        //GameObject textGO = new GameObject("Text Display");
        //TextMesh text = textGO.AddComponent<TextMesh>();

        //text.transform.parent = newHex.transform;
        //text.transform.position = newHex.transform.position;
        //text.transform.localEulerAngles = new Vector3(180, 0.0f, 0.0f);
        //text.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);

        //text.text = newHex.ToString();
        //text.characterSize = 0.2f;
        //text.fontSize = 60;
        //text.anchor = TextAnchor.MiddleCenter;

        Vector3 position;
        position.x = (x + y * 0.5f - y / 2) * (Tile.innerRadius * 2f);
        position.y = transform.position.y;
        position.z = y * (Tile.outerRadius * 1.5f);

        newHex.transform.position = position;
        newHex.name = newHex.ToString();

        map.Add(newHex);

        grid.Add(newHex.ToString(), newHex);

        if (x > 0)
        {
            map[i].SetNeighbour(HexDirection.W, map[i - 1]);
        }
        if (y > 0)
        {
            if ((y & 1) == 0)
            {
                map[i].SetNeighbour(HexDirection.SE, map[i - mapWidth]);
                if (x > 0)
                {
                    map[i].SetNeighbour(HexDirection.SW, map[i - mapWidth - 1]);
                }
            }
            else
            {
                map[i].SetNeighbour(HexDirection.SW, map[i - mapWidth]);
                if (x < mapWidth - 1)
                {
                    map[i].SetNeighbour(HexDirection.SE, map[i - mapWidth + 1]);
                }
            }
        }

        if (generateWithColour)
        {
            map[i].walkableColour = traversableTileColour;
            map[i].notWalkableColour = blockedTileColour;
            map[i].connectionColour = connectionColour;
        }
    }

    private void GameStateChanged(Game_state _oldstate, Game_state _newstate)
    {
        // ensure this script knows it's in over-world state
        if (_newstate == Game_state.battle)
        {
            Tile[] tiles = GetComponentsInChildren<Tile>();

            foreach (Tile tile in tiles)
            {
                tile.ShowGrid(true);
            }
        }
        else
        {
            Tile[] tiles = GetComponentsInChildren<Tile>();

            foreach (Tile tile in tiles)
            {
                tile.ShowGrid(false);
            }
        }
    }

    public Tile[] GetTilesWithinDistance(Tile centerTile, int range)
    {
        List<Tile> openList = new List<Tile>();
        List<Tile> returnList = new List<Tile>();
        Tile currentTile;

        openList.Add(centerTile);

        while (openList.Count != 0)
        {
            currentTile = openList[0];
            openList.Remove(currentTile);

            foreach (Tile neighbour in currentTile.Neighbours)
            {
                if (neighbour.IsWalkable && !returnList.Contains(neighbour))
                {
                    neighbour.GScore = Tile.Distance(currentTile, neighbour) + currentTile.GScore;
                    if (neighbour.GScore < range + 1)
                    {
                        openList.Add(neighbour);
                        returnList.Add(neighbour);
                    }
                }
            }

            openList.Sort((x, y) => x.GScore.CompareTo(y.GScore));
        }

        foreach (Tile hex in returnList)
        {
            hex.GScore = 0;
        }

        return returnList.ToArray();
    }

    //public Tile[] GetTilesWithinRangeOf(Tile centerTile, int range)
    //{
    //    //Return tiles range steps from centre, http://www.redblobgames.com/grids/hexagons/#range

    //    List<Tile> result = new List<Tile>();

    //    for (int dx = -range; dx <= range; dx++)
    //    {
    //        for (int dy = Mathf.Max(-range, -dx - range); dy <= Mathf.Min(range, -dx + range); dy++)
    //        {
    //            Tile h = GetTileAt(centerTile.Q + dx, centerTile.R + dy);

    //            if (h != null)
    //            {
    //                result.Add(h);
    //            }
    //        }
    //    }

    //    return result.ToArray();
    //}

    public Tile GetTileAt(int x, int z)
    {
        string index = string.Format("{0},{1}", x, z);

        if (grid.ContainsKey(index.ToString()))
        {
            return grid[index.ToString()];
        }

        return null;
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