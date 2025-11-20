using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Setup/NpcData", fileName = "NpcData")]
public class NpcData : ScriptableObject
{
    public NpcPartData Hair;
    public NpcPartData Chest;
    public NpcPartData Legs;
    public List<object> AvailablePaths;
}