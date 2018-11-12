using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileUnit : AI_Agent
{
    [Header("Missile Unit")]
    [SerializeField]
    private int rechargeTurns = 1;

    [SerializeField]
    private bool friendlyFire = true;

    [SerializeField]
    private GameObject missilePrefab = null;

    private float missileSpawnDelay = 1.09f;

    private int turns = 0;

    private bool readyToAttack = false;

    private int missileFallCount = 0;

    private bool doneMissiles = false;

    private bool running = false;

    private Hex runHex = null;
    private SFXEvent sfx;

    public Hex RunHex { get { return runHex; } set { runHex = value; } }

    protected override void Start()
    {
        base.Start();
        turns = rechargeTurns;
        readyToAttack = false;
        sfx = GetComponent<SFXEvent>();
    }

    protected override void Die()
    {
        base.Die();

        Manager.instance.HexHighlighter.RemoveHighlights(this);
        Manager.instance.UIController.UnlockUI();
    }

    public override void Spawn(Hex a_startingTile)
    {
        base.Spawn(a_startingTile);
    }

    protected override IEnumerator DoTurn(GridManager a_grid)
    {
        if (running)
        {
            turnComplete = true;
            yield break;
        }

        if (readyToAttack)
        {
            Manager.instance.HexHighlighter.RemoveHighlights(this);

            StartCoroutine(ShootMissiles());
            doneMissiles = false;

            yield return new WaitUntil(() => doneMissiles == true);



            turns = rechargeTurns;

            readyToAttack = false;
        }

        if (turns == 1 || rechargeTurns == 0)
        {
            Invoke("ChooseAreaToAttack", damageDelayTimer + 0.1f);
        }

        if (turns > 0)
            turns--;

        turnComplete = true;
        yield break;
    }

    private IEnumerator ShootMissiles()
    {
        doneMissiles = false;

        yield return new WaitForSeconds(missileSpawnDelay);

        GameObject missileGO = Instantiate(missilePrefab, projectilePosition.position, missilePrefab.transform.rotation);
        Missile missile = missileGO.transform.GetChild(0).GetComponent<Missile>();
        missile.TriggerUp();
        animator.SetTrigger("BasicAttack");

        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => missile.Done);

        missileGO.transform.position = Vector3.zero;
        Destroy(missileGO);

        foreach (Hex tile in tilesToAttack)
        {
            StartCoroutine(MissileDown(tile));
        }

        yield return new WaitForSeconds(0.1f);

        sfx.PlaySFX("MissileDown");

        yield return new WaitUntil(() => missileFallCount == 0);

        doneMissiles = true;

        yield return new WaitForSeconds(0.1f);

        FinishedAction();
    }

    private IEnumerator MissileDown(Hex a_tile)
    {
        missileFallCount++;

        GameObject missileGO = Instantiate(missilePrefab);
        missileGO.transform.position = a_tile.transform.position;
        Missile missile = missileGO.transform.GetChild(0).GetComponent<Missile>();
        missile.TriggerDown();

        yield return new WaitForSeconds(0.001f);

        yield return new WaitUntil(() => missile.Done);


        missileGO.transform.position = Vector3.zero;
        Destroy(missileGO);

        if (a_tile.CurrentUnit != null)
        {
            if (friendlyFire || a_tile.CurrentUnit.Team == TEAM.player)
            {
                a_tile.CurrentUnit.TakeDamage(damage, this);
            }
        }

        missileFallCount--;
    }

    private void ChooseAreaToAttack()
    {
        if (turns == 1 || rechargeTurns == 0)
        {
            // Select Area to shoot to
            Hex furthestPlayerTile = null;
            if (Manager.instance.PlayerController.EarthUnit.IsDead || Manager.instance.PlayerController.EarthUnit == null)
            {
                furthestPlayerTile = Manager.instance.PlayerController.LightningUnit.CurrentTile;
            }
            else if (Manager.instance.PlayerController.LightningUnit.IsDead || Manager.instance.PlayerController.LightningUnit == null)
            {
                furthestPlayerTile = Manager.instance.PlayerController.EarthUnit.CurrentTile;
            }
            else
            {
                float distanceToEarth = Vector3.Distance(CurrentTile.transform.position, Manager.instance.PlayerController.EarthUnit.CurrentTile.transform.position);
                float distanceToLightning = Vector3.Distance(CurrentTile.transform.position, Manager.instance.PlayerController.LightningUnit.CurrentTile.transform.position);
                furthestPlayerTile = distanceToEarth > distanceToLightning ? Manager.instance.PlayerController.EarthUnit.CurrentTile : Manager.instance.PlayerController.LightningUnit.CurrentTile;
            }
            // Attack the hex behind the furthest player
            Hex hexToAttack = furthestPlayerTile.GetNeighbour(HexUtility.VecToHexDirection(furthestPlayerTile.transform.position - CurrentTile.transform.position));

            tilesToAttack = Manager.instance.Grid.GetTilesWithinDistance(hexToAttack, 2, false, true).ToArray();

            // Rotate to the direction
            StartCoroutine(Rotate(hexToAttack.transform.position));

            // Highlight the area around the hex
            Manager.instance.HexHighlighter.HighLightArea(new List<Hex>(tilesToAttack), Color.yellow, Color.yellow, this, null, HighlightType.FLAME);

            readyToAttack = true;
        }
    }

    public override void TakeDamage(int a_damageAmount, Unit a_damageFrom)
    {
        if (Manager.instance.PlayerController.EncounterHasBoss && Manager.instance.PlayerController.EncounterBossDamageGoal <= (MaxHealth - CurrentHealth) + a_damageAmount)
        {
            int remainingHp = CurrentHealth - a_damageAmount;
            if (remainingHp <= 0)
            {
                base.TakeDamage(CurrentHealth - 1, a_damageFrom);
            }
            else
            {
                base.TakeDamage(a_damageAmount, a_damageFrom);
            }

            running = true;
            Manager.instance.HexHighlighter.RemoveHighlights(this);
            if (RunHex == null)
            {
                Debug.LogError("The missile unit doesn't have a hex to run too. Set this on the spawner for the missile unit.", gameObject);
                Die();
            }
            else
            {
                Manager.instance.UIController.LockUI();
                StartCoroutine(WalkToHex(RunHex, Die));
            }

        }
        else
        {
            base.TakeDamage(a_damageAmount, a_damageFrom);
        }

        Manager.instance.PlayerController.EncounterBossDamage += a_damageAmount;
    }

}
