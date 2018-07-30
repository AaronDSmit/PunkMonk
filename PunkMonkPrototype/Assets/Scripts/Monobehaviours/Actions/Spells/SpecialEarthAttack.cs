using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEarthAttack : Spell {

    [SerializeField] private float damageAmount;

    public override void CastSpell(Tile[] a_tile, System.Action a_onStartCB, System.Action a_onFinishCB)
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
