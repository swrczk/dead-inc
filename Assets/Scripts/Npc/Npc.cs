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
        Debug.Log($"[NPC] Setup called for id={id}, npcData={npcData?.name}");
        Id = id;
        _data = npcData;
        head.sprite = _data.Head.Icon;
        body.sprite = _data.Body.Icon;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Czy ten NPC jest wra¿liwy na dany item – na podstawie Weakness
    /// przypiêtego do Head/Body i Weakness z MurderousItemData.
    /// </summary>
    public bool IsVulnerableTo(MurderousItemData item)
    {
        if (_data == null || item == null || item.Weakness == null)
            return false;

        // g³owa
        if (_data.Head != null && _data.Head.Weakness == item.Weakness)
            return true;

        // cia³o
        if (_data.Body != null && _data.Body.Weakness == item.Weakness)
            return true;

        return false;
    }

    /// <summary>
    /// Zabija NPC, jeœli item pasuje do jego Weakness.
    /// Zg³asza kill do punktów i systemu ticketów.
    /// </summary>
    public void Kill(MurderousItemData usedItem)
    {
        if (!IsVulnerableTo(usedItem))
            return;

        // zatrzymanie ruchu
        if (pathController != null)
            pathController.enabled = false;

        // punkty za zabicie
        if (ScoreManager.Instance != null)
        {
            int baseKillPoints = 5; // albo wyci¹gniête z usedItem / NpcData
            ScoreManager.Instance.OnNpcKilled(baseKillPoints);
        }

        // ustalamy, która czêœæ cia³a by³a "trafiona" – przyda siê do ticketów
        NpcPartData killedPart = null;
        if (_data.Head != null && _data.Head.Weakness == usedItem.Weakness)
            killedPart = _data.Head;
        else if (_data.Body != null && _data.Body.Weakness == usedItem.Weakness)
            killedPart = _data.Body;

        // zg³oszenie do JiraTaskManager, ¿eby liczy³y siê tickety
        var jira = FindObjectOfType<JiraTaskManager>();
        if (jira != null && killedPart != null)
        {
            jira.ReportKill(killedPart, usedItem);
        }

        // faktyczne "zabicie"
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        NpcKilledSignal.Invoke(Id);
        NpcTypeKilledSignal.Invoke(_data);
    }
}
