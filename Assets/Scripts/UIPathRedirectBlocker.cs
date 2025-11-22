using UnityEngine;
using UnityEngine.EventSystems;

public class UIPathRedirectBlocker : MonoBehaviour, IPointerClickHandler
{
    [Header("Visuals")]
    [Tooltip("Visual representation of the pallet / barrier.")]
    public GameObject visualBlock;

    [Header("Controlled path")]
    [Tooltip("Path that this pallet is blocking.")]
    public WaypointPath sourcePath;

    [Header("Detour path")]
    [Tooltip("Path to which NPCs should be redirected when the pallet blocks the alley.")]
    public WaypointPath detourPath;

    [Header("Setup")]
    [Tooltip("Should the pallet start in blocked state.")]
    public bool startBlocked = false;

    [Tooltip("If true, blockWaypointIndex will be found automatically as the closest waypoint to this object.")]
    public bool autoFindBlockWaypoint = true;

    [Tooltip("Index of waypoint before which NPCs will be redirected. If autoFindBlockWaypoint = true and this is < 0, it will be set automatically.")]
    public int blockWaypointIndex = -1;

    [Header("Live redirect while blocked")]
    [Tooltip("If true, NPCs reaching the blocked point will be redirected continuously while the pallet is blocked.")]
    public bool liveRedirectWhileBlocked = true;

    [Tooltip("How often (in seconds) to check for NPCs that should be redirected while blocked.")]
    public float liveCheckInterval = 0.25f;

    private bool isBlocked;
    private float liveCheckTimer;

    private void Start()
    {
        if (autoFindBlockWaypoint && sourcePath != null && blockWaypointIndex < 0)
        {
            blockWaypointIndex = sourcePath.GetClosestWaypointIndex(transform.position);
        }

        SetBlocked(startBlocked, applyToNpcs: false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ToggleBlocked();
    }

    public void ToggleBlocked()
    {
        SetBlocked(!isBlocked, applyToNpcs: true);
    }

    private void Update()
    {
        if (!isBlocked || !liveRedirectWhileBlocked)
            return;

        if (sourcePath == null || detourPath == null)
            return;

        liveCheckTimer += Time.deltaTime;
        if (liveCheckTimer >= liveCheckInterval)
        {
            liveCheckTimer = 0f;
            EnforceLiveRedirectForAllNpcs();
        }
    }

    private void SetBlocked(bool blocked, bool applyToNpcs)
    {
        isBlocked = blocked;

        if (visualBlock != null)
        {
            visualBlock.SetActive(isBlocked);
        }

        if (isBlocked && detourPath == null)
        {
            Debug.LogWarning($"[UIPathRedirectBlocker] {name}: detourPath is null while trying to block.");
        }

        if (applyToNpcs && isBlocked && detourPath != null)
        {
            RedirectExistingNpcsBeforeBlockPoint();
        }
    }

    /// <summary>
    /// Called once when pallet is toggled to blocked, to immediately reroute NPCs
    /// that are still before the block waypoint on the source path.
    /// </summary>
    private void RedirectExistingNpcsBeforeBlockPoint()
    {
        if (sourcePath == null || detourPath == null)
            return;

        WaypointMover[] movers = FindObjectsOfType<WaypointMover>();

        foreach (var mover in movers)
        {
            if (mover == null || mover.Path == null)
                continue;

            // Only NPCs currently on the controlled path
            if (mover.Path != sourcePath)
                continue;

            // If NPC is already past the block waypoint, do not touch it
            if (blockWaypointIndex >= 0 && mover.currentIndex > blockWaypointIndex)
                continue;

            // Redirect this NPC to detour path
            mover.SwitchToPath(detourPath, snapToClosestPoint: true);
        }
    }

    /// <summary>
    /// Called periodically while pallet is blocked, to ensure NPCs that reach
    /// or pass the block waypoint on the source path are redirected to detour.
    /// This fixes the issue where NPC would pass through on the second loop.
    /// </summary>
    private void EnforceLiveRedirectForAllNpcs()
    {
        if (sourcePath == null || detourPath == null)
            return;

        WaypointMover[] movers = FindObjectsOfType<WaypointMover>();

        foreach (var mover in movers)
        {
            if (mover == null || mover.Path == null)
                continue;

            // Only NPCs currently on the controlled path
            if (mover.Path != sourcePath)
                continue;

            // If we have a valid block waypoint index, redirect NPCs that reached or passed it
            if (blockWaypointIndex >= 0)
            {
                if (mover.currentIndex < blockWaypointIndex)
                    continue;
            }

            mover.SwitchToPath(detourPath, snapToClosestPoint: true);
        }
    }
}
