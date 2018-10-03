using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileUnit : AI_Agent
{
    bool charged = false;

    protected override IEnumerator DoTurn(GridManager a_grid)
    {

        if (charged)
        {
            // Remove the highlight
            Manager.instance.HexHighlighter.RemoveHighlights(this);

            // Shoot Missiles
            StartCoroutine(BasicAttackDamageDelay(damageDelayTimer, FinishedAction));
        }
        else
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
        }

        charged = !charged;

        turnComplete = true;
        yield break;
    }
}
