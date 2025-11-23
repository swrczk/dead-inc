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

    [Tooltip("Index of waypoint before which NPCs will be redirected. (Not strictly needed in the new logic, but kept for compatibility.)")]
    public int blockWaypointIndex = -1;

    [Header("Live redirect while blocked")]
    [Tooltip("If true, NPCs on the source path will be continuously redirected to detour while blocked.")]
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

        // initial state, but do not touch NPCs yet
        SetBlocked(startBlocked, applyToNpcs: false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ToggleBlocked();
    }

    public void ToggleBlocked()
    {
        // when user clicks, we want to affect NPCs
        SetBlocked(!isBlocked, applyToNpcs: true);
    }

    private void Update()
    {
        // while blocked, continuously ensure no NPCs stay on sourcePath
        if (!isBlocked || !liveRedirectWhileBlocked)
            return;

        if (sourcePath == null || detourPath == null)
            return;

        liveCheckTimer += Time.deltaTime;
        if (liveCheckTimer >= liveCheckInterval)
        {
            liveCheckTimer = 0f;
            RedirectAllNpcsOnPath(sourcePath, detourPath);
        }
    }

    private void SetBlocked(bool blocked, bool applyToNpcs)
    {
        isBlocked = blocked;

        if (visualBlock != null)
        {
            visualBlock.SetActive(isBlocked);
        }

        // reset timer for live redirect
        liveCheckTimer = 0f;

        if (!applyToNpcs)
            return;

        if (isBlocked)
        {
            // closing the alley: push NPCs from source to detour
            if (sourcePath == null || detourPath == null)
            {
                Debug.LogWarning($"[UIPathRedirectBlocker] {name}: Cannot block, missing sourcePath or detourPath.");
                return;
            }

            RedirectAllNpcsOnPath(sourcePath, detourPath);
        }
        else
        {
            // opening the alley: bring NPCs back from detour to source
            if (sourcePath == null || detourPath == null)
            {
                Debug.LogWarning($"[UIPathRedirectBlocker] {name}: Cannot unblock fully, missing sourcePath or detourPath.");
                return;
            }

            RedirectAllNpcsOnPath(detourPath, sourcePath);
        }
    }

    /// <summary>
    /// Redirects all NPCs currently on 'fromPath' to 'toPath'.
    /// Uses snapToClosestPoint = true so they join the new path smoothly near their current position.
    /// </summary>
    private void RedirectAllNpcsOnPath(WaypointPath fromPath, WaypointPath toPath)
    {
        if (fromPath == null || toPath == null)
            return;

        var movers = FindObjectsOfType<WaypointMover>();

        foreach (var mover in movers)
        {
            if (mover == null || mover.Path == null)
                continue;

            if (mover.Path != fromPath)
                continue;

            mover.SwitchToPath(toPath, snapToClosestPoint: true);
        }
    }
}
