using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthUnit : Unit
{
    [Header("Basic Attack")]

    [SerializeField]
    private int basicDamage;

    [SerializeField]
    private float basicDamgeDelayTimer = 1;

    [SerializeField]
    private int coneAreaLength = 4;

    [SerializeField]
    private GameObject basicParticles = null;

    [SerializeField]
    private float basicSpeed = 0.1f;

    [Header("Special Attack")]

    [SerializeField]
    private int[] specialDamage;
    [SerializeField]
    private float specialheight;
    [SerializeField]
    private float specialJumpTime;
    [SerializeField]
    private float specialDamgeDelayTimer;
    [SerializeField]
    private int specialDamageArea = 1;

    [SerializeField]
    private AnimationCurve YCurve;
    [SerializeField]
    private AnimationCurve ZCurve;

    [SerializeField]
    private GameObject specialLandParticles = null;

    private Vector3 specialTargetPosition;
    private Vector3 specialStartPosition;
    private float specialTimer;
    private bool specialAttack = false;
    private Vector3 specialVecBetween;
    private Hex[] specialTiles;
    private System.Action specialFinishedFunc;
    private float specialGlamCamTimer = 0;

    private Hex[] basicTiles;

    public int ConeRange
    {
        get
        {
            return coneAreaLength;
        }

        set
        {
            coneAreaLength = value;
        }
    }

    public int BasicAttackDamage
    {
        get { return basicDamage; }
    }

    public int MinSpecialDamage
    {
        get { return specialDamage[specialDamage.Length - 1]; }
    }

    public int GetSpecialDamage(int a_distance)
    {
        if (a_distance < specialDamage.Length && a_distance >= 0)
        {
            return specialDamage[a_distance];
        }

        return 0;
    }

    public int SpecialDamageArea
    {
        get { return specialDamageArea; }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void DoBasicAttack(Hex[] a_targetTiles, System.Action a_start, System.Action a_finished)
    {
        //call the start call back function
        a_start();

        CanAttack = false;

        //store the target tile
        basicTiles = a_targetTiles;


        //call the basicAttackDamageDelay coroutine 
        StartCoroutine(BasicAttackDamageDelay(basicDamgeDelayTimer, a_finished));

    }

    protected override void DoSpecialAttack(Hex[] a_targetTiles, System.Action a_start, System.Action a_finished)
    {
        //call the start call back function
        a_start();

        Manager.instance.PlayerController.CurrentVolt--;
        hasUsedSpecialAttack = true;

        CanSpecialAttack = false;

        //store the target tile
        specialTiles = a_targetTiles;

        //store the finished function call
        specialFinishedFunc = a_finished;

        specialAttackSFX.Post(gameObject);

        //store the start position
        specialStartPosition = transform.position;

        //store the target position
        specialTargetPosition = a_targetTiles[0].transform.position;


        //make sure the target's y is the same as the start's y
        specialTargetPosition.y = specialStartPosition.y;

        //get the vector between the start position and the target position
        specialVecBetween = specialTargetPosition - specialStartPosition;


         specialGlamCamTimer = 0;

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

                    // Spawn the land particles
                    Destroy(Instantiate(specialLandParticles, transform.position, specialLandParticles.transform.rotation), 10);
                }
            }
        }
    }

    private IEnumerator BasicAttackDamageDelay(float a_timer, System.Action a_finished, float a_glamCamDelay = 0)
    {
        //wait for glamCam to catch up
        yield return new WaitForSeconds(a_glamCamDelay);

        //wait for timer before runing code
        yield return new WaitForSeconds(a_timer);

        //go though each tile and deal damage to the enemy
        foreach (Hex hex in basicTiles)
        {
            float distance = Vector3.Distance(hex.transform.position, transform.position);
            StartCoroutine(BasicAttackTileHitDelay(distance * basicSpeed, hex));
        }

        //call the finished call back function
        a_finished();
    }

    private IEnumerator BasicAttackTileHitDelay(float a_timer, Hex a_tile)
    {
        yield return new WaitForSeconds(a_timer);

        Destroy(Instantiate(basicParticles, a_tile.transform.position, basicParticles.transform.rotation), 10);

        if (a_tile.CurrentUnit != null)
        {
            if (a_tile.CurrentUnit.Team != TEAM.player)
            {
                a_tile.CurrentUnit.TakeDamage(basicDamage, this);
            }
        }

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
                    x.CurrentUnit.TakeDamage(specialDamage[HexUtility.Distance(specialTiles[0], x) - 1], this);
                }
            }
        }

        //change the position of the camera to the position of the unit
        cameraController.transform.position = transform.position;




        //call the finished call back function
        specialFinishedFunc();
    }

}