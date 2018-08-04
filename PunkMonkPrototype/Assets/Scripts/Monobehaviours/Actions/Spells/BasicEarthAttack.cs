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

[CreateAssetMenu(fileName = "EarthBasicAttack", menuName = "Attacks/new EarthBasicAttack", order = 0)]
public class BasicEarthAttack : Spell
{
    [SerializeField] private float damageAmount;

    public override void CastSpell(Unit a_unit, Hex[] a_tile, System.Action a_onStartCB, System.Action a_onFinishCB)
    {

        base.CastSpell(a_unit, a_tile, a_onStartCB, a_onFinishCB);
        foreach (Hex hex in a_tile)
        {
            if (hex.CurrentUnit != null)
            {
                hex.CurrentUnit.TakeDamage(Element.earth, damageAmount);
            }
        }

        a_onFinishCB();

    }

}
