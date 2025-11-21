using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Setup/MurderousItemData", fileName = "MurderousItem")]
public class MurderousItemData : ScriptableObject
{
    public Sprite Icon;
    public WeaknessTraitData Weakness;   // ten sam typ co w NpcPartData.Weakness
    public AudioClip Sound;
}