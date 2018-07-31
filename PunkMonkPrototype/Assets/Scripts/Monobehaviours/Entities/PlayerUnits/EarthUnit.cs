using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthUnit : Unit
{

    [SerializeField] private float specialDamage;
    [SerializeField] private float specialheight;
    [SerializeField] private float specialSpeedOfJump;

    [SerializeField] private AnimationCurve upYCurve;
    [SerializeField] private AnimationCurve upZCurve;

    [SerializeField] private AnimationCurve downYCurve;
    [SerializeField] private AnimationCurve downZCurve;

    private Vector3 targetPosition;
    private Vector3 startPosition;
    private float lerpDistance;

    private bool specialAttack = false;
    private bool jumping = false;



    private Vector3 groundVecBetween;
    private Vector3 highVec;


    protected override void Awake()
    {
        base.Awake();

        element = Element.earth;
    }

    protected override void DoBasicAttack(Tile[] targetTiles, System.Action start, System.Action finished)
    {



    }

    protected override void DoSpecialAttack(Tile[] targetTiles, System.Action start, System.Action finished)
    {
        groundVecBetween = targetTiles[0].transform.position - CurrentTile.transform.position;

        // transform.position = targetTiles[0].transform.position;


        highVec = groundVecBetween.normalized * (groundVecBetween.magnitude / 2) + CurrentTile.transform.position;
        highVec.y = specialheight;

        targetPosition = targetTiles[0].transform.position;

        startPosition = CurrentTile.transform.position;

        jumping = true;
        specialAttack = true;
    }

    private void Update()
    {
        if (specialAttack)
        {
            if (jumping)
            {
                lerpDistance += Time.deltaTime * specialSpeedOfJump;
                transform.position = startPosition + groundVecBetween.normalized * upZCurve.Evaluate(lerpDistance) * (groundVecBetween.magnitude / 2);
                transform.position = new Vector3(transform.position.x, startPosition.y + upYCurve.Evaluate(lerpDistance) * specialheight, transform.position.z);
                if (lerpDistance > 1)
                {
                    jumping = false;
                    lerpDistance = 0;
                    transform.position = highVec;
                }
            }
            else
            {
                lerpDistance += Time.deltaTime * specialSpeedOfJump;
                transform.position = highVec + groundVecBetween.normalized * downZCurve.Evaluate(lerpDistance) * (groundVecBetween.magnitude / 2);
                transform.position = new Vector3(transform.position.x, highVec.y + downZCurve.Evaluate(lerpDistance) * -specialheight, transform.position.z);
                if (lerpDistance < 0)
                {
                    specialAttack = false;
                    lerpDistance = 0;
                }
            }
        }
    }

}