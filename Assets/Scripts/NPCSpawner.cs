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

    public void TrySpawnNPC(NpcData npcData)
    // public void TrySpawnNPC( )
    {
        if (npcPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning("NPCSpawner: Brak prefab lub spawnPoint.");
            return;
        }

        if (shoppingPaths == null || shoppingPaths.Length == 0 || exitPath == null)
        {
            Debug.LogWarning("NPCSpawner: Brak przypisanych �cie�ek.");
            return;
        }

        // Limit NPC na scenie
        if (maxNpcsAlive > 0)
        {
            int alive = FindObjectsOfType<NPCPathController>().Length;
            if (alive >= maxNpcsAlive)
                return;
        }

        // Zbierz wszystkie ISTNIEJ�CE �cie�ki (ignorujemy isBlocked,
        // bo palety maj� dzia�a� tylko na ju� chodz�cych NPC)
        List<WaypointPath> available = new List<WaypointPath>();
        foreach (var p in shoppingPaths)
        {
            if (p != null)
                available.Add(p);
        }

        if (available.Count == 0)
        {
            Debug.LogWarning("NPCSpawner: Brak dost�pnych �cie�ek (wszystkie null).");
            return;
        }

        WaypointPath chosenShoppingPath = available[Random.Range(0, available.Count)];
        int laps = Random.Range(minLaps, maxLaps + 1);

        Transform parent = supermarketCanvasTransform != null ? supermarketCanvasTransform : null;

        var npcObj = Instantiate(
            npcPrefab, 
            spawnPoint.position,
            Quaternion.identity,
            parent    // NPC jako child canvasa (je�li ustawiony)
        );
        
        npcObj.Setup(npcData);

        var controller = npcObj.GetComponent<NPCPathController>();
        if (controller == null)
        {
            Debug.LogError("NPCSpawner: Prefab NPC nie ma NPCPathController.");
            return;
        }

        controller.Init(chosenShoppingPath, exitPath, laps);
    }
}
