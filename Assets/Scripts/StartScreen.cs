using System.IO;
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
    private VideoClip videoToPlay;

    [SerializeField]
    private GameObject screenToHide;

    private void Start()
    {
        button.onClick.AddListener(() => StartGame());
        PlayAndHide();
    }

    private async void PlayAndHide()
    {
        videoPlayer.source = VideoSource.Url;
        string path = Path.Combine(Application.streamingAssetsPath, videoToPlay.name + ".mp4");
        path = path.Replace("\\", "/");
        videoPlayer.url = path;

        await UniTask.WaitUntil(() => videoPlayer.isPlaying);
        await UniTask.WaitUntil(() => !videoPlayer.isPlaying);

        screenToHide.SetActive(false);
    }

    private void StartGame()
    {
        SceneManager.LoadScene("Market");
    }
}