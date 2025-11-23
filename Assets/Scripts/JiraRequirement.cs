using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JiraRequirement : MonoBehaviour
{
    public bool IsCompleted => _currentCount >= _requirement.Amount;

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

    private TaskRequirement _requirement;

    private int _currentCount;

    public void Setup(TaskRequirement taskRequirementData)
    {
        _requirement = taskRequirementData;
        _currentCount = 0;
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
        repetitionsText.text = $"x{_requirement.Amount - _currentCount}";
        completedMark.SetActive(IsCompleted);
    }

    public bool TryToUpdate(NpcData npc, MurderousItemData item)
    {
        if (_requirement.RequiredBodyPart != null && _requirement.RequiredBodyPart != npc.Body &&
            _requirement.RequiredBodyPart != npc.Head)
        {
            return false;
        }

        if (_requirement.ItemToUse != null && _requirement.ItemToUse != item)
        {
            return false;
        }

        if (_requirement.WeaknessToUse != null)
        {
            if (item == null || item.Weakness != _requirement.WeaknessToUse) return false;
        }

        _currentCount++;
        UpdateUI();
        return true;
    }
}