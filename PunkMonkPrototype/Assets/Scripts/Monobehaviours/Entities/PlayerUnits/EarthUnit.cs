using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthUnit : Unit
{
    protected override void Awake()
    {
        base.Awake();

        element = Element.earth;
    }
}