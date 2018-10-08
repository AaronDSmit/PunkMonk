using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileUnit : AI_Agent
{
    [SerializeField]
    int rechargeTurns = 1;

    int turns = 0;

    bool readyToAttack = false;

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
            // Remove the highlight
            Manager.instance.HexHighlighter.RemoveHighlights(this);

            // Shoot Missiles
            StartCoroutine(BasicAttackDamageDelay(damageDelayTimer, FinishedAction));

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
