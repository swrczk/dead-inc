using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text finalScoreText;

    [SerializeField]
    private TMP_Text bodyCountText;

    [SerializeField]
    private TMP_Text taskCountText;

    [SerializeField]
    private Button restartButton;

    [SerializeField]
    private Button menuButton;

    private void Start()
    {
        restartButton.onClick.AddListener(OnRestartButton);
        menuButton.onClick.AddListener(OnQuitButton);
    }

    private void OnEnable()
    {
        finalScoreText.text = ScoreManager.Instance.CurrentScore.ToString();
        bodyCountText.text = ScoreManager.Instance.BodyCount.ToString();
        taskCountText.text = ScoreManager.Instance.JiraTicketsDoneCount.ToString();
    }

    private void OnRestartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnQuitButton()
    {
        SceneManager.LoadScene(0);
    }
}