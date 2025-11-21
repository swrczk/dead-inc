using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Setup/NpcData", fileName = "NpcData")]
public class NpcData : ScriptableObject
{
    public NpcPartData Head;
    public NpcPartData Body;
    public List<object> AvailablePaths;
}