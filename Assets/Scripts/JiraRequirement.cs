using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JiraRequirement : MonoBehaviour
{
    [SerializeField]
    private Image bodyPartIcon;

    [SerializeField]
    private Image itemIcon;

    [SerializeField]
    private Image weaknessIcon;

    [SerializeField]
    private TMP_Text repetitionsText;

    [SerializeField]
    private GameObject completedMark;

    public TaskRequirement Requirement { get; private set; }

    public int CurrentCount { get; private set; }
    public bool IsCompleted =>   CurrentCount >= Requirement.Amount;

    public void Setup(TaskRequirement taskRequirementData)
    {
        Requirement = taskRequirementData;
        CurrentCount = 0;
        var hasItem = taskRequirementData.ItemToUse != null;
        var hasBody = taskRequirementData.RequiredBodyPart != null;
        var hasWeakness = taskRequirementData.WeaknessToUse != null;

        bodyPartIcon.gameObject.SetActive(hasBody);
        itemIcon.gameObject.SetActive(hasItem);
        weaknessIcon.gameObject.SetActive(hasWeakness);

        if (hasItem)
        {
            itemIcon.sprite = taskRequirementData.ItemToUse.Icon;
        }

        if (hasBody)
        {
            bodyPartIcon.sprite = taskRequirementData.RequiredBodyPart.Icon;
        }

        if (hasWeakness)
        {
            weaknessIcon.sprite = taskRequirementData.WeaknessToUse.Icon;
        }

        repetitionsText.text = $"x{taskRequirementData.Amount}";
        UpdateUI();
    }

    private void UpdateUI()
    { 
            repetitionsText.text = $"x{Requirement.Amount - CurrentCount}"; 
            completedMark.SetActive(IsCompleted); 
    } 

    public bool TryToUpdate(NpcData npc, MurderousItemData item)
    {
        // BODY PART
        if (Requirement.RequiredBodyPart != null && Requirement.RequiredBodyPart != npc.Body &&
            Requirement.RequiredBodyPart != npc.Head)
        {
            return false;
        }

        // KONKRETNY ITEM
        if (Requirement.ItemToUse != null && Requirement.ItemToUse != item)
        {
            return false;
        }

        // KONKRETNA SŁABOŚĆ (jeśli używacie WeaknessTraitData)
        if (Requirement.WeaknessToUse != null)
        {
            // przykładowa logika, zależy jak to u Ciebie działa:
            if (item == null || item.Weakness != Requirement.WeaknessToUse) return false;
        }

        CurrentCount++;
        UpdateUI();
        return true;
    }
}