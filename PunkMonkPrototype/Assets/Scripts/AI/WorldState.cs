using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState
{
    private float distanceToPlayer = 0.0f;

    Unit[] players;

    public float DistanceToPlayer { get { return distanceToPlayer; } }
}
