using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NpcPartData", menuName = "Setup/NpcPartData")]
public class NpcPartData : ScriptableObject
{
    public BodyPart BodyPart;
    public Image Icon;
    public WeaknessTraitData Weakness;
}

public enum BodyPart
{
    Head,
    Body,
}