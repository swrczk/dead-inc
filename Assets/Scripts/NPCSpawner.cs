using UnityEngine;
using System.Collections.Generic;

public class NPCSpawner : MonoBehaviour
{
    [Header("NPC")]
    public GameObject npcPrefab;
    public Transform spawnPoint;

    [Header("Canvas")]
    public Transform supermarketCanvasTransform;

    [Header("Œcie¿ki")]
    public WaypointPath[] shoppingPaths;
    public WaypointPath exitPath;

    [Header("Parametry spawnu")]
    public float spawnInterval = 3f;
    public int minLaps = 1;
    public int maxLaps = 3;
    public int maxNpcsAlive = 20;

    private float spawnTimer = 0f;

    private void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            TrySpawnNPC();
            spawnTimer = spawnInterval;
        }
    }

    private void TrySpawnNPC()
    {
        if (npcPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning("NPCSpawner: Brak prefab lub spawnPoint.");
            return;
        }

        if (shoppingPaths == null || shoppingPaths.Length == 0 || exitPath == null)
        {
            Debug.LogWarning("NPCSpawner: Brak przypisanych œcie¿ek.");
            return;
        }

        // Limit NPC na scenie
        if (maxNpcsAlive > 0)
        {
            int alive = FindObjectsOfType<NPCPathController>().Length;
            if (alive >= maxNpcsAlive)
                return;
        }

        // Zbierz wszystkie ISTNIEJ¥CE œcie¿ki (ignorujemy isBlocked,
        // bo palety maj¹ dzia³aæ tylko na ju¿ chodz¹cych NPC)
        List<WaypointPath> available = new List<WaypointPath>();
        foreach (var p in shoppingPaths)
        {
            if (p != null)
                available.Add(p);
        }

        if (available.Count == 0)
        {
            Debug.LogWarning("NPCSpawner: Brak dostêpnych œcie¿ek (wszystkie null).");
            return;
        }

        WaypointPath chosenShoppingPath = available[Random.Range(0, available.Count)];
        int laps = Random.Range(minLaps, maxLaps + 1);

        Transform parent = supermarketCanvasTransform != null ? supermarketCanvasTransform : null;

        GameObject npcObj = Instantiate(
            npcPrefab,
            spawnPoint.position,
            Quaternion.identity,
            parent    // NPC jako child canvasa (jeœli ustawiony)
        );

        var controller = npcObj.GetComponent<NPCPathController>();
        if (controller == null)
        {
            Debug.LogError("NPCSpawner: Prefab NPC nie ma NPCPathController.");
            return;
        }

        controller.Init(chosenShoppingPath, exitPath, laps);
    }
}
