using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Setup/NpcData", fileName = "NpcData")]
public class NpcData : ScriptableObject
{
    public NpcPartData Head;
    public NpcPartData Body;
    public List<object> AvailablePaths;

    [Header("Wrażliwości (typy śmierci)")]
    [Tooltip("Lista typów ataku / przedmiotów, które mogą zabić ten typ NPC.")]
    public List<MurderousItemData> Vulnerabilities;

    public bool IsVulnerableTo(MurderousItemData item)
    {
        if (item == null || Vulnerabilities == null)
            return false;

        return Vulnerabilities.Contains(item);
    }
}
