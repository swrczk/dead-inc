using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Panel z podsumowaniem (root GameObject, kt?ry w??czamy na ko?cu gry).")]
    public GameObject gameOverPanel;

    [Tooltip("Tekst z wynikiem gracza.")]
    public Text finalScoreText;

    [Tooltip("Tekst z najlepszym wynikiem (opcjonalnie).")]
    public Text bestScoreText;

    [Tooltip("Sufiks, np. ' pkt'.")]
    public string pointsSuffix = " pkt";

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (GameTimer.Instance != null)
            GameTimer.Instance.GameEnded += OnGameEnded;
    }

    private void OnDestroy()
    {
        if (GameTimer.Instance != null)
            GameTimer.Instance.GameEnded -= OnGameEnded;
    }

    private void OnGameEnded()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        int score = 0;
        if (ScoreManager.Instance != null)
            score = ScoreManager.Instance.CurrentScore;

        if (finalScoreText != null)
            finalScoreText.text = "Wynik: " + score + pointsSuffix;

        if (bestScoreText != null)
        {
            int best = PlayerPrefs.GetInt("BestScore", 0);
            if (score > best)
            {
                best = score;
                PlayerPrefs.SetInt("BestScore", best);
            }

            bestScoreText.text = "Najlepszy: " + best + pointsSuffix;
        }
    }

    // Podpi?? pod przycisk "Zagraj jeszcze raz"
    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Podpi?? pod przycisk "Wyj?cie" (opcjonalnie)
    public void OnQuitButton()
    {
        Application.Quit();
    }
}
