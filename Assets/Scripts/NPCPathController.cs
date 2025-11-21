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

    public void Init(WaypointPath shopping, WaypointPath exit, int laps)
    {
        mover = GetComponent<WaypointMover>();
        if (mover == null)
        {
            Debug.LogError($"{name}: Brak komponentu WaypointMover.");
            return;
        }

        shoppingPath = shopping;
        exitPath = exit;
        lapsToDo = Mathf.Max(1, laps);

        // podpinamy siê pod eventy z movera
        mover.LoopCompleted += OnLoopCompleted;
        mover.PathFinished += OnPathFinished;

        StartShopping();
        initialized = true;
    }

    private void Start()
    {
        if (!initialized)
        {
            Debug.LogWarning($"{name}: NPCPathController nie zosta³ zainicjalizowany przez spawner.");
        }
    }

    private void StartShopping()
    {
        if (shoppingPath == null)
        {
            Debug.LogError($"{name}: Brak shoppingPath w NPCPathController.");
            return;
        }

        state = NPCState.Shopping;
        lapsDone = 0;

        mover.loop = true;
        mover.pingPong = false;
        mover.SwitchToPath(shoppingPath, snapToClosestPoint: false);
    }

    private void GoToExit()
    {
        if (exitPath == null)
        {
            Debug.LogError($"{name}: Brak exitPath w NPCPathController.");
            return;
        }

        state = NPCState.Exiting;

        // Œcie¿ka do wyjœcia – jednorazowa, bez loopa
        mover.loop = false;
        mover.pingPong = false;
        mover.SwitchToPath(exitPath, snapToClosestPoint: false);
    }

    private void OnLoopCompleted()
    {
        if (state != NPCState.Shopping)
            return;

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
            // dotar³ do wyjœcia – usuwamy NPC
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
