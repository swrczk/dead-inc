using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(menuName = "Setup/JiraTaskData", fileName = "JiraTaskData")]
public class JiraTaskData : ScriptableObject
{
    public int Points;
    public List<TaskRequirement> Required;
}

[Serializable]
public class TaskRequirement
{
    public NpcPartData RequiredBodyPart;
    public MurderousItemData ItemToUse;
    public WeaknessTraitData WeaknessToUse;
    public int Amount;
}