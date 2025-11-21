using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameplayFlow", menuName = "GameplayFlow")]
public class GameplayFlow : ScriptableObject
{
    public List<SingleStage> Stages;
}

[Serializable]
public class SingleStage
{
    [SerializeField]
    public List<NpcSet> AvailableNpcs;

    public float NpcSpawnDelay;
    public bool IsInfinite;

    public List<NpcData> GetNpcQueue()
    {
        var result = new List<NpcData>();
        
        foreach (var npcSet in AvailableNpcs)
        {
            if (npcSet == null || npcSet.NpcType == null) continue;

            for (int i = 0; i < npcSet.Amount; i++)
            {
                result.Add(npcSet.NpcType);
            }
        }
        
        Shuffle(result);

        return result;
    }

    private void Shuffle(List<NpcData> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}

[Serializable]
public class NpcSet
{
    public NpcData NpcType;

    [Range(1, 10)]
    public int Amount;

    [Range(1, 10)]
    public float Speed;
}