using UnityEngine;

[CreateAssetMenu(fileName = "NpcPartData", menuName = "Setup/NpcPartData")]
public class NpcPartData : ScriptableObject
{
    public Sprite Icon;
    public WeaknessTraitData Weakness;
}