using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Setup/MurderousItemData", fileName = "MurderousItemData")]
public class MurderousItemData : ScriptableObject
{
    public List<WeaknessTraitData> AffectsOn;
    public Image Icon;
}