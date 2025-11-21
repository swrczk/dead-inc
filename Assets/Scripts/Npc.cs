using UnityEngine;
using UnityEngine.UI;

public class Npc : MonoBehaviour
{
    [SerializeField]
    private Image head;

    [SerializeField]
    private Image body;

    private NpcData _data;

    public void Setup(NpcData npcData)
    {
        _data = npcData;
        head.sprite = _data.Head.Icon;
        body.sprite = _data.Body.Icon;
    }
}