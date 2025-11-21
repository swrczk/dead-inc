using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private string pointsSuffix = " pkt";

    [SerializeField]
    private int baseKillPoints;

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
        NpcTypeKilledSignal.AddListener(OnNpcKilled);
        JiraTicketCompleted.AddListener(OnJiraTicketCompleted);
        UpdateScoreUI();
    }

    private void OnJiraTicketCompleted(int points)
    {
        AddPoints(points);
    }

    private void OnNpcKilled(NpcData npcData, MurderousItemData murderousItemData)
    {
        AddPoints(baseKillPoints);
    }

    public void AddPoints(int amount)
    {
        CurrentScore += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = CurrentScore + pointsSuffix;
        }
    }
}