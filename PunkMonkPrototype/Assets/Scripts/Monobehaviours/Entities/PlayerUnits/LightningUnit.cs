using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningUnit : Unit
{

    [Header("Special Attack")]

    [SerializeField]
    private float particleLifetime = 1f;
    [SerializeField]
    private int specialDamage = 1;
    [SerializeField] private float specialDelayTime = 3;
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
    [SerializeField] private float basicDamageDelayTime = 1;
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private GameObject lightningGunParticles;

    private List<AI_Agent> basicEnemies;
    private List<LivingEntity> basicSortedList = new List<LivingEntity>();
    private List<LivingEntity> basicFinalTargets = new List<LivingEntity>();
    private List<LivingEntity> basicDirtyList = new List<LivingEntity>();
    private GameObject[] basicLightningGOs = new GameObject[3];
    private AI_Controller AIController;
    private System.Action basicFinishedFunc;

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

        //store the finished function call
        specialFinishedFunction = finished;

        specialLightningAnimation = true;

        specialLightningTimer = 3f;

        Vector3 startTilePos = specialTiles[0].transform.position;
        Vector3 endTilePos = specialTiles[specialTiles.Count - 1].transform.position;

        startTilePos.y = transform.position.y;
        endTilePos.y = startTilePos.y;


        //call the basicAttackDamageDelay coroutine 
        StartCoroutine(SpecialAttackDamageDelay(specialDelayTime, particleLifetime));

    }

    private IEnumerator SpecialAttackDamageDelay(float startDelay ,float a_timer)
    {
        yield return new WaitForSeconds(startDelay);

        yield return new WaitForEndOfFrame();
        specialLightningGameObject = Instantiate(lightningPrefab);

        specialLightningGameObject.transform.GetChild(0).position = specialTiles[0].transform.position + (transform.up * 0.8f);
        specialLightningGameObject.transform.GetChild(1).position = specialTiles[specialTiles.Count - 1].transform.position + (transform.up * 0.8f);

        yield return new WaitForEndOfFrame();
        GameObject tempGameobject1 = Instantiate(lightningPrefab);

        tempGameobject1.transform.GetChild(1).position = specialLightningGameObject.transform.GetChild(0).position + Vector3.up * -0.5f;
        tempGameobject1.transform.GetChild(0).position = specialLightningGameObject.transform.GetChild(1).position + Vector3.up * 0.5f;

         yield return new WaitForSeconds(a_timer);

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
        Destroy(specialLightningGameObject);

        //call the finished call back function
        specialFinishedFunction();
    }

    private IEnumerator BasicAttackDamageDelay(float a_timer, float a_lightningLifetime, float glamCamDelay = 0)
    {
        if (glamCamDelay != 0)
        {
            yield return new WaitForSeconds(glamCamDelay);
        }

        yield return new WaitForSeconds(a_timer);

        lightningGunParticles.SetActive(true);

        basicLightningGOs[0] = Instantiate(lightningPrefab);

        //basicLightningGOs[0].transform.GetChild(0).position = basicFinalTargets[0].transform.position + (transform.up * 0.8f);
        basicLightningGOs[0].transform.GetChild(0).position = projectilePosition.position;
        basicLightningGOs[0].transform.GetChild(1).position = basicFinalTargets[1].transform.position + (transform.up * 0.8f);


        for (int i = 1; i < basicFinalTargets.Count - 1; i++)
        {
            //yield return new WaitForSeconds(a_lightningLifetime);

            if (i < basicFinalTargets.Count - 1)
            {
                basicLightningGOs[i] = Instantiate(lightningPrefab);

                basicLightningGOs[i].transform.GetChild(0).position = basicFinalTargets[i].transform.position + (transform.up * 0.8f);
                basicLightningGOs[i].transform.GetChild(1).position = basicFinalTargets[i + 1].transform.position + (transform.up * 0.8f);
                StartCoroutine(DamageDelay(basicBounceDamage, basicFinalTargets[i], basicDamageDelayTime));
            }
            else
            {
                basicLightningGOs[i].transform.GetChild(1).position = basicLightningGOs[i].transform.GetChild(0).position;
                break;
            }

        }

        StartCoroutine(DamageDelay(basicBounceDamage, basicFinalTargets[basicFinalTargets.Count - 1], basicDamageDelayTime));

        yield return new WaitForSeconds(a_lightningLifetime);

        lightningGunParticles.SetActive(false);

        basicFinishedFunc();

        foreach (var basicLightningGO in basicLightningGOs)
        {
            Destroy(basicLightningGO);
        }

        basicDirtyList.Clear();
        basicFinalTargets.Clear();
        basicSortedList.Clear();

    }

    private IEnumerator DamageDelay(int a_damage, LivingEntity a_unit, float a_waitTime)
    {
        yield return new WaitForSeconds(a_waitTime);

        if (a_unit != null)
        {
            if (a_unit.Team != TEAM.player)
            {
                a_unit.TakeDamage(a_damage, this);
            }
        }
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