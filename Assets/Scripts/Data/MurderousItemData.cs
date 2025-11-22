using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "Setup/MurderousItemData", fileName = "MurderousItem")]
public class MurderousItemData : ScriptableObject
{
    public Sprite Icon;
    public WeaknessTraitData Weakness;
    public AudioClip Sound;
    public VideoClip Clip;
    public bool WasPlayed = false;
}