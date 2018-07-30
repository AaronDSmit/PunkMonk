using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The earth character basic spell is a :
/// 
/// 1) Object must belong to a Spell Prefab.
/// 2) Object must call the base class's CastSpell fuction.
///
/// </summary>

public class BasicEarthAttack : Spell
{
    [SerializeField] private float damageAmount;
    [SerializeField] private float Range;

    private Vector3 targetPosition;

    public override void CastSpell(Tile[] a_tile, System.Action a_onStartCB, System.Action a_onFinishCB)
    {
        base.CastSpell(a_tile, a_onStartCB, a_onFinishCB);
        targetPosition = a_tile[0].transform.position;

    }

}
