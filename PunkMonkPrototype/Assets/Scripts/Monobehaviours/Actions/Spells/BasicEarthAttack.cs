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
    [SerializeField] float damageAmount;

    protected override void CastSpell(Tile[] a_tile, System.Action a_onStartCB, System.Action a_onFinishCB)
    {
        base.CastSpell(a_tile, a_onStartCB, a_onFinishCB);
        foreach (Tile hex in a_tile)
        {
            if (hex.CurrentUnit != null)
            {
                hex.CurrentUnit.TakeDamage(Element.EARTH, damageAmount);
            }
        }
    }

}
