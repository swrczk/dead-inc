using UnityEngine;
using BrunoMikoski.AnimationSequencer;
using Cysharp.Threading.Tasks;
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
    private GameOverUI gameOverUI;

    [SerializeField]
    private AudioClip endShiftSound;

    [SerializeField]
    private AnimationSequencerController endShiftAnimation;


    private float _elapsedTime = 0f;  
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
            EndGame().Forget();
            return;
        }

        UpdateTimerUI();
    }

    private async UniTask EndGame()
    {
        if (!_isRunning)
            return;
        
        _isRunning = false;
        ShiftEndedSignal.Invoke();
        
        SoundManager.Instance.Play(endShiftSound);
        endShiftAnimation.Play();
        await UniTask.WaitUntil(() => !endShiftAnimation.IsPlaying);
        
        gameOverUI.gameObject.SetActive(true);

    }

    private void UpdateTimerUI()
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
}