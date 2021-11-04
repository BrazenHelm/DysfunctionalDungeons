using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonArcherAI : EnemyAIType
{
    public override IEnumerator DoTurn(List<UnitController> playerUnits, int mostRecentActor)
    {
        Hex myHex = mover.CurrentHex;
        Hex theirHex;

        // first select unit to attack - closest one
        int unitToAttack = -1;
        int distanceToClosestUnit = 999;

        for (int i = 0; i < playerUnits.Count; i++)
        {
            int j = (mostRecentActor - i);
            if (j < 0) j += playerUnits.Count;

            theirHex = playerUnits[j].Mover.CurrentHex;
            int distance = hexGrid.Distance(myHex, theirHex);
            if (distance < distanceToClosestUnit)
            {
                distanceToClosestUnit = distance;
                unitToAttack = j;
            }
        }

        theirHex = playerUnits[unitToAttack].Mover.CurrentHex;

        // if no unit found to attack, do nothing
        if (unitToAttack == -1)
        {
            Debug.Log("nothing to attack");
            yield break;
        }

        Debug.Log("attacking unit " + unitToAttack + " (distance = " + distanceToClosestUnit + ")");

        int range = fighter.Range();
        int speed = mover.GetSpeed();

        if (distanceToClosestUnit == range)
        {
            controller.QueueAttack(playerUnits[unitToAttack].Fighter);
            yield return controller.PerformQueuedAction();
        }

        if (distanceToClosestUnit < range)
        {
            List<Hex> flight = hexGrid.FindFlight(myHex, theirHex, range, speed);
            controller.QueueMovement(flight);
            yield return controller.PerformQueuedAction();

            controller.QueueAttack(playerUnits[unitToAttack].Fighter);
            yield return controller.PerformQueuedAction();
        }

        if (distanceToClosestUnit > range)
        {
            List<Hex> approach = hexGrid.FindApproach(myHex, theirHex, range);
            if (approach.Count > speed * 2 + 1)
                approach = approach.GetRange(0, speed * 2 + 1);

            controller.QueueMovement(approach);
            yield return controller.PerformQueuedAction();

            if (approach.Count <= speed + 1)
            {
                controller.QueueAttack(playerUnits[unitToAttack].Fighter);
                yield return controller.PerformQueuedAction();
            }
        }

        EndTurn();
    }
}
