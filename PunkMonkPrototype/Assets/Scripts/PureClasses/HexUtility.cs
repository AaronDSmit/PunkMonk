using UnityEngine;

public static class HexUtility
{
    public const float outerRadius = 1;

    public const float innerRadius = outerRadius * 0.866025404f;

    public static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius)
    };

    public static Vector3 GetFirstCorner(HexDirection direction)
    {
        return corners[(int)direction];
    }

    public static Vector3 GetSecondCorner(HexDirection direction)
    {
        return corners[(int)direction + 1];
    }

    public static float Distance(Hex a, Hex b)
    {
        // convert offset coordinate to cube coordinate
        CubeCoord ac = HexCoordinate.OffsetToCube(a.Coordinates);
        CubeCoord bc = HexCoordinate.OffsetToCube(b.Coordinates);

        return CubeDistance(ac, bc);
    }

    private static float CubeDistance(CubeCoord a, CubeCoord b)
    {
        int dQ = Mathf.Abs(a.q - b.q);
        int dR = Mathf.Abs(a.r - b.r);

        return Mathf.Max(dQ, dR, Mathf.Abs(a.s - b.s));
    }
}