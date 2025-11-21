using UnityEngine;
using System;

public class WaypointMover : MonoBehaviour
{
    [Header("Ustawienia ruchu")]
    public WaypointPath path;
    public float moveSpeed = 2f;
    public float reachDistance = 0.05f;
    public float waitTimeAtWaypoint = 0.5f;

    [Header("Tryb poruszania")]
    public bool loop = true;      // jeœli true – po ostatnim waypointcie wracamy na pocz¹tek
    public bool pingPong = false; // jeœli true – chodzimy tam i z powrotem

    // Eventy (u¿ywane przez NPCPathController – mo¿esz zostawiæ nawet jeœli jeszcze go nie u¿ywasz)
    public event Action LoopCompleted;   // wywo³ywany przy powrocie z koñca œcie¿ki na pocz¹tek (loop)
    public event Action PathFinished;    // wywo³ywany, gdy œcie¿ka jednorazowa siê koñczy

    [HideInInspector] public int currentIndex = 0;
    private int direction = 1;   // 1 – przód, -1 – ty³ (dla ping-ponga)
    protected float waitTimer = 0f;

    private void Start()
    {
        if (path == null || path.waypoints == null || path.waypoints.Length == 0)
        {
            Debug.LogWarning($"{name}: Brak przypisanej œcie¿ki lub waypointów.");
            enabled = false;
            return;
        }

        // Startujemy z pierwszego waypointa
        transform.position = path.waypoints[currentIndex].position;
    }

    private void Update()
    {
        if (path == null || path.waypoints == null || path.waypoints.Length == 0)
            return;

        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        Transform target = path.waypoints[currentIndex];

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= reachDistance)
        {
            OnReachWaypoint();
        }
    }

    private void OnReachWaypoint()
    {
        if (waitTimeAtWaypoint > 0f)
            waitTimer = waitTimeAtWaypoint;

        // Tryb ping-pong (tam i z powrotem)
        if (pingPong && path.waypoints.Length > 1)
        {
            if (currentIndex == path.waypoints.Length - 1)
            {
                direction = -1;
            }
            else if (currentIndex == 0)
            {
                direction = 1;
            }

            currentIndex += direction;
        }
        else
        {
            // Zwyk³y przebieg od 0 do end
            currentIndex++;

            if (currentIndex >= path.waypoints.Length)
            {
                if (loop)
                {
                    // Wracamy na pocz¹tek œcie¿ki
                    currentIndex = 0;
                    LoopCompleted?.Invoke();
                }
                else
                {
                    // Koniec œcie¿ki jednorazowej
                    currentIndex = path.waypoints.Length - 1;
                    PathFinished?.Invoke();
                    enabled = false;
                }
            }
        }
    }

    /// <summary>
    /// Zmienia œcie¿kê w locie. U¿ywane m.in. przez PathRedirectZone.
    /// </summary>
    public void SwitchToPath(WaypointPath newPath, bool snapToClosestPoint = true)
    {
        if (newPath == null || newPath.waypoints == null || newPath.waypoints.Length == 0)
        {
            Debug.LogWarning($"{name}: SwitchToPath – newPath jest niepoprawna.");
            return;
        }

        path = newPath;

        if (snapToClosestPoint)
        {
            // ZnajdŸ najbli¿szy waypoint nowej œcie¿ki
            currentIndex = path.GetClosestWaypointIndex(transform.position);
        }
        else
        {
            // Zaczynamy od pierwszego waypointa nowej œcie¿ki
            currentIndex = 0;
            transform.position = path.waypoints[currentIndex].position;
        }

        // Reset wewnêtrznego stanu
        waitTimer = 0f;
        direction = 1;
        enabled = true;
    }
}
