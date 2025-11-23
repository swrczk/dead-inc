using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    public RectTransform[] waypoints;

    [SerializeField]
    private Color color = Color.white;

    [SerializeField]
    private Vector3 offset;

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2)
            return;

        for (var i = 0; i < waypoints.Length - 1; i++)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(waypoints[i].position, 0.1f);
            Gizmos.DrawLine(waypoints[i].position + offset, waypoints[i + 1].position + offset);
        }

        Gizmos.DrawSphere(waypoints[waypoints.Length - 1].position, 0.1f);
    }
}