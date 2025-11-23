using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HintPanel : MonoBehaviour
{
    [SerializeField]
    private HintPanelRow hintPanelRow;

    [SerializeField]
    private GameObject leftPanel;

    [SerializeField]
    private GameObject rightPanel;

    private List<MurderousItemClickableCanvas> _itemsList;


    public void Show(GameplayFlow flow, int stageIndex)
    {
        _itemsList ??= FindObjectsOfType<MurderousItemClickableCanvas>().ToList();
        var uniqueNpcs = new HashSet<NpcPartData>();

        for (var i = 0; i <= stageIndex; i++)
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


        var uniqueNpcsWeakness = new HashSet<WeaknessTraitData>();

        foreach (var npcPartData in uniqueNpcs)
        {
            uniqueNpcsWeakness.Add(npcPartData.Weakness);
        }

        var itemsToShow = _itemsList.Where(i => uniqueNpcsWeakness.Contains(i.itemData.Weakness)).ToList();

        ClearChildren(rightPanel);
        foreach (var item in itemsToShow)
        {
            var row = Instantiate(hintPanelRow, rightPanel.transform);
            row.Setup(item.Icon, item.itemData.Weakness.Icon);
        }
    }

    private void ClearChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform) Destroy(child.gameObject);
    }
}