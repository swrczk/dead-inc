using System.Collections.Generic;
using UnityEngine;

public class HintPanel : MonoBehaviour
{
    [SerializeField]
    private HintPanelRow hintPanelRow;
    
    [SerializeField]
    private GameObject leftPanel;
    [SerializeField]
    private GameObject rightPanel;
    public void Show(GameplayFlow flow, int stageIndex)
    {
        var uniqueNpcs = new HashSet<NpcPartData>();

        for (int i = 0; i <= stageIndex; i++)
        {
            var stage = flow.Stages[i];
            foreach (var npc in stage.AvailableNpcs)
            {
                uniqueNpcs.Add(npc.NpcType.Head);
                uniqueNpcs.Add(npc.NpcType.Body);
            }
        }

        ClearChildren(leftPanel);
        foreach (var npc in uniqueNpcs)
        {
            var row = Instantiate(hintPanelRow, leftPanel.transform);
            row.Setup(npc.Icon, npc.Weakness.Icon);
        }
    }public void ClearChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform)
            Destroy(child.gameObject);
    }
}