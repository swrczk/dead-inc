using UnityEngine;
using System.Collections.Generic;

public class NPCSpawner : MonoBehaviour
{
    [Header("NPC")]
    public Npc npcPrefab;
    public Transform spawnPoint;

    [Header("Canvas")]
    public Transform supermarketCanvasTransform;

    [Header("Paths")]
    public WaypointPath[] shoppingPaths;
    public WaypointPath exitPath;

    [Header("Parametry spawnu")]
    public float spawnInterval = 3f;
    public int minLaps = 1;
    public int maxLaps = 3;
    public int maxNpcsAlive = 20;

    private float spawnTimer = 0f;
    
    private int npcCount = 0;

    // private void Update()
    // {
    //     spawnTimer -= Time.deltaTime;
    //
    //     if (spawnTimer <= 0f)
    //     {
    //         TrySpawnNPC();
    //         spawnTimer = spawnInterval;
    //     }
    // }

    public bool TrySpawnNPC(NpcData npcData, out Npc npcObj) 
    {
        npcObj = null;
        if (npcPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning("NPCSpawner: Brak prefab lub spawnPoint.");
            return false;
        }

        if (shoppingPaths == null || shoppingPaths.Length == 0 || exitPath == null)
        {
            Debug.LogWarning("NPCSpawner: Brak przypisanych �cie�ek.");
            return false;
        }

        // Limit NPC na scenie
        if (maxNpcsAlive > 0)
        {
            var alive = FindObjectsOfType<NPCPathController>().Length;
            if (alive >= maxNpcsAlive) return false;
        }

        // Zbierz wszystkie ISTNIEJ�CE �cie�ki (ignorujemy isBlocked,
        // bo palety maj� dzia�a� tylko na ju� chodz�cych NPC)
        var available = new List<WaypointPath>();
        foreach (var p in shoppingPaths)
        {
            if (p != null) available.Add(p);
        }

        var chosenShoppingPath = available[Random.Range(0, available.Count)];
        var laps = Random.Range(minLaps, maxLaps + 1);

        var parent = supermarketCanvasTransform != null ? supermarketCanvasTransform : null;

        npcObj = Instantiate(npcPrefab, spawnPoint.position, Quaternion.identity, parent);

        npcObj.Setup($"npc_id_{++npcCount}",npcData);

        var controller = npcObj.GetComponent<NPCPathController>();
        controller.Init(chosenShoppingPath, exitPath, laps);

        return npcObj;
    }
}
