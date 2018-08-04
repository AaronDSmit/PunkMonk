public enum HexDirection
{
    NE, E, SE, SW, W, NW
}

public static class HexDirectionUtility
{
    public static HexDirection Opposite(this HexDirection direction)
    {
        return (int)direction < 3 ? (direction + 3) : (direction - 3);
    }

    public static HexDirection Previous(this HexDirection direction)
    {
        return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
    }

    public static HexDirection Next(this HexDirection direction)
    {
        return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
    }

    public static string ToString(this HexDirection direction)
    {
        switch (direction)
        {
            case HexDirection.NE:
                return "NE";
            case HexDirection.E:
                return "E";
            case HexDirection.SE:
                return "SE";
            case HexDirection.SW:
                return "SW";
            case HexDirection.W:
                return "W";
            case HexDirection.NW:
                return "NW";
            default:
                return "";
        }
    }

    public static int DirectionCount()
    {
        return 6;
    }

    public static bool ValidDirection(this HexDirection direction)
    {
        return (int)direction < (int)HexDirection.NW && (int)direction >= 0;
    }
}