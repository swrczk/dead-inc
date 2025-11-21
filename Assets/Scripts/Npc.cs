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
        if (_data == null)
            return false;

        return _data.IsVulnerableTo(item);
    }

    /// <summary>
    /// Zabija NPC tym przedmiotem (jeœli jeszcze ¿yje).
    /// </summary>
    public void Kill(MurderousItemData usedItem)
    {
        if (!gameObject.activeInHierarchy)
            return;

        // Zatrzymaj ruch po œcie¿kach
        if (pathController != null)
        {
            pathController.enabled = false;
        }

        // Punkty za zabicie
        if (ScoreManager.Instance != null)
        {
            int baseKillPoints = 5; // mo¿esz póŸniej zró¿nicowaæ
            ScoreManager.Instance.OnNpcKilled(baseKillPoints);
        }

        // Zg³oœ zabójstwo do JiraTaskManager (¿eby liczy³y siê tickety)
        JiraTaskManager jira = FindObjectOfType<JiraTaskManager>();
        if (jira != null && _data != null)
        {
            // tutaj mo¿esz wybraæ, czy raportowaæ g³owê czy cia³o – na razie g³owa
            jira.ReportKill(_data.Head, usedItem);
        }

        // To odpali Twoje sygna³y w OnDestroy
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // Twoje istniej¹ce sygna³y
        NpcKilledSignal.Invoke(Id);
        NpcTypeKilledSignal.Invoke(_data);
    }
}
