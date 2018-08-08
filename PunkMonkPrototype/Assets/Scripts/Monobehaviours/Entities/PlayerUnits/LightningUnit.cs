using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningUnit : Unit
{
    [SerializeField] private GameObject lightningPrefab;

    [Header("Basic Attack")]

    [SerializeField]
    private float basicDamage;
    [SerializeField] private float basicDamgeDelayTimer;
    [SerializeField] private float electricityLifetime;

    private System.Action basicFinishedFunc;
    private Hex basicTile;
    private bool basicLightningAnimation;
    private float basicLightningTimer;
    GameObject basicLightningGO;

    [Header("Special Attack")]

    [SerializeField]
    private bool specialRehitEnemies;
    [SerializeField] private int specialRange;
    [SerializeField] private int specialAmountOfBounces;
    [SerializeField] private float specialLightningTimer;


    private List<AI_Agent> specialEnemies;
    private List<Entity> specialSortedList = new List<Entity>();
    private List<Entity> specialFinalTargets = new List<Entity>();
    private List<Entity> specialDirtyList = new List<Entity>();
    private GameObject specialNextTarget;
    private GameObject specialLightningGO;
    private AI_Controller specialAIController;




    //TODO - Add partical effect for basic attack



    protected override void Awake()
    {
        base.Awake();
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
        basicLightningGO.transform.GetChild(0).position = basicTile.transform.position + (transform.up * 0.8f);
        basicLightningGO.transform.GetChild(1).position = transform.position + (transform.up * 0.8f);


        Destroy(basicLightningGO, electricityLifetime);

        //call the basicAttackDamageDelay coroutine 
        StartCoroutine(BasicAttackDamageDelay(basicDamgeDelayTimer));

    }

    protected override void DoSpecialAttack(Hex[] targetTiles, System.Action start, System.Action finished)
    {
        start();


        specialLightningGO = Instantiate(lightningPrefab);


        if (specialAIController == null)
        {
            specialAIController = GameObject.FindGameObjectWithTag("AI_Controller").GetComponent<AI_Controller>();
        }

        specialEnemies = specialAIController.Agents;

        specialFinalTargets.Add(this);

        specialFinalTargets.Add(targetTiles[0].CurrentUnit);


        for (int i = 0; i < specialAmountOfBounces - 1; i++)
        {
            foreach (var enemy in specialEnemies)
            {
                if (enemy != specialFinalTargets[i + 1])
                {
                    if (HexUtility.Distance(specialFinalTargets[i].CurrentTile, enemy.CurrentTile) < specialRange)
                    {
                        specialSortedList.Add(enemy);
                    }
                }
            }



            specialSortedList.Sort((a, b) => (HexUtility.Distance(currentTile, a.CurrentTile).CompareTo(HexUtility.Distance(currentTile, b.CurrentTile))));

            if(specialRehitEnemies == false)
            {
                foreach(var x in specialSortedList)
                {
                    if(!specialFinalTargets.Contains(x))
                    {
                        specialFinalTargets.Add(x);
                    }
                }
            }
            else
            {
                if (specialSortedList.Count > 1)
                {
                    if (specialSortedList[0] == specialFinalTargets[i - 1])
                    {
                        specialFinalTargets.Add(specialSortedList[0]);
                    }
                    else
                    {
                        specialFinalTargets.Add(specialSortedList[1]);
                    }
                }
                else
                {
                    specialFinalTargets.Add(specialSortedList[0]);
                }
            }

            //if (specialRehitEnemies == false)
            //{
            //    specialDirtyList.Add(specialSortedList[0]);
            //}

            specialSortedList.Clear();

        }

        specialLightningGO.transform.GetChild(0).position = specialFinalTargets[0].transform.position + (transform.up * 0.8f);
        specialLightningGO.transform.GetChild(1).position = specialFinalTargets[1].transform.position + (transform.up * 0.8f);

        StartCoroutine(SpecialAttackDamageDelay(electricityLifetime));

    }


    private IEnumerator SpecialAttackDamageDelay(float a_timer)
    {
        for (int i = 1; i < specialAmountOfBounces; i++)
        {
            yield return new WaitForSeconds(a_timer);

            if (i + 1 <= specialAmountOfBounces)
            {
                if (i + 1 < specialFinalTargets.Count)
                {
                    specialLightningGO.transform.GetChild(0).position = specialFinalTargets[i].transform.position + (transform.up * 0.8f);
                    specialLightningGO.transform.GetChild(1).position = specialFinalTargets[i + 1].transform.position + (transform.up * 0.8f);
                }
                else
                {
                    specialLightningGO.transform.GetChild(1).position = specialLightningGO.transform.GetChild(0).position;
                    break;
                }
            }
        }

        yield return new WaitForSeconds(a_timer);

        Destroy(specialLightningGO);
        specialDirtyList.Clear();
        specialFinalTargets.Clear();
        specialSortedList.Clear();

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
                basicTile.CurrentUnit.TakeDamage(basicDamage);
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