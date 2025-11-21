using UnityEngine;
using UnityEngine.UI;

public class JiraTaskUIEntry : MonoBehaviour
{
    [Header("Ikony celu")]
    [Tooltip("Ikona cechy NPC (np. w³osy, ubranie).")]
    public Image bodyPartIcon;

    [Tooltip("Ikona sposobu zabicia / przedmiotu (np. kabel, piorun, ogieñ).")]
    public Image itemIcon;

    [Header("Punkty")]
    [Tooltip("Tekst z iloœci¹ punktów, np. +15.")]
    public Text pointsText;

    [Tooltip("T³o / badge punktów, zmienia kolor w zale¿noœci od nagrody.")]
    public Image pointsBackground;

    public Color lowPointsColor = Color.green;
    public Color mediumPointsColor = Color.yellow;
    public Color highPointsColor = Color.red;

    [Tooltip("Punkty od których zaczyna siê œrednia nagroda.")]
    public int mediumPointsThreshold = 15;

    [Tooltip("Punkty od których zaczyna siê wysoka nagroda.")]
    public int highPointsThreshold = 30;

    [Header("Powtórzenia")]
    [Tooltip("Tekst z wymaganymi powtórzeniami, np. x3.")]
    public Text repetitionsText;

    [Tooltip("Tekst postêpu, np. 1/3.")]
    public Text progressText;

    [Header("Debug")]
    [Tooltip("Przycisk debugowy do rêcznego zwiêkszania progresu.")]
    public Button debugButton;

    [HideInInspector] public ActiveJiraTask activeTask;
    private JiraTaskManager manager;

    public void Init(ActiveJiraTask task, JiraTaskManager mgr)
    {
        activeTask = task;
        manager = mgr;

        if (activeTask != null && activeTask.Data != null)
        {
            var data = activeTask.Data;

            // Bierzemy pierwszy warunek Required jako „g³ówny”
            Task primary = null;
            if (data.Required != null && data.Required.Count > 0)
                primary = data.Required[0];

            // --- IKONA CECHY NPC (RequiredBodyPart.Icon) ---
            if (bodyPartIcon != null)
            {
                if (primary != null && primary.RequiredBodyPart != null && primary.RequiredBodyPart.Icon != null)
                {
                    bodyPartIcon.sprite = primary.RequiredBodyPart.Icon;
                    bodyPartIcon.enabled = true;
                }
                else
                {
                    bodyPartIcon.enabled = false;
                }
            }

            // --- IKONA PRZEDMIOTU / SPOSOBU ŒMIERCI (ItemToUse.Icon) ---
            if (itemIcon != null)
            {
                if (primary != null && primary.ItemToUse != null && primary.ItemToUse.Icon != null)
                {
                    itemIcon.sprite = primary.ItemToUse.Icon;
                    itemIcon.enabled = true;
                }
                else
                {
                    itemIcon.enabled = false;
                }
            }

            // --- PUNKTY ---
            if (pointsText != null)
            {
                pointsText.text = $"+{data.Points}";
            }

            if (pointsBackground != null)
            {
                Color c = lowPointsColor;
                if (data.Points >= highPointsThreshold)
                    c = highPointsColor;
                else if (data.Points >= mediumPointsThreshold)
                    c = mediumPointsColor;

                pointsBackground.color = c;
            }

            // --- POWTÓRZENIA ---
            if (repetitionsText != null)
            {
                repetitionsText.text = $"x{data.Amount}";
            }
        }

        UpdateProgressUI();

        if (debugButton != null)
        {
            debugButton.onClick.RemoveAllListeners();
            debugButton.onClick.AddListener(OnDebugButtonClicked);
        }
    }

    public void UpdateProgressUI()
    {
        if (activeTask == null || activeTask.Data == null || progressText == null)
            return;

        progressText.text = $"{activeTask.CurrentCount} / {activeTask.Data.Amount}";
    }

    private void OnDebugButtonClicked()
    {
        if (manager != null && activeTask != null)
        {
            manager.DebugIncrementTask(activeTask);
            UpdateProgressUI();
        }
    }
}