using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings")]
public class GameSettings : ScriptableObject
{
    public int Time;
    public int RewardPoints;
    public int PenaltyPoints;
}