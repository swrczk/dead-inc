using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [Header("NPC")]
    public Npc npcPrefab;

    public Transform spawnPoint;

    private int npcCount = 0;

    public bool TrySpawnNPC(NpcSet npcData, Vector3 gameSettingsNpcScale, out Npc npcObj)
    {
        npcObj = null;

        npcObj = Instantiate(npcPrefab, spawnPoint.position, Quaternion.identity, transform);

        npcObj.Setup($"npc_id_{++npcCount}", npcData.NpcType);
        npcObj.PathController.Init(npcData);
        npcObj.transform.localScale = gameSettingsNpcScale;

        return npcObj;
    }
}