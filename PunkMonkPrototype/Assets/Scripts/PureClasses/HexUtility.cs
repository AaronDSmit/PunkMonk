using UnityEngine;

public static class HexUtility
{
    public static float outerRadius;

    public static float innerRadius;

    public static void UpdateScale(float a_scale)
    {
        outerRadius = a_scale;
        innerRadius = outerRadius * 0.866025404f;

        corners = new Vector3[7];

        corners[0] = new Vector3(0f, 0f, outerRadius);
        corners[1] = new Vector3(innerRadius, 0f, 0.5f * outerRadius);
        corners[2] = new Vector3(innerRadius, 0f, -0.5f * outerRadius);
        corners[3] = new Vector3(0f, 0f, -outerRadius);
        corners[4] = new Vector3(-innerRadius, 0f, -0.5f * outerRadius);
        corners[5] = new Vector3(-innerRadius, 0f, 0.5f * outerRadius);
        corners[6] = new Vector3(0f, 0f, outerRadius);
    }

    public static Vector3[] corners;

    public static Vector3 GetFirstCorner(HexDirection direction)
    {
        Vector3 output = corners[(int)direction];

        return corners[(int)direction];
    }

    public static Vector3 GetSecondCorner(HexDirection direction)
    {
        return corners[(int)direction + 1];
    }

    public static int Distance(Hex a, Hex b)
    {
        // convert offset coordinate to cube coordinate
        CubeCoord ac = HexCoordinate.OffsetToCube(a.Coordinates);
        CubeCoord bc = HexCoordinate.OffsetToCube(b.Coordinates);

        return CubeDistance(ac, bc);
    }

    private static int CubeDistance(CubeCoord a, CubeCoord b)
    {
        int dQ = Mathf.Abs(a.q - b.q);
        int dR = Mathf.Abs(a.r - b.r);

        return Mathf.Max(dQ, dR, Mathf.Abs(a.s - b.s));
    }
}