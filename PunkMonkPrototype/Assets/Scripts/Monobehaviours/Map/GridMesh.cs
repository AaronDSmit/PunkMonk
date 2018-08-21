using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMesh : MonoBehaviour
{
    private Mesh mesh;
    private List<Vector3> vertices;
    private List<Vector2> uvs;
    private List<int> triangles;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Grid Mesh";
        vertices = new List<Vector3>();
        uvs = new List<Vector2>();
        triangles = new List<int>();
    }

    public void Triangulate(Hex[] hexes)
    {
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();

        for (int i = 0; i < hexes.Length; i++)
        {
            Triangulate(hexes[i]);
        }

        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);

        mesh.RecalculateNormals();
    }

    private void Triangulate(Hex hex)
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

    public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector3 uv3)
    {
        uvs.Add(uv1);
        uvs.Add(uv2);
        uvs.Add(uv3);
    }
}