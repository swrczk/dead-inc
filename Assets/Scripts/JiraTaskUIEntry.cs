using UnityEngine;
using UnityEngine.UI;

public class JiraTaskUIEntry : MonoBehaviour
{
    public Image bodyPartIcon;
    public Image itemIcon;
    public Text pointsText;

    [Tooltip("T?o / badge punkt?w, zmienia kolor w zale?no?ci od nagrody.")]
    public Image pointsBackground;

    public Color lowPointsColor = Color.green;
    public Color mediumPointsColor = Color.yellow;
    public Color highPointsColor = Color.red;

    [Tooltip("Punkty od kt?rych zaczyna si? ?rednia nagroda.")]
    public int mediumPointsThreshold = 15;

    [Tooltip("Punkty od kt?rych zaczyna si? wysoka nagroda.")]
    public int highPointsThreshold = 30;

    [Tooltip("  x3 ")]
    public Text repetitionsText;

    [Tooltip("  1/3 ")]
    public Text progressText;


    private JiraTaskData data;

    private int CurrentCount;
    public JiraTaskData Data => data;
    public bool IsCompleted { get; set; }


    public void Init(JiraTaskData task)
    { 
        data = task;
        Task primary = null;
        if (data.Required != null && data.Required.Count > 0) primary = data.Required[0];

        if (primary != null && primary.RequiredBodyPart != null && primary.RequiredBodyPart.Icon != null)
        {
            bodyPartIcon.sprite = primary.RequiredBodyPart.Icon;
            bodyPartIcon.enabled = true;
        }
        else
        {
            bodyPartIcon.enabled = false;
        }

        if (primary != null && primary.ItemToUse != null && primary.ItemToUse.Icon != null)
        {
            itemIcon.sprite = primary.ItemToUse.Icon;
            itemIcon.enabled = true;
        }
        else
        {
            itemIcon.enabled = false;
        }

        pointsText.text = $"+{data.Points}";
        Color c = lowPointsColor;
        if (data.Points >= highPointsThreshold)
            c = highPointsColor;
        else if (data.Points >= mediumPointsThreshold) c = mediumPointsColor;

        pointsBackground.color = c;
        repetitionsText.text = $"x{data.Amount}";

        UpdateProgressUI();
    }

    public void UpdateProgressUI()
    {
        progressText.text = $"{ CurrentCount} / {data.Amount}";
    }

    public bool TryUpdateProgress(NpcData npc, MurderousItemData item)
    {
        var result = true;
        foreach (var requirement in data.Required)
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
        }
        return result;
    }
}