using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    [Tooltip("Kolejne punkty, po których maj¹ chodziæ NPC.")]
    public Transform[] waypoints;

    [Header("Blokowanie œcie¿ki")]
    public bool isBlocked = false;

    public void SetBlocked(bool blocked)
    {
        isBlocked = blocked;
    }

    public int GetClosestWaypointIndex(Vector3 position)
    {
        if (waypoints == null || waypoints.Length == 0)
            return 0;

        int bestIndex = 0;
        float bestDistSq = float.MaxValue;

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            float distSq = (waypoints[i].position - position).sqrMagnitude;
            if (distSq < bestDistSq)
            {
                bestDistSq = distSq;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2)
            return;

        Gizmos.color = isBlocked ? Color.red : Color.yellow;

        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] == null || waypoints[i + 1] == null)
                continue;

            Gizmos.DrawSphere(waypoints[i].position, 0.1f);
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }

        Gizmos.DrawSphere(waypoints[waypoints.Length - 1].position, 0.1f);
    }
}
