using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIPathRedirectBlocker : MonoBehaviour, IPointerClickHandler
{
    [Header("Elementy blokera (UI)")]
    [Tooltip("Grafika palety / bariery, kt?ra ma si? pojawia?/znika?.")]
    public GameObject visualBlock;

    [Header("ścieżka kontrolowana przez paletę")]
    [Tooltip("ścieżka alejki, ktora ta paleta blokuje.")]
    public WaypointPath sourcePath;

    [Header("ścieżka objazdowa")]
    [Tooltip("ścieżka, na ktora NPC maja zosta? przeniesieni, gdy paleta blokuje alejk?.")]
    public WaypointPath detourPath;

    [Header("Ustawienia")]
    public bool startBlocked = false;

    [Tooltip("Je?li true, paleta sama wyznaczy waypoint blokady jako najbli?szy jej pozycji.")]
    public bool autoFindBlockWaypoint = true;

    [Tooltip(
        "Index waypointa, przed ktorym NPC b?d? przekierowywani. Jezli < 0 i autoFindBlockWaypoint = true, zostanie wyznaczony automatycznie.")]
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

        if (visualBlock != null) visualBlock.SetActive(isBlocked);

        if (sourcePath != null) sourcePath.SetBlocked(isBlocked);

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
            if (mover == null || mover.path == null) continue;

            // interesuj? nas tylko NPC na tej alejce
            if (mover.path != sourcePath) continue;

            // je?li NPC jest dalej ni? punkt blokady ? nie dotykamy go
            if (blockWaypointIndex >= 0 && mover.currentIndex > blockWaypointIndex) continue;

            // tego NPC przekierowujemy na objazd
            mover.SwitchToPath(detourPath, snapToClosestPoint: true);
        }
    }
}