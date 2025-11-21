using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField]
    private GameplayFlow gameplayFlow;

    private GameSettings _gameSettings;
    private NPCSpawner _npcSpawner;
    
    private int _currentStage;
    private float _time;
    private CancellationTokenSource finishGameTokenSource = new CancellationTokenSource();
    
    private List<Npc> _currentNpcs = new List<Npc>();

    void Awake()
    {
        _gameSettings = Resources.LoadAll<GameSettings>("").First();
        _npcSpawner = FindObjectOfType<NPCSpawner>();
    }

    private void Start()
    {
        NpcKilledSignal.AddListener(RemoveNpc);
        Run();
    }

    private void Update()
    {
        _time += Time.deltaTime;
    }

    private void RemoveNpc(string npcId)
    {
        var toRemove = _currentNpcs.FirstOrDefault(n => n.Id == npcId);
        if (toRemove != null)
            _currentNpcs.Remove(toRemove);
    }

    private async void Run()
    {
        foreach (var stage in gameplayFlow.Stages)
        {
            do
            {
                await HandleSingleStage(stage);
                await UniTask.WaitUntil(() => _currentNpcs.Count == 0, cancellationToken: finishGameTokenSource.Token);

                if (finishGameTokenSource.IsCancellationRequested)
                {
                    return;
                }
            } while (stage.IsInfinite);
        }
    }

    private async UniTask HandleSingleStage(SingleStage stage)
    {
        foreach (var npcData in stage.GetNpcQueue())
        {
            if (_npcSpawner.TrySpawnNPC(npcData, out var npcObj))
            {
                _currentNpcs.Add(npcObj);
                await UniTask.Delay((int) (stage.NpcSpawnDelay * 1000));
            }
        }
    }
    
    
}