using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthUnit : Unit
{

    [Header("Special Attack")]

    [SerializeField] private float specialDamage;
    [SerializeField] private float specialheight;
    [SerializeField] private float specialJumpTime;
    [SerializeField] private AnimationCurve YCurve;
    [SerializeField] private AnimationCurve ZCurve;

    private Vector3 specialTargetPosition;
    private Vector3 specialStartPosition;
    private float specialTimer;
    private bool specialAttack = false;
    private Vector3 specialVecBetween;
    private Tile[] specialTiles;


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
        //store the target tile
        specialTiles = targetTiles;

        //store the start position
        specialStartPosition = transform.position;

        //store the target position
        specialTargetPosition = targetTiles[0].transform.position;
        
        //make sure the target's y is the same as the start's y
        specialTargetPosition.y = specialStartPosition.y;

        //get the vector between the start position and the target position
        specialVecBetween = specialTargetPosition - specialStartPosition;

        //Start the Update Loop
        specialAttack = true;
    }

    private void Update()
    {
        //if special Attack is true
        if (specialAttack)
        {
            //Update the timer
            specialTimer += Time.deltaTime;
            
            //set the new xz position is equal to the current distance though the z animationCurve
            transform.position = specialStartPosition + specialVecBetween.normalized * ZCurve.Evaluate(specialTimer / specialJumpTime) * (specialVecBetween.magnitude);

            //set the new y position to the current distance though the y animationCurve
            transform.position = new Vector3(transform.position.x, specialStartPosition.y + YCurve.Evaluate(specialTimer / specialJumpTime) * specialheight, transform.position.z);

            //if the current timer is grater then the overall time
            if (specialTimer > specialJumpTime)
            {
                //finish the animation
                specialAttack = false;

                //reset Timer
                specialTimer -= specialJumpTime;

                //make sure we are at the target position
                transform.position = specialTargetPosition;

                //go though each tile and deal damage to the enemy
                foreach(Tile x in specialTiles)
                {
                    if(x.CurrentUnit != null)
                    {
                        x.CurrentUnit.TakeDamage(Element.earth, specialDamage);
                    }
                }

                //exit current tile
                currentTile.Exit();

                //enter target tile
                specialTiles[0].Enter(this);

            }
        }
    }

}