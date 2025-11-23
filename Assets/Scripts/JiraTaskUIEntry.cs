using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class JiraTaskUIEntry : MonoBehaviour
{
    public JiraTaskData Data { get; private set; }

    public bool IsCompleted { get; private set; }

    [SerializeField]
    private TMP_Text pointsText;

    [SerializeField]
    private TMP_Text taskNumber;

    [SerializeField]
    private JiraRequirement requirementRow;

    [SerializeField]
    private GameObject rowsContainer;

    [Header("Duszki")]
    [SerializeField]
    private GameObject amountContainer;

    [SerializeField]
    private JiraTicketAmountElement amountElement;


    private readonly List<JiraRequirement> _requirements = new List<JiraRequirement>();
    private int _totalRequiredKills;


    public void Setup(JiraTaskData task, int taskIndex)
    {
        Data = task;
        ClearChildren(rowsContainer);
        foreach (var requirements in Data.Required)
        {
            var row = Instantiate(requirementRow, rowsContainer.transform);
            row.Setup(requirements);
            _requirements.Add(row);
        }

        taskNumber.text = $"#{taskIndex}";
        pointsText.text = $"+{Data.Points}";
        UpdateProgressUI();
    }

    private void ClearChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform) Destroy(child.gameObject);
    }

    private void UpdateProgressUI()
    {
        // progressText.text = $"{_currentCount} / {Data.Amount}";
    }

    public bool TryUpdateProgress(NpcData npc, MurderousItemData item)
    {
        if (IsCompleted) return false;

        var anyRequirementMatched = false;

        foreach (var requirement in _requirements)
        {
            if (requirement.TryToUpdate(npc,   item))
            {
                anyRequirementMatched = true;
            }
        }

        if (!anyRequirementMatched) return false;

        UpdateProgressUI();

        IsCompleted = _requirements.All(r => r.IsCompleted);

        return IsCompleted;
    }
}