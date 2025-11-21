using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class HintsManager : MonoBehaviour
{
    [SerializeField]
    private Button hintButton;
    
    [SerializeField]
    private GameFlowManager gameFlowManager;


    [SerializeField]
    private float hideAfterSec;

    [SerializeField]
    private HintPanel panel;

    private List<MurderousItemData> items = new List<MurderousItemData>();
    void Start()
    {
        hintButton.onClick.AddListener(ShowHintPanel); 
        items = FindObjectsOfType<MurderousItemData>().ToList(); 
    }

    private async void ShowHintPanel()
    {
        panel.gameObject.SetActive(true);
        panel.Show(gameFlowManager.GameplayFlow, gameFlowManager.currentStageIndex);
        
        await UniTask.Delay((int) (hideAfterSec * 1000));
        
        panel.gameObject.SetActive(false);
    }
}