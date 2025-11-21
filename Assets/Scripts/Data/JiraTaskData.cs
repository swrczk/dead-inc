using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(menuName = "Setup/JiraTaskData", fileName = "JiraTaskData")]
public class JiraTaskData : ScriptableObject
{
    public int Amount;
    public int Points;
    public List<Task> Required;
    // public List<Task> Forbidden;
}

[Serializable]
public class Task
{
    public NpcPartData RequiredBodyPart;
    public MurderousItemData ItemToUse;
    
}
