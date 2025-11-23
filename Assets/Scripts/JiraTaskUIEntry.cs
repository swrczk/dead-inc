using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrunoMikoski.AnimationSequencer;
using Cysharp.Threading.Tasks;
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
    [SerializeField]
    private AnimationSequencerController completeAnimation;
    [SerializeField]
    private TMP_Text pointsAnimationText; 
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
        pointsAnimationText.text = $"+{Data.Points}";
    }

    private void ClearChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform) Destroy(child.gameObject);
    }

    public async UniTask<bool> TryUpdateProgress(NpcData npc, MurderousItemData item)
    {
        if (IsCompleted) return false;

        var anyRequirementMatched = false;

        foreach (var requirement in _requirements)
        {
            if (requirement.TryToUpdate(npc, item))
            {
                anyRequirementMatched = true;
            }
        }

        if (!anyRequirementMatched) return false;

        IsCompleted = _requirements.All(r => r.IsCompleted);

        if (IsCompleted)
        {
            
            await  PlayAnimation();
            Destroy( gameObject);
        }
        return IsCompleted;
    }

    private async UniTask PlayAnimation()
    {
        completeAnimation.Play();
        await UniTask.WaitUntil(() => completeAnimation.IsPlaying);
        await UniTask.WaitUntil(() => !completeAnimation.IsPlaying);
    }
}