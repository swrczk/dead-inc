using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class StartScreen : MonoBehaviour
{ 
    [SerializeField]
    private Button button; 
    [SerializeField]
    private VideoPlayer videoPlayer;
    [SerializeField]
    private GameObject screenToHide;
    private void Start()
    {
        button.onClick.AddListener(() => StartGame());
        PlayAndHide();
    }

    private async void PlayAndHide()
    {
        await UniTask.WaitUntil(()=>videoPlayer.isPlaying);
        await UniTask.WaitUntil(()=>!videoPlayer.isPlaying);
        screenToHide.SetActive(false);
    }

    private void StartGame()
    {
        SceneManager.LoadScene("Market");
    }
}
