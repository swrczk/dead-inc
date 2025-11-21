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
    public List<NpcSet> AvailableNpcs;
    public bool IsInfinite;
}

[Serializable]
public class NpcSet
{
    public NpcData NpcType;
    [Range(1,10)]
    public int Amount;
    [Range(1,10)]
    public float Speed;
}