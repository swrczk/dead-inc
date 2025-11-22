using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public static VideoManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<VideoManager>();
                _instance ??= new GameObject("VideoManager").AddComponent<VideoManager>();
            }

            return _instance;
        }
    }


    private static VideoManager _instance;

    [SerializeField]
    private CutscenePanel _cutscenePanel;

    public async UniTask Play(VideoClip clip)
    {
        _cutscenePanel.gameObject.SetActive(true);
        Time.timeScale = 0f;
        await _cutscenePanel.Play(clip);
        Time.timeScale = 1f;
        _cutscenePanel.gameObject.SetActive(false);
    }
}