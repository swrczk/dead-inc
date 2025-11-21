using UnityEngine;
using UnityEngine.UI;

public class HintPanelRow : MonoBehaviour
{
    [SerializeField]
    private Image leftImage;

    [SerializeField]
    private Image rightImage;

    public void Setup(Sprite leftIcon, Sprite rightIcon)
    {
        leftImage.sprite = leftIcon;
        rightImage.sprite = rightIcon;
    }
}