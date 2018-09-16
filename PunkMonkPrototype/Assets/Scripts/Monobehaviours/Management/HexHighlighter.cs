using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;

public class HexHighlighter : MonoBehaviour
{
    #region Unity Inspector Fields

    [Tooltip("Percentage Distance from edge to centre of hex")]
    [SerializeField]
    private float borderThickness = 0.1f;

    [Tooltip("Material used for outer border of area")]
    [SerializeField]
    private Material borderMaterial = null;

    [Tooltip("Material used for inside of area")]
    [SerializeField]
    private Material fillMaterial = null;

    #endregion

    #region Reference Fields

    private Transform holder;

    #endregion

    #region Local Fields

    private Dictionary<MonoBehaviour, List<GameObject>> highlightedAreas;

    private float inverseThickness;

    private List<Hex> border;

    private List<Vector3> vertices;
    private List<Vector2> uvs;
    private List<int> triangles;

    #endregion

    #region Public Methods

    public void Init()
    {
        border = new List<Hex>();

        vertices = new List<Vector3>();
        uvs = new List<Vector2>();
        triangles = new List<int>();

        highlightedAreas = new Dictionary<MonoBehaviour, List<GameObject>>();
    }

    /// <summary>
    /// Add a new highlighted area that belongs to a script
    /// </summary>
    /// <param name="a_area">List of hexes that make an area</param>
    /// <param name="a_borderColour">Colour of line that marks edge of area</param>
    /// <param name="a_fillColour">Colour of the filled area inside the border</param>
    /// <param name="a_script">The script which owns this area</param>
    /// <param name="a_excludedHexes">List of hexes that shouldn't be highlighted</param>
    public void HighLightArea(List<Hex> a_area, Color a_borderColour, Color a_fillColour, MonoBehaviour a_script, List<Hex> a_excludedHexes = null)
    {
        // invert the thickness available in the inspector
        inverseThickness = 1.0f - borderThickness;

        // And new dictionary key if doesn't exist
        if (!highlightedAreas.ContainsKey(a_script))
        {
            highlightedAreas.Add(a_script, new List<GameObject>());
        }

        // Create two new game objects, one for border and one for inner area
        GameObject areaGO = new GameObject("AreaHighlight");
        areaGO.transform.SetParent(holder, false);

        // border gameObject is a child of the area gameObject
        GameObject borderGO = new GameObject("BorderHighlight");
        borderGO.transform.SetParent(areaGO.transform, false);

        borderGO.transform.position = new Vector3(0.0f, areaGO.transform.position.y + 0.01f);

        // add the area gameObject to the script which called this function
        highlightedAreas[a_script].Add(areaGO);

        // Add a MeshRenderer and MeshFilter to both gameObjects
        MeshRenderer areaRenderer = areaGO.AddComponent<MeshRenderer>();
        areaRenderer.material = fillMaterial;
        areaRenderer.material.color = a_fillColour;

        MeshRenderer borderRenderer = borderGO.AddComponent<MeshRenderer>();
        borderRenderer.material = borderMaterial;
        borderRenderer.material.color = a_borderColour;

        Mesh borderMesh = borderGO.AddComponent<MeshFilter>().mesh;
        Mesh areaMesh = areaGO.AddComponent<MeshFilter>().mesh;

        Clear();

        TriangulateBorder(ref a_area, ref a_excludedHexes);

        borderMesh.SetVertices(vertices);
        borderMesh.SetUVs(0, uvs);
        borderMesh.SetTriangles(triangles, 0);

        borderMesh.RecalculateNormals();

        Clear();

        TriangulateArea(ref a_area, ref a_excludedHexes);

        areaMesh.SetVertices(vertices);
        areaMesh.SetUVs(0, uvs);
        areaMesh.SetTriangles(triangles, 0);

        areaMesh.RecalculateNormals();
    }

    // Remove all highlighted areas for a particular script
    public void RemoveHighlights(MonoBehaviour a_script)
    {
        if (highlightedAreas.ContainsKey(a_script))
        {
            List<GameObject> objects = highlightedAreas[a_script];

            for (int i = objects.Count - 1; i >= 0; i--)
            {
                Destroy(objects[i]);
            }

            highlightedAreas.Remove(a_script);
        }
    }

    #endregion

    #region Unity Life-cycle Methods

    private void Awake()
    {
        
        Transform grid = GameObject.FindGameObjectWithTag("Grid").transform;

        holder = new GameObject("Highlight Container").transform;

        holder.position = new Vector3(0.0f, grid.position.y + 0.05f);
    }

    #endregion

    #region local Functions

    private void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;

        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    void Triangulate(Hex hex)
    {
        Vector3 center = hex.transform.localPosition;

        for (int i = 0; i < 6; i++)
        {
            AddTriangle(
                center,
                center + HexUtility.corners[i],
                center + HexUtility.corners[i + 1]
            );
        }
    }

