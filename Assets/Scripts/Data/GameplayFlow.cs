using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameplayFlow 
{
    public List<SingleStage> Stages;
}

[Serializable]
public class SingleStage
{
    public List<NpcSet> AvailableNpcs;
    public float NpcSpawnDelay = 1;
    public bool IsInfinite;

    public List<NpcSet> GetNpcQueue()
    {
        var result = new List<NpcSet>();

        foreach (var npcSet in AvailableNpcs)
        {
            if (npcSet == null || npcSet.NpcType == null) continue;

            for (int i = 0; i < npcSet.Amount; i++)
            {
                result.Add(npcSet);
            }
        }

        Shuffle(result);

        return result;
    }

    private void Shuffle(List<NpcSet> list)
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
    public int Amount = 1;

    [Range(1, 1000)]
    public float Speed = 1;

    public WaypointPath shoppingPath;
    public WaypointPath exitPath;
    public int lapsToDo = 1;
} 