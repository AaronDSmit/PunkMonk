using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileUnit : AI_Agent
{
    [SerializeField]
    int rechargeTurns = 1;

    bool charged = false;

    int turns = 0;

    protected override void Start()
    {
        base.Start();
        turns = rechargeTurns;
    }

    protected override void Die()
    {
        base.Die();

        Manager.instance.HexHighlighter.RemoveHighlights(this);
    }

    protected override IEnumerator DoTurn(GridManager a_grid)
    {
        if (charged)
        {
            // Remove the highlight
            Manager.instance.HexHighlighter.RemoveHighlights(this);

            // Shoot Missiles
            StartCoroutine(BasicAttackDamageDelay(damageDelayTimer, FinishedAction));

            charged = false;
            turns = rechargeTurns;
        }
        else
        {
            if (turns == 1)
            {
                // Select Area to shoot to
                float distanceToEarth = Vector3.Distance(CurrentTile.transform.position, Manager.instance.PlayerController.EarthUnit.CurrentTile.transform.position);
                float distanceToLightning = Vector3.Distance(CurrentTile.transform.position, Manager.instance.PlayerController.LightningUnit.CurrentTile.transform.position);
                Hex furthestPlayerTile = distanceToEarth > distanceToLightning ? Manager.instance.PlayerController.EarthUnit.CurrentTile : Manager.instance.PlayerController.LightningUnit.CurrentTile;

                // Attack the hex behind the furthest player
                Hex hexToAttack = furthestPlayerTile.GetNeighbour(HexUtility.VecToHexDirection(furthestPlayerTile.transform.position - CurrentTile.transform.position));

                tilesToAttack = a_grid.GetTilesWithinDistance(hexToAttack, 2, true, true).ToArray();

                // Rotate to the direction
                StartCoroutine(Rotate(hexToAttack.transform.position));

                // Highlight the area around the hex
                Manager.instance.HexHighlighter.HighLightArea(new List<Hex>(tilesToAttack), Color.yellow, Color.yellow, this);

                charged = true;
            }
            else
            {
                turns -= 1;
            }
        }

        turnComplete = true;
        yield break;
    }

    public override void TakeDamage(int a_damageAmount, Unit a_damageFrom)
    {
        base.TakeDamage(a_damageAmount, a_damageFrom);

        Manager.instance.PlayerController.EncounterBossDamage += a_damageAmount;
    }

}
