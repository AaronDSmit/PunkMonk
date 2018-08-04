using UnityEngine;

[System.Serializable]
public struct CubeCoord
{
    // Cube storage, cube constructor
    public readonly int q, r, s;

    public CubeCoord(int _q, int _r, int _s)
    {
        q = _q;
        r = _r;
        s = _s;

        Debug.Assert(q + r + s == 0);
    }

    public CubeCoord(int _q, int _r)
    {
        q = _q;
        r = _r;
        s = -q - r;

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

public static class HexCoordinate
{
    public static OffsetCoord CubeToOffset(CubeCoord h)
    {
        int col = h.q + (h.r - (h.r & 1)) / 2;
        int row = h.r;

        return new OffsetCoord(col, row);
    }

    public static CubeCoord OffsetToCube(OffsetCoord h)
    {
        int q = h.x - (h.y - (h.y & 1)) / 2;
        int r = h.y;
        int s = -q - r;

        return new CubeCoord(q, r, s);
    }

    public static OffsetCoord PositionToOffset(Vector3 position)
    {
        float x = position.x / (HexUtility.innerRadius * 2f);
        float y = -x;

        float offset = position.z / (HexUtility.outerRadius * 3f);
        x -= offset;
        y -= offset;

        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-x - y);

        if (iX + iY + iZ != 0)
        {
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x - y - iZ);

            if (dX > dY && dX > dZ)
            {
                iX = -iY - iZ;
            }
            else if (dZ > dY)
            {
                iZ = -iX - iY;
            }
        }

        return CubeToOffset(new CubeCoord(iX, iZ));
    }

    public static CubeCoord PositionToCube(Vector3 position)
    {
        float x = position.x / (HexUtility.innerRadius * 2f);
        float y = -x;

        float offset = position.z / (HexUtility.outerRadius * 3f);
        x -= offset;
        y -= offset;

        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-x - y);

        if (iX + iY + iZ != 0)
        {
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x - y - iZ);

            if (dX > dY && dX > dZ)
            {
                iX = -iY - iZ;
            }
            else if (dZ > dY)
            {
                iZ = -iX - iY;
            }
        }

        return new CubeCoord(iX, iZ);
    }
}