using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI")]
    [Tooltip("Tekst w top barze, np. '2137 pkt'.")]
    public TextMeshProUGUI scoreText;

    [Header("Ustawienia")]
    [Tooltip("Prefix/suffix do wyœwietlania punktów, np. ' pkt'.")]
    public string pointsSuffix = " pkt";

    public int CurrentScore { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        UpdateScoreUI();
    }

    public void AddPoints(int amount)
    {
        CurrentScore += amount;
        if (CurrentScore < 0)
            CurrentScore = 0;

        UpdateScoreUI();
    }

    /// <summary>
    /// Hook, który zawo³asz przy zabiciu NPC (z dowoln¹ iloœci¹ punktów).
    /// </summary>
    public void OnNpcKilled(int baseKillPoints)
    {
        AddPoints(baseKillPoints);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = CurrentScore.ToString() + pointsSuffix;
        }
    }
}
