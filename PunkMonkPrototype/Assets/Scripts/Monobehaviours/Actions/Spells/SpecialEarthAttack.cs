using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EarthSpecialAttack", menuName = "Attacks/new EarthSpecialAttack", order = 0)]
public class SpecialEarthAttack : Spell {



    [SerializeField] private float damageAmount;
    [SerializeField] private float hight;
    [SerializeField] private float speedOfJump;

    private Vector3 targetPosition;


    private Vector3 groundVecBetween;
    private Vector3 highVec;

    private Vector3 highVecVetween;

    public override void CastSpell(Unit a_unit, Hex[] a_tile, System.Action a_onStartCB, System.Action a_onFinishCB)
    {

        base.CastSpell(a_unit, a_tile, a_onStartCB, a_onFinishCB);

        groundVecBetween = a_tile[0].transform.position - a_unit.CurrentTile.transform.position;

        highVec = groundVecBetween.normalized * (groundVecBetween.magnitude / 2);
        highVec.y = hight;

        targetPosition = Vector3.Slerp(a_unit.CurrentTile.transform.position, highVec, speedOfJump * Time.deltaTime);
        targetPosition = a_tile[0].transform.position;


    }

}
