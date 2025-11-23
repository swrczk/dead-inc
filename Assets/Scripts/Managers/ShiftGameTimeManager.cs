using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class ShiftGameTimeManager : MonoBehaviour
{
    public static ShiftGameTimeManager Instance { get; private set; }

    [SerializeField]
    private float gameDuration = 120f;

    [SerializeField]
    private int workStartHour = 8;

    [SerializeField]
    private int workEndHour = 16;

    [SerializeField]
    private TextMeshProUGUI timerText;

    [SerializeField]
    private Image timeCircleImage;
 

    [SerializeField]
    private GameOverUI gameOverUI;
 

    [SerializeField]
    private ChatMessageData halftimeMessage;

    private float _elapsedTime = 0f; // ile realnych sekund min?o w tej rundzie
    private bool _isRunning = false; 
 
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
        gameOverUI.gameObject.SetActive(false);
        _elapsedTime = 0f; 
        _isRunning = true;
        UpdateTimerUI(); 
 
    } 

    private void Update()
    {
        if (!_isRunning)
            return;

        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= gameDuration)
        {
            _elapsedTime = gameDuration;
            UpdateTimerUI();
            EndGame();
            return;
        }

        UpdateTimerUI();
    }

    private void EndGame()
    {
        if (!_isRunning)
            return;

        gameOverUI.gameObject.SetActive(true);
        _isRunning = false;

        ShiftEndedSignal.Invoke();
    }

    private void UpdateTimerUI()
    {
        // --- aktualizacja zegara tekstowego ---
        if (timerText != null)
        {
            var totalWorkMinutes = Mathf.Max(0, (workEndHour - workStartHour) * 60);

            if (totalWorkMinutes == 0 || gameDuration <= 0f)
            {
                timerText.text = $"{workStartHour:00}:00";
            }
            else
            {
                var t = Mathf.Clamp01(_elapsedTime / gameDuration);
                var passedMinutesFloat = t * totalWorkMinutes;
                var passedMinutes = Mathf.FloorToInt(passedMinutesFloat);

                var currentTotalMinutes = workStartHour * 60 + passedMinutes;
                var currentHour = currentTotalMinutes / 60;
                var currentMinute = currentTotalMinutes % 60;

                timerText.text = $"{currentHour:00}:{currentMinute:00}";
            }
        }

        // --- aktualizacja okr?gu czasu ---
        if (timeCircleImage != null && gameDuration > 0f)
        {
            var t = Mathf.Clamp01(_elapsedTime / gameDuration); // 0..1 (ile min?o)
            var remaining01 = 1f - t; // 1..0 (ile zosta?o)

            timeCircleImage.fillAmount = remaining01;
        }
    }

    public float GetRemainingRealTime()
    {
        return Mathf.Max(0f, gameDuration - _elapsedTime);
    }

    public void ApplyTimePenalty(float penaltySeconds)
    {
        if (penaltySeconds <= 0f)
            return;

        // je?li gra ju? nie trwa, nie ma sensu nak?ada? kary
        if (!_isRunning)
            return;

        // zwi?kszamy elapsedTime => mniej czasu zostaje
        _elapsedTime += penaltySeconds;

        // upewniamy si?, ?e nie przekroczymy ko?ca rundy
        if (_elapsedTime >= gameDuration)
        {
            _elapsedTime = gameDuration;
            UpdateTimerUI();
            EndGame();
            return;
        }

        // od?wie? UI po zmianie czasu
        UpdateTimerUI();

        Debug.Log(
            $"[GameTimer] Kara czasowa: -{penaltySeconds}s, elapsed={_elapsedTime}, remaining={GetRemainingRealTime()}");
    }
}