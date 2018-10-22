﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningUnit : Unit
{
    [SerializeField] private GameObject lightningPrefab;

    [Header("Special Attack")]

    [SerializeField]
    private int specialDamage = 1;
    [SerializeField] private float specialDamgeDelayTimer = 3;
    //[SerializeField] private float specialElectricityLifetime = 3;

    private System.Action specialFinishedFunction;
    private List<Hex> specialTiles = new List<Hex>();
    private bool specialLightningAnimation;
    private float specialLightningTimer;
    private GameObject specialLightningGameObject;

    [Header("Basic Attack")]

    [SerializeField]
    private bool basicRehitEnemies = false;
    [SerializeField] private int basicRange = 3;
    [SerializeField] private int basicAmountOfBounces = 3;
    [SerializeField] private float basicLightningLifeTime = 1;
    [SerializeField] private int basicBounceDamage = 1;
    [SerializeField] private int basicFinalDamage = 1;
    [SerializeField] private float basicDelayTime = 1;

    private List<AI_Agent> basicEnemies;
    private List<LivingEntity> basicSortedList = new List<LivingEntity>();
    private List<LivingEntity> basicFinalTargets = new List<LivingEntity>();
    private List<LivingEntity> basicDirtyList = new List<LivingEntity>();
    private GameObject basicLightningGO;
    private AI_Controller AIController;
    private System.Action basicFinishedFunc;




    //TODO - Add partical effect for basic attack

    public int SpecialAttackDamage
    {
        get { return specialDamage; }
    }

    public int MaxBasicDamage
    {
        get { return basicFinalDamage; }
    }

    public int MinBasicDamage
    {
        get { return basicBounceDamage; }
    }


    protected override void Awake()
    {
        base.Awake();
    }

    protected override void DoBasicAttack(Hex[] targetTiles, System.Action start, System.Action finished)
    {

        start();

        CanAttack = false;

        basicFinishedFunc = finished;


        basicAttackSFX.Post(gameObject);


        if (AIController == null)
        {
            AIController = GameObject.FindGameObjectWithTag("AI_Controller").GetComponent<AI_Controller>();
        }

        basicEnemies = AIController.Agents;

        basicFinalTargets.Add(this);

        basicFinalTargets.Add(targetTiles[0].CurrentUnit);


        for (int i = 1; i < basicAmountOfBounces; i++)
        {
            foreach (var enemy in basicEnemies)
            {
                if (i < basicFinalTargets.Count)
                {
                    if (enemy != basicFinalTargets[i])
                    {
                        if (HexUtility.Distance(basicFinalTargets[i].CurrentTile, enemy.CurrentTile) <= basicRange)
                        {
                            basicSortedList.Add(enemy);
                        }
                    }
                }

            }



            basicSortedList.Sort((a, b) => (HexUtility.Distance(basicFinalTargets[i].CurrentTile, a.CurrentTile).CompareTo(HexUtility.Distance(basicFinalTargets[i].CurrentTile, b.CurrentTile))));

            if (basicRehitEnemies == false)
            {
                foreach (var x in basicSortedList)
                {
                    if (!basicFinalTargets.Contains(x))
                    {
                        basicFinalTargets.Add(x);
                        break;
                    }
                }
            }
            else
            {
                if (basicSortedList.Count > 1)
                {
                    if (basicSortedList[0] == basicFinalTargets[i - 1])
                    {
                        basicFinalTargets.Add(basicSortedList[1]);
                    }
                    else
                    {
                        basicFinalTargets.Add(basicSortedList[0]);
                    }
                }
                else if (basicSortedList.Count != 0)
                {
                    basicFinalTargets.Add(basicSortedList[0]);
                }
            }

            basicSortedList.Clear();

        }

        Vector3 tilePos = basicFinalTargets[1].transform.position;
        tilePos.y = transform.position.y;


        StartCoroutine(BasicAttackDamageDelay(basicDelayTime, basicLightningLifeTime));
    }

    protected override void DoSpecialAttack(Hex[] targetTiles, System.Action start, System.Action finished)
    {

        //call the start call back function
        start();

        Manager.instance.PlayerController.CurrentVolt--;
        hasUsedSpecialAttack = true;

        CanSpecialAttack = false;

        //store the target tile
        specialTiles.AddRange(targetTiles);

        specialAttackSFX.Post(gameObject);

        //store the finished function call
        specialFinishedFunction = finished;

        specialLightningAnimation = true;

        specialLightningTimer = 3f;

        Vector3 startTilePos = specialTiles[0].transform.position;
        Vector3 endTilePos = specialTiles[specialTiles.Count - 1].transform.position;

        startTilePos.y = transform.position.y;
        endTilePos.y = startTilePos.y;


        //call the basicAttackDamageDelay coroutine 
        StartCoroutine(SpecialAttackDamageDelay(specialDamgeDelayTimer));

    }


    private IEnumerator SpecialAttackDamageDelay(float a_timer, float glamCamDelay = 0)
    {
        yield return new WaitForSeconds(glamCamDelay);

        yield return new WaitForEndOfFrame();
        specialLightningGameObject = Instantiate(lightningPrefab);

        specialLightningGameObject.transform.GetChild(0).position = specialTiles[0].transform.position + (transform.up * 0.8f);
        specialLightningGameObject.transform.GetChild(1).position = specialTiles[specialTiles.Count - 1].transform.position + (transform.up * 0.8f);

        yield return new WaitForEndOfFrame();
        GameObject tempGameobject1 = Instantiate(specialLightningGameObject);

        tempGameobject1.transform.GetChild(0).position += Vector3.up * -0.5f;
        tempGameobject1.transform.GetChild(1).position += Vector3.up * 0.5f;

        yield return new WaitForEndOfFrame();
        GameObject tempGameobject2 = Instantiate(specialLightningGameObject);

        tempGameobject2.transform.GetChild(0).position += Vector3.up * -0.5f;
        tempGameobject2.transform.GetChild(1).position += Vector3.up * 0.5f;


        if (glamCamDelay == 0)
        {
            //wait for timer before runing code
            yield return new WaitForSeconds(a_timer);
        }


        foreach (var tile in specialTiles)
        {
            //if there is a unit
            if (tile.CurrentUnit != null)
            {
                //make sure we arent damaging the player or team
                if (tile.CurrentUnit.Team != TEAM.player)
                {
                    //deal damage to that unit
                    tile.CurrentUnit.TakeDamage(specialDamage, this);
                }
            }
        }


        specialTiles.Clear();

        Destroy(tempGameobject1);
        Destroy(tempGameobject2);
        Destroy(specialLightningGameObject);

        //call the finished call back function
        specialFinishedFunction();


    }


    private IEnumerator SpecialAttackDamageDelay(int a_damage, LivingEntity a_unit)
    {
        yield return null;
        if (a_unit != null)
        {
            if (a_unit.Team != TEAM.player)
            {
                a_unit.TakeDamage(a_damage, this);
            }
        }
    }

    private IEnumerator BasicAttackDamageDelay(float a_timer, float a_lightningLifetime, float glamCamDelay = 0)
    {
        if (glamCamDelay == 0)
        {
            yield return new WaitForSeconds(a_timer);
        }

        yield return new WaitForSeconds(glamCamDelay);

        basicLightningGO = Instantiate(lightningPrefab);

        basicLightningGO.transform.GetChild(0).position = basicFinalTargets[0].transform.position + (transform.up * 0.8f);
        basicLightningGO.transform.GetChild(1).position = basicFinalTargets[1].transform.position + (transform.up * 0.8f);



        for (int i = 1; i < basicFinalTargets.Count - 1; i++)
        {
            yield return new WaitForSeconds(a_lightningLifetime);


            if (i + 1 < basicFinalTargets.Count)
            {
                basicLightningGO.transform.GetChild(0).position = basicFinalTargets[i].transform.position + (transform.up * 0.8f);
                basicLightningGO.transform.GetChild(1).position = basicFinalTargets[i + 1].transform.position + (transform.up * 0.8f);
                StartCoroutine(SpecialAttackDamageDelay(basicBounceDamage, basicFinalTargets[i]));
            }
            else
            {
                basicLightningGO.transform.GetChild(1).position = basicLightningGO.transform.GetChild(0).position;
                break;
            }

        }

        yield return new WaitForSeconds(a_lightningLifetime);

        StartCoroutine(SpecialAttackDamageDelay(basicFinalDamage, basicFinalTargets[basicFinalTargets.Count - 1]));

        basicFinishedFunc();

        Destroy(basicLightningGO);
        basicDirtyList.Clear();
        basicFinalTargets.Clear();
        basicSortedList.Clear();



    }


    private void Update()
    {
        if (specialLightningAnimation)
        {
            specialLightningTimer -= Time.deltaTime;

            if (specialLightningTimer < 0)
            {
                specialLightningAnimation = false;
                specialLightningTimer += 3;
                Destroy(specialLightningGameObject);
            }
        }
    }


}