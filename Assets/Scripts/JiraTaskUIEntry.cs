using TMPro;
using UnityEngine;

public class JiraTaskUIEntry : MonoBehaviour
{
    public JiraTaskData Data { get; private set; }

    public bool IsCompleted { get; private set; }

    [SerializeField]
    private TMP_Text pointsText;

    // [Tooltip("  1/3 ")]
    // [SerializeField]
    // private TMP_Text progressText;

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


    
    private int _currentCount;


    public void Setup(JiraTaskData task)
    {
        Data = task;

        foreach (var requirements in Data.Required)
        {
            var row = Instantiate(requirementRow, rowsContainer.transform);
            row.Setup(requirements);
        }

        pointsText.text = $"+{Data.Points}";
        UpdateProgressUI();
    }

    private void UpdateProgressUI()
    {
        // progressText.text = $"{_currentCount} / {Data.Amount}";
    }

    public bool TryUpdateProgress(NpcData npc, MurderousItemData item)
    {
        var result = true;
        foreach (var requirement in Data.Required)
        {
            if (requirement.RequiredBodyPart != null && requirement.RequiredBodyPart != npc.Body &&
                requirement.RequiredBodyPart != npc.Head)
            {
                result = false;
            }

            if (requirement.ItemToUse != null && requirement.ItemToUse != item)
            {
                result = false;
            }

            if (requirement.WeaknessToUse != null && requirement.WeaknessToUse != item)
            {
                result = false;
            }
        }

        return result;
    }
}