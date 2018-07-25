
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

    private void Awake()
    {
        Tile[] tiles = GetComponentsInChildren<Tile>();

        foreach (Tile tile in tiles)
        {
            grid.Add(tile.ToString(), tile);
        }
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

        return (transform.childCount > 0);
    }

    private void CreateTile(int x, int y, int i)
    {
        Tile newHex = Instantiate(tilePrefab, new Vector3(), Quaternion.Euler(-90.0f, 0.0f, 0.0f), transform);

        newHex.Init(x, y);

        Vector3 position;
        position.x = (x + y * 0.5f - y / 2) * (Tile.innerRadius * 2f);
        position.y = 0f;
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