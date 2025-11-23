using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    public RectTransform[] waypoints;


    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2)
            return;

        for (var i = 0; i < waypoints.Length - 1; i++)
        {
            Gizmos.DrawSphere(waypoints[i].position, 0.1f);
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }

        Gizmos.DrawSphere(waypoints[waypoints.Length - 1].position, 0.1f);
    }
}