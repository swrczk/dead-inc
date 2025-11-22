using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }

    [Header("Czas trwania rundy (realny)")]
    [Tooltip("Czas trwania rundy w sekundach (realny czas).")]
    public float gameDuration = 120f;

    [Header("Zakres czasu pracy (wirtualny zegar)")]
    [Tooltip("Godzina rozpocz?cia pracy (0-23).")]
    public int workStartHour = 8;

    [Tooltip("Godzina zako?czenia pracy (0-23).")]
    public int workEndHour = 16;

    [Header("UI")]
    [Tooltip("Tekst w topbarze pokazuj?cy czas, np. 08:00, 12:37, 15:59.")]
    public TextMeshProUGUI timerText;

    [Tooltip("Obrazek okr?gu pokazuj?cy pozosta?y czas (Image typu Filled, Radial).")]
    public Image timeCircleImage;

    [Header("Ustawienia")]
    [Tooltip("Czy timer ma wystartowa? automatycznie w Start().")]
    public bool autoStart = true;

    [Tooltip("Je?li true, po ko?cu gry ustawiamy Time.timeScale = 0.")]
    public bool stopTimeOnEnd = true;

    [Header("Chat messages")]
    [Tooltip("Message shown once when half of the time has passed.")]
    public ChatMessageData halftimeMessage;

    private float elapsedTime = 0f;   // ile realnych sekund min?o w tej rundzie
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
        elapsedTime = 0f;
        halftimeMessageSent = false;
        UpdateTimerUI();  // na start np. 08:00 i pe?ne k?ko

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

        // half-time notification
        if (!halftimeMessageSent && gameDuration > 0f && elapsedTime >= gameDuration * 0.5f)
        {
            halftimeMessageSent = true;

            if (halftimeMessage != null && ChatPopupManager.Instance != null)
            {
                ChatPopupManager.Instance.ShowMessage(halftimeMessage);
            }
        }

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
        if (!isRunning)
            return;

        isRunning = false;

        if (stopTimeOnEnd)
            Time.timeScale = 0f;

        GameEnded?.Invoke();
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
            float remaining01 = 1f - t;                          // 1..0 (ile zosta?o)

            timeCircleImage.fillAmount = remaining01;
        }
    }

    public float GetRemainingRealTime()
    {
        return Mathf.Max(0f, gameDuration - elapsedTime);
    }

    public float GetElapsedRealTime()
    {
        return elapsedTime;
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

        Debug.Log($"[GameTimer] Kara czasowa: -{penaltySeconds}s, elapsed={elapsedTime}, remaining={GetRemainingRealTime()}");
    }
}
