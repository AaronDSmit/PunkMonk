using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthUnit : Unit
{
    [Header("Special Attack")]

    [SerializeField]
    private float specialDamage;
    [SerializeField] private float specialheight;
    [SerializeField] private float specialJumpTime;
    [SerializeField] private float specialDamgeDelayTimer;
    [SerializeField] private AnimationCurve YCurve;
    [SerializeField] private AnimationCurve ZCurve;

    private Vector3 specialTargetPosition;
    private Vector3 specialStartPosition;
    private float specialTimer;
    private bool specialAttack = false;
    private Vector3 specialVecBetween;
    private Hex[] specialTiles;
    private System.Action specialFinishedFunc;
    private float specialGlamCamTimer = 0;

    [Header("Basic Attack")]

    [SerializeField]
    private float basicDamage;
    [SerializeField] private float basicDamgeDelayTimer = 1;
    [SerializeField] private int coneRange = 4;

    private Hex[] basicTiles;

    public int ConeRange
    {
        get
        {
            return coneRange;
        }

        set
        {
            coneRange = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void DoBasicAttack(Hex[] targetTiles, System.Action start, System.Action finished)
    {
        //call the start call back function
        start();

        canAttack = false;

        //store the target tile
        basicTiles = targetTiles;

        Vector3 tilePos = new Vector3(targetTiles[0].transform.position.x, transform.position.y, targetTiles[0].transform.position.z);


        if (glamCam)
        {
            if (Random.Range(0, 100) <= 50)
            {
                cameraController.PlayGlamCam(transform.position, tilePos - transform.position, GlamCamType.EARTH_BASIC);
                StartCoroutine(BasicAttackDamageDelay(basicDamgeDelayTimer, finished, 2));
                return;
            }
        }
        //call the basicAttackDamageDelay coroutine 
        StartCoroutine(BasicAttackDamageDelay(basicDamgeDelayTimer, finished));

    }

    protected override void DoSpecialAttack(Hex[] targetTiles, System.Action start, System.Action finished)
    {
        //call the start call back function
        start();

        CanSpecialAttack = false;


        //store the target tile
        specialTiles = targetTiles;

        //store the finished function call
        specialFinishedFunc = finished;

        //store the start position
        specialStartPosition = transform.position;

        //store the target position
        specialTargetPosition = targetTiles[0].transform.position;


        //make sure the target's y is the same as the start's y
        specialTargetPosition.y = specialStartPosition.y;

        //get the vector between the start position and the target position
        specialVecBetween = specialTargetPosition - specialStartPosition;

        //start the glamCam
        if (glamCam)
        {
            if (Random.Range(0, 100) <= 50)
            {
                cameraController.PlayGlamCam(specialStartPosition, specialVecBetween, GlamCamType.EARTH_SPECIAL);
                specialGlamCamTimer = 2;
            }

            else
            {
                specialGlamCamTimer = 0;
            }
        }
        //Start the Update Loop
        specialAttack = true;
    }



    private void Update()
    {
        //if special Attack is true
        if (specialAttack)
        {
            specialGlamCamTimer -= Time.deltaTime;

            if (specialGlamCamTimer <= 0)
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

                    //call the specialAttackDamageDelay coroutine 
                    StartCoroutine(SpecialAttackDamageDelay(specialDamgeDelayTimer));

                    //exit current tile
                    currentTile.Exit();
                    currentTile = specialTiles[0];

                    //enter target tile
                    currentTile.Enter(this);
                }
            }
        }
    }

    private IEnumerator BasicAttackDamageDelay(float a_timer, System.Action a_finished, float glamCamDelay = 0)
    {
        //wait for glamCam to catch up
        yield return new WaitForSeconds(glamCamDelay);



        //wait for timer before runing code
        yield return new WaitForSeconds(a_timer);

        //go though each tile and deal damage to the enemy
        foreach (Hex x in basicTiles)
        {
            //if there is a unit
            if (x.CurrentUnit != null)
            {
                //make sure we arent damaging the player or team
                if (x.CurrentUnit.Team != TEAM.player)
                {
                    //deal damage to that unit
                    x.CurrentUnit.TakeDamage(basicDamage);
                }
            }
        }

        cameraController.TurnOffGlamCam();



        //call the finished call back function
        a_finished();
    }


    private IEnumerator SpecialAttackDamageDelay(float a_timer)
    {

        //wait for timer before runing code
        yield return new WaitForSeconds(a_timer);

        //go though each tile and deal damage to the enemy
        foreach (Hex x in specialTiles)
        {
            //if there is a unit
            if (x.CurrentUnit != null)
            {
                //make sure we arent damaging the player or team
                if (x.CurrentUnit.Team != TEAM.player)
                {
                    //deal damage to that unit
                    x.CurrentUnit.TakeDamage(specialDamage);
                }
            }
        }

        //turn off glamCam  
        cameraController.TurnOffGlamCam();


        //call the finished call back function
        specialFinishedFunc();
    }

}