using UnityEngine;
using System;

public class WaypointMover : MonoBehaviour
{
    public event Action PathEnded;

    [Header("Ustawienia ruchu")]
    public float reachDistance = 0.05f;

    public float waitTimeAtWaypoint = 0.5f;

    private int _currentIndex = 0;
    private float _waitTimer = 0f;
    private float _moveSpeed = 2f;
    private bool _forceStopNpc;
    private bool _isSetup;

    public WaypointPath Path { get; private set; }

    public void Setup(NpcSet npcData)
    {
        _moveSpeed = npcData.Speed;
        Path = npcData.shoppingPath;
        transform.position = Path.waypoints[_currentIndex].position;
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

        Transform target = Path.waypoints[_currentIndex];

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
        
        _currentIndex++; 
        if (_currentIndex >= Path.waypoints.Length)
        {
            PathEnded.Invoke();
        }
    }
}