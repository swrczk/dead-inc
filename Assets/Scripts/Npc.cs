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

    private void OnDestroy()
    {
        NpcKilledSignal.Invoke(Id);
        NpcTypeKilledSignal.Invoke(_data);
    }
}