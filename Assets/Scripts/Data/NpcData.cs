using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Setup/NpcData", fileName = "NpcData")]
public class NpcData : ScriptableObject
{
    public NpcPartData Head;
    public NpcPartData Body;

    // TODO: AvailablePaths — do usuniêcia lub zamiany, bo List<object> nic nie daje
    public List<object> AvailablePaths;

    [Header("Wra¿liwoœci NPC")]
    public List<MurderousItemData> Vulnerabilities;

    public bool IsVulnerableTo(MurderousItemData item)
    {
        if (Vulnerabilities == null || item == null)
            return false;

        return Vulnerabilities.Contains(item);
    }
}
