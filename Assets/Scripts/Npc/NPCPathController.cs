using UnityEngine;

public class NPCPathController : MonoBehaviour
{
    public WaypointPath shoppingPath;
    public WaypointPath exitPath;
    public int lapsToDo = 1;

    private WaypointMover mover;
    private int lapsDone = 0;

    private enum NPCState
    {
        None,
        Shopping,
        Exiting
    }

    private NPCState state = NPCState.None;
    private bool initialized = false;


    public void Init(NpcSet npcData)
    {
        mover = GetComponent<WaypointMover>();
        mover.Setup(npcData);
        if (mover == null)
        {
            Debug.LogError($"{name}: Brak komponentu WaypointMover.");
            return;
        }

        shoppingPath = npcData.shoppingPath;
        exitPath = npcData.exitPath;
        lapsToDo = npcData.lapsToDo;

        // podpinamy si? pod eventy z movera
        mover.LoopCompleted += OnLoopCompleted;
        mover.PathFinished += OnPathFinished;

        StartShopping();
        initialized = true;
    }

    private void StartShopping()
    {

        state = NPCState.Shopping;
        lapsDone = 0;

        mover.loop = true;
        mover.pingPong = false;
        mover.SwitchToPath(shoppingPath, snapToClosestPoint: false);
    }

    private void GoToExit()
    {
        state = NPCState.Exiting;

        // ?cie?ka do wyj?cia ? jednorazowa, bez loopa
        mover.loop = false;
        mover.pingPong = false;
        mover.SwitchToPath(exitPath, snapToClosestPoint: false);
    }

    private void OnLoopCompleted()
    {
        if (state != NPCState.Shopping) return;

        lapsDone++;

        if (lapsDone >= lapsToDo)
        {
            GoToExit();
        }
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
            mover.LoopCompleted -= OnLoopCompleted;
            mover.PathFinished -= OnPathFinished;
        }
    }
}