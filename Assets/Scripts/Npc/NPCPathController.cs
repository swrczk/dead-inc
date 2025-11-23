using UnityEngine;

public class NPCPathController : MonoBehaviour
{
    private WaypointMover mover;

    private enum NPCState
    {
        None,
        Shopping,
        Exiting
    }

    private NPCState state = NPCState.None;


    public void Init(NpcSet npcData)
    {
        mover = GetComponent<WaypointMover>();
        mover.Setup(npcData);
        if (mover == null)
        {
            Debug.LogError($"{name}: Brak komponentu WaypointMover.");
            return;
        }

        mover.PathFinished += OnPathFinished;

        StartShopping();
    }

    public void StopMoving()
    {
        mover.Stop();
    }

    private void StartShopping()
    {
        state = NPCState.Shopping; 
        mover.loop = true;
        mover.pingPong = false;
    }


    private void OnPathFinished()
    {
        if (state == NPCState.Exiting)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (mover != null)
        {
            mover.PathFinished -= OnPathFinished;
        }
    }
}