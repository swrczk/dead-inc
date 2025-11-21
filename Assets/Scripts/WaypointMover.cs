using UnityEngine;
using System;

public class WaypointMover : MonoBehaviour
{
    [Header("Ustawienia ruchu")]
    public float reachDistance = 0.05f;

    public float waitTimeAtWaypoint = 0.5f;

    [Header("Tryb poruszania")]
    public bool loop = true; // jeśli true – po ostatnim waypointcie wracamy na początek

    public bool pingPong = false; // jeśli true – chodzimy tam i z powrotem

    // Eventy (używane przez NPCPathController – możesz zostawić nawet jeśli jeszcze go nie używasz)
    public event Action LoopCompleted; // wywoływany przy powrocie z końca ścieżki na początek (loop)
    public event Action PathFinished; // wywoływany, gdy ścieżka jednorazowa się kończy

    [HideInInspector]
    public int currentIndex = 0;

    private int _direction = 1; // 1 – przód, -1 – tył (dla ping-ponga)
    protected float WaitTimer = 0f;
    private float _moveSpeed = 2f;

    public WaypointPath Path { get; private set; }

    public void Setup(NpcSet npcData)
    {
        _moveSpeed = npcData.Speed;
        Path = npcData.shoppingPath;
        transform.position = Path.waypoints[currentIndex].position;
    }


    private void Update()
    {
        if (Path == null || Path.waypoints == null || Path.waypoints.Length == 0) return;

        if (WaitTimer > 0f)
        {
            WaitTimer -= Time.deltaTime;
            return;
        }

        Transform target = Path.waypoints[currentIndex];

        transform.position = Vector3.MoveTowards(transform.position, target.position, _moveSpeed * Time.deltaTime);

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= reachDistance)
        {
            OnReachWaypoint();
        }
    }

    private void OnReachWaypoint()
    {
        if (waitTimeAtWaypoint > 0f) WaitTimer = waitTimeAtWaypoint;


        if (pingPong && Path.waypoints.Length > 1)
        {
            if (currentIndex == Path.waypoints.Length - 1)
            {
                _direction = -1;
            }
            else if (currentIndex == 0)
            {
                _direction = 1;
            }

            currentIndex += _direction;
        }
        else
        {
            // Zwykły przebieg od 0 do end
            currentIndex++;

            if (currentIndex >= Path.waypoints.Length)
            {
                if (loop)
                {
                    // Wracamy na początek ścieżki
                    currentIndex = 0;
                    LoopCompleted?.Invoke();
                }
                else
                {
                    // Koniec ścieżki jednorazowej
                    currentIndex = Path.waypoints.Length - 1;
                    PathFinished?.Invoke();
                    enabled = false;
                }
            }
        }
    }

    /// <summary>
    /// Zmienia ścieżkę w locie. Używane m.in. przez PathRedirectZone.
    /// </summary>
    public void SwitchToPath(WaypointPath newPath, bool snapToClosestPoint = true)
    {
        Path = newPath;

        if (snapToClosestPoint)
        {
            currentIndex = Path.GetClosestWaypointIndex(transform.position);
        }
        else
        {
            currentIndex = 0;
            transform.position = Path.waypoints[currentIndex].position;
        }

        WaitTimer = 0f;
        _direction = 1;
        enabled = true;
    }
}