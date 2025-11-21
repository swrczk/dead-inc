using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JiraRequirement : MonoBehaviour
{
    [SerializeField]
    private Image bodyPartIcon;

    [SerializeField]
    private Image itemIcon;

    [Tooltip("  x3 ")]
    [SerializeField]
    private TMP_Text repetitionsText;

    public void Setup(Task taskData)
    {
        //
    }
}