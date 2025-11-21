using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIPathRedirectBlocker : MonoBehaviour, IPointerClickHandler
{
    [Header("Elementy blokera (UI)")]
    [Tooltip("Grafika palety / bariery, która ma siê pojawiaæ/znikaæ.")]
    public GameObject visualBlock;

    [Header("Œcie¿ka kontrolowana przez paletê")]
    [Tooltip("Œcie¿ka alejki, któr¹ ta paleta blokuje.")]
    public WaypointPath sourcePath;

    [Header("Œcie¿ka objazdowa")]
    [Tooltip("Œcie¿ka, na któr¹ NPC maj¹ zostaæ przeniesieni, gdy paleta blokuje alejkê.")]
    public WaypointPath detourPath;

    [Header("Ustawienia")]
    public bool startBlocked = false;

    [Tooltip("Jeœli true, paleta sama wyznaczy waypoint blokady jako najbli¿szy jej pozycji.")]
    public bool autoFindBlockWaypoint = true;

    [Tooltip("Index waypointa, przed którym NPC bêd¹ przekierowywani. Jeœli < 0 i autoFindBlockWaypoint = true, zostanie wyznaczony automatycznie.")]
    public int blockWaypointIndex = -1;

    private bool isBlocked;

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

    private void SetBlocked(bool blocked, bool applyToNpcs)
    {
        isBlocked = blocked;

        if (visualBlock != null)
            visualBlock.SetActive(isBlocked);

        if (sourcePath != null)
            sourcePath.SetBlocked(isBlocked);

        if (applyToNpcs && isBlocked && sourcePath != null && detourPath != null)
        {
            RedirectExistingNpcsBeforeBlockPoint();
        }
    }

    private void RedirectExistingNpcsBeforeBlockPoint()
    {
        WaypointMover[] movers = FindObjectsOfType<WaypointMover>();

        foreach (var mover in movers)
        {
            if (mover == null || mover.path == null)
                continue;

            // interesuj¹ nas tylko NPC na tej alejce
            if (mover.path != sourcePath)
                continue;

            // jeœli NPC jest dalej ni¿ punkt blokady – nie dotykamy go
            if (blockWaypointIndex >= 0 && mover.currentIndex > blockWaypointIndex)
                continue;

            // tego NPC przekierowujemy na objazd
            mover.SwitchToPath(detourPath, snapToClosestPoint: true);
        }
    }
}
