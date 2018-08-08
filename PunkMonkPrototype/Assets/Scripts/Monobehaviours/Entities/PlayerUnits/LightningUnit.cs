using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningUnit : Unit
{
    [SerializeField] private GameObject lightningPrefab;

    [Header("Basic Attack")]

    [SerializeField] private float basicDamage = 1;
    [SerializeField] private float basicDamgeDelayTimer = 3;
    [SerializeField] private float basicElectricityLifetime = 3;

    private System.Action basicFinishedFunc;
    private Hex basicTile;
    private bool basicLightningAnimation;
    private float basicLightningTimer;
    private GameObject basicLightningGO;

    [Header("Special Attack")]

    [SerializeField] private bool specialRehitEnemies = false;
    [SerializeField] private int specialRange = 3;
    [SerializeField] private int specialAmountOfBounces = 3;
    [SerializeField] private float specialLightningLifeTime = 1;
    [SerializeField] private float specialBounceDamage = 1;
    [SerializeField] private float specialFinalDamage = 1;
    [SerializeField] private float specialDelayTime = 1;

    private List<AI_Agent> specialEnemies;
    private List<Entity> specialSortedList = new List<Entity>();
    private List<Entity> specialFinalTargets = new List<Entity>();
    private List<Entity> specialDirtyList = new List<Entity>();
    private GameObject specialLightningGO;
    private AI_Controller specialAIController;
    private System.Action specialFinishedFunc;




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


        Destroy(basicLightningGO, basicElectricityLifetime);

        //call the basicAttackDamageDelay coroutine 
        StartCoroutine(BasicAttackDamageDelay(basicDamgeDelayTimer));

    }

    protected override void DoSpecialAttack(Hex[] targetTiles, System.Action start, System.Action finished)
    {
        start();


        specialFinishedFunc = finished;

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
                if (i + 1 < specialFinalTargets.Count)
                {
                    if (enemy != specialFinalTargets[i + 1])
                    {
                        if (HexUtility.Distance(specialFinalTargets[i].CurrentTile, enemy.CurrentTile) < specialRange)
                        {
                            specialSortedList.Add(enemy);
                        }
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
                    if (i - 1 < specialFinalTargets.Count)
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
                }
                else
                {
                    specialFinalTargets.Add(specialSortedList[0]);
                }
            }

            specialSortedList.Clear();

        }

        specialLightningGO.transform.GetChild(0).position = specialFinalTargets[0].transform.position + (transform.up * 0.8f);
        specialLightningGO.transform.GetChild(1).position = specialFinalTargets[1].transform.position + (transform.up * 0.8f);

        StartCoroutine(SpecialAttackDamageDelay(specialLightningLifeTime));

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
                    StartCoroutine(SpecialAttackDamageDelay(specialDelayTime, specialBounceDamage, specialFinalTargets[i]));
                }
                else
                {
                    specialLightningGO.transform.GetChild(1).position = specialLightningGO.transform.GetChild(0).position;
                    break;
                }
            }
        }

        yield return new WaitForSeconds(a_timer);

        StartCoroutine(SpecialAttackDamageDelay(specialDelayTime, specialFinalDamage, specialFinalTargets[specialFinalTargets.Count - 1]));


        specialFinishedFunc();

        Destroy(specialLightningGO);
        specialDirtyList.Clear();
        specialFinalTargets.Clear();
        specialSortedList.Clear();

    }


    private IEnumerator SpecialAttackDamageDelay(float a_timer, float a_damage, Entity a_unit)
    {

        yield return new WaitForSeconds(a_timer);

        if(a_unit != null)
        {
            if(a_unit.Team != TEAM.player)
            {
                a_unit.TakeDamage(a_damage);
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