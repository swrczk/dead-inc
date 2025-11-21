using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MurderousItemClickable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Dane przedmiotu")]
    [Tooltip("Typ ataku / obra¿eñ, jaki zadaje ten przedmiot.")]
    public MurderousItemData itemData;

    [Header("Zasiêg dzia³ania (Canvas)")]
    [Tooltip("Wizualizacja zasiêgu (okr¹g jako child na Canvasie).")]
    public RectTransform rangeVisual;

    [Tooltip("Zasiêg w jednostkach Canvas (w przybli¿eniu piksele).")]
    public float rangeRadius = 150f;

    [Header("Ustawienia")]
    [Tooltip("Czy po trafieniu ma zabiæ tylko jednego NPC (pierwszego w zasiêgu).")]
    public bool killOnlyFirst = true;

    private void Awake()
    {
        if (rangeVisual != null)
        {
            rangeVisual.gameObject.SetActive(false);
            rangeVisual.sizeDelta = new Vector2(rangeRadius * 2f, rangeRadius * 2f);
        }
    }

    // Podœwietl zasiêg po najechaniu
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (rangeVisual != null)
            rangeVisual.gameObject.SetActive(true);
    }

    // Ukryj zasiêg po zjechaniu kursorem
    public void OnPointerExit(PointerEventData eventData)
    {
        if (rangeVisual != null)
            rangeVisual.gameObject.SetActive(false);
    }

    // Próba zabicia po klikniêciu
    public void OnPointerClick(PointerEventData eventData)
    {
        TryKillNpcInRange();
    }

    private void TryKillNpcInRange()
    {
        if (itemData == null)
        {
            Debug.LogWarning($"{name}: MurderousItemClickable nie ma przypisanego itemData.");
            return;
        }

        Npc[] allNpcs = FindObjectsOfType<Npc>();
        if (allNpcs == null || allNpcs.Length == 0)
            return;

        Vector3 center = transform.position; // Canvas w Screen Space Overlay – OK
        float rangeSq = rangeRadius * rangeRadius;
        bool killed = false;

        foreach (var npc in allNpcs)
        {
            if (npc == null || !npc.gameObject.activeInHierarchy)
                continue;

            float distSq = (npc.transform.position - center).sqrMagnitude;
            if (distSq <= rangeSq)
            {
                if (npc.IsVulnerableTo(itemData))
                {
                    npc.Kill(itemData);
                    killed = true;

                    if (killOnlyFirst)
                        break;
                }
            }
        }

        if (!killed)
        {
            // Klik w pustkê – nic siê nie dzieje (na razie bez kary).
            // Tu mo¿esz kiedyœ dodaæ feedback dŸwiêkowy / wizualny.
        }
    }
}
