using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Setup/NpcData", fileName = "NpcData")]
public class NpcData : ScriptableObject
{
    public NpcPartData Head;
    public NpcPartData Body;
    public List<object> AvailablePaths;

    [Header("Wra¿liwoœci (typy œmierci)")]
    [Tooltip("Lista typów ataku / przedmiotów, które mog¹ zabiæ ten typ NPC.")]
    public List<MurderousItemData> Vulnerabilities;

    public bool IsVulnerableTo(MurderousItemData item)
    {
        if (item == null || Vulnerabilities == null)
            return false;

        return Vulnerabilities.Contains(item);
    }
}
