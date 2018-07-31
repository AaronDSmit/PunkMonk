using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningUnit : Unit
{
    protected override void Awake()
    {
        base.Awake();

        element = Element.lightning;
    }

    protected override void DoBasicAttack(Tile[] targetTiles, System.Action start, System.Action finished)
    {

    }

    protected override void DoSpecialAttack(Tile[] targetTiles, System.Action start, System.Action finished)
    {

    }
}