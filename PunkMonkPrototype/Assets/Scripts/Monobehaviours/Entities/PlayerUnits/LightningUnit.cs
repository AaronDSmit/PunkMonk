using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningUnit : Unit
{
    [SerializeField] private GameObject lightningPrefab;

    [Header("Basic Attack")]

    [SerializeField] private float basicDamage;
    [SerializeField] private float basicDamgeDelayTimer;
    private System.Action basicFinishedFunc;
    private Hex basicTile;
    private bool basicLightningAnimation;
    private float basicLightningTimer;
    GameObject basicLightningGO;

    [Header("Special Attack")]

    private GameObject[] specialEnemies;
    [SerializeField] private bool specialREHitEnemies;
    [SerializeField] private int specialRange;
    SortedList<int, GameObject> specialSortedList = new SortedList<int, GameObject>();




    //TODO - Add partical effect for basic attack



    protected override void Awake()
    {
        base.Awake();

        element = Element.lightning;
    }

    protected override void DoBasicAttack(Hex[] targetTiles, System.Action start, System.Action finished)
    {
        //call the start call back function
        start();

        //store the target tile
        basicTile = targetTiles[0];

        //store the finished function call
        basicFinishedFunc = finished;

        basicLightningAnimation = true;

        basicLightningTimer = 3f;

        basicLightningGO = Instantiate(lightningPrefab);
        basicLightningGO.transform.GetChild(0).position = basicTile.transform.position;
        basicLightningGO.transform.GetChild(1).position = transform.position;


        //call the basicAttackDamageDelay coroutine 
        StartCoroutine(BasicAttackDamageDelay(basicDamgeDelayTimer));

    }

    protected override void DoSpecialAttack(Hex[] targetTiles, System.Action start, System.Action finished)
    {
        start();

        specialEnemies = GameObject.FindGameObjectsWithTag("Enemies");


        foreach(var x in specialEnemies)
        {  
            if(HexUtility.Distance(currentTile, x.GetComponent<Entity>().CurrentTile) < specialRange)
            {
                specialSortedList.Add(HexUtility.Distance(currentTile, x.GetComponent<Entity>().CurrentTile), x);

            }
        }
    }

    private IEnumerator BasicAttackDamageDelay(float a_timer)
    {
        //wait for timer before runing code
        yield return new WaitForSeconds(a_timer);


        //if there is a unit
        if (basicTile.CurrentUnit != null)
        {
            //make sure we arent damaging the player or team
            if (basicTile.CurrentUnit.Team != TEAM.player)
            {
                //deal damage to that unit
                basicTile.CurrentUnit.TakeDamage(Element.lightning, basicDamage);
            }
        }
        //call the finished call back function
        basicFinishedFunc();
    }


    private void Update()
    {
        if(basicLightningAnimation)
        {
            basicLightningTimer -= Time.deltaTime;
            
            if(basicLightningTimer < 0)
            {
                basicLightningAnimation = false;
                basicLightningTimer += 3;
                Destroy(basicLightningGO);
            }
        }
    }


}