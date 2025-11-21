using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // dla Graphic (Image, Text itp.)

public class OverheatController : MonoBehaviour
{
    [Header("Konfiguracja przegrzania")]
    [Tooltip("Maksymalna liczba u¿yæ w oknie czasu (np. 3 klikniêcia).")]
    public int maxUsesInWindow = 3;

    [Tooltip("D³ugoœæ okna czasu w sekundach (np. 5 sekund).")]
    public float windowDuration = 5f;

    [Tooltip("Kara czasowa za przegrzanie (w sekundach).")]
    public float timePenalty = 5f;

    [Tooltip("Czy po przegrzaniu tymczasowo blokowaæ obiekt.")]
    public bool lockOnOverheat = true;

    [Tooltip("Na ile sekund zablokowaæ obiekt po przegrzaniu.")]
    public float lockDuration = 2f;

    [Header("Wizualizacja przegrzania (kolor)")]
    [Tooltip("Element UI, którego kolor ma siê zmieniaæ (Image, TextMeshProUGUI itp.).")]
    public Graphic heatGraphic;

    [Tooltip("Jeœli nie ustawisz heatGraphic, spróbujê u¿yæ SpriteRenderer na tym obiekcie.")]
    public bool useSpriteRendererIfNoGraphic = true;

    [Tooltip("Kolor, do którego 'idzie' przedmiot przy pe³nym przegrzaniu.")]
    public Color overheatedColor = Color.red;

    [Header("Podgl¹d stanu (read-only)")]
    [SerializeField] private bool isOverheated;
    [SerializeField, Range(0f, 1f)] private float currentHeatRatio;

    public bool IsOverheated => isOverheated;
    public float CurrentHeatRatio => currentHeatRatio;

    private readonly Queue<float> _useTimestamps = new Queue<float>();
    private float _unlockTime;

    // referencje do koloru bazowego
    private SpriteRenderer _spriteRenderer;
    private Color _baseColor;
    private bool _hasBaseColor;

    private void Awake()
    {
        // jeœli nie przypiêto grafiki, spróbuj j¹ znaleŸæ automatycznie
        if (heatGraphic == null)
        {
            heatGraphic = GetComponent<Graphic>();
        }

        if (heatGraphic != null)
        {
            _baseColor = heatGraphic.color;
            _hasBaseColor = true;
        }
        else if (useSpriteRendererIfNoGraphic)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer != null)
            {
                _baseColor = _spriteRenderer.color;
                _hasBaseColor = true;
            }
        }

        if (!_hasBaseColor)
        {
            Debug.LogWarning($"[Overheat] {name}: brak Graphic ani SpriteRenderer – nie bêdê zmienia³ koloru.");
        }
    }

    /// <summary>
    /// Wywo³uj to ZA KA¯DYM RAZEM, gdy obiekt zosta³ u¿yty NIEPOTRZEBNIE
    /// (np. klikniêcie, które nie zabi³o ¿adnego prawid³owego NPC).
    /// Zwraca true, jeœli obiekt nie jest aktualnie zablokowany.
    /// </summary>
    public bool RegisterUse()
    {
        float now = Time.time;

        // jeœli jest jeszcze blokada po przegrzaniu – ignorujemy klik
        if (isOverheated && lockOnOverheat && now < _unlockTime)
        {
            Debug.Log($"[Overheat] {name} jest zablokowany do {_unlockTime}, klik zignorowany.");
            return false;
        }

        // jeœli czas blokady min¹³ – odblokuj
        if (isOverheated && lockOnOverheat && now >= _unlockTime)
        {
            isOverheated = false;
            Debug.Log($"[Overheat] {name} odblokowany.");
        }

        // wyrzuæ u¿ycia starsze ni¿ windowDuration
        while (_useTimestamps.Count > 0 && now - _useTimestamps.Peek() > windowDuration)
        {
            _useTimestamps.Dequeue();
        }

        // dodaj bie¿¹ce u¿ycie
        _useTimestamps.Enqueue(now);

        // update „poziomu przegrzania” 0..1
        currentHeatRatio = Mathf.Clamp01((float)_useTimestamps.Count / maxUsesInWindow);

        Debug.Log($"[Overheat] {name}: usesInWindow={_useTimestamps.Count}/{maxUsesInWindow}, heat={currentHeatRatio}");

        // po ka¿dym niepotrzebnym u¿yciu aktualizujemy kolor
        UpdateHeatColor();

        // przekroczenie limitu (np. 4 klikniêcie przy max=3)
        if (_useTimestamps.Count > maxUsesInWindow)
        {
            TriggerOverheat(now);
        }

        return true;
    }

    private void TriggerOverheat(float now)
    {
        Debug.Log($"[Overheat] {name} PRZEGRZANY! Kara: -{timePenalty}s");

        isOverheated = true;

        if (lockOnOverheat)
        {
            _unlockTime = now + lockDuration;
        }

        // na³ó¿ karê na timer (zak³adamy GameTimer.Instance z metod¹ ApplyTimePenalty)
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.ApplyTimePenalty(timePenalty);
        }
        else
        {
            Debug.LogWarning("[Overheat] GameTimer.Instance == null – nie mogê na³o¿yæ kary czasowej.");
        }

        // po przegrzaniu mo¿esz zacz¹æ liczyæ od nowa
        _useTimestamps.Clear();
        currentHeatRatio = 0f;

        // zresetuj kolor do bazowego
        UpdateHeatColor();
    }

    private void UpdateHeatColor()
    {
        if (!_hasBaseColor)
            return;

        // currentHeatRatio 0..1 – im bli¿ej limitu, tym bardziej czerwony
        float t = currentHeatRatio;

        Color targetColor = Color.Lerp(_baseColor, overheatedColor, t);

        if (heatGraphic != null)
        {
            heatGraphic.color = targetColor;
        }
        else if (_spriteRenderer != null)
        {
            _spriteRenderer.color = targetColor;
        }
    }
}
