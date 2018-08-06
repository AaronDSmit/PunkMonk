using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningUnit : Unit
{
    [SerializeField] private GameObject lightningPrefab;

    [Header("Basic Attack")]

    [SerializeField] private float basicDamage;
    [SerializeField] private float basicDamgeDelayTimer;
    [SerializeField] private float electricityLifetime;

    private System.Action basicFinishedFunc;
    private Hex basicTile;
    private bool basicLightningAnimation;
    private float basicLightningTimer;
    GameObject basicLightningGO;

    [Header("Special Attack")]

    [SerializeField] private bool specialRehitEnemies;
    [SerializeField] private int specialRange;
    [SerializeField] private int specialAmountOfBounces;
    

    private GameObject[] specialEnemies;
    private List<GameObject> specialSortedList = new List<GameObject>();
    private GameObject specialNextTarget;
    private List<GameObject> specialFinalTargets;
    private List<GameObject> specialDirtyList;



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


        Destroy(basicLightningGO, electricityLifetime);

        //call the basicAttackDamageDelay coroutine dw
        StartCoroutine(BasicAttackDamageDelay(basicDamgeDelayTimer));

    }

    protected override void DoSpecialAttack(Hex[] targetTiles, System.Action start, System.Action finished)
    {
        start();

        specialEnemies = GameObject.FindGameObjectsWithTag("Enemies");

        for (int i = 0; i < specialAmountOfBounces; i++)
        {
            foreach (var x in specialEnemies)
            {
                if (HexUtility.Distance(currentTile, x.GetComponent<Entity>().CurrentTile) < specialRange)
                {
                    specialSortedList.Add(x);
                }
            }

            //specialSortedList.Sort(HexUtility.Distance(currentTile, x.GetComponent<Entity>().CurrentTile),

            if (specialRehitEnemies == false)
            {
                foreach (var y in specialDirtyList)
                {
                    if (y == specialSortedList[0])
                    {
                        //specialSortedList.Remove();
                    }
                }
            }
            
            specialFinalTargets.Add(specialSortedList[0]);

            if(specialRehitEnemies)
            {
                specialDirtyList.Add(specialSortedList[0]);
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
        if (basicLightningAnimation)
        {
            basicLightningTimer -= Time.deltaTime;

            if (basicLightningTimer < 0)
            {
                basicLightningAnimation = false;
                basicLightningTimer += 3;
                Destroy(basicLightningGO);
            }
        }
    }


}