    private void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector3 uv3)
    {
        uvs.Add(uv1);
        uvs.Add(uv2);
        uvs.Add(uv3);
    }

    private void Clear()
    {
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
        border.Clear();
    }

    private void TriangulateArea(ref List<Hex> a_area, ref List<Hex> a_exclude)
    {
        bool process = false;

        foreach (Hex tile in a_area)
        {
            process = a_exclude != null ? !a_exclude.Contains(tile) : true;

            // Don't process tiles in the excluded list
            if (process)
            {
                Triangulate(tile);
            }
        }
    }

    private void TriangulateBorder(ref List<Hex> a_area, ref List<Hex> a_exclude)
    {
        bool process = false;

        // Process each hex in area by checking if one of their neighbours are outside the area, and add them to the border if true
        foreach (Hex tile in a_area)
        {
            process = a_exclude != null ? !a_exclude.Contains(tile) : true;

            // Don't process tiles in the excluded list
            if (process)
            {
                foreach (Hex neighbour in tile.Neighbours)
                {
                    // check if the neighbour is outside of the area, if it's on the excluded list then don't check (useful when a unit is blocking a hex but shouldn’t have a border)
                    if (!a_area.Contains(neighbour) && (a_exclude != null && !a_exclude.Contains(neighbour)))
                    {
                        border.Add(tile);
                        break; // break because if one neighbour is outside the area then it's a border hex
                    }
                }
            }
        }

        // process each hex in border and add the points that border the outside of the area
        foreach (Hex tile in border)
        {
            for (int i = 0; i < tile.Neighbours.Count; i++)
            {
                HexDirection direction = HexDirectionUtility.DirectionFromNeighbour(tile, tile.Neighbours[i]);

                Vector3 centre = tile.transform.localPosition;

                process = a_exclude != null ? !a_exclude.Contains(tile.Neighbours[i]) : true;

                if (!a_area.Contains(tile.Neighbours[i]) && process)
                {
                    AddTriangle(
                        centre + HexUtility.GetFirstCorner(direction),
                        centre + HexUtility.GetSecondCorner(direction),
                        centre + HexUtility.GetFirstCorner(direction) * inverseThickness
                        );

                    AddTriangleUV(
                        centre + HexUtility.GetFirstCorner(direction),
                        centre + HexUtility.GetSecondCorner(direction),
                        centre + HexUtility.GetFirstCorner(direction) * inverseThickness
                        );

                    AddTriangle(
                        centre + HexUtility.GetSecondCorner(direction) * inverseThickness,
                        centre + HexUtility.GetFirstCorner(direction) * inverseThickness,
                        centre + HexUtility.GetSecondCorner(direction)
                        );

                    AddTriangleUV(
                        centre + HexUtility.GetSecondCorner(direction) * inverseThickness,
                        centre + HexUtility.GetFirstCorner(direction) * inverseThickness,
                        centre + HexUtility.GetSecondCorner(direction)
                        );
                }
                else if (border.Contains(tile.Neighbours[i]))
                {
                    int closestCorner = ClosestToOutside(tile, centre + HexUtility.GetFirstCorner(direction), centre + HexUtility.GetSecondCorner(direction), ref a_area, ref a_exclude);

                    if (closestCorner == 2)
                    {
                        AddTriangle(
                        centre + HexUtility.GetSecondCorner(direction),
                        centre + HexUtility.GetSecondCorner(direction) * inverseThickness,
                        centre + HexUtility.GetSecondCorner(direction) + (HexUtility.GetFirstCorner(direction) - HexUtility.GetSecondCorner(direction)) * borderThickness
                        );

                        AddTriangleUV(
                        centre + HexUtility.GetSecondCorner(direction),
                        centre + HexUtility.GetSecondCorner(direction) * inverseThickness,
                        centre + HexUtility.GetSecondCorner(direction) + (HexUtility.GetFirstCorner(direction) - HexUtility.GetSecondCorner(direction)) * borderThickness
                        );
                    }
                    else if (closestCorner == 1)
                    {
                        AddTriangle(
                        centre + HexUtility.GetFirstCorner(direction),
                        centre + HexUtility.GetFirstCorner(direction) + (HexUtility.GetSecondCorner(direction) - HexUtility.GetFirstCorner(direction)) * borderThickness,
                        centre + HexUtility.GetFirstCorner(direction) * inverseThickness
                        );

                        AddTriangleUV(
                       centre + HexUtility.GetFirstCorner(direction),
                       centre + HexUtility.GetFirstCorner(direction) + (HexUtility.GetSecondCorner(direction) - HexUtility.GetFirstCorner(direction)) * borderThickness,
                       centre + HexUtility.GetFirstCorner(direction) * inverseThickness
                       );
                    }
                }
            }
        }
    }

    private int ClosestToOutside(Hex a_hex, Vector3 a_first, Vector3 a_second, ref List<Hex> a_area, ref List<Hex> a_exclude)
    {
        foreach (Hex neighbour in a_hex.Neighbours)
        {
            if (!a_area.Contains(neighbour))
            {
                float dist1 = Vector3.Distance(a_first, neighbour.transform.position);

                if (dist1 < 1.3f)
                {
                    return 1;
                }

                float dist2 = Vector3.Distance(a_second, neighbour.transform.position);

                if (dist2 < 1.3f)
                {
                    return 2;
                }
            }
        }

        return 0;
    }

    #endregion
}