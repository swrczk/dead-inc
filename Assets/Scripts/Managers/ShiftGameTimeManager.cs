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
    private bool autoStart = true;

    [SerializeField]
    private TMP_Text Points;

    [SerializeField]
    private GameObject Result;

    [SerializeField]
    private bool stopTimeOnEnd = true;

    [SerializeField]
    private ChatMessageData halftimeMessage;

    private float elapsedTime = 0f; // ile realnych sekund min?o w tej rundzie
    private bool isRunning = false;
    private bool halftimeMessageSent = false;

    public event Action GameEnded;

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
        Result.SetActive(false);
        elapsedTime = 0f;
        halftimeMessageSent = false;
        UpdateTimerUI(); // na start np. 08:00 i pe?ne k?ko

        if (autoStart)
            StartTimer();
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    private void Update()
    {
        if (!isRunning)
            return;

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= gameDuration)
        {
            elapsedTime = gameDuration;
            UpdateTimerUI();
            EndGame();
            return;
        }

        UpdateTimerUI();
    }

    private void EndGame()
    {
        Result.SetActive(false);
        Points.text = $"{ScoreManager.Instance.CurrentScore} pkt";
        if (!isRunning)
            return;

        isRunning = false;

        ShiftEndedSignal.Invoke();
    }

    private void UpdateTimerUI()
    {
        // --- aktualizacja zegara tekstowego ---
        if (timerText != null)
        {
            int totalWorkMinutes = Mathf.Max(0, (workEndHour - workStartHour) * 60);

            if (totalWorkMinutes == 0 || gameDuration <= 0f)
            {
                timerText.text = $"{workStartHour:00}:00";
            }
            else
            {
                float t = Mathf.Clamp01(elapsedTime / gameDuration);
                float passedMinutesFloat = t * totalWorkMinutes;
                int passedMinutes = Mathf.FloorToInt(passedMinutesFloat);

                int currentTotalMinutes = workStartHour * 60 + passedMinutes;
                int currentHour = currentTotalMinutes / 60;
                int currentMinute = currentTotalMinutes % 60;

                timerText.text = $"{currentHour:00}:{currentMinute:00}";
            }
        }

        // --- aktualizacja okr?gu czasu ---
        if (timeCircleImage != null && gameDuration > 0f)
        {
            float t = Mathf.Clamp01(elapsedTime / gameDuration); // 0..1 (ile min?o)
            float remaining01 = 1f - t; // 1..0 (ile zosta?o)

            timeCircleImage.fillAmount = remaining01;
        }
    }

    public float GetRemainingRealTime()
    {
        return Mathf.Max(0f, gameDuration - elapsedTime);
    }

    public void ApplyTimePenalty(float penaltySeconds)
    {
        if (penaltySeconds <= 0f)
            return;

        // je?li gra ju? nie trwa, nie ma sensu nak?ada? kary
        if (!isRunning)
            return;

        // zwi?kszamy elapsedTime => mniej czasu zostaje
        elapsedTime += penaltySeconds;

        // upewniamy si?, ?e nie przekroczymy ko?ca rundy
        if (elapsedTime >= gameDuration)
        {
            elapsedTime = gameDuration;
            UpdateTimerUI();
            EndGame();
            return;
        }

        // od?wie? UI po zmianie czasu
        UpdateTimerUI();

        Debug.Log(
            $"[GameTimer] Kara czasowa: -{penaltySeconds}s, elapsed={elapsedTime}, remaining={GetRemainingRealTime()}");
    }
}