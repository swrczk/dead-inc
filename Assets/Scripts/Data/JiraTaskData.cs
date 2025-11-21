using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(menuName = "Setup/JiraTaskData", fileName = "JiraTaskData")]
public class JiraTaskData : ScriptableObject
{
    public int Amount;
    public int Points;
    public List<Task> Required;
    public List<Task> Forbidden;
}

[Serializable]
public class Task
{
    public NpcPartData RequiredBodyPart;
    public MurderousItemData ItemToUse;

    [Header("Ikony do UI (opcjonalne)")]
    public Sprite BodyPartIcon;   // np. blond w³osy / koszula w kratê
    public Sprite ItemIcon;       // np. piorun / kabel / p³omieñ
}
