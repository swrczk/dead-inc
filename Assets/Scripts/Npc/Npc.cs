using UnityEngine;
using UnityEngine.UI;
using BrunoMikoski.AnimationSequencer;
using Cysharp.Threading.Tasks;

public class Npc : MonoBehaviour
{
    [SerializeField]
    private Image head;

    [SerializeField]
    private Image body;

    [SerializeField]
    private NPCPathController pathController;

    public NPCPathController PathController => pathController;

    public AnimationSequencerController controller;

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
    /// Czy ten NPC jest wra?liwy na dany item ? na podstawie Weakness
    /// przypi?tego do Head/Body i Weakness z MurderousItemData.
    /// </summary>
    public bool IsVulnerableTo(MurderousItemData item)
    {
        Debug.Log($"[NPC {name}] IsVulnerableTo start: " + $"_data={_data}, " +
                  $"head={_data?.Head}, body={_data?.Body}, " + $"item={item}, item.Weakness={item?.Weakness}");

        if (_data == null || item == null || item.Weakness == null)
        {
            Debug.LogWarning($"[NPC {name}] IsVulnerableTo -> FALSE, pow?d: " +
                             $"{(_data == null ? "_data==null; " : "")}" + $"{(item == null ? "item==null; " : "")}" +
                             $"{(item != null && item.Weakness == null ? "item.Weakness==null; " : "")}");
            return false;
        }

        Debug.Log($"[NPC {name}] Weakness: " + $"Head={_data.Head?.Weakness}, Body={_data.Body?.Weakness}, " +
                  $"Item={item.Weakness}");

        bool headMatch = _data.Head != null && _data.Head.Weakness == item.Weakness;
        bool bodyMatch = _data.Body != null && _data.Body.Weakness == item.Weakness;

        Debug.Log($"[NPC {name}] headMatch={headMatch}, bodyMatch={bodyMatch}");

        if (headMatch || bodyMatch) return true;

        return false;
    }

    public async void Kill(MurderousItemData usedItem)
    {
        if (!IsVulnerableTo(usedItem)) return;
        
        if (!usedItem.WasPlayed)
        {
            usedItem.WasPlayed = true;

            if (usedItem.Clip != null) await VideoManager.Instance.Play(usedItem.Clip);
        }

        SoundManager.Instance.Play(usedItem.Sound);
        controller.Play();
        await UniTask.WaitUntil(() => controller.IsPlaying);
        await UniTask.WaitUntil(() => !controller.IsPlaying);

        NpcTypeKilledSignal.Invoke(_data, usedItem);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        NpcDissappearedSignal.Invoke(Id);
    }
}