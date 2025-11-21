using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Setup/MurderousItemData", fileName = "MurderousItemData")]
public class MurderousItemData : ScriptableObject
{
    public List<WeaknessTraitData> AffectsOn;
    public Sprite Icon;
}