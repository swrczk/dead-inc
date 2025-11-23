using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class OverheatController : MonoBehaviour
{
    [Header("Overheat config")]
    [Tooltip("Max number of uses in time window (e.g. 3 clicks).")]
    public int maxUsesInWindow = 3;

    [Tooltip("Time window length in seconds (e.g. 5 seconds).")]
    public float windowDuration = 5f;

    [Tooltip("Time penalty in seconds applied on overheat.")]
    public float timePenalty = 5f;

    [Tooltip("Should item be locked for some time after overheat.")]
    public bool lockOnOverheat = true;

    [Tooltip("Lock duration after overheat in seconds.")]
    public float lockDuration = 2f;

    [Header("Overheat visuals (color)")]
    [Tooltip("UI element whose color will change (Image, TMP etc.).")]
    public Graphic heatGraphic;

    [Tooltip("If heatGraphic is not set, try to use SpriteRenderer on this object.")]
    public bool useSpriteRendererIfNoGraphic = true;

    [Tooltip("Target color when item is fully overheated.")]
    public Color overheatedColor = Color.red;

    [Header("Chat message on first overheat")]
    public ChatMessageData overheatMessage;

    private bool hasShownOverheatMessage = false;

    [Header("State (read-only)")]
    [SerializeField]
    private bool isOverheated;

    [SerializeField, Range(0f, 1f)]
    private float currentHeatRatio;

    public bool IsOverheated => isOverheated;
    public float CurrentHeatRatio => currentHeatRatio;

    private readonly Queue<float> _useTimestamps = new Queue<float>();
    private float _unlockTime;

    // base color references
    private SpriteRenderer _spriteRenderer;
    private Color _baseColor;
    private bool _hasBaseColor;

    private void Awake()
    {
        // try to auto-detect graphic if not set
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
            Debug.LogWarning($"[Overheat] {name}: no Graphic or SpriteRenderer found, color will not change.");
        }
    }

    /// <summary>
    /// Call this EVERY TIME the item was used unnecessarily
    /// (click that did not kill any valid NPC).
    /// Returns true if item is not currently locked.
    /// </summary>
    public bool RegisterUse()
    {
        float now = Time.time;

        // if still locked after previous overheat, ignore click
        if (isOverheated && lockOnOverheat && now < _unlockTime)
        {
            Debug.Log($"[Overheat] {name} is locked until {_unlockTime}, click ignored.");
            return false;
        }

        // unlock when lock time passed
        if (isOverheated && lockOnOverheat && now >= _unlockTime)
        {
            isOverheated = false;
            Debug.Log($"[Overheat] {name} unlocked.");
        }

        // remove uses older than windowDuration
        while (_useTimestamps.Count > 0 && now - _useTimestamps.Peek() > windowDuration)
        {
            _useTimestamps.Dequeue();
        }

        // add current use
        _useTimestamps.Enqueue(now);

        // recalc heat ratio 0..1
        currentHeatRatio = Mathf.Clamp01((float)_useTimestamps.Count / maxUsesInWindow);

        Debug.Log($"[Overheat] {name}: usesInWindow={_useTimestamps.Count}/{maxUsesInWindow}, heat={currentHeatRatio}");

        // update color for each unnecessary use
        UpdateHeatColor();

        // exceeded limit (e.g. 4th click when max=3)
        if (_useTimestamps.Count > maxUsesInWindow)
        {
            TriggerOverheat(now);
        }

        return true;
    }

    private void TriggerOverheat(float now)
    {
        Debug.Log($"[Overheat] {name} OVERHEATED! Penalty: -{timePenalty}s");

        isOverheated = true;

        if (lockOnOverheat)
        {
            _unlockTime = now + lockDuration;
        }

        ShiftGameTimeManager.Instance.ApplyTimePenalty(timePenalty);

        _useTimestamps.Clear();
        currentHeatRatio = 0f;

        UpdateHeatColor();
    }

    private void UpdateHeatColor()
    {
        if (!_hasBaseColor)
            return;

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