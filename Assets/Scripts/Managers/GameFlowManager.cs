using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField]
    private GameplayFlow gameplayFlow;

    [SerializeField]
    private GameSettings _gameSettings;

    private NPCSpawner _npcSpawner;

    private int _currentStageIndex;
    private float _time;
    private CancellationTokenSource finishGameTokenSource = new CancellationTokenSource();

    private List<Npc> _currentNpcs = new List<Npc>();
    public GameplayFlow GameplayFlow => gameplayFlow;

    public int CurrentStageIndex => _currentStageIndex;

    void Awake()
    {
        _npcSpawner = FindObjectOfType<NPCSpawner>();
    }

    private void Start()
    {
        Run();
    }

    private void Update()
    {
        _time += Time.deltaTime;
    }

    private async void Run()
    {
        Debug.Log("[GameFlow] Run() started");

        _currentStageIndex = 0;

        foreach (var stage in gameplayFlow.Stages)
        {
            Debug.Log($"[GameFlow] --- Stage {CurrentStageIndex} START ---");

            do
            {
                await HandleSingleStage(stage);

                Debug.Log($"[GameFlow] Waiting for all NPCs to die... Current count: {_currentNpcs.Count}");

                await UniTask.WaitUntil(
                    () => _currentNpcs.Count == 0,
                    cancellationToken: finishGameTokenSource.Token
                );

                if (finishGameTokenSource.IsCancellationRequested)
                {
                    Debug.LogWarning("[GameFlow] Game cancelled – stopping Run()");
                    return;
                }

                Debug.Log($"[GameFlow] Stage {CurrentStageIndex} wave finished. Infinite? {stage.IsInfinite}");
            } while (stage.IsInfinite);

            Debug.Log($"[GameFlow] --- Stage {CurrentStageIndex} COMPLETE ---");
            _currentStageIndex = CurrentStageIndex + 1;
        }

        Debug.Log("[GameFlow] GAME COMPLETE – All stages finished!");
    }

    private async UniTask HandleSingleStage(SingleStage stage)
    {
        Debug.Log("[GameFlow] HandleSingleStage → Preparing NPC queue");

        var queue = stage.GetNpcQueue();

        Debug.Log($"[GameFlow] Queue size: {queue.Count} | Spawn delay: {stage.NpcSpawnDelay}s");

        foreach (var npcData in queue)
        {
            Debug.Log($"[GameFlow] Attempting to spawn NPC: {npcData.NpcType.name}");

            if (_npcSpawner.TrySpawnNPC(npcData, _gameSettings.NpcScale, out var npcObj))
            {
                _currentNpcs.Add(npcObj);
                Debug.Log($"[GameFlow] Spawned NPC {npcObj.Id}. Active NPC count: {_currentNpcs.Count}");
            }
            else
            {
                Debug.LogError($"[GameFlow] FAILED to spawn NPC {npcData.NpcType.name}");
            }

            await UniTask.Delay((int)(stage.NpcSpawnDelay * 1000));
        }

        Debug.Log("[GameFlow] Finished spawning all NPC for this wave");
    }
}