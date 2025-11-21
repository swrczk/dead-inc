using System;
using System.Linq;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField]
    private GameplayFlow gameplayFlow;

    private GameSettings _gameSettings;
    private NPCSpawner _npcSpawner;
    
    private int _currentStage;
    private float _time;

    void Awake()
    {
        _gameSettings = Resources.LoadAll<GameSettings>("").First();
        _npcSpawner = FindObjectOfType<NPCSpawner>();
    }

    private void Update()
    {
        _time += Time.deltaTime;
    }
    
    // private 
    
    
}