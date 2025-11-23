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

    void Start()
    {
        hintButton.onClick.AddListener(() => ShowHintPanel().Forget());
        panel.gameObject.SetActive(false);
    }

    private async UniTask ShowHintPanel()
    {
        panel.gameObject.SetActive(true);
        panel.Show(gameFlowManager.GameplayFlow, gameFlowManager.CurrentStageIndex);

        await UniTask.Delay((int) (hideAfterSec * 1000));

        panel.gameObject.SetActive(false);
    }
}