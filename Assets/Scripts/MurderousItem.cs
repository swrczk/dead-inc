using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MurderousItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    IPointerClickHandler
{
    public Sprite Icon => itemImage.sprite;

    public MurderousItemData ItemData => itemData;

    [SerializeField]
    private MurderousItemData itemData;

    [SerializeField]
    private Image itemImage;

    [SerializeField]
    private RectTransform rangeVisual;

    [SerializeField]
    private float rangeRadius = 150f;

    [SerializeField]
    private float npcHitSlack = 20f;

    [SerializeField]
    private bool killOnlyFirst = true;

    [SerializeField]
    private AudioClip wrongItemUsedClip;

    private RectTransform _rectTransform;


    private Graphic _rangeGraphic;
    private SpriteRenderer _rangeSprite;

    private Canvas _canvas;
    private Camera _uiCamera;

    private void Awake()
    {
        itemData.WasPlayed = false;
        _rectTransform = GetComponent<RectTransform>();

        if (rangeVisual != null)
        {
            _rangeGraphic = rangeVisual.GetComponentInChildren<Graphic>(true);
            _rangeSprite = rangeVisual.GetComponentInChildren<SpriteRenderer>(true);
            HideRange();
        }

        _canvas = GetComponentInParent<Canvas>();
        if (_canvas != null)
            _uiCamera = _canvas.worldCamera;

        Debug.Log($"[MurderousItemClickableCanvas] Awake on {name}");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowRange();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideRange();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var itemWorldPos = _rectTransform.TransformPoint(_rectTransform.rect.center);
        var itemPos2D = new Vector2(itemWorldPos.x, itemWorldPos.y);

        var allNpcs = FindObjectsOfType<Npc>();
        if (allNpcs == null || allNpcs.Length == 0)
            return;

        var killedSomeone = false;

        foreach (var npc in allNpcs)
        {
            var npcRect = npc.GetComponent<RectTransform>();
            var npcWorldPos = npcRect.TransformPoint(npcRect.rect.center);

            var inRange = false;
            if (rangeVisual != null)
            {
                var screenCenter = RectTransformUtility.WorldToScreenPoint(_uiCamera, npcWorldPos);

                var testPoints = new Vector2[]
                {
                    screenCenter,
                    screenCenter + new Vector2(npcHitSlack, 0),
                    screenCenter + new Vector2(-npcHitSlack, 0),
                    screenCenter + new Vector2(0, npcHitSlack),
                    screenCenter + new Vector2(0, -npcHitSlack),
                };

                foreach (var p in testPoints)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(rangeVisual, p, _uiCamera))
                    {
                        inRange = true;
                        break;
                    }
                }
            }
            else
            {
                var dist = Vector2.Distance(
                    itemPos2D,
                    new Vector2(npcWorldPos.x, npcWorldPos.y)
                );
                inRange = dist <= rangeRadius;
            }

            if (!inRange)
                continue;

            if (npc.IsVulnerableTo(itemData))
            {
                npc.Kill(itemData);
                killedSomeone = true;

                if (killOnlyFirst)
                    break;
            }
        }

        if (!killedSomeone)
        {
            SoundManager.Instance.Play(wrongItemUsedClip);
        }
    }

    private void ShowRange()
    {
        if (rangeVisual == null) return;

        rangeVisual.gameObject.SetActive(true);

        if (_rangeGraphic != null)
            _rangeGraphic.enabled = true;

        if (_rangeSprite != null)
            _rangeSprite.enabled = true;
    }

    private void HideRange()
    {
        if (rangeVisual == null) return;

        rangeVisual.gameObject.SetActive(false);

        if (_rangeGraphic != null)
            _rangeGraphic.enabled = false;

        if (_rangeSprite != null)
            _rangeSprite.enabled = false;
    }
}