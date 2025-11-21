using UnityEngine;
using UnityEngine.UI;

public class Npc : MonoBehaviour
{
    [SerializeField]
    private Image head;

    [SerializeField]
    private Image body;

    [SerializeField]
    private NPCPathController pathController;

    public NPCPathController PathController => pathController;

    private NpcData _data;

    public string Id { get; private set; }

    public void Setup(string id, NpcData npcData)
    {
        Id = id;
        _data = npcData;
        head.sprite = _data.Head.Icon;
        body.sprite = _data.Body.Icon;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Sprawdza, czy ten NPC jest wra¿liwy na dany typ ataku / przedmiot.
    /// </summary>
    public bool IsVulnerableTo(MurderousItemData item)
    {
        if (_data == null || _data.Vulnerabilities == null || item == null)
            return false;

        return _data.Vulnerabilities.Contains(item);
    }

    /// <summary>
    /// Zabija NPC tym przedmiotem.
    /// </summary>
    public void Kill(MurderousItemData usedItem)
    {
        if (!gameObject.activeInHierarchy)
            return;

        // Zatrzymaj ruch
        if (pathController != null)
            pathController.enabled = false;

        // Punkty za zabicie
        if (ScoreManager.Instance != null)
        {
            int baseKillPoints = 5;
            ScoreManager.Instance.OnNpcKilled(baseKillPoints);
        }

        // Zg³oœ zabójstwo do tasków
        JiraTaskManager jira = FindObjectOfType<JiraTaskManager>();
        if (jira != null && _data != null)
        {
            jira.ReportKill(_data.Head, usedItem);
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        NpcKilledSignal.Invoke(Id);
        NpcTypeKilledSignal.Invoke(_data);
    }
}
