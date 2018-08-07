﻿using System.Collections;
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

    [SerializeField] private bool specialRehitEnemies;
    [SerializeField] private int specialRange;
    [SerializeField] private int specialAmountOfBounces;
    [SerializeField] private float specialLightningTimer;


    private GameObject[] specialEnemies;
    private List<GameObject> specialSortedList = new List<GameObject>();
    private List<GameObject> specialFinalTargets;
    private List<GameObject> specialDirtyList;
    private GameObject specialNextTarget;
    private GameObject specialLightningGO;



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

        specialEnemies = GameObject.FindGameObjectsWithTag("Enemies");

        specialFinalTargets.Add(currentTile.gameObject);

        for (int i = 0; i < specialAmountOfBounces; i++)
        {
            foreach (var x in specialEnemies)
            {
                if (HexUtility.Distance(specialFinalTargets[i].GetComponent<Entity>().CurrentTile, x.GetComponent<Entity>().CurrentTile) < specialRange)
                {
                    specialSortedList.Add(x);
                }
            }


            if (specialRehitEnemies == false)
            {
                foreach (var x in specialSortedList)
                {
                    foreach (var y in specialDirtyList)
                    {
                        if (x == y)
                        {
                            specialSortedList.Remove(y);
                        }
                    }
                }
            }

            specialSortedList.Sort((a, b) => (HexUtility.Distance(currentTile, a.GetComponent<Entity>().CurrentTile).CompareTo(HexUtility.Distance(currentTile, b.GetComponent<Entity>().CurrentTile))));


            specialFinalTargets.Add(specialSortedList[0]);

            if (specialRehitEnemies == false)
            {
                specialDirtyList.Add(specialSortedList[0]);
            }
        }

        specialLightningGO.transform.GetChild(0).position = specialFinalTargets[0].transform.position + (transform.up * 0.8f);
        specialLightningGO.transform.GetChild(1).position = specialFinalTargets[1].transform.position + (transform.up * 0.8f);

        StartCoroutine(SpecialAttackDamageDelay(specialLightningTimer));

    }


    private IEnumerator SpecialAttackDamageDelay(float a_timer)
    {
        for (int i = 1; i < specialAmountOfBounces - 1; i++)
        {
            yield return new WaitForSeconds(a_timer);

            specialLightningGO.transform.GetChild(0).position = specialFinalTargets[i].transform.position + (transform.up * 0.8f);
            specialLightningGO.transform.GetChild(1).position = specialFinalTargets[i + 1].transform.position + (transform.up * 0.8f);
        }

        Destroy(specialLightningGO);
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