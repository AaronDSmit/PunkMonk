using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningUnit : Unit
{
    [SerializeField] private GameObject lightningPrefab;

    [Header("Basic Attack")]

    [SerializeField]
    private float basicDamage = 1;
    [SerializeField] private float basicDamgeDelayTimer = 3;
    [SerializeField] private float basicElectricityLifetime = 3;

    private System.Action basicFinishedFunc;
    private Hex basicTile;
    private bool basicLightningAnimation;
    private float basicLightningTimer;
    private GameObject basicLightningGO;

    [Header("Special Attack")]

    [SerializeField]
    private bool specialRehitEnemies = false;
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

        CanAttack = false;

        //store the target tile
        basicTile = targetTiles[0];

        //store the finished function call
        basicFinishedFunc = finished;

        basicLightningAnimation = true;

        basicLightningTimer = 3f;

        Vector3 tilePos = basicTile.transform.position;

        tilePos.y = transform.position.y;

        if (glamCam)
        {
            if (Random.Range(0, 100) <= 50)
            {
                cameraController.PlayGlamCam(transform.position, tilePos - transform.position, GlamCamType.LIGHNING_BASIC);
                StartCoroutine(BasicAttackDamageDelay(basicDamgeDelayTimer, 2));
                return;
            }
        }

        //call the basicAttackDamageDelay coroutine 
        StartCoroutine(BasicAttackDamageDelay(basicDamgeDelayTimer));

    }

    protected override void DoSpecialAttack(Hex[] targetTiles, System.Action start, System.Action finished)
    {
        start();

        CanSpecialAttack = false;

        specialFinishedFunc = finished;


        if (specialAIController == null)
        {
            specialAIController = GameObject.FindGameObjectWithTag("AI_Controller").GetComponent<AI_Controller>();
        }

        specialEnemies = specialAIController.Agents;

        specialFinalTargets.Add(this);

        specialFinalTargets.Add(targetTiles[0].CurrentUnit);


        for (int i = 1; i < specialAmountOfBounces; i++)
        {
            foreach (var enemy in specialEnemies)
            {
                if (i < specialFinalTargets.Count)
                {
                    if (enemy != specialFinalTargets[i])
                    {
                        if (HexUtility.Distance(specialFinalTargets[i].CurrentTile, enemy.CurrentTile) <= specialRange)
                        {
                            specialSortedList.Add(enemy);
                        }
                    }
                }

            }



            specialSortedList.Sort((a, b) => (HexUtility.Distance(specialFinalTargets[i].CurrentTile, a.CurrentTile).CompareTo(HexUtility.Distance(specialFinalTargets[i].CurrentTile, b.CurrentTile))));

            if (specialRehitEnemies == false)
            {
                foreach (var x in specialSortedList)
                {
                    if (!specialFinalTargets.Contains(x))
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
                        specialFinalTargets.Add(specialSortedList[1]);
                    }
                    else
                    {
                        specialFinalTargets.Add(specialSortedList[0]);
                    }
                }
                else if (specialSortedList.Count != 0)
                {
                    specialFinalTargets.Add(specialSortedList[0]);
                }
            }

            specialSortedList.Clear();

        }

        Vector3 tilePos = specialFinalTargets[1].transform.position;
        tilePos.y = transform.position.y;

        cameraController.SpecialLightningCinemachine.LookAt = specialFinalTargets[1].transform;

        if (glamCam)
        {
            if (Random.Range(0, 100) <= 50)
            {
                cameraController.PlayGlamCam(transform.position, tilePos - transform.position, GlamCamType.LIGHNING_SPECIAL);
                StartCoroutine(SpecialAttackDamageDelay(basicDamgeDelayTimer, 2));
                return;
            }
        }


        StartCoroutine(SpecialAttackDamageDelay(specialLightningLifeTime));

    }


    private IEnumerator SpecialAttackDamageDelay(float a_timer, float glamCamDelay = 0)
    {

        yield return new WaitForSeconds(glamCamDelay);

        specialLightningGO = Instantiate(lightningPrefab);

        specialLightningGO.transform.GetChild(0).position = specialFinalTargets[0].transform.position + (transform.up * 0.8f);
        specialLightningGO.transform.GetChild(1).position = specialFinalTargets[1].transform.position + (transform.up * 0.8f);



        for (int i = 1; i < specialFinalTargets.Count - 1; i++)
        {
            yield return new WaitForSeconds(a_timer);


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

        yield return new WaitForSeconds(a_timer);

        StartCoroutine(SpecialAttackDamageDelay(specialDelayTime, specialFinalDamage, specialFinalTargets[specialFinalTargets.Count - 1]));

        cameraController.TurnOffGlamCam();


        specialFinishedFunc();

        Destroy(specialLightningGO);
        specialDirtyList.Clear();
        specialFinalTargets.Clear();
        specialSortedList.Clear();

    }


    private IEnumerator SpecialAttackDamageDelay(float a_timer, float a_damage, Entity a_unit)
    {

        yield return new WaitForSeconds(a_timer);

        if (a_unit != null)
        {
            if (a_unit.Team != TEAM.player)
            {
                a_unit.TakeDamage(a_damage);
            }
        }

    }


    private IEnumerator BasicAttackDamageDelay(float a_timer, float glamCamDelay = 0)
    {
        yield return new WaitForSeconds(glamCamDelay);

        yield return new WaitForEndOfFrame();
        basicLightningGO = Instantiate(lightningPrefab);

        basicLightningGO.transform.GetChild(0).position = basicTile.transform.position + (transform.up * 0.8f);
        basicLightningGO.transform.GetChild(1).position = transform.position + (transform.up * 0.8f);

        yield return new WaitForEndOfFrame();
        GameObject tempGameobject1 = Instantiate(basicLightningGO);

        tempGameobject1.transform.GetChild(0).position += Vector3.up * -0.5f;
        tempGameobject1.transform.GetChild(1).position += Vector3.up * 0.5f;

        yield return new WaitForEndOfFrame();
        GameObject tempGameobject2 = Instantiate(basicLightningGO);

        tempGameobject2.transform.GetChild(0).position += Vector3.up * -0.5f;
        tempGameobject2.transform.GetChild(1).position += Vector3.up * 0.5f;



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

        cameraController.TurnOffGlamCam();


        Destroy(tempGameobject1);
        Destroy(tempGameobject2);
        Destroy(basicLightningGO);

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