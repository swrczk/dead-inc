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
    private WaypointMover mover;

    [SerializeField]
    private AnimationSequencerController controller;

    private NpcData _data;

    public string Id { get; private set; }


    public void Setup(string id, NpcSet npcData)
    {
        Id = id;
        _data = npcData.NpcType;
        head.sprite = _data.Head.Icon;
        body.sprite = _data.Body.Icon;

        gameObject.SetActive(true);
        mover.Setup(npcData);
    }

    public bool IsVulnerableTo(MurderousItemData item)
    {
        if (_data == null) return false;
        var headMatch = _data.Head != null && _data.Head.Weakness == item.Weakness;
        var bodyMatch = _data.Body != null && _data.Body.Weakness == item.Weakness;

        if (headMatch || bodyMatch) return true;

        return false;
    }

    public async void Kill(MurderousItemData usedItem)
    {
        mover.Stop();
        SoundManager.Instance.Play(usedItem.Sound);
        controller.Play();
        await UniTask.WaitUntil(() => controller.IsPlaying);
        await UniTask.WaitUntil(() => !controller.IsPlaying);

        if (!usedItem.WasPlayed)
        {
            usedItem.WasPlayed = true;

            if (usedItem.Clip != null) await VideoManager.Instance.Play(usedItem.Clip);
        }

        NpcTypeKilledSignal.Invoke(_data, usedItem);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        NpcDissappearedSignal.Invoke(Id);
    }
}