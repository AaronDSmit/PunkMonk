using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileUnit : AI_Agent
{
    [SerializeField]
    private int rechargeTurns = 1;

    [SerializeField]
    private GameObject missilePrefab = null;

    private int turns = 0;

    private bool readyToAttack = false;

    private int missileFallCount = 0;

    protected override void Start()
    {
        base.Start();
        turns = rechargeTurns;
        readyToAttack = false;
    }

    protected override void Die()
    {
        base.Die();

        Manager.instance.HexHighlighter.RemoveHighlights(this);
    }

    public override void Spawn(Hex a_startingTile)
    {
        base.Spawn(a_startingTile);
    }

    protected override IEnumerator DoTurn(GridManager a_grid)
    {
        if (readyToAttack)
        {
            Manager.instance.HexHighlighter.RemoveHighlights(this);

            StartCoroutine(ShootMissiles());

            yield return new WaitForSeconds(2f);
            yield return new WaitUntil(() => missileFallCount == 0);

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
        GameObject missileParent = Instantiate(missilePrefab);
        missileParent.transform.position = transform.position;
        Missile missile = missileParent.transform.GetChild(0).GetComponent<Missile>();
        missile.TriggerUp();

        yield return new WaitForSeconds(0.001f);
        yield return new WaitUntil(() => missile.Done);

        missileParent.transform.position = Vector3.zero;
        Destroy(missileParent);

        foreach (Hex tile in tilesToAttack)
        {
            StartCoroutine(MissileDown(tile));
        }

        yield return new WaitUntil(() => missileFallCount == 0);

        FinishedAction();
    }

    private IEnumerator MissileDown(Hex a_tile)
    {
        missileFallCount++;

        GameObject missileParent = Instantiate(missilePrefab);
        missileParent.transform.position = a_tile.transform.position;
        Missile missile = missileParent.transform.GetChild(0).GetComponent<Missile>();
        missile.TriggerDown();

        yield return new WaitForSeconds(0.001f);
        yield return new WaitUntil(() => missile.Done);

        missileParent.transform.position = Vector3.zero;
        Destroy(missileParent);

        if (a_tile.CurrentUnit != null)
        {
            if (a_tile.CurrentUnit.Team != TEAM.ai)
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
            float distanceToEarth = Vector3.Distance(CurrentTile.transform.position, Manager.instance.PlayerController.EarthUnit.CurrentTile.transform.position);
            float distanceToLightning = Vector3.Distance(CurrentTile.transform.position, Manager.instance.PlayerController.LightningUnit.CurrentTile.transform.position);
            Hex furthestPlayerTile = distanceToEarth > distanceToLightning ? Manager.instance.PlayerController.EarthUnit.CurrentTile : Manager.instance.PlayerController.LightningUnit.CurrentTile;

            // Attack the hex behind the furthest player
            Hex hexToAttack = furthestPlayerTile.GetNeighbour(HexUtility.VecToHexDirection(furthestPlayerTile.transform.position - CurrentTile.transform.position));

            tilesToAttack = Manager.instance.Grid.GetTilesWithinDistance(hexToAttack, 2, true, true).ToArray();

            // Rotate to the direction
            StartCoroutine(Rotate(hexToAttack.transform.position));

            // Highlight the area around the hex
            Manager.instance.HexHighlighter.HighLightArea(new List<Hex>(tilesToAttack), Color.yellow, Color.yellow, this);

            readyToAttack = true;
        }
    }

    public override void TakeDamage(int a_damageAmount, Unit a_damageFrom)
    {
        base.TakeDamage(a_damageAmount, a_damageFrom);

        Manager.instance.PlayerController.EncounterBossDamage += a_damageAmount;
    }

}
