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
 
    public event Action LoopCompleted; 
    public event Action PathFinished; 

    [HideInInspector]
    public int currentIndex = 0;

    private int _direction = 1;  
    private float _waitTimer = 0f;
    private float _moveSpeed = 2f;
    private bool _forceStopNpc;
    private bool _isSetup;

    public WaypointPath Path { get; private set; }

    public void Setup(NpcSet npcData)
    {
        _moveSpeed = npcData.Speed;
        Path = npcData.shoppingPath;
        transform.position = Path.waypoints[currentIndex].position;
        _forceStopNpc = false;
        _isSetup = true;
    }

    public void Stop()
    {
        _forceStopNpc = true;
    }


    private void Update()
    {
        if (!_isSetup || _forceStopNpc) return;

        if (_waitTimer > 0f)
        {
            _waitTimer -= Time.deltaTime;
            return;
        }

        Transform target = Path.waypoints[currentIndex];

        transform.position = Vector3.MoveTowards(transform.position, target.position, _moveSpeed * Time.deltaTime);

        var distance = Vector3.Distance(transform.position, target.position);
        if (distance <= reachDistance)
        {
            OnReachWaypoint();
        }
    }

    private void OnReachWaypoint()
    {
        if (waitTimeAtWaypoint > 0f) _waitTimer = waitTimeAtWaypoint;


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
  
}