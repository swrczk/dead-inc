using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text finalScoreText;

    [SerializeField]
    private string pointsSuffix = " pkt";

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
        finalScoreText.text = ScoreManager.Instance.CurrentScore + pointsSuffix;
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