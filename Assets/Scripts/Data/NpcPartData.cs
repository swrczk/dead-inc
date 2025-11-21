using UnityEngine;

[CreateAssetMenu(fileName = "NpcPartData", menuName = "Setup/NpcPartData")]
public class NpcPartData : ScriptableObject
{
    public BodyPart BodyPart;
    public Sprite Icon;
    public WeaknessTraitData Weakness;
}

public enum BodyPart
{
    Head,
    Body,
}